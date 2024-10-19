#region Copyright and MIT License
/* MIT License
 *
 * Copyright © 2024 Lucas Ontivero
 * 
 * Modifications Copyright © 2024 Svante Seleborg
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
*/
#endregion Copyright and MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Slip39;
/// <summary>
/// Slip39 is a library for Shamir's Secret Sharing with SLIP-0039 enhancements.
/// </summary>
internal static class AssemblyDoc { }

/// <summary>The main entry point for the library is through the <see cref="IShamirsSecretSharing"/> interface, which
/// has a default implementation in <see cref="ShamirsSecretSharing"/>. The only other public type of importance is
/// <see cref="Share"/> which is how generated shares are represented and returned. It also has a static method to
/// parse a share string into a <see cref="Share"/> object.</summary>
internal static class NamespaceDoc { }

/// <summary>
/// A class for implementing Shamir's Secret Sharing with SLIP-0039 enhancements.
/// </summary>
public class ShamirsSecretSharing(IRandom random) : IShamirsSecretSharing
{
    private record MemberData(int MemberThreshold, int MemberIndex, byte[] Value);

    /// <summary>
    /// The data for a group or member share
    /// </summary>
    /// <param name="Index">The group or member index</param>
    /// <param name="Value">The raw share value, as big endian bit/byte array.</param>
    /// <remarks>
    /// Since shares are guaranteed to consist of a number of 10-bit parts, the (Value.Length * 8) % 10 least
    /// significant bits should be discarded.
    /// </remarks>
    private record ShareData(int Index, byte[] Value);

    private record CommonParameters(int Id, int IterationExponent, int GroupThreshold, Dictionary<int, List<MemberData>> Groups);

    private const int SECRET_INDEX = 255;
    private const int DIGEST_INDEX = 254;
    private const int MAX_SHARE_COUNT = 16;
    private const int DIGEST_LENGTH_BYTES = 4;
    private const int BASE_ITERATION_COUNT = 10000;
    private const int ROUND_COUNT = 4;

    /// <summary>
    /// Generates SLIP-0039 shares from a given master secret.
    /// </summary>
    /// <param name="extendable"></param>
    /// <param name="iterationExponent">Exponent to determine the number of iterations for the encryption
    /// algorithm.</param>
    /// <param name="groupThreshold">The number of groups required to reconstruct the secret.</param>
    /// <param name="groups">Array of tuples where each tuple represents (groupThreshold, shareCount) for each
    /// group.</param>
    /// <param name="passphrase">The passphrase used for encryption.</param>
    /// <param name="masterSecret">The secret to be split into shares.</param>
    /// <returns>A list of shares that can be used to reconstruct the secret.</returns>
    /// <exception cref="ArgumentException">Thrown when inputs do not meet the required constraints.</exception>
    public Share[] GenerateShares(bool extendable, int iterationExponent, int groupThreshold, Group[] groups, string passphrase, byte[] masterSecret)
    {
        // Validating seed strength and format
        if (masterSecret.Length * 8 < Share.MinStrengthBits || masterSecret.Length % 2 != 0)
        {
            throw new ArgumentException("Master secret entropy must be at least 128 bits and multiple of 16 bits.");
        }

        // Validating group constraints
        if (groupThreshold > MAX_SHARE_COUNT)
        {
            throw new ArgumentException("More than 16 groups are not supported.");
        }

        if (groupThreshold > groups.Length)
        {
            throw new ArgumentException("Group threshold should not exceed number of groups.");
        }

        if (groups.Any(group => group is { GroupThreshold: 1, GroupCount: > 1 }))
        {
            throw new ArgumentException("Can only generate one share for threshold = 1.");
        }

        if (groups.Any(group => group.GroupThreshold > group.GroupCount))
        {
            throw new ArgumentException("Number of shares must not be less than threshold.");
        }

        int id = SharePrefix.GenerateId(random);

        // Encrypt the secret using the passphrase and identifier
        byte[] encryptedSecret = Encrypt(id, iterationExponent, masterSecret, passphrase, extendable);

        // Split the encrypted secret into first level group shares
        ShareData[] groupShares = SplitSecret(groupThreshold, groups.Length, encryptedSecret);

        // Split each group share into member shares and create the final second level share objects
        List<Share> shares = [];
        foreach (ShareData groupShare in groupShares)
        {
            Group group = groups[groupShare.Index];

            ShareData[] memberShares = SplitSecret(group.GroupThreshold, group.GroupCount, groupShare.Value);
            foreach (ShareData memberShare in memberShares)
            {
                ShareParameters parameters = new()
                {
                    Id = id,
                    Extendable = extendable,
                    IterationExponent = iterationExponent,
                    GroupThreshold = groupThreshold,
                    GroupCount = groups.Length,
                    GroupIndex = groupShare.Index,
                    MemberIndex = memberShare.Index,
                    MemberThreshold = group.GroupThreshold
                };
                SharePrefix sharePrefix = new(parameters);
                shares.Add(new(sharePrefix, memberShare.Value));
            }
        }

        return [.. shares];
    }

