# Prime numbers in C#

[![prime-numbers](https://github.com/mycplus/csharp-examples/actions/workflows/prime-numbers.yml/badge.svg)](https://github.com/mycplus/csharp-examples/actions/workflows/prime-numbers.yml)

Companion code for [Prime Number Programs in C, C++, Java, Python, C#, PHP and JavaScript](https://www.mycplus.com/computer-science/algorithms/prime-number-program/) on MYCPLUS: a primality test by trial division up to the square root, and the Sieve of Eratosthenes. The same program exists in seven languages and every version prints the same output.

| Path | What it is |
| --- | --- |
| `src/Primes/Primes.cs` | `IsPrime(long)`, a `BitArray` sieve and the demo |
| `tests/PrimesTests/` | Dependency-free tests |

Requires the .NET 10 SDK.

## Build and test

```sh
dotnet build Primes.slnx -c Release
dotnet run --project tests/PrimesTests -c Release --no-build
dotnet run --project src/Primes -c Release --no-build
```

## What the build checks

- Builds on Linux, Windows and macOS with nullable reference types, the `latest-recommended` analyzers and warnings as errors.
- Trial division agrees with the sieve on every number below 200,000.
- Known primes (including 2147483647, 1000000007 and 999999999989) and composites (including negatives, 0, 1, squares of primes and Carmichael numbers 561 and 1105) are classified correctly.
- The sieve reproduces the published prime counts: 168 below 1,000, 78,498 below 1,000,000 and 664,579 below 10,000,000.
- The demo prints exactly `tests/expected/primes.txt`, the same file in all seven repositories.
