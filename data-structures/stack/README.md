# Stack in C#

[![stack](https://github.com/mycplus/csharp-examples/actions/workflows/stack.yml/badge.svg)](https://github.com/mycplus/csharp-examples/actions/workflows/stack.yml)

Companion code for [Stack Implementation in C, C++, Java, Python and C#](https://www.mycplus.com/computer-science/data-structures/stack-implementation/) on MYCPLUS.

| Path | What it is |
| --- | --- |
| `src/StackDemo/ArrayStack.cs` | A generic growable array-backed stack that clears popped slots |
| `src/StackDemo/LinkedStack.cs` | A generic linked-list stack |
| `src/StackDemo/Program.cs` | The article's demo, with a bracket checker on `Stack<char>` |
| `src/StackGrowth/` | Counts `Stack<int>` reallocations over 1,000,000 pushes |
| `tests/StackTests/` | Dependency-free tests, and the demo's expected output |

Requires the .NET 10 SDK.

## Build and test

```sh
dotnet build Stack.slnx -c Release
dotnet run --project tests/StackTests -c Release --no-build
dotnet run --project src/StackDemo -c Release --no-build
```

## What the build checks

- Builds on Linux, Windows and macOS with nullable reference types on, the `latest-recommended` analyzer set, and warnings as errors. CA1711 (the `Stack` name suffix) is suppressed so the class names match the article.
- Both stacks agree with `System.Collections.Generic.Stack<T>` over 100,000 random operations.
- `Pop()` and `Peek()` on an empty stack throw `InvalidOperationException`; `TryPop` returns `false`.
- `ArrayStack` stores `null` as an ordinary value and grows to 1,000,000 elements.
- The bracket checker handles crossed pairs, a lone closer and 100,000 levels of nesting.
- The demo prints exactly the output shown in the article.
