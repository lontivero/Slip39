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

using Slip39;
using Xunit;

namespace Slip39.Test;

/// <summary>
/// See https://bips.dev/39/
/// </summary>
public class TestInterop
{
    [Fact]
    public void TestSlip39MnemonicCards()
    {
        // Data taken from using https://github.com/pjkundert/python-slip39

        const string secret = "b036a46abc14d26d5f34f63128bc5b2e";
        string[] shares =
        [
            "august warmth acid acid desire regular scene luck umbrella debut prune retreat acne silent fortune paper pancake ruler envy manual",
            "august warmth acid agency believe ancient acid national review bike crowd academic laundry reunion retreat adapt pipeline together trust total",
        ];

        ShamirsSecretSharing sss = new ShamirsSecretSharing(new FakeRandom());
        byte[] bytes = sss.CombineShares(shares.Select(Share.Parse).ToArray());

        Assert.Equal(secret.ToUpper(), bytes.ToHex());   
    }

    [Fact]
    public void TestBip39Mnemonic()
    {
        const string hex = "b036a46abc14d26d5f34f63128bc5b2e";
        const string bip39 = "rabbit release boy join essay cute lamp paddle couple echo bitter fresh";

        byte[] bytes = bip39.FromBip39();
        Assert.Equal(hex.ToUpper(), bytes.ToHex());

        string mnemonic = bytes.ToBip39();
        Assert.Equal(bip39, mnemonic);
    }
}
