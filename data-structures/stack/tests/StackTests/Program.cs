using System;
using System.Collections.Generic;
using System.IO;

namespace MyCPlus.Stack.Tests;

// Dependency-free tests: dotnet run --project tests/StackTests
public static class Program
{
    private static int s_failures;

    private static void Check(bool ok, string what)
    {
        if (!ok)
        {
            Console.Error.WriteLine($"CHECK failed: {what}");
            s_failures++;
        }
    }

    private static ulong s_state = 88172645463325252UL;

    private static ulong NextRand()
    {
        s_state ^= s_state << 13;
        s_state ^= s_state >> 7;
        s_state ^= s_state << 17;
        return s_state;
    }

    // Random operations checked against System.Collections.Generic.Stack<T>.
    private static void Differential(string name, Action<string> push, Func<string> pop,
                                     Func<string> peek, Func<int> count)
    {
        var model = new Stack<string>();
        for (int i = 0; i < 100_000; i++)
        {
            ulong r = NextRand();
            if (r % 3 != 0)
            {
                string v = $"value-{r % 100_000}";
                push(v);
                model.Push(v);
            }
            else if (model.Count > 0)
            {
                Check(pop() == model.Pop(), $"{name}: pop order");
            }
            Check(count() == model.Count, $"{name}: count");
            if (model.Count > 0)
                Check(peek() == model.Peek(), $"{name}: peek");
        }
    }

    private static void EmptyThrows(string name, Action op)
    {
        try
        {
            op();
            Check(false, $"{name}: expected InvalidOperationException");
        }
        catch (InvalidOperationException e)
        {
            Check(e.Message == "Pop or Peek on an empty stack.", $"{name}: message");
        }
    }

    public static int Main()
    {
        var a = new ArrayStack<string>();
        Differential("ArrayStack", a.Push, a.Pop, a.Peek, () => a.Count);
        var l = new LinkedStack<string>();
        Differential("LinkedStack", l.Push, l.Pop, l.Peek, () => l.Count);

        var ea = new ArrayStack<int>();
        EmptyThrows("ArrayStack.Pop", () => ea.Pop());
        EmptyThrows("ArrayStack.Peek", () => ea.Peek());
        Check(!ea.TryPop(out int unused) && unused == 0, "ArrayStack.TryPop on empty");
        var el = new LinkedStack<int>();
        EmptyThrows("LinkedStack.Pop", () => el.Pop());
        EmptyThrows("LinkedStack.Peek", () => el.Peek());
        Check(el.IsEmpty && el.Count == 0, "LinkedStack starts empty");

        var big = new ArrayStack<int>();
        for (int i = 0; i < 1_000_000; i++)
            big.Push(i);
        Check(big.Count == 1_000_000 && big.Peek() == 999_999, "growth");

        var nulls = new ArrayStack<string?>();
        nulls.Push(null);
        Check(nulls.Count == 1 && nulls.Pop() is null, "null is a value");

        Check(MyCPlus.Stack.Program.Balanced("") && MyCPlus.Stack.Program.Balanced("{[()()]}"),
              "balanced");
        Check(!MyCPlus.Stack.Program.Balanced("([)]") && !MyCPlus.Stack.Program.Balanced(")"),
              "unbalanced");
        Check(MyCPlus.Stack.Program.Balanced(new string('(', 100_000) + new string(')', 100_000)),
              "deep nesting");

        // The demo prints exactly the article's output.
        var sw = new StringWriter();
        TextWriter original = Console.Out;
        Console.SetOut(sw);
        MyCPlus.Stack.Program.Main();
        Console.SetOut(original);
        string expected = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "StackDemo.txt"));
        Check(sw.ToString().Replace("\r", "", StringComparison.Ordinal) ==
              expected.Replace("\r", "", StringComparison.Ordinal), "demo output");

        if (s_failures != 0)
        {
            Console.Error.WriteLine($"{s_failures} check(s) failed");
            return 1;
        }
        Console.WriteLine("stacks: all tests passed");
        return 0;
    }
}
