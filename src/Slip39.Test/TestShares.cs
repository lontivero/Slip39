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

using Xunit;

namespace Slip39.Test;

public class TestShares
{
    private static readonly byte[] MS = "ABCDEFGHIJKLMNOP"u8.ToArray();

    [Fact]
    public void TestBasicSharingRandom()
    {
        FakeRandom random = new FakeRandom();
        byte[] secret = new byte[16];
        random.GetBytes(secret);
        ShamirsSecretSharing sss = new ShamirsSecretSharing(random);
        Share[] shares = sss.GenerateShares(true, 0, 1, [new Group(3, 5)], string.Empty, secret);
        Assert.Equal(
            sss.CombineShares(shares[..3], string.Empty),
            sss.CombineShares(shares[2..], string.Empty)
        );
    }

    [Fact]
    public void TestBasicSharingFixed()
    {
        ShamirsSecretSharing sss = new ShamirsSecretSharing(new FakeRandom());
        Share[] shares = sss.GenerateShares(true, 0, 1, [new Group(3, 5)], string.Empty, MS);
        Assert.Equal(MS, sss.CombineShares(shares[..3], string.Empty));
        Assert.Equal(MS, sss.CombineShares(shares[1..4], string.Empty));
        Assert.Throws<ArgumentException>(() =>
            sss.CombineShares(shares[..2], string.Empty)
        );
    }

    [Fact]
    public void TestPassphrase()
    {
        ShamirsSecretSharing sss = new ShamirsSecretSharing(new FakeRandom());
        Share[] shares = sss.GenerateShares(true, 0, 1, [new Group(3, 5)], "TREZOR", MS);
        Assert.Equal(MS, sss.CombineShares(shares[1..4], "TREZOR"));
        Assert.NotEqual(MS, sss.CombineShares(shares[1..4], string.Empty));
    }

    [Fact]
    public void TestNonExtendable()
    {
        ShamirsSecretSharing sss = new ShamirsSecretSharing(new FakeRandom());
        Share[] shares = sss.GenerateShares(false, 0, 1, [new Group(3, 5)], string.Empty, MS);
        Assert.Equal(MS, sss.CombineShares(shares[1..4], string.Empty));
    }

    [Fact]
    public void TestIterationExponent()
    {
        ShamirsSecretSharing sss = new ShamirsSecretSharing(new FakeRandom());
        Share[] shares = sss.GenerateShares(true, iterationExponent: 1, 1, [new Group(3, 5)], "TREZOR", MS);
        Assert.Equal(MS, sss.CombineShares(shares[1..4], "TREZOR"));
        Assert.NotEqual(MS, sss.CombineShares(shares[1..4], string.Empty));

        shares = sss.GenerateShares(true, iterationExponent: 2, 1, [new Group(3, 5)], "TREZOR", MS);
        Assert.Equal(MS, sss.CombineShares(shares[1..4], "TREZOR"));
        Assert.NotEqual(MS, sss.CombineShares(shares[1..4], string.Empty));
    }

    [Fact]
    public void TestGroupSharing()
    {
        int groupThreshold = 2;
        Group[] groups = [new Group(3, 5), new Group(2, 3), new Group(2, 5), new Group(1, 1),];
        ShamirsSecretSharing sss = new ShamirsSecretSharing(new FakeRandom());
        Share[] shares = sss.GenerateShares(true, 0, groupThreshold, groups, string.Empty, MS);
        Share[][] shareGroupings = shares.GroupBy(x => x.SharePrefix.GroupIndex).Select(x => x.ToArray()).ToArray();

        // Test all valid combinations of mnemonics.
        foreach ((Share[] shares, int memberThreshold)[] combinations in
            Combinations(shareGroupings.Zip(groups.Select(g => g.GroupThreshold)), groupThreshold))
        {
            foreach (Share[] group1Subset in Combinations(combinations[0].shares, combinations[0].memberThreshold))
            {
                foreach (Share[] group2Subset in Combinations(combinations[1].shares, combinations[1].memberThreshold))
                {
                    Share[] shareSubset = group1Subset.ArrayConcat(group2Subset);
                    shareSubset = [.. shareSubset.OrderBy(x => Guid.NewGuid())];
                    Assert.Equal(MS, sss.CombineShares(shareSubset, string.Empty));
                }
            }
        }

        Assert.Equal(MS, sss.CombineShares([shareGroupings[2][0], shareGroupings[2][2], shareGroupings[3][0]], string.Empty));
        Assert.Equal(MS, sss.CombineShares([shareGroupings[2][3], shareGroupings[3][0], shareGroupings[2][4]], string.Empty));

        Assert.Throws<ArgumentException>(() =>
            sss.CombineShares(shareGroupings[0][2..].ArrayConcat(shareGroupings[1][..1]), string.Empty)
        );

        Assert.Throws<ArgumentException>(() =>
            sss.CombineShares(shareGroupings[0][1..4], string.Empty)
        );
    }

