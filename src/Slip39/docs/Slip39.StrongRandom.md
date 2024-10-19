#### [Slip39](index.md 'index')
### [Slip39](Slip39.md 'Slip39')

## StrongRandom Class

An [IRandom](Slip39.md#Slip39.IRandom 'Slip39.IRandom') implementation using [System.Security.Cryptography.RandomNumberGenerator](https://docs.microsoft.com/en-us/dotnet/api/System.Security.Cryptography.RandomNumberGenerator 'System.Security.Cryptography.RandomNumberGenerator').

```csharp
public class StrongRandom :
Slip39.IRandom
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; StrongRandom

Implements [IRandom](Slip39.md#Slip39.IRandom 'Slip39.IRandom')
### Methods

<a name='Slip39.StrongRandom.GetBytes(byte[])'></a>

## StrongRandom.GetBytes(byte[]) Method

Fill the provided buffer with random byte values.

```csharp
public void GetBytes(byte[] buffer);
```
#### Parameters

<a name='Slip39.StrongRandom.GetBytes(byte[]).buffer'></a>

`buffer` [System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

The buffer to fill.

Implements [GetBytes(byte[])](Slip39.md#Slip39.IRandom.GetBytes(byte[]) 'Slip39.IRandom.GetBytes(byte[])')

<a name='Slip39.StrongRandom.GetBytes(int)'></a>

## StrongRandom.GetBytes(int) Method

Get a [byte[]](https://docs.microsoft.com/en-us/dotnet/api/byte[] 'byte[]') with random bytes.

```csharp
public byte[] GetBytes(int count);
```
#### Parameters

<a name='Slip39.StrongRandom.GetBytes(int).count'></a>

`count` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

The number of random bytes to get.

Implements [GetBytes(int)](Slip39.md#Slip39.IRandom.GetBytes(int) 'Slip39.IRandom.GetBytes(int)')

#### Returns
[System.Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte 'System.Byte')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
A [byte[]](https://docs.microsoft.com/en-us/dotnet/api/byte[] 'byte[]') with random bytes.