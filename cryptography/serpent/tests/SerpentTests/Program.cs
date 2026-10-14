using System;
using System.Collections.Generic;
using System.IO;

namespace MyCPlus.Cryptography.Tests;

// Every NESSIE single-block vector in both directions, plus key and block
// length validation. Dependency-free: dotnet run --project tests/SerpentTests
public static class Program
{
    private static bool Rejected(Action action)
    {
        try
        {
            action();
            return false;
        }
        catch (ArgumentException)
        {
            return true;
        }
    }

    public static int Main(string[] args)
    {
        string path = args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "serpent_nessie.txt");
        int total = 0, failures = 0;
        var passedByBits = new Dictionary<int, int> { [128] = 0, [192] = 0, [256] = 0 };
        foreach (string line in File.ReadLines(path))
        {
            string[] f = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (line.StartsWith('#') || f.Length != 3)
                continue;
            total++;
            byte[] key = Convert.FromHexString(f[0]);
            byte[] plain = Convert.FromHexString(f[1]);
            byte[] expected = Convert.FromHexString(f[2]);
            using var cipher = new Serpent(key);
            if (cipher.Encrypt(plain).AsSpan().SequenceEqual(expected) &&
                cipher.Decrypt(expected).AsSpan().SequenceEqual(plain))
                passedByBits[key.Length * 8]++;
            else
                failures++;
        }
        Console.WriteLine($"{total} vectors: {passedByBits[128]} with 128-bit keys, {passedByBits[192]} with 192-bit, " +
                          $"{passedByBits[256]} with 256-bit; {failures} failed");

        bool badLengths = Rejected(() => new Serpent([]).Dispose()) &&
                          Rejected(() => new Serpent(new byte[33]).Dispose());
        bool oneByte = !Rejected(() => new Serpent(new byte[1]).Dispose());
        bool badBlock = Rejected(() =>
        {
            using var s = new Serpent(new byte[16]);
            s.Encrypt(new byte[15]);
        });
        Console.WriteLine($"rejects 0- and 33-byte keys: {(badLengths ? "yes" : "no")}; " +
                          $"accepts a 1-byte key: {(oneByte ? "yes" : "no")}");
        return failures == 0 && total > 0 && badLengths && oneByte && badBlock ? 0 : 1;
    }
}
