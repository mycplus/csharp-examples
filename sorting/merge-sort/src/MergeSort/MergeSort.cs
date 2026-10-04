// MergeSort.cs - top-down merge sort in C#
using System;

namespace MyCPlus.Sorting;

public static class MergeSort
{
    // Stable: on a tie the left element goes first.
    public static void Sort(int[] a)
    {
        int[] buf = new int[a.Length];       // one buffer for every merge
        SortRange(a, buf, 0, a.Length);
    }

    private static void SortRange(int[] a, int[] buf, int lo, int hi)   // [lo, hi)
    {
        if (hi - lo < 2)
            return;
        int mid = lo + (hi - lo) / 2;
        SortRange(a, buf, lo, mid);
        SortRange(a, buf, mid, hi);
        int i = lo, j = mid, k = lo;
        while (i < mid && j < hi)
            buf[k++] = (a[j] < a[i]) ? a[j++] : a[i++];
        while (i < mid) buf[k++] = a[i++];
        while (j < hi) buf[k++] = a[j++];
        Array.Copy(buf, lo, a, lo, hi - lo);
    }
}
