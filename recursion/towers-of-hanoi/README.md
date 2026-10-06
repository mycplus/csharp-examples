# Towers of Hanoi in C#

[![towers-of-hanoi](https://github.com/mycplus/csharp-examples/actions/workflows/towers-of-hanoi.yml/badge.svg)](https://github.com/mycplus/csharp-examples/actions/workflows/towers-of-hanoi.yml)

Companion code for [Towers of Hanoi: Recursive Algorithm With Code in C, C++, Java, Python and C#](https://www.mycplus.com/computer-science/algorithms/towers-of-hanoi/) on MYCPLUS.

| File | What it is |
| --- | --- |
| `src/TowersOfHanoi/Program.cs` | The article's listing: prints the moves for 3 disks |
| `src/TowersOfHanoi/HanoiSolver.cs` | `Solve` and `SolveIterative` with an `Action<Move>`, `MoveAt` for any single move, and `MoveCount` as a `BigInteger` |
| `pitfalls/Pitfalls/Program.cs` | `shift`: `(1 << n) - 1` gives 0 moves for 32 disks; `base-case-one 0`: stack overflow terminates the process |
| `tests/TowersOfHanoiTests/Program.cs` | Dependency-free tests: simulation, solver agreement, 64-disk `MoveAt` against a reference, and the listing's output |

Requires the .NET 10 SDK.

## Build and test

```sh
dotnet build TowersOfHanoi.slnx -c Release
dotnet run --project tests/TowersOfHanoiTests -c Release --no-build
dotnet run --project src/TowersOfHanoi -c Release --no-build
```

## What the build checks

- Builds on Linux, Windows and macOS with `latest-recommended` analyzers and warnings as errors.
- Both solvers produce identical, legal, complete sequences for 0 to 18 disks, and `MoveAt` agrees with an independent reference on 2,000 moves of the 64-disk puzzle.
- The listing prints exactly the output shown in the article, and the pitfalls behave the way the article describes.
