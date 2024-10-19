# SLIP39

A C# implementation of the [SLIP39](https://github.com/satoshilabs/slips/blob/master/slip-0039.md) for Shamir's
Secret-Sharing for Mnemonic Codes.

The code is heavily "inspired" on the [Bitcoin Wallet library in Rust
implementation](https://github.com/rust-bitcoin/rust-wallet/blob/master/src/sss.rs)

It has been refactored to more idiomatic C#, as well as adding a few features and improvements by [Svante
Seleborg](https://github.com/xecrets) . Some convenience extensions for simpler use cases were added, a simple command
line interface was implemented, and XML comments were added. Also support for URL safe base64 encoding of the shares
was added as a more compact, but equivalent, representation of the shares. Limited BIP-39 support was added to allow
for mnemonic generation and recovery of the master secret in BIP-39 format.

# DISCLAIMER

This project is still in early development phase. Use it at your own risk.

## Description

SLIP-0039 describes a standard and interoperable implementation of Shamir's secret sharing (SSS). SSS splits a secret
into unique parts which can be distributed among participants, and requires a specified minimum number of parts to be
supplied in order to reconstruct the original secret. Knowledge of fewer than the required number of parts does not
leak information about the secret.


## Specification
See https://github.com/satoshilabs/slips/blob/master/slip-0039.md for the full SLIP-0039 specification.

## Using

See the [documentation](src/Slip39/docs/index.md 'XML Documentation') for the full API documentation.

```c#
/**
 * 4 groups of shares.
 * = two for Alice
 * = one for friends and
 * = one for family members
 * Two of these group shares are required to reconstruct the master secret.
 */
var shares = Slip39.Generate(
    extendable: true,
    iterationExponent: 0,
    groupThreshold: 2, 
    groups: [
      // Alice's group shares. 1 is enough to reconstruct a group share,
      // therefore she needs at least two group shares to be reconstructed,
      new Group(1, 1),
      new Group(1, 1),
      // 3 of 5 Friends' shares are required to reconstruct this group share
      new Group(3, 5),
      // 2 of 6 Family's shares are required to reconstruct this group share
      new Group(2, 6),
    ],
    passphrase: "TREZOR",
    masterSecret: "ABCDEFGHIJKLMNOP"u8.ToArray(),
    );

var recoveredSecret = Slip39.Combine(shares, passphrase);
Assert.Equal(masterSecret, recoveredSecret);
```

## Testing

```bash
$ dotnet test

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.8.2+699d445a1a (64-bit .NET 8.0.7)
[xUnit.net 00:00:00.05]   Discovering: Slip39
[xUnit.net 00:00:00.09]   Discovered:  Slip39
[xUnit.net 00:00:00.09]   Starting:    Slip39
  Passed Slip39.ShamirMnemonicTests.TestInvalidSharing [25 ms]
  Passed Slip39.ShamirMnemonicTests.TestPassphrase [27 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 1. Valid mnemonic without sharing (128 bits)) [14 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 2. Mnemonic with invalid checksum (128 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 3. Mnemonic with invalid padding (128 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 4. Basic sharing 2-of-3 (128 bits)) [24 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 5. Basic sharing 2-of-3 (128 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 6. Mnemonics with different identifiers (128 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 7. Mnemonics with different iteration exponents (128 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 8. Mnemonics with mismatching group thresholds (128 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 9. Mnemonics with mismatching group counts (128 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 10. Mnemonics with greater group threshold than group counts (128 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 11. Mnemonics with duplicate member indices (128 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 12. Mnemonics with mismatching member thresholds (128 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 13. Mnemonics giving an invalid digest (128 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 14. Insufficient number of groups (128 bits, case 1)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 15. Insufficient number of groups (128 bits, case 2)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 16. Threshold number of groups, but insufficient number of members in one group (128 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 17. Threshold number of groups and members in each group (128 bits, case 1)) [6 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 18. Threshold number of groups and members in each group (128 bits, case 2)) [6 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 19. Threshold number of groups and members in each group (128 bits, case 3)) [6 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 20. Valid mnemonic without sharing (256 bits)) [6 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 21. Mnemonic with invalid checksum (256 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 22. Mnemonic with invalid padding (256 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 23. Basic sharing 2-of-3 (256 bits)) [24 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 24. Basic sharing 2-of-3 (256 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 25. Mnemonics with different identifiers (256 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 26. Mnemonics with different iteration exponents (256 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 27. Mnemonics with mismatching group thresholds (256 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 28. Mnemonics with mismatching group counts (256 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 29. Mnemonics with greater group threshold than group counts (256 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 30. Mnemonics with duplicate member indices (256 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 31. Mnemonics with mismatching member thresholds (256 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 32. Mnemonics giving an invalid digest (256 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 33. Insufficient number of groups (256 bits, case 1)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 34. Insufficient number of groups (256 bits, case 2)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 35. Threshold number of groups, but insufficient number of members in one group (256 bits)) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 36. Threshold number of groups and members in each group (256 bits, case 1)) [6 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 37. Threshold number of groups and members in each group (256 bits, case 2)) [6 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 38. Threshold number of groups and members in each group (256 bits, case 3)) [6 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 39. Mnemonic with insufficient length) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 40. Mnemonic with invalid master secret length) [< 1 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 41. Valid mnemonics which can detect some errors in modular arithmetic) [6 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 42. Valid extendable mnemonic without sharing (128 bits)) [48 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 43. Extendable basic sharing 2-of-3 (128 bits)) [6 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 44. Valid extendable mnemonic without sharing (256 bits)) [48 ms]
  Passed Slip39.ShamirMnemonicTests.TestVectors(test: 45. Extendable basic sharing 2-of-3 (256 bits)) [6 ms]
  Passed Slip39.ShamirMnemonicTests.TestNonExtendable [12 ms]
  Passed Slip39.ShamirMnemonicTests.TestAllGroupsExist [23 ms]
  Passed Slip39.ShamirMnemonicTests.TestIterationExponent [109 ms]
  Passed Slip39.ShamirMnemonicTests.TestBasicSharingFixed [23 ms]
  Passed Slip39.ShamirMnemonicTests.TestGroupSharingThreshold1 [151 ms]
  Passed Slip39.ShamirMnemonicTests.TestBasicSharingRandom [17 ms]
  Passed Slip39.ShamirMnemonicTests.TestGroupSharing [1 s]

Test Run Successful.
Total tests: 54
     Passed: 54
 Total time: 2.1716 Seconds

```

# LICENSE

[MIT License](LICENSE)
