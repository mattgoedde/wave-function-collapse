using PerlinNoise.Core.Abstractions;

namespace PerlinNoise.Core.Implementations;

public class PerlinNoiseGenerator : INoiseGenerator
{
    private const int PermutationTableSize = 256;
    private int[]? _permutation;

    public float[,] GenerateNoise(int width, int height, int seed)
    {
        InitializePermutationTable(seed);
        var noise = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noise[x, y] = Sample(x * 0.1f, y * 0.1f);
            }
        }

        return noise;
    }

    private void InitializePermutationTable(int seed)
    {
        var random = new Random(seed);
        var basePermutation = new int[PermutationTableSize];

        for (int i = 0; i < PermutationTableSize; i++)
        {
            basePermutation[i] = i;
        }

        // Fisher-Yates shuffle for deterministic randomization
        for (int i = PermutationTableSize - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (basePermutation[i], basePermutation[j]) = (basePermutation[j], basePermutation[i]);
        }

        // Duplicate the permutation table to avoid index wrapping
        _permutation = new int[PermutationTableSize * 2];
        Array.Copy(basePermutation, 0, _permutation, 0, PermutationTableSize);
        Array.Copy(basePermutation, 0, _permutation, PermutationTableSize, PermutationTableSize);
    }

    private float Sample(float x, float y)
    {
        int xi = (int)Math.Floor(x) & (PermutationTableSize - 1);
        int yi = (int)Math.Floor(y) & (PermutationTableSize - 1);

        float xf = x - (float)Math.Floor(x);
        float yf = y - (float)Math.Floor(y);

        // Fade curves
        float u = Fade(xf);
        float v = Fade(yf);

        // Hash the grid coordinates
        int h00 = Hash(_permutation![xi], yi);
        int h10 = Hash(_permutation[xi + 1], yi);
        int h01 = Hash(_permutation[xi], yi + 1);
        int h11 = Hash(_permutation[xi + 1], yi + 1);

        // Calculate gradient dot products
        float g00 = Dot(h00, xf, yf);
        float g10 = Dot(h10, xf - 1, yf);
        float g01 = Dot(h01, xf, yf - 1);
        float g11 = Dot(h11, xf - 1, yf - 1);

        // Interpolation
        float lx0 = Lerp(g00, g10, u);
        float lx1 = Lerp(g01, g11, u);
        float result = Lerp(lx0, lx1, v);

        // Normalize from [-1, 1] to [0, 1]
        return (result + 1) / 2;
    }

    private int Hash(int x, int y)
    {
        return _permutation![(_permutation[x] + y) & (PermutationTableSize - 1)];
    }

    private float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    private float Dot(int hash, float x, float y)
    {
        return (hash & 1) switch
        {
            0 when (hash & 2) == 0 => x + y,
            0 => -x + y,
            _ when (hash & 2) == 0 => x - y,
            _ => -x - y
        };
    }
}
