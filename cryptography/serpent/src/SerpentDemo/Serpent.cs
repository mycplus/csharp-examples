using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Security.Cryptography;

namespace MyCPlus.Cryptography;

/// <summary>
/// The Serpent block cipher (128-bit blocks, keys up to 256 bits). Bytes are
/// read as little-endian 32-bit words, the order the NESSIE test vectors and
/// libgcrypt use.
/// </summary>
/// <remarks>
/// For learning and testing. It has not been reviewed for side channels; use a
/// maintained cryptographic library (AesGcm or ChaCha20Poly1305 in .NET) for
/// anything that protects real data.
/// </remarks>
public sealed class Serpent : IDisposable
{
    public const int BlockSize = 16;
    public const int MaxKeySize = 32;

    private static readonly byte[][] Sbox =
    [
        [3, 8, 15, 1, 10, 6, 5, 11, 14, 13, 4, 2, 7, 0, 9, 12],
        [15, 12, 2, 7, 9, 0, 5, 10, 1, 11, 14, 8, 6, 13, 3, 4],
        [8, 6, 7, 9, 3, 12, 10, 15, 13, 1, 14, 4, 0, 11, 5, 2],
        [0, 15, 11, 8, 12, 9, 6, 3, 13, 1, 2, 4, 10, 7, 5, 14],
        [1, 15, 8, 3, 12, 0, 11, 6, 2, 5, 4, 10, 9, 14, 7, 13],
        [15, 5, 2, 11, 4, 10, 9, 12, 0, 3, 14, 8, 13, 6, 7, 1],
        [7, 2, 12, 5, 8, 4, 6, 11, 14, 9, 1, 15, 13, 3, 10, 0],
        [1, 13, 15, 0, 14, 8, 2, 11, 7, 4, 12, 10, 9, 3, 5, 6],
    ];

    // Inverse S-boxes, computed from Sbox so there is no second table to mistype.
    private static readonly byte[][] SboxInverse = Invert(Sbox);

    private const uint Phi = 0x9e3779b9;   // fractional part of the golden ratio

    private readonly uint[] _subkeys = new uint[33 * 4];   // K0..K32, four words each

    /// <summary>Expands a key of 1 to 32 bytes; shorter keys are padded as the specification requires.</summary>
    public Serpent(ReadOnlySpan<byte> key)
    {
        if (key.Length is 0 or > MaxKeySize)
            throw new ArgumentException("Serpent keys are 1 to 32 bytes.", nameof(key));

        // Pad to 256 bits: one 1 bit after the key, then zeros.
        Span<byte> padded = stackalloc byte[MaxKeySize];
        padded.Clear();
        key.CopyTo(padded);
        if (key.Length < MaxKeySize)
            padded[key.Length] = 1;

        // w[0..7] hold the padded key; w[8..139] are the 132 prekey words.
        Span<uint> w = stackalloc uint[140];
        for (int i = 0; i < 8; i++)
            w[i] = BinaryPrimitives.ReadUInt32LittleEndian(padded[(4 * i)..]);
        for (int i = 8; i < 140; i++)
            w[i] = BitOperations.RotateLeft(w[i - 8] ^ w[i - 5] ^ w[i - 3] ^ w[i - 1] ^ Phi ^ (uint)(i - 8), 11);

        // Subkey i passes prekeys 4i..4i+3 through S-box (3 - i) mod 8.
        for (int i = 0; i < 33; i++)
        {
            Span<uint> k = _subkeys.AsSpan(4 * i, 4);
            w.Slice(8 + 4 * i, 4).CopyTo(k);
            ApplySbox(Sbox[(35 - i) % 8], k);        // 35 = 3 mod 8, never negative
        }
        CryptographicOperations.ZeroMemory(padded);
        CryptographicOperations.ZeroMemory(System.Runtime.InteropServices.MemoryMarshal.AsBytes(w));
    }

    /// <summary>Encrypts one 16-byte block from <paramref name="input"/> into <paramref name="output"/>.</summary>
    public void Encrypt(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Span<uint> x = stackalloc uint[4];
        Load(input, x);
        for (int r = 0; r < 32; r++)
        {
            XorKey(x, r);
            ApplySbox(Sbox[r % 8], x);
            if (r < 31)
                LinearTransform(x);
            else
                XorKey(x, 32);                   // the last round replaces LT with K32
        }
        Store(x, output);
    }

