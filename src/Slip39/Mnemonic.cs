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

using Slip39.Properties;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Slip39;

internal static class Mnemonic
{
    private static readonly string[] _slip39WordList = LoadWordlist(Resources.Slip39WordList, 1024);

    /// <summary>
    /// See https://bips.dev/39/ .
    /// </summary>
    private static readonly string[] _bip39WordList = LoadWordlist(Resources.Bip39WordList, 2048);

    private static readonly Dictionary<string, int> _slip39WordMap = _slip39WordList
            .Select((word, i) => (word, i)).ToDictionary(t => t.word, t => t.i);

    private static readonly Dictionary<string, int> _bip39WordMap = _bip39WordList
            .Select((word, i) => (word, i)).ToDictionary(t => t.word, t => t.i);

    public static string ToSlip39(IEnumerable<int> words) => string.Join(" ", words.Select(i => _slip39WordList[i]));

    public static string ToBip39(IEnumerable<int> words) => string.Join(" ", words.Select(i => _bip39WordList[i]));

    public static int[] FromSlip39(string mnemonic) => ToIndices(mnemonic, _slip39WordMap);

    public static int[] FromBip39(string mnemonic) => ToIndices(mnemonic, _bip39WordMap);

    private static int[] ToIndices(string mnemonic, Dictionary<string, int> wordMap)
    {
        try
        {
            return mnemonic.Split()
                           .Select(word => wordMap[word.ToLower()])
                           .ToArray();
        }
        catch (KeyNotFoundException keyError)
        {
            throw new Slip39Exception($"Invalid mnemonic word '{keyError.Message}'.");
        }
    }

    private static string[] LoadWordlist(string words, int size)
    {
        string[] wordlist = words.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);

        return wordlist.Length == size
            ? wordlist
            : throw new InvalidOperationException($"The wordlist should contain 1024 words, but it contains {wordlist.Length} words.");
    }
}
