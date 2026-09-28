using System;
using System.Collections.Generic;

namespace MyCPlus.Stack;

// Counts how often Stack<int> reallocates over a million pushes.
// EnsureCapacity(0) never grows the stack; it returns the current capacity.
public static class Program
{
    public static void Main()
    {
        var s = new Stack<int>();
        int last = s.EnsureCapacity(0);
        int reallocations = 0;
        for (int i = 0; i < 1_000_000; i++)
        {
            s.Push(i);
            int capacity = s.EnsureCapacity(0);
            if (capacity != last)
            {
                reallocations++;
                last = capacity;
            }
        }
        Console.WriteLine($"Stack<int>: {reallocations} reallocations for 1000000 pushes, " +
                          $"final capacity {last} (.NET {Environment.Version})");
    }
}
