// InsertionSort.cs - insertion sort in C#
namespace MyCPlus.Sorting;

public static class InsertionSort
{
    // Stable, in place.
    public static void Sort(int[] a)
    {
        for (int i = 1; i < a.Length; i++)
        {
            int v = a[i];
            int j = i;
            while (j > 0 && v < a[j - 1])
            {
                a[j] = a[j - 1];
                j--;
            }
            a[j] = v;
        }
    }
}