    /// <summary>Decrypts one 16-byte block from <paramref name="input"/> into <paramref name="output"/>.</summary>
    public void Decrypt(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Span<uint> x = stackalloc uint[4];
        Load(input, x);
        for (int r = 31; r >= 0; r--)
        {
            if (r == 31)
                XorKey(x, 32);
            else
                InverseLinearTransform(x);
            ApplySbox(SboxInverse[r % 8], x);
            XorKey(x, r);
        }
        Store(x, output);
    }

    public byte[] Encrypt(ReadOnlySpan<byte> input)
    {
        var output = new byte[BlockSize];
        Encrypt(input, output);
        return output;
    }

    public byte[] Decrypt(ReadOnlySpan<byte> input)
    {
        var output = new byte[BlockSize];
        Decrypt(input, output);
        return output;
    }

    /// <summary>Overwrites the expanded key.</summary>
    public void Dispose() =>
        CryptographicOperations.ZeroMemory(System.Runtime.InteropServices.MemoryMarshal.AsBytes(_subkeys.AsSpan()));

    // Bit j of x[0..3] forms a 4-bit input (x[0] is the low bit).
    private static void ApplySbox(byte[] box, Span<uint> x)
    {
        uint y0 = 0, y1 = 0, y2 = 0, y3 = 0;
        for (int j = 0; j < 32; j++)
        {
            int input = (int)((x[0] >> j) & 1 | ((x[1] >> j) & 1) << 1 | ((x[2] >> j) & 1) << 2 | ((x[3] >> j) & 1) << 3);
            uint output = box[input];
            y0 |= (output & 1) << j;
            y1 |= ((output >> 1) & 1) << j;
            y2 |= ((output >> 2) & 1) << j;
            y3 |= ((output >> 3) & 1) << j;
        }
        x[0] = y0;
        x[1] = y1;
        x[2] = y2;
        x[3] = y3;
    }

    private static void LinearTransform(Span<uint> x)
    {
        x[0] = BitOperations.RotateLeft(x[0], 13);
        x[2] = BitOperations.RotateLeft(x[2], 3);
        x[1] ^= x[0] ^ x[2];
        x[3] ^= x[2] ^ (x[0] << 3);
        x[1] = BitOperations.RotateLeft(x[1], 1);
        x[3] = BitOperations.RotateLeft(x[3], 7);
        x[0] ^= x[1] ^ x[3];
        x[2] ^= x[3] ^ (x[1] << 7);
        x[0] = BitOperations.RotateLeft(x[0], 5);
        x[2] = BitOperations.RotateLeft(x[2], 22);
    }

    private static void InverseLinearTransform(Span<uint> x)
    {
        x[2] = BitOperations.RotateRight(x[2], 22);
        x[0] = BitOperations.RotateRight(x[0], 5);
        x[2] ^= x[3] ^ (x[1] << 7);
        x[0] ^= x[1] ^ x[3];
        x[3] = BitOperations.RotateRight(x[3], 7);
        x[1] = BitOperations.RotateRight(x[1], 1);
        x[3] ^= x[2] ^ (x[0] << 3);
        x[1] ^= x[0] ^ x[2];
        x[2] = BitOperations.RotateRight(x[2], 3);
        x[0] = BitOperations.RotateRight(x[0], 13);
    }

    private void XorKey(Span<uint> x, int round)
    {
        for (int i = 0; i < 4; i++)
            x[i] ^= _subkeys[4 * round + i];
    }

    private static void Load(ReadOnlySpan<byte> input, Span<uint> x)
    {
        if (input.Length != BlockSize)
            throw new ArgumentException("Serpent blocks are 16 bytes.", nameof(input));
        for (int i = 0; i < 4; i++)
            x[i] = BinaryPrimitives.ReadUInt32LittleEndian(input[(4 * i)..]);
    }

    private static void Store(ReadOnlySpan<uint> x, Span<byte> output)
    {
        if (output.Length != BlockSize)
            throw new ArgumentException("Serpent blocks are 16 bytes.", nameof(output));
        for (int i = 0; i < 4; i++)
            BinaryPrimitives.WriteUInt32LittleEndian(output[(4 * i)..], x[i]);
    }

    private static byte[][] Invert(byte[][] boxes)
    {
        var inverse = new byte[boxes.Length][];
        for (int s = 0; s < boxes.Length; s++)
        {
            inverse[s] = new byte[16];
            for (int i = 0; i < 16; i++)
                inverse[s][boxes[s][i]] = (byte)i;
        }
        return inverse;
    }
}
