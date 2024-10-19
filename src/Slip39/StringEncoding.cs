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
/// The master secret string encoding
/// </summary>
public enum StringEncoding
{
    /// <summary>
    /// The encoding is unknown. This is not a valid value.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// No encoding, the string is the raw value. Internally it's a UTF-8 string, with 0xFF padding if necessary.
    /// </summary>
    None,

    /// <summary>
    /// The secret is string encoded as a URL-safe base64 string.
    /// </summary>
    Base64,

    /// <summary>
    /// The secret is string encoded as a hexadecimal string.
    /// </summary>
    Hex,

    /// <summary>
    /// The secret is string encoded as a BIP-0039 mnemonic string.
    /// <see href="https://bips.dev/39/"/>.
    /// </summary>
    Bip39,
}
