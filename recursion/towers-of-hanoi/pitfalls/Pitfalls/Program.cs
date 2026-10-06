// DO NOT COPY these methods: each one is a mistake the article describes.
// Run: dotnet run --project pitfalls/Pitfalls -- shift
//      dotnet run --project pitfalls/Pitfalls -- base-case-one N
using System;
using System.Globalization;

namespace MyCPlus.Recursion.Pitfalls;

public static class Program
{
    // 2^n - 1 computed with a shift. C# masks the shift count to its low
    // 5 bits for int (6 bits for long), so 1 << 32 is 1 and the "move count"
    // for 32 disks is 0.
    private static void ShiftOverflow()
    {
        foreach (int n in new[] { 3, 30, 31, 32, 33 })
        {
            int moves = (1 << n) - 1;
            Console.WriteLine($"{n} disks: {moves} moves (int)");
        }
        foreach (int n in new[] { 63, 64 })
        {
            long moves = (1L << n) - 1;
            Console.WriteLine($"{n} disks: {moves} moves (long)");
        }
    }

    // The base case is n == 1, so n == 0 never reaches it. .NET cannot catch
    // a stack overflow: the runtime terminates the process.
    private static long BaseCaseOne(int n, char source, char target, char spare)
    {
        if (n == 1)
            return 1;
        return BaseCaseOne(n - 1, source, spare, target) + 1
             + BaseCaseOne(n - 1, spare, target, source);
    }

    public static int Main(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);
        if (args.Length >= 1 && args[0] == "shift")
        {
            ShiftOverflow();
            return 0;
        }
        if (args.Length == 2 && args[0] == "base-case-one")
        {
            int n = int.Parse(args[1], CultureInfo.InvariantCulture);
            Console.WriteLine($"solving for {n} disks");
            Console.WriteLine($"{n} disks: {BaseCaseOne(n, 'A', 'C', 'B')} moves");
            return 0;
        }
        Console.Error.WriteLine("usage: Pitfalls shift | base-case-one N");
        return 2;
    }
}
