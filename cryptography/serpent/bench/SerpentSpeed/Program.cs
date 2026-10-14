using System;
using System.Diagnostics;

namespace MyCPlus.Cryptography.Bench;

// Encryption speed in ECB mode over a buffer of N MiB (default 16), median of
// 5 runs after one warm-up run for the JIT. Every language's benchmark fills
// the buffer the same way and prints the XOR of all ciphertext blocks.
// Usage: dotnet run --project bench/SerpentSpeed -c Release -- [MiB]
public static class Program
{
    public static void Main(string[] args)
    {
        int mib = args.Length > 0 ? int.Parse(args[0], System.Globalization.CultureInfo.InvariantCulture) : 16;
        int bytes = mib << 20;
        byte[] input = new byte[bytes], output = new byte[bytes];
        for (int i = 0; i < bytes; i++)
            input[i] = (byte)(unchecked((uint)i * 2654435761u) >> 24);
        byte[] key = new byte[32];
        for (int i = 0; i < 32; i++)
            key[i] = (byte)i;
        using var cipher = new Serpent(key);

        double[] mbps = new double[5];
        for (int run = -1; run < 5; run++)                       // run -1 warms up the JIT
        {
            long start = Stopwatch.GetTimestamp();
            for (int off = 0; off < bytes; off += 16)
                cipher.Encrypt(input.AsSpan(off, 16), output.AsSpan(off, 16));
            double seconds = Stopwatch.GetElapsedTime(start).TotalSeconds;
            if (run >= 0)
                mbps[run] = bytes / 1e6 / seconds;
        }
        Array.Sort(mbps);
        byte[] fold = new byte[16];
        for (int i = 0; i < bytes; i++)
            fold[i % 16] ^= output[i];
        Console.WriteLine($"{".NET " + Environment.Version.ToString(2),-12} {mbps[2],8:F2} MB/s (median of 5, {mib} MiB)  " +
                          $"xor of blocks {Convert.ToHexStringLower(fold)}");
    }
}
