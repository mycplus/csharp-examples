using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace MyCPlus.Recursion.Tests;

// Dependency-free tests: dotnet run --project tests/TowersOfHanoiTests
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

    // Applies the moves to three pegs; false on any illegal move or if the
    // tower is not on C, largest at the bottom, at the end.
    private static bool LegalAndSolved(int n, List<Move> moves)
    {
        var pegs = new[] { new Stack<int>(), new Stack<int>(), new Stack<int>() };
        for (int d = n; d >= 1; d--)
            pegs[0].Push(d);
        foreach (Move m in moves)
        {
            int f = m.From - 'A', t = m.To - 'A';
            if (f is < 0 or > 2 || t is < 0 or > 2 || f == t)
                return false;
            if (!pegs[f].TryPeek(out int top) || top != m.Disk)
                return false;
            if (pegs[t].TryPeek(out int under) && under < m.Disk)
                return false;
            pegs[t].Push(pegs[f].Pop());
        }
        return pegs[0].Count == 0 && pegs[1].Count == 0
            && pegs[2].SequenceEqual(Enumerable.Range(1, n));
    }

    // Independent reference for move k: descend the recursion, no bit tricks.
    private static Move Reference(int n, ulong k, char s, char t, char v)
    {
        while (true)
        {
            ulong mid = 1UL << (n - 1);
            if (k == mid)
                return new Move(n, s, t);
            if (k < mid)
                (t, v) = (v, t);
            else
            {
                k -= mid;
                (s, v) = (v, s);
            }
            n--;
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

    private static bool Throws(Action action)
    {
        try
        {
            action();
        }
        catch (ArgumentException)
        {
            return true;
        }
        return false;
    }

    public static int Main()
    {
        for (int n = 0; n <= 18; n++)
        {
            List<Move> rec = HanoiSolver.Moves(n);
            var it = new List<Move>();
            HanoiSolver.SolveIterative(n, 'A', 'C', 'B', it.Add);
            Check(LegalAndSolved(n, rec), $"recursive legal and solved, n={n}");
            Check(rec.SequenceEqual(it), $"iterative == recursive, n={n}");
            Check(rec.Count == HanoiSolver.MoveCount(n), $"count, n={n}");
            bool allMatch = true;
            for (int i = 0; i < rec.Count; i++)
                allMatch &= HanoiSolver.MoveAt(n, (ulong)i + 1) == rec[i];
            Check(allMatch, $"MoveAt matches, n={n}");
        }

        Check(HanoiSolver.MoveCount(64) == ulong.MaxValue, "2^64 - 1");
        Check(HanoiSolver.MoveCount(100) == BigInteger.Pow(2, 100) - 1, "2^100 - 1");
        Check(Throws(() => HanoiSolver.MoveCount(-1)), "MoveCount(-1)");
        Check(Throws(() => HanoiSolver.Solve(-1, 'A', 'C', 'B', _ => { })), "Solve(-1)");
        Check(Throws(() => HanoiSolver.MoveAt(3, 0)), "MoveAt k=0");
        Check(Throws(() => HanoiSolver.MoveAt(3, 8)), "MoveAt k=8");
        Check(Throws(() => HanoiSolver.MoveAt(65, 1)), "MoveAt n=65");
        Check(Throws(() => HanoiSolver.SolveIterative(65, 'A', 'C', 'B', _ => { })), "SolveIterative(65)");

        // MoveAt for 64 disks: the last move, the 64 moves with
        // k | (k - 1) == 2^64 - 1, and random moves, against the reference.
        var ks = new List<ulong> { 1, ulong.MaxValue };
        for (int j = 0; j < 64; j++)
            ks.Add(ulong.MaxValue - ((1UL << j) - 1));
        while (ks.Count < 2000)
        {
            ulong k = NextRand();
            ks.Add(k == 0 ? 1 : k);
        }
        Check(ks.All(k => HanoiSolver.MoveAt(64, k) == Reference(64, k, 'A', 'C', 'B')),
              "MoveAt(64, k) against the reference");

        // The listing prints the article's output.
        var output = new StringWriter();
        TextWriter saved = Console.Out;
        Console.SetOut(output);
        try
        {
            TowersOfHanoi.Main();
        }
        finally
        {
            Console.SetOut(saved);
        }
        string expected = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "TowersOfHanoi.txt"));
        Check(output.ToString().Replace("\r", "", StringComparison.Ordinal)
              == expected.Replace("\r", "", StringComparison.Ordinal), "listing output");

        if (s_failures != 0)
        {
            Console.Error.WriteLine($"{s_failures} check(s) failed");
            return 1;
        }
        Console.WriteLine("all tests passed");
        return 0;
    }
}
