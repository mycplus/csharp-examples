// Program.cs - sorts the article's array with InsertionSort.Sort
using System;

namespace MyCPlus.Sorting;

public static class Program
{
    public static void Main()
    {
        int[] data = { 29, 10, 14, 37, 13, 5, 41, 22 };
        InsertionSort.Sort(data);
        Console.WriteLine("Sorted: [" + string.Join(", ", data) + "]");
    }
}
