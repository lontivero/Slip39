#### [Slip39](index.md 'index')

## Slip39 Namespace

The main entry point for the library is through the [IShamirsSecretSharing](Slip39.md#Slip39.IShamirsSecretSharing 'Slip39.IShamirsSecretSharing') interface, which  
            has a default implementation in [ShamirsSecretSharing](Slip39.ShamirsSecretSharing.md 'Slip39.ShamirsSecretSharing'). The only other public type of importance is  
            [Share](Slip39.Share.md 'Slip39.Share') which is how generated shares are represented and returned. It also has a static method to  
            parse a share string into a [Share](Slip39.Share.md 'Slip39.Share') object.

| Classes | |
| :--- | :--- |
| [Extensions](Slip39.Extensions.md 'Slip39.Extensions') | A set of useful extensions. |
| [Group](Slip39.Group.md 'Slip39.Group') | Defines the main parameter of a group. |
| [ShamirsSecretSharing](Slip39.ShamirsSecretSharing.md 'Slip39.ShamirsSecretSharing') | A class for implementing Shamir's Secret Sharing with SLIP-0039 enhancements. |
| [Share](Slip39.Share.md 'Slip39.Share') | A representation of a Shamir secret sharing share. |
| [Slip39Exception](Slip39.Slip39Exception.md 'Slip39.Slip39Exception') | Creates an exception for catcheable Slip39 errors. This will only be thrown when the error is due to incorrect<br/>input data. |
| [StrongRandom](Slip39.StrongRandom.md 'Slip39.StrongRandom') | An [IRandom](Slip39.md#Slip39.IRandom 'Slip39.IRandom') implementation using [System.Security.Cryptography.RandomNumberGenerator](https://docs.microsoft.com/en-us/dotnet/api/System.Security.Cryptography.RandomNumberGenerator 'System.Security.Cryptography.RandomNumberGenerator'). |
### Interfaces

<a name='Slip39.IRandom'></a>

## IRandom Interface

Generate sequences of random bytes.

```csharp
public interface IRandom
```

Derived  
&#8627; [StrongRandom](Slip39.StrongRandom.md 'Slip39.StrongRandom')
### Methods

<a name='Slip39.IRandom.GetBytes(byte[])'></a>

## IRandom.GetBytes(byte[]) Method

Fill the provided buffer with random byte values.

```csharp
void GetBytes(byte[] buffer);
```
#### Parameters

<a name='Slip39.IRandom.GetBytes(byte[]).buffer'></a>

`buffer` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The buffer to fill.

<a name='Slip39.IRandom.GetBytes(int)'></a>

## IRandom.GetBytes(int) Method

Get a [byte[]](https://docs.microsoft.com/en-us/dotnet/api/byte[] 'byte[]') with random bytes.

```csharp
byte[] GetBytes(int count);
```
#### Parameters

<a name='Slip39.IRandom.GetBytes(int).count'></a>

`count` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

The number of random bytes to get.

#### Returns
[System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
A [byte[]](https://docs.microsoft.com/en-us/dotnet/api/byte[] 'byte[]') with random bytes.

<a name='Slip39.IShamirsSecretSharing'></a>

## IShamirsSecretSharing Interface

Implement Shamirs Secret Sharing according to <seealso href="https://github.com/satoshilabs/slips/blob/master/slip-0039.md">  
SLIP-039</seealso>.

```csharp
public interface IShamirsSecretSharing
```

Derived  
&#8627; [ShamirsSecretSharing](Slip39.ShamirsSecretSharing.md 'Slip39.ShamirsSecretSharing')
### Methods

<a name='Slip39.IShamirsSecretSharing.CombineShares(Slip39.Share[],string)'></a>

## IShamirsSecretSharing.CombineShares(Share[], string) Method

Recover the master secret from an appropriate set of shares.

```csharp
byte[] CombineShares(Slip39.Share[] shares, string passphrase);
```
#### Parameters

<a name='Slip39.IShamirsSecretSharing.CombineShares(Slip39.Share[],string).shares'></a>

`shares` [Share](Slip39.Share.md 'Slip39.Share')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The shares to recover the secret from.

<a name='Slip39.IShamirsSecretSharing.CombineShares(Slip39.Share[],string).passphrase'></a>

`passphrase` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The (optional) passphrase to decrypt the encrypted master secret with.

#### Returns
[System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
The original master secret.

<a name='Slip39.IShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[])'></a>

## IShamirsSecretSharing.GenerateShares(bool, int, int, Group[], string, byte[]) Method

Generate shares according to the given parameters.

```csharp
Slip39.Share[] GenerateShares(bool extendable, int iterationExponent, int groupThreshold, Slip39.Group[] groups, string passphrase, byte[] masterSecret);
```
#### Parameters

<a name='Slip39.IShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]).extendable'></a>

`extendable` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

Indicates that the id is used as salt in the encryption of the master secret when  
            false.

<a name='Slip39.IShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]).iterationExponent'></a>

`iterationExponent` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

The total number of iterations to be used in PBKDF2, calculated as  
            10000×2^e.

<a name='Slip39.IShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]).groupThreshold'></a>

`groupThreshold` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

The number of group shares needed to reconstruct the master secret.

<a name='Slip39.IShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]).groups'></a>

`groups` [Group](Slip39.Group.md 'Slip39.Group')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The group definitions as a [Group[]](https://docs.microsoft.com/en-us/dotnet/api/Group[] 'Group[]').

<a name='Slip39.IShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]).passphrase'></a>

`passphrase` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The (optional) passphrase used to encrypt the master secret.

<a name='Slip39.IShamirsSecretSharing.GenerateShares(bool,int,int,Slip39.Group[],string,byte[]).masterSecret'></a>

`masterSecret` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The master secret, at least 128 bits and a multiple of 16 bits.

#### Returns
[Share](Slip39.Share.md 'Slip39.Share')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
A [Share[]](https://docs.microsoft.com/en-us/dotnet/api/Share[] 'Share[]') with shares that can be distributed according to the threshold  
            parameters.

| Enums | |
| :--- | :--- |
| [StringEncoding](Slip39.StringEncoding.md 'Slip39.StringEncoding') | The master secret string encoding |
