# Serpent Block Cipher in C#

[![serpent](https://github.com/mycplus/csharp-examples/actions/workflows/serpent.yml/badge.svg)](https://github.com/mycplus/csharp-examples/actions/workflows/serpent.yml)

Companion code for [Serpent Cipher Explained](https://www.mycplus.com/computer-science/algorithms/serpent-cipher/) on MYCPLUS: a span-based C# implementation of the Serpent block cipher (128-bit blocks, keys up to 256 bits) for .NET 10, checked against 1,043 published test vectors.

> For learning and testing. This code has not been reviewed for side channels. To protect real data, use `AesGcm` or `ChaCha20Poly1305` from `System.Security.Cryptography`.

| File | What it is |
| --- | --- |
| `src/SerpentDemo/Serpent.cs` | `Serpent` (disposable): key schedule, `Encrypt` and `Decrypt` of one 16-byte block; the inverse S-boxes are computed from the forward ones |
| `src/SerpentDemo/Program.cs` | Encrypts one block with a 256-bit key and checks two known vectors |
| `tests/SerpentTests/Program.cs` | Runs every vector in `tests/vectors/serpent_nessie.txt` in both directions and checks key and block lengths |
| `bench/SerpentSpeed/Program.cs` | Throughput in ECB mode over 16 MiB |

The test vectors are the NESSIE set as distributed with [Botan](https://github.com/randombit/botan) (`src/tests/data/block/serpent.vec`).

## Build and run

.NET 10 SDK.

```sh
dotnet build Serpent.slnx -c Release
dotnet run --project tests/SerpentTests -c Release --no-build
dotnet run --project src/SerpentDemo -c Release --no-build
dotnet run --project bench/SerpentSpeed -c Release --no-build
```

## What the build checks

- Builds with nullable reference types, `latest-recommended` analyzers and warnings as errors, on Linux, Windows and macOS.
- All 1,043 vectors (128-, 192- and 256-bit keys) encrypt and decrypt correctly; keys of 0 and 33 bytes and a 15-byte block throw `ArgumentException`.
- The demo prints exactly `tests/expected/SerpentDemo.txt`.