    /// <summary>
    /// Combines shares to reconstruct the original secret.
    /// </summary>
    /// <param name="shares">The array of shares to combine.</param>
    /// <param name="passphrase">The passphrase used for decrypting the shares.</param>
    /// <returns>The reconstructed secret.</returns>
    /// <exception cref="ArgumentException">Thrown when the shares are insufficient or invalid.</exception>
    public byte[] CombineShares(Share[] shares, string passphrase)
    {
        // Preprocess the shares to extract group and member information
        CommonParameters common = Preprocess(shares);

        // Validating group constraints 
        if (common.Groups.Count < common.GroupThreshold)
        {
            throw new ArgumentException("Need shares from more groups to reconstruct secret.");
        }

        if (common.Groups.Count != common.GroupThreshold)
        {
            throw new ArgumentException("Shares from too many groups.");
        }

        if (common.Groups.Any(group => group.Value[0].MemberThreshold != group.Value.Count))
        {
            throw new ArgumentException("For every group, number of member shares should match member threshold.");
        }

        if (common.Groups.Any(group => group.Value.Select(v => v.MemberThreshold).ToHashSet().Count > 1))
        {
            throw new ArgumentException("Member threshold must be the same within a group.");
        }

        // Recover secrets for each group and then combine them to get the final secret
        List<ShareData> groupSecrets = [];
        foreach (KeyValuePair<int, List<MemberData>> group in common.Groups)
        {
            byte[] recoveredSecret = RecoverSecret(group.Value[0].MemberThreshold, group.Value.Select(v => new ShareData(v.MemberIndex, v.Value)).ToArray());
            groupSecrets.Add(new ShareData(group.Key, recoveredSecret));
        }

        byte[] finalRecoveredSecret = RecoverSecret(common.GroupThreshold, [.. groupSecrets]);

        // Decrypt the secret using the passphrase
        byte[] decryptedMasterSecret = Decrypt(common.Id, common.IterationExponent, finalRecoveredSecret, passphrase, shares[0].SharePrefix.Extendable);
        return decryptedMasterSecret;
    }

    /// <summary>
    /// Preprocesses the shares to group them by group index and validate constraints.
    /// </summary>
    /// <param name="shares">The array of shares to preprocess.</param>
    /// <returns>A tuple containing identifiers and group information for the shares.</returns>
    /// <exception cref="ArgumentException">Thrown when the shares do not meet the required constraints.</exception>
    private static CommonParameters Preprocess(Share[] shares)
    {
        if (shares.Length < 1)
        {
            throw new ArgumentException("Need at least one share to reconstruct secret.");
        }

        CommonParameters parameters = new(Id: shares[0].SharePrefix.Id,
                                          IterationExponent: shares[0].SharePrefix.IterationExponent,
                                          GroupThreshold: shares[0].SharePrefix.GroupThreshold,
                                          []);

        if (shares.Any(s => s.SharePrefix.Id != parameters.Id))
        {
            throw new ArgumentException("Shares do not belong to the same secret.");
        }

        if (shares.Any(s => s.SharePrefix.IterationExponent != parameters.IterationExponent))
        {
            throw new ArgumentException("Shares do not have the same iteration exponent.");
        }

        if (shares.Any(s => s.SharePrefix.GroupThreshold != parameters.GroupThreshold))
        {
            throw new ArgumentException("Shares do not have the same group threshold.");
        }

        if (shares.Any(s => s.SharePrefix.GroupCount != shares[0].SharePrefix.GroupCount))
        {
            throw new ArgumentException("Shares do not have the same group count.");
        }

        if (shares.Any(s => s.SharePrefix.GroupThreshold > s.SharePrefix.GroupCount))
        {
            throw new ArgumentException("Group threshold is greater than group count.");
        }

        // Group the shares by group index
        foreach (Share share in shares)
        {
            if (!parameters.Groups.TryGetValue(share.SharePrefix.GroupIndex, out List<MemberData>? value))
            {
                value = [];
                parameters.Groups[share.SharePrefix.GroupIndex] = value;
            }

            value.Add(new MemberData(MemberThreshold: share.SharePrefix.MemberThreshold, MemberIndex: share.SharePrefix.MemberIndex, Value: share.Value));
        }

        return parameters;
    }

