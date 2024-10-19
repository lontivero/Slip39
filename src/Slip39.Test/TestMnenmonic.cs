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

using Xunit;

namespace Slip39.Test;

public class TestMnenmonic
{
    private static readonly byte[] _masterSecret = "ABCDEFGHIJKLMNOP"u8.ToArray();

    private static readonly string[] mnemonics =
    [
        "vocal again academic acne both insect modern making forbid grief flavor faint intimate senior priority satoshi aunt screw finance silent",
        "vocal again academic agree alive race acne husky priority skunk salt device taught hearing mama scout marvel daisy justice wits",
        "vocal again academic amazing ambition window equip paid amuse knife family intimate yoga destroy greatest retreat step finance funding client",
        "vocal again academic arcade breathe domain style greatest work spend secret believe hamster museum elephant render forward reunion hush benefit",
        "vocal again academic axle careful cylinder impact parking shrimp ancient forget element domestic package flavor morning glimpse visual says device"
    ];

    [Fact]
    public void TestGenerateMnemonics()
    {
        ShamirsSecretSharing sss = new(new FakeRandom());
        Share[] shares = sss.GenerateShares(true, 0, 1, [new Group(3, 5)], string.Empty, _masterSecret);
        Assert.Equal(mnemonics.Length, shares.Length);
        for (int i = 0; i < shares.Length; ++i)
        {
            Assert.Equal(mnemonics[i], shares[i].ToString());
        }

        for (int i = 0; i < shares.Length; ++i)
        {
            if (!mnemonics[i].TryParse(out Share share))
            {
                Assert.Fail("Failed to parse mnemonic");
            }
            Assert.True(share.Value.SequenceEqual(shares[i].Value));
        }
    }
}
