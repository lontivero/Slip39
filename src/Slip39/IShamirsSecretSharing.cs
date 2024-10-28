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

namespace Slip39;

/// <summary>
/// Implement Shamirs Secret Sharing according to <seealso
/// href="https://github.com/satoshilabs/slips/blob/master/slip-0039.md">
/// SLIP-039</seealso>.
/// </summary>
public interface IShamirsSecretSharing
{
    /// <summary>
    /// Generate shares according to the given parameters.
    /// </summary>
    /// <param name="extendable">Indicates that the id is used as salt in the encryption of the master secret when
    /// false.</param>
    /// <param name="iterationExponent">The total number of iterations to be used in PBKDF2, calculated as
    /// 10000×2^e.</param>
    /// <param name="groupThreshold">The number of group shares needed to reconstruct the master secret.</param>
    /// <param name="groups">The group definitions as a <see cref="T:Group[]"/>.</param>
    /// <param name="passphrase">The (optional) passphrase used to encrypt the master secret.</param>
    /// <param name="masterSecret">The master secret, at least 128 bits and a multiple of 16 bits.</param>
    /// <returns>A <see cref="T:Share[]"/> with shares that can be distributed according to the threshold
    /// parameters.</returns>
    Share[] GenerateShares(bool extendable, int iterationExponent, int groupThreshold, Group[] groups,
        string passphrase, byte[] masterSecret);

    /// <summary>
    /// Recover the master secret from an appropriate set of shares.
    /// </summary>
    /// <param name="shares">The shares to recover the secret from.</param>
    /// <param name="passphrase">The (optional) passphrase to decrypt the encrypted master secret with.</param>
    /// <returns>The original master secret.</returns>
    byte[] CombineShares(Share[] shares, string passphrase);
}