    /// <summary>
    /// Recovers the secret from a set of shares.
    /// </summary>
    /// <param name="threshold">The number of shares required to reconstruct the secret.</param>
    /// <param name="shares">The shares to be used for reconstruction.</param>
    /// <returns>The recovered secret.</returns>
    /// <exception cref="ArgumentException">Thrown when the share digests are incorrect.</exception>
    private static byte[] RecoverSecret(int threshold, ShareData[] shares)
    {
        // If the threshold is 1, simply return the first share's value
        if (threshold == 1)
        {
            return shares[0].Value;
        }

        // Interpolate the shares to recover the shared secret and digest
        byte[] sharedSecret = Interpolate(shares, SECRET_INDEX);
        byte[] digestShare = Interpolate(shares, DIGEST_INDEX);

        // Verify the share digest according to SLIP-0039 specification of RecoverSecret()
        return BitConverter.ToInt32(ShareDigest(digestShare[DIGEST_LENGTH_BYTES..], sharedSecret)) !=
            BitConverter.ToInt32(digestShare.AsSpan()[..DIGEST_LENGTH_BYTES])
                ? throw new ArgumentException("Share digest is incorrect.")
                : sharedSecret;
    }

    /// <summary>
    /// Split a secret into N member shares, with a member threshold T
    /// </summary>
    /// <param name="threshold">Member threshold for group i, a positive integer, 1 ≤ Ti ≤ Ni.</param>
    /// <param name="shareCount">Total number of members in group i, a positive integer, 1 ≤ Ni ≤ 16.</param>
    /// <param name="sharedSecret">Encrypted master secret or a group share, an octet string.</param>
    /// <returns>An array with size Ni with the member shares for this group.</returns>
    /// <exception cref="ArgumentException"></exception>
    private ShareData[] SplitSecret(int threshold, int shareCount, byte[] sharedSecret)
    {
        if (threshold < 1)
        {
            throw new ArgumentException("Sharing threshold must be > 1.");
        }

        if (shareCount > MAX_SHARE_COUNT)
        {
            throw new ArgumentException("Too many shares.");
        }

        if (threshold > shareCount)
        {
            throw new ArgumentException("Number of shares should be at least equal to threshold.");
        }

        List<ShareData> shares = [];

        if (threshold == 1)
        {
            for (byte i = 0; i < shareCount; i++)
            {
                shares.Add(new(i, [.. sharedSecret]));
            }
            return [.. shares];
        }

        int randomSharesCount = Math.Max(threshold - 2, 0);

        for (int i = 0; i < randomSharesCount; i++)
        {
            byte[] share = new byte[sharedSecret.Length];
            random.GetBytes(share);
            shares.Add(new(i, share));
        }

        List<ShareData> baseShares = new(shares);
        byte[] randomPart = new byte[sharedSecret.Length - DIGEST_LENGTH_BYTES];
        random.GetBytes(randomPart);

        byte[] digest = ShareDigest(randomPart, sharedSecret);
        baseShares.Add(new(DIGEST_INDEX, digest.ArrayConcat(randomPart)));
        baseShares.Add(new(SECRET_INDEX, sharedSecret));

        for (int i = randomSharesCount; i < shareCount; i++)
        {
            byte[] interpolatedShare = Interpolate([.. baseShares], i);
            shares.Add(new(i, interpolatedShare));
        }

        return [.. shares];
    }

