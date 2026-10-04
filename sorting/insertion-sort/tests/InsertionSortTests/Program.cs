using System;
using System.IO;
using System.Linq;

namespace MyCPlus.Sorting.Tests;

// Dependency-free tests: dotnet run --project tests/InsertionSortTests
public static class Program
{
    private static int s_failures;
    private static readonly int[] s_one = { 42 };

    private static void Check(bool ok, string what)
    {
        if (!ok)
        {
            Console.Error.WriteLine($"CHECK failed: {what}");
            s_failures++;
        }
    }

    private static ulong s_state = 0x9E3779B97F4A7C15UL;

    private static ulong NextRand()
    {
        s_state ^= s_state << 13;
        s_state ^= s_state >> 7;
        s_state ^= s_state << 17;
        return s_state;
    }

    // Sorts a copy with InsertionSort.Sort and compares it with Array.Sort.
    private static void Against(int[] input, string what)
    {
        int[] expected = (int[])input.Clone();
        int[] actual = (int[])input.Clone();
        Array.Sort(expected);
        InsertionSort.Sort(actual);
        Check(actual.SequenceEqual(expected), what);
    }

    public static int Main()
    {
        // 20,000 random arrays of 0 to 64 elements: every third one draws
        // from only 4 values, and about 1 value in 8 is MinValue or MaxValue.
        for (int t = 0; t < 20_000; t++)
        {
            int n = (int)(NextRand() % 65);
            int range = t % 3 == 0 ? 4 : 1000;
            int[] a = new int[n];
            for (int i = 0; i < n; i++)
            {
                ulong r = NextRand() % 16;
                a[i] = r == 0 ? int.MinValue
                     : r == 1 ? int.MaxValue
                     : (int)(NextRand() % (ulong)range) - range / 2;
            }
            Against(a, $"random case {t}");
        }

        int[] big = new int[20_000];
        for (int i = 0; i < big.Length; i++)
            big[i] = unchecked((int)NextRand());
        Against(big, "20,000 random ints");
        Against(Enumerable.Range(1, 2_000).Reverse().ToArray(), "reversed");
        Against(Array.Empty<int>(), "empty array");
        Against(s_one, "one element");

        var original = Console.Out;
        using var sw = new StringWriter();
        Console.SetOut(sw);
        MyCPlus.Sorting.Program.Main();
        Console.SetOut(original);
        string expectedOutput = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "InsertionSort.txt"));
        Check(sw.ToString().Replace("\r", "", StringComparison.Ordinal) ==
              expectedOutput.Replace("\r", "", StringComparison.Ordinal), "program output");

        if (s_failures != 0)
        {
            Console.Error.WriteLine($"{s_failures} check(s) failed");
            return 1;
        }
        Console.WriteLine("InsertionSort: all tests passed");
        return 0;
    }
}
