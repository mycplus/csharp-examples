// Program.cs - the recursive Towers of Hanoi solution in C#.
// Run: dotnet run
using System;

namespace MyCPlus.Recursion;

public static class TowersOfHanoi
{
    // Moves n disks from source to target, using spare as the third peg,
    // prints each move, and returns the number of moves made.
    public static ulong Hanoi(int n, char source, char target, char spare)
    {
        if (n <= 0)
            return 0;                                   // nothing to move

        ulong moves = Hanoi(n - 1, source, spare, target);
        Console.WriteLine($"Move disk {n} from {source} to {target}");
        moves++;
        moves += Hanoi(n - 1, spare, target, source);
        return moves;
    }

    public static void Main()
    {
        const int n = 3;
        ulong moves = Hanoi(n, 'A', 'C', 'B');
        Console.WriteLine($"{n} disks: {moves} moves");
    }
}
