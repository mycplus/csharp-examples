using System;
using System.Collections;
using System.Linq;

namespace MyCPlus.Primes;

public static class Primes
{
    // Trial division by 2, 3 and then 6k - 1 and 6k + 1 up to sqrt(n).
    // i <= n / i is the overflow-safe form of i * i <= n.
    public static bool IsPrime(long n)
    {
        if (n < 2)
            return false;
        if (n < 4)
            return true;                         // 2 and 3
        if (n % 2 == 0 || n % 3 == 0)
            return false;
        for (long i = 5; i <= n / i; i += 6)
        {
            if (n % i == 0 || n % (i + 2) == 0)
                return false;
        }
        return true;
    }

    // flags[k] is true when k is prime, for 0 <= k < limit.
    public static BitArray Sieve(int limit)
    {
        var flags = new BitArray(limit, true);
        for (int k = 0; k < Math.Min(limit, 2); k++)
            flags[k] = false;                    // 0 and 1
        for (int p = 2; limit > 0 && p <= (limit - 1) / p; p++)
        {
            if (!flags[p])
                continue;
            for (long m = (long)p * p; m < limit; m += p)   // long: m + p may pass int.MaxValue
                flags[(int)m] = false;
        }
        return flags;
    }

    public static int CountPrimesBelow(int limit) => Sieve(limit).Cast<bool>().Count(f => f);

    public static void Main()
    {
        BitArray flags = Sieve(100);
        Console.WriteLine("primes below 100: " +
            string.Join(' ', Enumerable.Range(0, 100).Where(k => flags[k])));
        Console.WriteLine($"primes below 1000: {CountPrimesBelow(1000)}");
        Console.WriteLine($"primes below 1000000: {CountPrimesBelow(1_000_000)}");

        long[] samples = [-7, 0, 1, 2, 91, 97];
        Console.WriteLine("is_prime: " +
            string.Join(", ", samples.Select(n => $"{n} {(IsPrime(n) ? "true" : "false")}")));
        Console.WriteLine($"is_prime(2147483647) = {(IsPrime(2147483647) ? "true" : "false")}");
        Console.WriteLine($"is_prime(1000000007) = {(IsPrime(1000000007) ? "true" : "false")}");
    }
}
