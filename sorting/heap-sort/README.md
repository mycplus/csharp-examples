# Heap Sort in C#

[![heap-sort](https://github.com/mycplus/csharp-examples/actions/workflows/heap-sort.yml/badge.svg)](https://github.com/mycplus/csharp-examples/actions/workflows/heap-sort.yml)

Companion code for [Heap Sort in C, C++, Java, Python and C#](https://www.mycplus.com/computer-science/algorithms/heap-sort/) on MYCPLUS.

| Path | What it is |
| --- | --- |
| `src/HeapSort/HeapSort.cs` | `HeapSort.Sort(int[])`, the article's listing |
| `src/HeapSort/Program.cs` | Sorts the article's array and prints it |
| `tests/HeapSortTests/` | Dependency-free tests against `Array.Sort`, and the program's expected output |

Requires the .NET 10 SDK.

## Build and test

```sh
dotnet build HeapSort.slnx -c Release
dotnet run --project tests/HeapSortTests -c Release --no-build
dotnet run --project src/HeapSort -c Release --no-build
```

## What the build checks

- Builds on Linux, Windows and macOS with nullable reference types on, the `latest-recommended` analyzer set, and warnings as errors.
- `HeapSort.Sort` agrees with `Array.Sort` on 20,000 random arrays of 0 to 64 elements, including `int.MinValue`, `int.MaxValue` and heavy duplication, and on 20,000 random ints, 2,000 reversed values, an empty array and one element.
- The program prints exactly the output shown in the article.
