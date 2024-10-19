#### [Slip39](index.md 'index')
### [Slip39](Slip39.md 'Slip39')

## Group Class

Defines the main parameter of a group.

```csharp
public class Group :
System.IEquatable<Slip39.Group>
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; Group

Implements [System.IEquatable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1 'System.IEquatable`1')[Group](Slip39.Group.md 'Slip39.Group')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1 'System.IEquatable`1')
### Constructors

<a name='Slip39.Group.Group(int,int)'></a>

## Group(int, int) Constructor

Defines the main parameter of a group.

```csharp
public Group(int GroupThreshold, int GroupCount);
```
#### Parameters

<a name='Slip39.Group.Group(int,int).GroupThreshold'></a>

`GroupThreshold` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

Member threshold for group i, a positive integer, 1 ≤ Ti ≤ Ni.

<a name='Slip39.Group.Group(int,int).GroupCount'></a>

`GroupCount` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

Total number of members in group i, a positive integer, 1 ≤ Ni ≤ 16.
### Properties

<a name='Slip39.Group.GroupCount'></a>

## Group.GroupCount Property

Total number of members in group i, a positive integer, 1 ≤ Ni ≤ 16.

```csharp
public int GroupCount { get; set; }
```

#### Property Value
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

<a name='Slip39.Group.GroupThreshold'></a>

## Group.GroupThreshold Property

Member threshold for group i, a positive integer, 1 ≤ Ti ≤ Ni.

```csharp
public int GroupThreshold { get; set; }
```

#### Property Value
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')