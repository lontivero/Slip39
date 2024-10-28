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

using System.CommandLine;
using System.Diagnostics;

namespace Slip39.Cli;

internal class App(IShamirsSecretSharing sss)
{
    public async Task<int> Run(string[] args)
    {
        StrongRandom random = new();

        Option<bool> debugOption = new(
            name: "--debug",
            description: "Perform a debug break.");

        Option<StringEncoding> formatOption = new(
            name: "--format",
            description: "Specify the format of the secret.");

        Option<int> countOption = new(
           name: "--count",
           description: "The number of shares to generate.");

        Option<int> thresholdOption = new(
            name: "--threshold",
            description: "The number of shares required to recover the master secret.");

        Option<string> secretOption = new(
            name: "--secret",
            description: "The secret to split into shares.");

        Command splitCommand = new(name: "split", description: "Split the secret into shares")
        {
            countOption,
            thresholdOption,
            secretOption,
        };

        splitCommand.SetHandler((threshold, count, secret, format) =>
        {
            Share[] shares = sss.GenerateShares(threshold, count, secret, format);
            foreach (Share share in shares)
            {
                Console.WriteLine(share);
            }
        }, thresholdOption, countOption, secretOption, formatOption);

        Option<string[]> shareOption = new(
        name: "--share",
        description: "Combine this and any additional shares to recover the master secret.")
        {
            AllowMultipleArgumentsPerToken = true,
            IsRequired = true,
        };

        Command combineCommand = new(name: "combine", description: "Combine the given number of shares and recover the secret.")
        {
            shareOption,
        };

        combineCommand.SetHandler((sh, format, debug) =>
        {
            if (debug)
            {
                Debugger.Launch();
            }

            byte[] masterSecret = sss.CombineShares(sh.Select(sh => Share.Parse(sh)).ToArray());
            string secret = format switch
            {
                StringEncoding.Base64 => masterSecret.ToUrlSafeBase64(),
                StringEncoding.Hex => masterSecret.ToHex(),
                StringEncoding.Bip39 => masterSecret.ToBip39(),
                StringEncoding.None => masterSecret.ToSecretString(),
                _ => throw new NotSupportedException("This format is not supported."),
            };
            Console.WriteLine(secret);
        }, shareOption, formatOption, debugOption);

        RootCommand rootCommand = new("Simple app to generate Shamir secret shares and combine them again.")
        {
            splitCommand,
            combineCommand,
        };
        rootCommand.AddGlobalOption(debugOption);
        rootCommand.AddGlobalOption(formatOption);

        return await rootCommand.InvokeAsync(args);
    }
}
