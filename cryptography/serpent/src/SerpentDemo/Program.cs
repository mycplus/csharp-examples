using System;
using System.Text;

namespace MyCPlus.Cryptography;

// Encrypts and decrypts one block with a 256-bit key, then checks two
// published test vectors: one in NESSIE byte order, and one from the Serpent
// AES submission, which writes every value byte-reversed.
// Run: dotnet run --project src/SerpentDemo
public static class Program
{
    private static string Hex(ReadOnlySpan<byte> bytes) => Convert.ToHexStringLower(bytes);

    public static void Main()
    {
        byte[] key = new byte[32];
        for (int i = 0; i < key.Length; i++)
            key[i] = (byte)i;                                   // 00 01 02 ... 1f
        byte[] plain = Encoding.ASCII.GetBytes("Serpent: 128-bit");   // 16 bytes

        byte[] encrypted, back;
        using (var cipher = new Serpent(key))
        {
            encrypted = cipher.Encrypt(plain);
            back = cipher.Decrypt(encrypted);
        }

        Console.WriteLine($"{"key",-11}{Hex(key)}");
        Console.WriteLine($"{"plaintext",-11}{Hex(plain)}");
        Console.WriteLine($"{"ciphertext",-11}{Hex(encrypted)}");
        Console.WriteLine($"decrypts back: {(back.AsSpan().SequenceEqual(plain) ? "yes" : "no")}");

        // NESSIE: 128-bit key 80 00 .. 00, zero block -> 264e5481...
        byte[] k128 = new byte[16];
        k128[0] = 0x80;
        using var nessieCipher = new Serpent(k128);
        bool nessieOk = Hex(nessieCipher.Encrypt(new byte[16])) == "264e5481eff42a4606abda06c0bfda3d";
        Console.WriteLine($"NESSIE vector:          {(nessieOk ? "match" : "MISMATCH")}");

        // AES submission: 256-bit zero key, zero block -> 89104945... once the
        // output bytes are reversed (the key and block are all zero either way).
        using var zeroCipher = new Serpent(new byte[32]);
        byte[] output = zeroCipher.Encrypt(new byte[16]);
        Array.Reverse(output);
        bool submissionOk = Hex(output) == "8910494504181950f98dd998a82b6749";
        Console.WriteLine($"AES submission vector:  {(submissionOk ? "match" : "MISMATCH")}");
    }
}
