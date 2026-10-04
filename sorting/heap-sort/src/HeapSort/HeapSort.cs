// HeapSort.cs - heap sort in C#
namespace MyCPlus.Sorting;

public static class HeapSort
{
    // Max-heap in the array: children of i at 2i + 1 and 2i + 2. Not stable.
    public static void Sort(int[] a)
    {
        int n = a.Length;
        for (int i = n / 2 - 1; i >= 0; i--)
            SiftDown(a, i, n);
        for (int end = n - 1; end > 0; end--)
        {
            int t = a[0]; a[0] = a[end]; a[end] = t;
            SiftDown(a, 0, end);
        }
    }

    private static void SiftDown(int[] a, int root, int n)
    {
        while (true)
        {
            int child = 2 * root + 1;
            if (child >= n)
                return;
            if (child + 1 < n && a[child] < a[child + 1])
                child++;
            if (a[root] >= a[child])
                return;
            int t = a[root]; a[root] = a[child]; a[child] = t;
            root = child;
        }
    }
}
