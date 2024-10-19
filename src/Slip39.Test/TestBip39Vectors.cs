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

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Xunit;
using Slip39.Test.Properties;

namespace Slip39.Test;

/// <summary>
/// See https://bips.dev/39/
/// </summary>
public class TestBip39Vectors
{
    [Theory]
    [MemberData(nameof(Bip39TestVector.TestCasesData), MemberType = typeof(Bip39TestVector))]
    public void TestAllVectors(Bip39TestVector test)
    {
        if (!string.IsNullOrEmpty(test.SecretHex))
        {
            byte[] secret = test.SecretHex.FromHex();
            string mnemonic = secret.ToBip39();
            Assert.Equal(test.Mnemonics, mnemonic.Split());

            mnemonic = string.Join(" ", test.Mnemonics);
            secret = mnemonic.FromBip39();
            Assert.Equal(test.SecretHex.ToUpper(), secret.ToHex());
        }
    }
}

public record Bip39TestVector(string SecretHex, string[] Mnemonics, string Seed,  string Xprv)
{
    private static IEnumerable<Bip39TestVector> VectorsData()
    {
        string vectorsJson = Resources.bip39vectors_json;
        IEnumerable<object[]> vectors = JsonConvert.DeserializeObject<IEnumerable<object[]>>(vectorsJson)
            ?? throw new InvalidOperationException("Deserialization of 'vectors.json' failed.");
        foreach (object[] x in vectors)
        {
            yield return new(
                SecretHex: (string)x[0],
                Mnemonics: ((string)x[1]).Split(),
                Seed: (string)x[2],
                Xprv: (string)x[3]
            );
        }
    }

    private static readonly Bip39TestVector[] TestCases = VectorsData().ToArray();

    public static TheoryData<Bip39TestVector> TestCasesData => new TheoryData<Bip39TestVector>(TestCases);
}
