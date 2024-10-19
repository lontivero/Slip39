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

using System.Security.Cryptography;

namespace Slip39;

/// <summary>
/// An <see cref="IRandom"/> implementation using <see cref="RandomNumberGenerator"/>.
/// </summary>
public class StrongRandom : IRandom
{
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    /// <inheritdoc />
    public void GetBytes(byte[] buffer)
    {
        _rng.GetBytes(buffer);
    }

    /// <inheritdoc />
    public byte[] GetBytes(int count)
    {
        byte[] bytes = new byte[count];
        _rng.GetBytes(bytes);
        return bytes;
    }
}
