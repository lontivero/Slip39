#region Copyright and MIT License
/* MIT License
 *
 * Copyright © 2024 Svante Seleborg
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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Slip39;

/// <summary>
/// A set of useful extensions.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Convenience method to generate shares for the simple case of a single group of shares with a single threshold,
    /// and no encryption.
    /// </summary>
    /// <param name="sss">The <see cref="IShamirsSecretSharing"/> instance.</param>
    /// <param name="memberThreshold">The member threshold.</param>
    /// <param name="memberCount">The total number of members.</param>
    /// <param name="masterSecret">The master secret to share as a string.</param>
    /// <returns>The set of shares, of which <paramref name="memberThreshold"/> are required to recover the
    /// secret.</returns>
    /// <remarks>
    /// The master secret is encoded as a <see cref="T:byte[]"/> of UTF-8 encoded bytes, and if required padded with 0xff
    /// to fulfil the requirements. When recovering the secret, any trailing 0xff bytes thus need to be trimmed before
    /// producing the original master secret string, <seealso cref="ToSecretString(byte[])"/>.
    /// </remarks>
    public static Share[] GenerateShares(this IShamirsSecretSharing sss, int memberThreshold,
        int memberCount, string masterSecret) =>
            sss.GenerateShares(memberThreshold, memberCount, masterSecret, StringEncoding.None);

    /// <summary>
    /// Convenience method to generate shares for the simple case of a single group of shares with a single threshold,
    /// and no encryption.
    /// </summary>
    /// <param name="sss">The <see cref="IShamirsSecretSharing"/> instance.</param>
    /// <param name="memberThreshold">The member threshold.</param>
    /// <param name="memberCount">The total number of members.</param>
    /// <param name="masterSecret">The master secret to share as a string.</param>
    /// <param name="encoding">The type of <see cref="StringEncoding"/> of the string master secret.</param>
    /// <returns>The set of shares, of which <paramref name="memberThreshold"/> are required to recover the
    /// secret.</returns>
    /// <remarks>
    /// The master secret is encoded as a <see cref="T:byte[]"/> of UTF-8 encoded bytes, and if required padded with
    /// 0xff to fulfil the requirements. When recovering the secret, any trailing 0xff bytes thus need to be trimmed
    /// before producing the original master secret string, <seealso cref="ToSecretString(byte[])"/>.
    /// </remarks>
    public static Share[] GenerateShares(this IShamirsSecretSharing sss, int memberThreshold,
        int memberCount, string masterSecret, StringEncoding encoding)
    {
        byte[] secretBytes;
        switch (encoding)
        {
            case StringEncoding.None:
                secretBytes = Encoding.UTF8.GetBytes(masterSecret);
                int paddedLength = secretBytes.Length < 16 ? 16 : secretBytes.Length;
                paddedLength += paddedLength % 2;
                secretBytes = secretBytes
                    .ArrayConcat(Enumerable.Repeat((byte)0xFF, paddedLength - secretBytes.Length).ToArray());
                break;

            case StringEncoding.Base64:
                secretBytes = masterSecret.FromUrlSafeBase64();
                break;

            case StringEncoding.Hex:
                secretBytes = masterSecret.FromHex();
                break;

            case StringEncoding.Bip39:
                secretBytes = masterSecret.FromBip39();
                break;

            default:
                throw new InvalidOperationException("Unknown secret string encoding.");
        }

        return sss.GenerateShares(extendable: true, iterationExponent: 0, 1,
            [new Group(memberThreshold, memberCount)], string.Empty, secretBytes);
    }

    /// <summary>
    /// Convenience method to generate shares for the simple case of a single group of shares with a single threshold,
    /// and no encryption.
    /// </summary>
    /// <param name="sss">The <see cref="IShamirsSecretSharing"/> instance.</param>
    /// <param name="memberThreshold">The member threshold.</param>
    /// <param name="memberCount">The total number of members.</param>
    /// <param name="masterSecret">The master secret to share.</param>
    /// <returns>The set of shares, of which <paramref name="memberThreshold"/> are required to recover the
    /// secret.</returns>
    public static Share[] GenerateShares(this IShamirsSecretSharing sss, int memberThreshold,
        int memberCount, byte[] masterSecret) =>
            sss.GenerateShares(extendable: true, iterationExponent: 0, 1, [new Group(memberThreshold, memberCount)],
                string.Empty, masterSecret);

    /// <summary>
    /// Try to parse a string into a <see cref="Share"/> instance.
    /// </summary>
    /// <param name="value">The string value to try to parse.</param>
    /// <param name="share">The resulting <see cref="Share"/> if successful.</param>
    /// <returns>True if the parse was successful.</returns>
    public static bool TryParse(this string value, out Share share)
    {
        try
        {
            share = Share.Parse(value);
            return true;
        }
        catch (ArgumentException)
        {
            share = null!;
            return false;
        }
    }

    /// <summary>
    /// Convert a recovered binary master secret to a UTF-8 encoded string, assuming it may have been padded with 0xFF
    /// bytes.
    /// </summary>
    /// <param name="masterSecret">The binary secret.</param>
    /// <returns>The original unpadded string.</returns>
    public static string ToSecretString(this byte[] masterSecret)
    {
        int paddingLength = 0;
        for (int i = masterSecret.Length - 1; i >= 0; --i)
        {
            if (masterSecret[i] != 0xFF)
            {
                break;
            }
            paddingLength++;
        }

        return Encoding.UTF8.GetString(masterSecret[..^paddingLength]);
    }

    /// <summary>
    /// Convenience method to recover a master secret from a set of shares, without a password.
    /// </summary>
    /// <param name="sss">The <see cref="IShamirsSecretSharing"/> instance.</param>
    /// <param name="shares">The shares to to recover from.</param>
    /// <returns>The recovered master secret.</returns>
    public static byte[] CombineShares(this IShamirsSecretSharing sss, Share[] shares) =>
        sss.CombineShares(shares, string.Empty);

    /// <summary>
    /// Contatenate two arrays.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <param name="first">The first array.</param>
    /// <param name="second">The second array.</param>
    /// <returns>A third array consisting of the concatenated result.</returns>
    public static T[] ArrayConcat<T>(this T[] first, T[] second)
    {
        T[] result = new T[first.Length + second.Length];
        Array.Copy(first, 0, result, 0, first.Length);
        Array.Copy(second, 0, result, first.Length, second.Length);
        return result;
    }

    /// <summary>
    /// A convenience method to contatenate a first element with an array.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <param name="first">The first element.</param>
    /// <param name="second">The second array.</param>
    /// <returns>A third array consisting of the concatenated result.</returns>
    public static T[] ArrayConcat<T>(this T first, T[] second) => new T[] { first }.ArrayConcat(second);

    /// <summary>
    /// Convert a byte array to a URL-safe base64 string.
    /// </summary>
    /// <param name="bytes">The bytes to convert.</param>
    /// <returns>The URL-safe base64 string.</returns>
    public static string ToUrlSafeBase64(this byte[] bytes) => Convert.ToBase64String(bytes)
        .Replace('+', '-').Replace('/', '_').Replace("=", "");

    /// <summary>
    /// Convert a URL-safe base64 string to a byte array.
    /// </summary>
    /// <param name="base64">The string to convert.</param>
    /// <returns>The converted bytes.</returns>
    public static byte[] FromUrlSafeBase64(this string base64) => Convert.FromBase64String(base64
        .Replace('-', '+').Replace('_', '/').PadRight(base64.Length + ((4 - (base64.Length % 4)) % 4), '='));

    /// <summary>
    /// Convert a hex string to a byte array.
    /// </summary>
    /// <param name="hex">The string to convert.</param>
    /// <returns>The converted bytes.</returns>
    public static byte[] FromHex(this string hex) => Convert.FromHexString(hex);

    /// <summary>
    /// Convert a bip 39 mnemonic string master secret to a byte array. See https://bips.dev/39/ .
    /// </summary>
    /// <param name="mnemonic">The list of mnemonic words.</param>
    /// <returns>The converted bytes.</returns>
    public static byte[] FromBip39(this string mnemonic)
    {
        int[] words = Mnemonic.FromBip39(mnemonic);
        int bitLength = words.Length * 11;
        byte[] bytes = new byte[(bitLength + 7) / 8];
        for (int i = 0; i < words.Length; ++i)
        {
            bytes.SetBits((offset: i * 11, length: 11), words[i]);
        }

        int csLength = bitLength / 32;
        int entropyLength = bitLength - csLength;
        int cs = bytes.GetBits((offset: entropyLength, length: csLength));
        byte[] entropy = bytes.AsSpan().Slice(0, entropyLength / 8).ToArray();
        byte[] sha256 = SHA256.HashData(entropy);
        int csCheck = sha256.GetBits((offset: 0, length: csLength));

        return cs != csCheck
            ? throw new ArgumentException("Invalid checksum.")
            : entropy;
    }

    /// <summary>
    /// Convert a byte array to a hex string.
    /// </summary>
    /// <param name="bytes">The bytes to convert.</param>
    /// <returns>The hex string.</returns>
    public static string ToHex(this byte[] bytes) => Convert.ToHexString(bytes);

    /// <summary>
    /// Convert a master secret byte array to a BIP-39 mnemonic string. See https://bips.dev/39/ .
    /// </summary>
    /// <param name="entropy">The bytes to convert.</param>
    /// <returns>The BIP 39 mnemonic string.</returns>
    public static string ToBip39(this byte[] entropy)
    {
        int csLength = entropy.Length * 8 / 32;
        byte[] sha256 = SHA256.HashData(entropy);
        byte[] bytes = entropy.ArrayConcat(sha256[..1]);
        int[] words = new int[((entropy.Length * 8) + csLength) / 11];
        for (int i = 0; i < words.Length; ++i)
        {
            words[i] = bytes.GetBits((offset: i * 11, length: 11));
        }
        return Mnemonic.ToBip39(words);
    }

    /// <summary>
    /// Get bits from a bit array, represented as a <see cref="T:byte[]"/>, where the bytes as well as the bits in the
    /// bytes are considered stored in big-endian order. This means that the most significant bit of the first byte is
    /// the first bit in the array, and the least significant bit of the last byte is the last bit. <seealso
    /// cref="SetBits(byte[], ValueTuple{int, int}, int)"/>
    /// </summary>
    /// <param name="bytes">The bytes holding the bits.</param>
    /// <param name="pos">The offset and length of the bit field to get.</param>
    /// <returns>An <see cref="int"/> that represents the bits.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <remarks>
    /// At most 31 bits can be gotten. The bits are returned in the 'length' least signficant bits of the result, with
    /// the first bit being the most significant.
    /// </remarks>
    public static int GetBits(this byte[] bytes, (int offset, int length) pos)
    {
        if (pos.length is < 1 or > 31)
        {
            throw new ArgumentOutOfRangeException(nameof(pos), "Count must be between 1 and 31.");
        }

        if (pos.offset < 0 || pos.offset + pos.length > bytes.Length * 8)
        {
            throw new ArgumentOutOfRangeException(nameof(pos), "Invalid offset and length range.");
        }

        int result = 0;
        for (int i = 0; i < pos.length; i++)
        {
            int byteIndex = (pos.offset + i) / 8;
            int bitIndex = 7 - ((pos.offset + i) % 8);

            if ((bytes[byteIndex] & (1 << bitIndex)) != 0)
            {
                result |= 1 << (pos.length - i - 1);
            }
        }
        return result;
    }

    /// <summary>
    /// Set bits in a bit array, represented as a <see cref="T:byte[]"/>, where the bytes as well as the bits in the
    /// bytes are considered stored in big-endian order. This means that the most significant bit of the first byte is
    /// the first bit in the array, and the least significant bit of the last byte is the last bit. <seealso
    /// cref="GetBits(byte[], ValueTuple{int, int})"/>
    /// </summary>
    /// <param name="bytes">The bytes holding the bits.</param>
    /// <param name="pos">The offset and length of the bit field to get.</param>
    /// <param name="value">The bits to set.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <remarks>
    /// At most 31 bits can be set. The bits are set from the 'length' least signficant bits of the value, with the
    /// most significant bit being the first.
    /// </remarks>
    public static void SetBits(this byte[] bytes, (int offset, int length) pos, int value)
    {
        if (pos.length is < 1 or > 31)
        {
            throw new ArgumentOutOfRangeException(nameof(pos), "Count must be between 1 and 31.");
        }

        if (pos.offset < 0 || pos.offset + pos.length > bytes.Length * 8)
        {
            throw new ArgumentOutOfRangeException(nameof(pos), "Invalid offset and length range.");
        }

        for (int i = 0; i < pos.length; i++)
        {
            int bitValue = (value >> (pos.length - i - 1)) & 1;
            int byteIndex = (pos.offset + i) / 8;
            int bitIndex = 7 - ((pos.offset + i) % 8);

            if (bitValue == 1)
            {
                bytes[byteIndex] |= (byte)(1 << bitIndex);
            }
            else
            {
                bytes[byteIndex] &= (byte)((1 << bitIndex) ^ 0xff);
            }
        }
    }
}