    private static byte[] ShareDigest(byte[] random, byte[] sharedSecret)
    {
        using HMACSHA256 hmac = new(random);
        byte[] hash = hmac.ComputeHash(sharedSecret);
        return hash[..4];
    }

    /// <summary>
    /// Interpolates the shares to recover the secret.
    /// </summary>
    /// <param name="shares">The shares used for interpolation.</param>
    /// <param name="x">The index of the value to interpolate (secret or digest).</param>
    /// <returns>The interpolated value.</returns>
    private static byte[] Interpolate(ShareData[] shares, int x)
    {
        HashSet<int> xCoordinates = shares.Select(share => share.Index).ToHashSet();
        if (xCoordinates.Count != shares.Length)
        {
            throw new ArgumentException("Need unique shares for interpolation.");
        }
        if (shares.Length < 1)
        {
            throw new ArgumentException("Need at least one share for interpolation.");
        }
        int len = shares[0].Value.Length;
        if (shares.Any(share => share.Value.Length != len))
        {
            throw new ArgumentException("Shares should have equal length.");
        }
        if (xCoordinates.Contains(x))
        {
            return shares.First(share => share.Index == x).Value;
        }

        static int Mod255(int n)
        {
            while (n < 0) n += 255;
            return n % 255;
        }

        int logProd = shares
            .Select(share => Log[share.Index ^ x])
            .Aggregate(0, (a, v) => a + v);

        byte[] result = new byte[len];
        foreach (ShareData share in shares)
        {
            int logBasis = Mod255(
                logProd - Log[share.Index ^ x]
                        - shares.Select(j => Log[j.Index ^ share.Index]).Aggregate(0, (a, v) => a + v)
            );

            for (int k = 0; k < share.Value.Length; k++)
            {
                result[k] ^= share.Value[k] != 0 ? Exp[Mod255(Log[share.Value[k]] + logBasis)] : (byte)0;
            }
        }

        return result;
    }

    private static byte[] Encrypt(int identifier, int iterationExponent, byte[] master, string passphrase, bool extendable) =>
        Crypt(identifier, iterationExponent, master, [0, 1, 2, 3], CheckPassphrase(passphrase), extendable);

    private static byte[] Decrypt(int identifier, int iterationExponent, byte[] master, string passphrase, bool extendable) =>
        Crypt(identifier, iterationExponent, master, [3, 2, 1, 0], CheckPassphrase(passphrase), extendable);

    private static string CheckPassphrase(string passphrase) =>
        passphrase.Any(c => c is < ' ' or > '~')
            ? throw new NotSupportedException("Passphrase can only contain printable ASCII.")
            : passphrase;

    private static byte[] Crypt(
        int identifier,
        int iterationExponent,
        byte[] masterSecret,
        byte[] range,
        string passphrase,
        bool extendable)
    {
        int len = masterSecret.Length / 2;
        byte[] left = masterSecret[..len];
        byte[] right = masterSecret[len..];
        foreach (byte i in range)
        {
            byte[] f = Feistel(identifier, iterationExponent, i, right, passphrase, extendable);
            (left, right) = (right, Xor(left, f));
        }
        return right.ArrayConcat(left);
    }

