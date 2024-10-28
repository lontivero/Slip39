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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Slip39;

/// <summary>
/// A representation of the share prefix, which consists of the parameters for the share.
/// </summary>
internal class SharePrefix
{
    private readonly byte[] _prefix;

    private static readonly List<(string name, int length)> _l = [
        (nameof(Id), 15),
        (nameof(Extendable), 1),
        (nameof(IterationExponent), 4),
        (nameof(GroupIndex), 4),
        (nameof(GroupThreshold), 4),
        (nameof(GroupCount), 4),
        (nameof(MemberIndex), 4),
        (nameof(MemberThreshold), 4),
    ];

    /// <summary>
    /// The total length in bits of the share prefix.
    /// </summary>
    public static readonly int LengthBits = _l.Sum(m => m.length);

    /// <summary>
    /// Generate a random id for the prefix.
    /// </summary>
    /// <param name="random">The <see cref="IRandom"/> instance to use.</param>
    /// <returns>An integer suitable as a random id for a share.</returns>
    public static int GenerateId(IRandom random) =>
        BitConverter.ToInt32(random.GetBytes(4)) % ((1 << (Pos(nameof(Id)).length + 1)) - 1);

    private static readonly int _lengthBytes = (LengthBits + 7) / 8;

    /// <summary>
    /// Instantiate an instance of <see cref="SharePrefix"/> using a provided set of parameters.
    /// </summary>
    /// <param name="parameters">The <see cref="ShareParameters"/> to use.</param>
    public SharePrefix(ShareParameters parameters)
    {
        _prefix = new byte[_lengthBytes];

        _prefix.SetBits(Pos(nameof(Id)), parameters.Id);
        _prefix.SetBits(Pos(nameof(Extendable)), parameters.Extendable ? 1 : 0);
        _prefix.SetBits(Pos(nameof(IterationExponent)), parameters.IterationExponent);
        _prefix.SetBits(Pos(nameof(GroupIndex)), parameters.GroupIndex);
        _prefix.SetBits(Pos(nameof(GroupThreshold)), parameters.GroupThreshold - 1);
        _prefix.SetBits(Pos(nameof(GroupCount)), parameters.GroupCount - 1);
        _prefix.SetBits(Pos(nameof(MemberIndex)), parameters.MemberIndex);
        _prefix.SetBits(Pos(nameof(MemberThreshold)), parameters.MemberThreshold - 1);
    }

    /// <summary>
    /// Instantiate a <see cref="SharePrefix"/> from a binary representation.
    /// </summary>
    /// <param name="prefix">A <see cref="T:byte[]"/> containing the binary prefix.</param>
    public SharePrefix(byte[] prefix)
    {
        _prefix = prefix;
    }

    /// <summary>
    /// The identifier (id) field is a random 15-bit value which is the same for all shares and is used to verify that
    /// the shares belong together.
    /// </summary>
    public int Id => _prefix.GetBits(Pos(nameof(Id)));

    /// <summary>
    /// The extendable backup flag (ext) field indicates that the id is used as
    /// salt in the encryption of the master secret when ext = 0.
    /// </summary>
    public bool Extendable => _prefix.GetBits(Pos(nameof(Extendable))) == 1;

    /// <summary>
    /// The iteration exponent (e) field indicates the total number of iterations to be used in PBKDF2. The number of
    /// iterations is calculated as 10000×2^e.
    /// </summary>
    public int IterationExponent => _prefix.GetBits(Pos(nameof(IterationExponent)));

    /// <summary>
    /// The group index (GI) field is the x value of the group share.
    /// </summary>
    public int GroupIndex => _prefix.GetBits(Pos(nameof(GroupIndex)));

    /// <summary>
    /// The group threshold (Gt) field indicates how many group shares are needed to reconstruct the master secret. The
    /// actual value is encoded as Gt = GT − 1, so a value of 0 indicates that a single group share is needed (GT = 1),
    /// a value of 1 indicates that two group shares are needed (GT = 2) etc.
    /// </summary>
    public int GroupThreshold => _prefix.GetBits(Pos(nameof(GroupThreshold))) + 1;

    /// <summary>
    /// The group count (g) indicates the total number of groups. The actual value is encoded as g = G − 1.
    /// </summary>
    public int GroupCount => _prefix.GetBits(Pos(nameof(GroupCount))) + 1;

    /// <summary>
    /// The member index (I) field is the x value of the member share in the given group.
    /// </summary>
    public int MemberIndex => _prefix.GetBits(Pos(nameof(MemberIndex)));

    /// <summary>
    /// The member threshold (t) field indicates how many member shares are needed to reconstruct the group share. The
    /// actual value is encoded as t = T − 1.
    /// </summary>
    public int MemberThreshold => _prefix.GetBits(Pos(nameof(MemberThreshold))) + 1;

    /// <summary>
    /// The binary representation of the share prefix.
    /// </summary>
    public byte[] PrefixValue => _prefix[.. _lengthBytes];

    private static (int offset, int length) Pos(string name)
    {
        int index = _l.Select((m, i) => new { i, m.name }).First(x => x.name == name).i;
        return (offset: _l.Take(index).Sum(x => x.length), _l[index].length);
    }
}
