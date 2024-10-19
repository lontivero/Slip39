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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Slip39.Test.Properties;

using Xunit;

namespace Slip39.Test;

public class TestSlip39Vectors
{
    [Theory]
    [MemberData(nameof(Slip39TestVector.TestCasesData), MemberType = typeof(Slip39TestVector))]
    public void TestAllVectors(Slip39TestVector test)
    {
        ShamirsSecretSharing sss = new(new FakeRandom());
        if (!string.IsNullOrEmpty(test.SecretHex))
        {
            Share[] shares = test.Mnemonics.Select((m) =>
            {
                if (!m.TryParse(out Share share))
                {
                    Assert.Fail($"Failed to parse mnemonic \"{m}\".");
                }
                return share;
            }).ToArray();
            byte[] secret = sss.CombineShares(shares, "TREZOR");
            Assert.Equal(test.SecretHex, Convert.ToHexString(secret).ToLower());

            //Assert.Equal(new BIP32Key(secret).ExtendedKey(), xprv);
        }
        else
        {
            Assert.Throws<ArgumentException>((Action)(() =>
            {
                Share[] shares = test.Mnemonics.Select((m) =>
                {
                    return !m.TryParse(out Share share)
                        ? throw new ArgumentException($"Failed to parse mnemonic \"{m}\".")
                        : share;
                }).ToArray();
                sss.CombineShares((Share[])shares, string.Empty);
                Assert.Fail($"Failed to raise exception for test vector \"{test.Description}\".");
            }));
        }
    }
}

public record Slip39TestVector(string Description, string[] Mnemonics, string SecretHex, string Xprv)
{
    private static IEnumerable<Slip39TestVector> VectorsData()
    {
        string vectorsJson = Resources.slip39vectors_json;
        IEnumerable<object[]> vectors = JsonConvert.DeserializeObject<IEnumerable<object[]>>(vectorsJson)
            ?? throw new InvalidOperationException("Deserialization of 'vectors.json' failed.");
        foreach (object[] x in vectors)
        {
            yield return new(
                Description: (string)x[0],
                Mnemonics: ((JArray)x[1]).Values<string>().Cast<string>().ToArray(),
                SecretHex: (string)x[2],
                Xprv: (string)x[3]
            );
        }
    }

    private static readonly Slip39TestVector[] TestCases = VectorsData().ToArray();

    public static TheoryData<Slip39TestVector> TestCasesData => new TheoryData<Slip39TestVector>(TestCases);

    public override string ToString() => Description;
}