    private static byte[] Feistel(int id, int iterationExponent, byte step, byte[] block, string passphrase, bool extendable)
    {
        byte[] key = step.ArrayConcat(Encoding.UTF8.GetBytes(passphrase));
        byte[] saltPrefix = extendable ? [] : "shamir"u8.ToArray().ArrayConcat([(byte)(id >> 8), (byte)(id & 0xff)]);
        byte[] salt = saltPrefix.ArrayConcat(block);
        int iters = (BASE_ITERATION_COUNT / ROUND_COUNT) << iterationExponent;
        using Rfc2898DeriveBytes pbkdf2 = new(key, salt, iters, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(block.Length);
    }

    private static byte[] Xor(byte[] a, byte[] b)
    {
        byte[] result = new byte[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            result[i] = (byte)(a[i] ^ b[i]);
        }
        return result;
    }

    private static readonly byte[] Exp = [
        1, 3, 5, 15, 17, 51, 85, 255, 26, 46, 114, 150, 161, 248, 19, 53, 95, 225, 56, 72, 216,
        115, 149, 164, 247, 2, 6, 10, 30, 34, 102, 170, 229, 52, 92, 228, 55, 89, 235, 38, 106,
        190, 217, 112, 144, 171, 230, 49, 83, 245, 4, 12, 20, 60, 68, 204, 79, 209, 104, 184, 211,
        110, 178, 205, 76, 212, 103, 169, 224, 59, 77, 215, 98, 166, 241, 8, 24, 40, 120, 136, 131,
        158, 185, 208, 107, 189, 220, 127, 129, 152, 179, 206, 73, 219, 118, 154, 181, 196, 87,
        249, 16, 48, 80, 240, 11, 29, 39, 105, 187, 214, 97, 163, 254, 25, 43, 125, 135, 146, 173,
        236, 47, 113, 147, 174, 233, 32, 96, 160, 251, 22, 58, 78, 210, 109, 183, 194, 93, 231, 50,
        86, 250, 21, 63, 65, 195, 94, 226, 61, 71, 201, 64, 192, 91, 237, 44, 116, 156, 191, 218,
        117, 159, 186, 213, 100, 172, 239, 42, 126, 130, 157, 188, 223, 122, 142, 137, 128, 155,
        182, 193, 88, 232, 35, 101, 175, 234, 37, 111, 177, 200, 67, 197, 84, 252, 31, 33, 99, 165,
        244, 7, 9, 27, 45, 119, 153, 176, 203, 70, 202, 69, 207, 74, 222, 121, 139, 134, 145, 168,
        227, 62, 66, 198, 81, 243, 14, 18, 54, 90, 238, 41, 123, 141, 140, 143, 138, 133, 148, 167,
        242, 13, 23, 57, 75, 221, 124, 132, 151, 162, 253, 28, 36, 108, 180, 199, 82, 246,
    ];
    private static readonly byte[] Log = [
        0, 0, 25, 1, 50, 2, 26, 198, 75, 199, 27, 104, 51, 238, 223, 3, 100, 4, 224, 14, 52, 141,
        129, 239, 76, 113, 8, 200, 248, 105, 28, 193, 125, 194, 29, 181, 249, 185, 39, 106, 77,
        228, 166, 114, 154, 201, 9, 120, 101, 47, 138, 5, 33, 15, 225, 36, 18, 240, 130, 69, 53,
        147, 218, 142, 150, 143, 219, 189, 54, 208, 206, 148, 19, 92, 210, 241, 64, 70, 131, 56,
        102, 221, 253, 48, 191, 6, 139, 98, 179, 37, 226, 152, 34, 136, 145, 16, 126, 110, 72, 195,
        163, 182, 30, 66, 58, 107, 40, 84, 250, 133, 61, 186, 43, 121, 10, 21, 155, 159, 94, 202,
        78, 212, 172, 229, 243, 115, 167, 87, 175, 88, 168, 80, 244, 234, 214, 116, 79, 174, 233,
        213, 231, 230, 173, 232, 44, 215, 117, 122, 235, 22, 11, 245, 89, 203, 95, 176, 156, 169,
        81, 160, 127, 12, 246, 111, 23, 196, 73, 236, 216, 67, 31, 45, 164, 118, 123, 183, 204,
        187, 62, 90, 251, 96, 177, 134, 59, 82, 161, 108, 170, 85, 41, 157, 151, 178, 135, 144, 97,
        190, 220, 252, 188, 149, 207, 205, 55, 63, 91, 209, 83, 57, 132, 60, 65, 162, 109, 71, 20,
        42, 158, 93, 86, 242, 211, 171, 68, 17, 146, 217, 35, 32, 46, 137, 180, 124, 184, 38, 119,
        153, 227, 165, 103, 74, 237, 222, 197, 49, 254, 24, 13, 99, 140, 128, 192, 247, 112, 7,
    ];
}
