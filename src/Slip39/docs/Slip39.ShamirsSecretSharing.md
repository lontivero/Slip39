#### [Slip39](index.md 'index')
### [Slip39](Slip39.md 'Slip39')

## ShamirsSecretSharing Class

A class for implementing Shamir's Secret Sharing with SLIP-0039 enhancements.

```csharp
public class ShamirsSecretSharing :
Slip39.IShamirsSecretSharing
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; ShamirsSecretSharing

Implements [IShamirsSecretSharing](Slip39.md#Slip39.IShamirsSecretSharing 'Slip39.IShamirsSecretSharing')
### Constructors

<a name='Slip39.ShamirsSecretSharing.ShamirsSecretSharing(Slip39.IRandom)'></a>

## ShamirsSecretSharing(IRandom) Constructor

A class for implementing Shamir's Secret Sharing with SLIP-0039 enhancements.

```csharp
public ShamirsSecretSharing(Slip39.IRandom random);
```
#### Parameters

<a name='Slip39.ShamirsSecretSharing.ShamirsSecretSharing(Slip39.IRandom).random'></a>

`random` [IRandom](Slip39.md#Slip39.IRandom 'Slip39.IRandom')
### Methods

<a name='Slip39.ShamirsSecretSharing.CombineShares(Slip39.Share[],string)'></a>

## ShamirsSecretSharing.CombineShares(Share[], string) Method

Combines shares to reconstruct the original secret.

```csharp
public byte[] CombineShares(Slip39.Share[] shares, string passphrase);
```
#### Parameters

<a name='Slip39.ShamirsSecretSharing.CombineShares(Slip39.Share[],string).shares'></a>

`shares` [Share](Slip39.Share.md 'Slip39.Share')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The array of shares to combine.

<a name='Slip39.ShamirsSecretSharing.CombineShares(Slip39.Share[],string).passphrase'></a>

`passphrase` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The passphrase used for decrypting the shares.

Implements [CombineShares(Share[], string)](Slip39.md#Slip39.IShamirsSecretSharing.CombineShares(Slip39.Share[],string) 'Slip39.IShamirsSecretSharing.CombineShares(Slip39.Share[], string)')

#### Returns
[System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
The reconstructed secret.

#### Exceptions

[System.ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentException 'System.ArgumentException')  
Thrown when the shares are insufficient or invalid.

<a name='Slip39.ShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[])'></a>

## ShamirsSecretSharing.GenerateShares(bool, int, int, Group[], string, byte[]) Method

Generates SLIP-0039 shares from a given master secret.

```csharp
public Slip39.Share[] GenerateShares(bool extendable, int iterationExponent, int groupThreshold, Slip39.Group[] groups, string passphrase, byte[] masterSecret);
```
#### Parameters

<a name='Slip39.ShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]).extendable'></a>

`extendable` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

<a name='Slip39.ShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]).iterationExponent'></a>

`iterationExponent` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

Exponent to determine the number of iterations for the encryption  
            algorithm.

<a name='Slip39.ShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]).groupThreshold'></a>

`groupThreshold` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

The number of groups required to reconstruct the secret.

<a name='Slip39.ShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]).groups'></a>

`groups` [Group](Slip39.Group.md 'Slip39.Group')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

Array of tuples where each tuple represents (groupThreshold, shareCount) for each  
            group.

<a name='Slip39.ShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]).passphrase'></a>

`passphrase` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The passphrase used for encryption.

<a name='Slip39.ShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]).masterSecret'></a>

`masterSecret` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The secret to be split into shares.

Implements [GenerateShares(bool, int, int, Group[], string, byte[])](Slip39.md#Slip39.IShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]) 'Slip39.IShamirsSecretSharing.GenerateShares(bool, int, int, Slip39.Group[], string, byte[])')

#### Returns
[Share](Slip39.Share.md 'Slip39.Share')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
A list of shares that can be used to reconstruct the secret.

#### Exceptions

[System.ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentException 'System.ArgumentException')  
Thrown when inputs do not meet the required constraints.