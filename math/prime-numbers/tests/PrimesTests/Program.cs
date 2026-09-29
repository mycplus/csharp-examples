using System;
using System.IO;

namespace MyCPlus.Primes.Tests;

// Dependency-free tests: dotnet run --project tests/PrimesTests
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

    public static int Main()
    {
        long[] primes = [2, 3, 5, 7, 11, 13, 97, 7919, 1000003, 2147483647, 1000000007, 999999999989];
        long[] composites = [long.MinValue, -7, -1, 0, 1, 4, 6, 9, 25, 49, 91, 121, 561, 1105, 7917,
                             1000003L * 1000003L, 1000003L * 1000033L, 2147483647L * 2];
        foreach (long p in primes)
            Check(Primes.IsPrime(p), $"{p} is prime");
        foreach (long c in composites)
            Check(!Primes.IsPrime(c), $"{c} is not prime");

        var flags = Primes.Sieve(200_000);
        for (int k = 0; k < flags.Length; k++)
        {
            if (Primes.IsPrime(k) != flags[k])
            {
                Check(false, $"trial division and sieve disagree at {k}");
                break;
            }
        }

        (int Limit, int Count)[] counts = [(0, 0), (1, 0), (2, 0), (3, 1), (10, 4), (100, 25),
                                           (1000, 168), (1_000_000, 78498), (10_000_000, 664579)];
        foreach (var (limit, count) in counts)
            Check(Primes.CountPrimesBelow(limit) == count, $"pi below {limit}");

        var sw = new StringWriter();
        TextWriter original = Console.Out;
        Console.SetOut(sw);
        Primes.Main();
        Console.SetOut(original);
        string expected = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "primes.txt"));
        Check(sw.ToString().Replace("\r", "", StringComparison.Ordinal) ==
              expected.Replace("\r", "", StringComparison.Ordinal), "demo output");

        if (s_failures != 0)
        {
            Console.Error.WriteLine($"{s_failures} check(s) failed");
            return 1;
        }
        Console.WriteLine("primes: all tests passed");
        return 0;
    }
}
