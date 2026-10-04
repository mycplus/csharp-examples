// SelectionSort.cs - selection sort in C#
namespace MyCPlus.Sorting;

public static class SelectionSort
{
    // In place, at most n - 1 swaps. Not stable.
    public static void Sort(int[] a)
    {
        for (int i = 0; i + 1 < a.Length; i++)
        {
            int min = i;
            for (int j = i + 1; j < a.Length; j++)
                if (a[j] < a[min])
                    min = j;
            if (min != i)
            {
                int t = a[i]; a[i] = a[min]; a[min] = t;
            }
        }
    }
}
