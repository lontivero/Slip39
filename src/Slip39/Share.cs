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
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Slip39;

/// <summary>
/// A representation of a Shamir secret sharing share.
/// </summary>
public partial class Share
{
    internal Share(SharePrefix sharePrefix, byte[] value)
    {
        SharePrefix = sharePrefix;
        Value = value;
    }

    internal SharePrefix SharePrefix { get; }

    /// <summary>
    /// The actual share value, exactly as long as the master secret.
    /// </summary>
    internal byte[] Value { get; }

    private const int CHECKSUM_LENGTH_WORDS = 3;
    private const int PREFIX_LENGTH_WORDS = 4;
    private const int MIN_STRENGTH_BITS = 128;
    private const int MIN_MNEMONIC_LENGTH_WORDS = PREFIX_LENGTH_WORDS + CHECKSUM_LENGTH_WORDS + ((MIN_STRENGTH_BITS + 9) / 10);

    internal static int MinStrengthBits => MIN_STRENGTH_BITS;

    /// <summary>
    /// Parse a share string into a <see cref="Share"/> object.
    /// </summary>
    /// <param name="stringValue">The string to parse.</param>
    /// <returns>A <see cref="Share"/> object.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Share Parse(string stringValue)
    {
        if (MnemonicWordsPattern().IsMatch(stringValue))
        {
            return ParseSlip39MnemonicWords(stringValue);
        }
        if (UrlSafeBase64Pattern().IsMatch(stringValue))
        {
            byte[] bytes = stringValue.FromUrlSafeBase64();
            return ShareFromBytes(bytes);
        }
        if (HexPattern().IsMatch(stringValue))
        {
            byte[] bytes = stringValue.FromHex();
            return ShareFromBytes(bytes);
        }
        throw new ArgumentException("Invalid share format.");
    }

    private static Share ShareFromBytes(byte[] bytes)
    {
        int[] words = new int[bytes.Length * 8 / 10];
        for (int i = 0; i < words.Length; ++i)
        {
            words[i] = bytes.GetBits((offset: i * 10, length: 10));
        }
        return FromMnemonicIndices(words);
    }

    private static Share ParseSlip39MnemonicWords(string stringValue)
    {
        int[] words = Mnemonic.FromSlip39(stringValue);

        return words.Length < MIN_MNEMONIC_LENGTH_WORDS
            ? throw new ArgumentException($"Invalid mnemonic length. The length of each mnemonic must be at least {MIN_MNEMONIC_LENGTH_WORDS} words.")
            : FromMnemonicIndices(words);
    }

    private static Share FromMnemonicIndices(int[] words)
    {
        byte[] bytes = WordsToBytes(words);
        SharePrefix sharePrefix = new SharePrefix(bytes);

        if (Checksum(words, sharePrefix.Extendable) != 1)
        {
            throw new ArgumentException($"Invalid checksum.");
        }

        int valueWords = words.Length - ((SharePrefix.LengthBits / 10) + CHECKSUM_LENGTH_WORDS);
        int paddingLength = 10 * valueWords % 16;
        if (paddingLength > 8)
        {
            throw new ArgumentException("Invalid padding length.");
        }

        int padding = paddingLength > 0 ? bytes.GetBits((offset: SharePrefix.LengthBits, length: paddingLength)) : 0;
        if (padding != 0)
        {
            throw new ArgumentException("Invalid padding value, it should be all zeroes.");
        }

        byte[] value = new byte[valueWords * 10 / 8];
        int offset = SharePrefix.LengthBits + paddingLength;
        for (int i = 0; i < value.Length; ++i)
        {
            value[i] = (byte)bytes.GetBits((offset: offset + (i * 8), length: 8));
        }
        return new Share(sharePrefix, value);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        int[] words = ToWords();

        return Mnemonic.ToSlip39(words);
    }

    private int[] ToWords()
    {
        int valueWordCount = ((8 * Value.Length) + 9) / 10;
        int padding = (valueWordCount * 10) - (Value.Length * 8);

        byte[] paddedValue = new byte[((Value.Length * 8) + padding + 7) / 8];
        for (int i = 0; i < Value.Length; ++i)
        {
            paddedValue.SetBits((offset: padding + (i * 8), length: 8), Value[i]);
        }

        int[] words = BytesToWords(SharePrefix.PrefixValue)
            .ArrayConcat(BytesToWords(paddedValue)).ArrayConcat(new int[CHECKSUM_LENGTH_WORDS]);

        int chk = Checksum(words, SharePrefix.Extendable) ^ 1;
        int len = words.Length;
        for (int i = 0; i < CHECKSUM_LENGTH_WORDS; i++)
        {
            words[len - CHECKSUM_LENGTH_WORDS + i] = (chk >> (10 * (2 - i))) & 1023;
        }

        return words;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <param name="format">How the string should be formated. Empty and "G" produces a list of space separated
    /// mnemonic words. "64" produces an URL-safe base 64 representation. "X" produces a hex string.</param>
    /// <returns>The string representation.</returns>
    /// <exception cref="FormatException"></exception>
    public string ToString(string format)
    {
        if (format.Length == 0 || format == "G")
        {
            return ToString();
        }
        if (format == "64")
        {
            int[] words = ToWords();
            byte[] bytes = WordsToBytes(words);
            return bytes.ToUrlSafeBase64();
        }
        if (format == "X")
        {
            int[] words = ToWords();
            byte[] bytes = WordsToBytes(words);
            return Convert.ToHexString(bytes);
        }
        throw new FormatException($"Unknown format string: '{format}'.");
    }

    private static int[] BytesToWords(byte[] bytes)
    {
        int[] words = new int[bytes.Length * 8 / 10];
        for (int i = 0; i < words.Length; ++i)
        {
            words[i] = bytes.GetBits((offset: i * 10, length: 10));
        }
        return words;
    }

    private static byte[] WordsToBytes(int[] words)
    {
        byte[] bytes = new byte[((words.Length * 10) + 7) / 8];
        for (int i = 0; i < words.Length; ++i)
        {
            bytes.SetBits((offset: i * 10, length: 10), words[i]);
        }
        return bytes;
    }

    private static readonly byte[] CustomizationStringOrig = "shamir"u8.ToArray();

    private static readonly byte[] CustomizationStringExtendable = "shamir_extendable"u8.ToArray();

    private static int Checksum(int[] values, bool extendable)
    {
        int[] gen = [
            0x00E0E040, 0x01C1C080, 0x03838100, 0x07070200, 0x0E0E0009,
            0x1C0C2412, 0x38086C24, 0x3090FC48, 0x21B1F890, 0x03F3F120,
        ];

        int chk = 1;
        byte[] customizationString = extendable ? CustomizationStringExtendable : CustomizationStringOrig;
        foreach (int v in customizationString.Select(x => (int)x).Concat(values))
        {
            int b = chk >> 20;
            chk = ((chk & 0xFFFFF) << 10) ^ v;
            for (int i = 0; i < 10; i++)
            {
                chk ^= ((b >> i) & 1) != 0 ? gen[i] : 0;
            }
        }

        return chk;
    }

    [GeneratedRegex(@"\s*(\w+\s+){19,}(\w+\s+)*(\w+\s*)")]
    private static partial Regex MnemonicWordsPattern();

    [GeneratedRegex(@"^[A-Za-z0-9_-]{34,}$")]
    private static partial Regex UrlSafeBase64Pattern();

    [GeneratedRegex(@"^([A-Fa-f0-9][A-Fa-f0-9]){25,}$")]
    private static partial Regex HexPattern();
}
