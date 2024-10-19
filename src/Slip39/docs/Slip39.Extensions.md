#### [Slip39](index.md 'index')
### [Slip39](Slip39.md 'Slip39')

## Extensions Class

A set of useful extensions.

```csharp
public static class Extensions
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; Extensions
### Methods

<a name='Slip39.Extensions.ArrayConcat_T_(thisT,T[])'></a>

## Extensions.ArrayConcat<T>(this T, T[]) Method

A convenience method to contatenate a first element with an array.

```csharp
public static T[] ArrayConcat<T>(this T first, T[] second);
```
#### Type parameters

<a name='Slip39.Extensions.ArrayConcat_T_(thisT,T[]).T'></a>

`T`

The type of the elements.
#### Parameters

<a name='Slip39.Extensions.ArrayConcat_T_(thisT,T[]).first'></a>

`first` [T](Slip39.Extensions.md#Slip39.Extensions.ArrayConcat_T_(thisT,T[]).T 'Slip39.Extensions.ArrayConcat<T>(this T, T[]).T')

The first element.

<a name='Slip39.Extensions.ArrayConcat_T_(thisT,T[]).second'></a>

`second` [T](Slip39.Extensions.md#Slip39.Extensions.ArrayConcat_T_(thisT,T[]).T 'Slip39.Extensions.ArrayConcat<T>(this T, T[]).T')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The second array.

#### Returns
[T](Slip39.Extensions.md#Slip39.Extensions.ArrayConcat_T_(thisT,T[]).T 'Slip39.Extensions.ArrayConcat<T>(this T, T[]).T')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
A third array consisting of the concatenated result.

<a name='Slip39.Extensions.ArrayConcat_T_(thisT[],T[])'></a>

## Extensions.ArrayConcat<T>(this T[], T[]) Method

Contatenate two arrays.

```csharp
public static T[] ArrayConcat<T>(this T[] first, T[] second);
```
#### Type parameters

<a name='Slip39.Extensions.ArrayConcat_T_(thisT[],T[]).T'></a>

`T`

The type of the elements.
#### Parameters

<a name='Slip39.Extensions.ArrayConcat_T_(thisT[],T[]).first'></a>

`first` [T](Slip39.Extensions.md#Slip39.Extensions.ArrayConcat_T_(thisT[],T[]).T 'Slip39.Extensions.ArrayConcat<T>(this T[], T[]).T')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The first array.

<a name='Slip39.Extensions.ArrayConcat_T_(thisT[],T[]).second'></a>

`second` [T](Slip39.Extensions.md#Slip39.Extensions.ArrayConcat_T_(thisT[],T[]).T 'Slip39.Extensions.ArrayConcat<T>(this T[], T[]).T')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The second array.

#### Returns
[T](Slip39.Extensions.md#Slip39.Extensions.ArrayConcat_T_(thisT[],T[]).T 'Slip39.Extensions.ArrayConcat<T>(this T[], T[]).T')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
A third array consisting of the concatenated result.

<a name='Slip39.Extensions.CombineShares(thisSlip39.IShamirsSecretSharing,Slip39.Share[])'></a>

## Extensions.CombineShares(this IShamirsSecretSharing, Share[]) Method

Convenience method to recover a master secret from a set of shares, without a password.

```csharp
public static byte[] CombineShares(this Slip39.IShamirsSecretSharing sss, Slip39.Share[] shares);
```
#### Parameters

<a name='Slip39.Extensions.CombineShares(thisSlip39.IShamirsSecretSharing,Slip39.Share[]).sss'></a>

`sss` [IShamirsSecretSharing](Slip39.md#Slip39.IShamirsSecretSharing 'Slip39.IShamirsSecretSharing')

The [IShamirsSecretSharing](Slip39.md#Slip39.IShamirsSecretSharing 'Slip39.IShamirsSecretSharing') instance.

<a name='Slip39.Extensions.CombineShares(thisSlip39.IShamirsSecretSharing,Slip39.Share[]).shares'></a>

`shares` [Share](Slip39.Share.md 'Slip39.Share')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The shares to to recover from.

#### Returns
[System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
The recovered master secret.

<a name='Slip39.Extensions.FromBip39(thisstring)'></a>

## Extensions.FromBip39(this string) Method

Convert a bip 39 mnemonic string master secret to a byte array. See https://bips.dev/39/ .

```csharp
public static byte[] FromBip39(this string mnemonic);
```
#### Parameters

<a name='Slip39.Extensions.FromBip39(thisstring).mnemonic'></a>

`mnemonic` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The list of mnemonic words.

#### Returns
[System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
The converted bytes.

<a name='Slip39.Extensions.FromHex(thisstring)'></a>

## Extensions.FromHex(this string) Method

Convert a hex string to a byte array.

```csharp
public static byte[] FromHex(this string hex);
```
#### Parameters

<a name='Slip39.Extensions.FromHex(thisstring).hex'></a>

`hex` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The string to convert.

#### Returns
[System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
The converted bytes.

<a name='Slip39.Extensions.FromUrlSafeBase64(thisstring)'></a>

## Extensions.FromUrlSafeBase64(this string) Method

Convert a URL-safe base64 string to a byte array.

```csharp
public static byte[] FromUrlSafeBase64(this string base64);
```
#### Parameters

<a name='Slip39.Extensions.FromUrlSafeBase64(thisstring).base64'></a>

`base64` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The string to convert.

#### Returns
[System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
The converted bytes.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,byte[])'></a>

## Extensions.GenerateShares(this IShamirsSecretSharing, int, int, byte[]) Method

Convenience method to generate shares for the simple case of a single group of shares with a single threshold,  
and no encryption.

```csharp
public static Slip39.Share[] GenerateShares(this Slip39.IShamirsSecretSharing sss, int memberThreshold, int memberCount, byte[] masterSecret);
```
#### Parameters

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,byte[]).sss'></a>

`sss` [IShamirsSecretSharing](Slip39.md#Slip39.IShamirsSecretSharing 'Slip39.IShamirsSecretSharing')

The [IShamirsSecretSharing](Slip39.md#Slip39.IShamirsSecretSharing 'Slip39.IShamirsSecretSharing') instance.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,byte[]).memberThreshold'></a>

`memberThreshold` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

The member threshold.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,byte[]).memberCount'></a>

`memberCount` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

The total number of members.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,byte[]).masterSecret'></a>

`masterSecret` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The master secret to share.

#### Returns
[Share](Slip39.Share.md 'Slip39.Share')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
The set of shares, of which [memberThreshold](Slip39.Extensions.md#Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,byte[]).memberThreshold 'Slip39.Extensions.GenerateShares(this Slip39.IShamirsSecretSharing, int, int, byte[]).memberThreshold') are required to recover the  
            secret.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string)'></a>

## Extensions.GenerateShares(this IShamirsSecretSharing, int, int, string) Method

Convenience method to generate shares for the simple case of a single group of shares with a single threshold,  
and no encryption.

```csharp
public static Slip39.Share[] GenerateShares(this Slip39.IShamirsSecretSharing sss, int memberThreshold, int memberCount, string masterSecret);
```
#### Parameters

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string).sss'></a>

`sss` [IShamirsSecretSharing](Slip39.md#Slip39.IShamirsSecretSharing 'Slip39.IShamirsSecretSharing')

The [IShamirsSecretSharing](Slip39.md#Slip39.IShamirsSecretSharing 'Slip39.IShamirsSecretSharing') instance.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string).memberThreshold'></a>

`memberThreshold` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

The member threshold.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string).memberCount'></a>

`memberCount` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

The total number of members.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string).masterSecret'></a>

`masterSecret` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The master secret to share as a string.

#### Returns
[Share](Slip39.Share.md 'Slip39.Share')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
The set of shares, of which [memberThreshold](Slip39.Extensions.md#Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string).memberThreshold 'Slip39.Extensions.GenerateShares(this Slip39.IShamirsSecretSharing, int, int, string).memberThreshold') are required to recover the  
            secret.

### Remarks
The master secret is encoded as a [byte[]](https://docs.microsoft.com/en-us/dotnet/api/byte[] 'byte[]') of UTF-8 encoded bytes, and if required padded with 0xff  
to fulfil the requirements. When recovering the secret, any trailing 0xff bytes thus need to be trimmed before  
producing the original master secret string, <seealso cref="M:Slip39.Extensions.ToSecretString(System.Byte[])"/>.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string,Slip39.StringEncoding)'></a>

## Extensions.GenerateShares(this IShamirsSecretSharing, int, int, string, StringEncoding) Method

Convenience method to generate shares for the simple case of a single group of shares with a single threshold,  
and no encryption.

```csharp
public static Slip39.Share[] GenerateShares(this Slip39.IShamirsSecretSharing sss, int memberThreshold, int memberCount, string masterSecret, Slip39.StringEncoding encoding);
```
#### Parameters

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string,Slip39.StringEncoding).sss'></a>

`sss` [IShamirsSecretSharing](Slip39.md#Slip39.IShamirsSecretSharing 'Slip39.IShamirsSecretSharing')

The [IShamirsSecretSharing](Slip39.md#Slip39.IShamirsSecretSharing 'Slip39.IShamirsSecretSharing') instance.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string,Slip39.StringEncoding).memberThreshold'></a>

`memberThreshold` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

The member threshold.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string,Slip39.StringEncoding).memberCount'></a>

`memberCount` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

The total number of members.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string,Slip39.StringEncoding).masterSecret'></a>

`masterSecret` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The master secret to share as a string.

<a name='Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string,Slip39.StringEncoding).encoding'></a>

`encoding` [StringEncoding](Slip39.StringEncoding.md 'Slip39.StringEncoding')

The type of [StringEncoding](Slip39.StringEncoding.md 'Slip39.StringEncoding') of the string master secret.

#### Returns
[Share](Slip39.Share.md 'Slip39.Share')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
The set of shares, of which [memberThreshold](Slip39.Extensions.md#Slip39.Extensions.GenerateShares(thisSlip39.IShamirsSecretSharing,int,int,string,Slip39.StringEncoding).memberThreshold 'Slip39.Extensions.GenerateShares(this Slip39.IShamirsSecretSharing, int, int, string, Slip39.StringEncoding).memberThreshold') are required to recover the  
            secret.

### Remarks
The master secret is encoded as a [byte[]](https://docs.microsoft.com/en-us/dotnet/api/byte[] 'byte[]') of UTF-8 encoded bytes, and if required padded with  
0xff to fulfil the requirements. When recovering the secret, any trailing 0xff bytes thus need to be trimmed  
before producing the original master secret string, <seealso cref="M:Slip39.Extensions.ToSecretString(System.Byte[])"/>.

<a name='Slip39.Extensions.ToBip39(thisbyte[])'></a>

## Extensions.ToBip39(this byte[]) Method

Convert a master secret byte array to a BIP-39 mnemonic string. See https://bips.dev/39/ .

```csharp
public static string ToBip39(this byte[] entropy);
```
#### Parameters

<a name='Slip39.Extensions.ToBip39(thisbyte[]).entropy'></a>

`entropy` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The bytes to convert.

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The BIP 39 mnemonic string.

<a name='Slip39.Extensions.ToHex(thisbyte[])'></a>

## Extensions.ToHex(this byte[]) Method

Convert a byte array to a hex string.

```csharp
public static string ToHex(this byte[] bytes);
```
#### Parameters

<a name='Slip39.Extensions.ToHex(thisbyte[]).bytes'></a>

`bytes` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The bytes to convert.

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The hex string.

<a name='Slip39.Extensions.ToSecretString(thisbyte[])'></a>

## Extensions.ToSecretString(this byte[]) Method

Convert a recovered binary master secret to a UTF-8 encoded string, assuming it may have been padded with 0xFF  
bytes.

```csharp
public static string ToSecretString(this byte[] masterSecret);
```
#### Parameters

<a name='Slip39.Extensions.ToSecretString(thisbyte[]).masterSecret'></a>

`masterSecret` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The binary secret.

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The original unpadded string.

<a name='Slip39.Extensions.ToUrlSafeBase64(thisbyte[])'></a>

## Extensions.ToUrlSafeBase64(this byte[]) Method

Convert a byte array to a URL-safe base64 string.

```csharp
public static string ToUrlSafeBase64(this byte[] bytes);
```
#### Parameters

<a name='Slip39.Extensions.ToUrlSafeBase64(thisbyte[]).bytes'></a>

`bytes` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The bytes to convert.

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The URL-safe base64 string.

<a name='Slip39.Extensions.TryParse(thisstring,Slip39.Share)'></a>

## Extensions.TryParse(this string, Share) Method

Try to parse a string into a [Share](Slip39.Share.md 'Slip39.Share') instance.

```csharp
public static bool TryParse(this string value, out Slip39.Share share);
```
#### Parameters

<a name='Slip39.Extensions.TryParse(thisstring,Slip39.Share).value'></a>

`value` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The string value to try to parse.

<a name='Slip39.Extensions.TryParse(thisstring,Slip39.Share).share'></a>

`share` [Share](Slip39.Share.md 'Slip39.Share')

The resulting [Share](Slip39.Share.md 'Slip39.Share') if successful.

#### Returns
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
True if the parse was successful.