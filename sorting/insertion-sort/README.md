# Insertion Sort in C#

[![insertion-sort](https://github.com/mycplus/csharp-examples/actions/workflows/insertion-sort.yml/badge.svg)](https://github.com/mycplus/csharp-examples/actions/workflows/insertion-sort.yml)

Companion code for [Insertion Sort in C, C++, Java, Python and C#](https://www.mycplus.com/computer-science/algorithms/insertion-sort/) on MYCPLUS.

| Path | What it is |
| --- | --- |
| `src/InsertionSort/InsertionSort.cs` | `InsertionSort.Sort(int[])`, the article's listing |
| `src/InsertionSort/Program.cs` | Sorts the article's array and prints it |
| `tests/InsertionSortTests/` | Dependency-free tests against `Array.Sort`, and the program's expected output |

Requires the .NET 10 SDK.

## Build and test

```sh
dotnet build InsertionSort.slnx -c Release
dotnet run --project tests/InsertionSortTests -c Release --no-build
dotnet run --project src/InsertionSort -c Release --no-build
```

## What the build checks

- Builds on Linux, Windows and macOS with nullable reference types on, the `latest-recommended` analyzer set, and warnings as errors.
- `InsertionSort.Sort` agrees with `Array.Sort` on 20,000 random arrays of 0 to 64 elements, including `int.MinValue`, `int.MaxValue` and heavy duplication, and on 20,000 random ints, 2,000 reversed values, an empty array and one element.
- The program prints exactly the output shown in the article.