    [Fact]
    public void TestGroupSharingThreshold1()
    {
        int groupThreshold = 1;
        int[] groupSizes = [5, 3, 5, 1];
        int[] memberThresholds = [3, 2, 2, 1];

        Group[] groups = [new Group(3, 5), new Group(2, 3), new Group(2, 5), new Group(1, 1),];
        ShamirsSecretSharing sss = new ShamirsSecretSharing(new FakeRandom());
        Share[] shares = sss.GenerateShares(true, 0, groupThreshold, groups, string.Empty, MS);
        Share[][] shareGroupings = shares.GroupBy(x => x.SharePrefix.GroupIndex).Select(x => x.ToArray()).ToArray();

        foreach ((Share[] groupShares, int memberThreshold) in shareGroupings.Zip(groups.Select(g => g.GroupThreshold)))
        {
            foreach (Share[] groupSubset in Combinations(groupShares, memberThreshold))
            {
                Share[] shareSubset = [.. groupSubset.OrderBy(_ => Guid.NewGuid())];
                Assert.Equal(MS, sss.CombineShares(shareSubset, string.Empty));
            }
        }
    }

    [Fact]
    public void TestAllGroupsExist()
    {
        ShamirsSecretSharing sss = new ShamirsSecretSharing(new FakeRandom());

        Group[] groups = [new Group(3, 5), new Group(1, 1), new Group(2, 3), new Group(2, 5), new Group(3, 5),];

        foreach (int groupThreshold in new int[] { 1, 2, 5 })
        {
            Share[] shares = sss.GenerateShares(true, 0, groupThreshold, groups, string.Empty, MS);
            Share[][] shareGroupings = shares.GroupBy(x => x.SharePrefix.GroupIndex).Select(x => x.ToArray()).ToArray();
            Assert.Equal(5, shareGroupings.Length);
            Assert.Equal(19, shareGroupings.Sum(g => g.Length));
        }
    }

    [Fact]
    public void TestInvalidSharing()
    {
        ShamirsSecretSharing sss = new ShamirsSecretSharing(new FakeRandom());

        Assert.Throws<ArgumentException>(() =>
            sss.GenerateShares(true, 0, 1, [new Group(2, 3)], string.Empty, MS.Take(14).ToArray())
        );

        Assert.Throws<ArgumentException>(() =>
            sss.GenerateShares(true, 0, 1, [new Group(2, 3)], string.Empty, [.. MS, .. "X"u8.ToArray()])
        );

        Assert.Throws<ArgumentException>(() =>
            sss.GenerateShares(true, 0, 3, [new Group(3, 5), new Group(2, 5)], string.Empty, MS)
        );

        Assert.Throws<ArgumentException>(() =>
            sss.GenerateShares(true, 0, 0, [new Group(3, 5), new Group(2, 5)], string.Empty, MS)
        );

        Assert.Throws<ArgumentException>(() =>
            sss.GenerateShares(true, 0, 2, [new Group(3, 2), new Group(2, 5)], string.Empty, MS)
        );

        Assert.Throws<ArgumentException>(() =>
            sss.GenerateShares(true, 0, 2, [new Group(0, 2), new Group(2, 5)], string.Empty, MS)
        );

        Assert.Throws<ArgumentException>(() =>
            sss.GenerateShares(true, 0, 2, [new Group(3, 5), new Group(1, 3), new Group(2, 5)], string.Empty, MS)
        );
    }

    private static T[][] Combinations<T>(IEnumerable<T> iterable, int r)
    {
        IEnumerable<IEnumerable<T>> InternalCombinations()
        {
            T[] pool = iterable.ToArray();
            int n = pool.Length;
            if (r > n) yield break;

            int[] indices = Enumerable.Range(0, r).ToArray();

            yield return indices.Select(i => pool[i]);

            while (true)
            {
                int i;
                for (i = r - 1; i >= 0; i--)
                {
                    if (indices[i] != i + n - r)
                    {
                        break;
                    }
                }

                if (i < 0) yield break;

                indices[i]++;
                for (int j = i + 1; j < r; j++)
                {
                    indices[j] = indices[j - 1] + 1;
                }

                yield return indices.Select(index => pool[index]);
            }
        }

        return InternalCombinations().Select(x => x.ToArray()).ToArray();
    }
}
