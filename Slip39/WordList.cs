using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Slip39;

public static class WordList
{
    private static readonly string[] Wordlist;
    private static readonly Dictionary<string, ushort> WordIndexMap;

    static WordList()
    {
        Wordlist = LoadWordlist();
        WordIndexMap = Wordlist
            .Select((word, i) => new {word, i})
            .ToDictionary(x => x.word, x => (ushort)x.i);
    }

    private static string[] LoadWordlist()
    {
        var wordlistPath = Path.Combine(AppContext.BaseDirectory, "wordlist.txt");
        var wordlist = File.ReadAllLines(wordlistPath)
                           .Select(word => word.Trim())
                           .ToArray();

        if (wordlist.Length != 1024)
        {
            throw new InvalidOperationException(
                $"The wordlist should contain 1024 words, but it contains {wordlist.Length} words."
            );
        }

        return wordlist;
    }

    public static ushort[] MnemonicToIndices(string mnemonic)
    {
        try
        {
            return mnemonic.Split()
                           .Select(word => WordIndexMap[word.ToLower()])
                           .ToArray();
        }
        catch (KeyNotFoundException keyError)
        {
            throw new MnemonicException($"Invalid mnemonic word {keyError.Message}.", keyError);
        }
    }
}

public class MnemonicException : Exception
{
    public MnemonicException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
