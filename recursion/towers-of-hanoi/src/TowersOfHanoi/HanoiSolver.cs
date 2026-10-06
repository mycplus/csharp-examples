using System;
using System.Collections.Generic;
using System.Numerics;

namespace MyCPlus.Recursion;

/// <summary>One move: disk 1 is the smallest.</summary>
public readonly record struct Move(int Disk, char From, char To);

/// <summary>Towers of Hanoi: recursive and iterative solvers, any single
/// move computed directly, and the exact move count.</summary>
public static class HanoiSolver
{
    /// <summary>Largest disk count MoveAt and SolveIterative accept:
    /// 2^64 - 1 is the largest move number a ulong holds.</summary>
    public const int MaxDisks = 64;

    /// <summary>2^n - 1, exactly, for any n &gt;= 0.</summary>
    public static BigInteger MoveCount(int n)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);
        return (BigInteger.One << n) - 1;
    }

    /// <summary>Recursive solution. Recursion depth is n + 1.</summary>
    public static void Solve(int n, char source, char target, char spare, Action<Move> visit)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);
        ArgumentNullException.ThrowIfNull(visit);
        SolveFrom(n, source, target, spare, visit);
    }

    private static void SolveFrom(int n, char source, char target, char spare, Action<Move> visit)
    {
        if (n == 0)
            return;
        SolveFrom(n - 1, source, spare, target, visit);
        visit(new Move(n, source, target));
        SolveFrom(n - 1, spare, target, source, visit);
    }

    /// <summary>All moves as a list. Limited to 25 disks (33,554,431 moves).</summary>
    public static List<Move> Moves(int n, char source = 'A', char target = 'C', char spare = 'B')
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(n, 25);
        var moves = new List<Move>();
        Solve(n, source, target, spare, moves.Add);
        return moves;
    }

    /// <summary>Move k (1 &lt;= k &lt;= 2^n - 1) of the n-disk solution,
    /// computed from the bits of k.</summary>
    public static Move MoveAt(int n, ulong k, char source = 'A', char target = 'C', char spare = 'B')
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(n, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(n, MaxDisks);
        ulong last = n == 64 ? ulong.MaxValue : (1UL << n) - 1;
        if (k == 0 || k > last)
            throw new ArgumentOutOfRangeException(nameof(k), k, "k must be 1 to 2^n - 1.");

        // The bit formula moves the tower from peg index 0 to index 2 when n
        // is odd and to index 1 when n is even.
        char[] peg = n % 2 == 1 ? [source, spare, target] : [source, target, spare];
        // (k | (k - 1)) + 1 wraps to 0 when n == 64; reduce modulo 3 first.
        return new Move(BitOperations.TrailingZeroCount(k) + 1,
                        peg[(k & (k - 1)) % 3],
                        peg[((k | (k - 1)) % 3 + 1) % 3]);
    }

    /// <summary>Iterative solution: the same moves as Solve, with no recursion.</summary>
    public static void SolveIterative(int n, char source, char target, char spare, Action<Move> visit)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(n, MaxDisks);
        ArgumentNullException.ThrowIfNull(visit);
        if (n == 0)
            return;
        ulong last = n == 64 ? ulong.MaxValue : (1UL << n) - 1;
        for (ulong k = 1; ; k++)
        {
            visit(MoveAt(n, k, source, target, spare));
            if (k == last)               // not k <= last: last may be ulong.MaxValue
                break;
        }
    }
}
