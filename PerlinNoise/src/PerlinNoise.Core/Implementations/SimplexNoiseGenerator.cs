namespace PerlinNoise.Core.Implementations;

using PerlinNoise.Core.Abstractions;

public class SimplexNoiseGenerator : INoiseGenerator
{
    private static readonly int[] Permutation = new int[512];
    private static readonly int[] PermutationMod12 = new int[512];

    private static readonly float[] Gradient3 =
    {
        1, 1, 0, -1, 1, 0, 1, -1, 0, -1, -1, 0,
        1, 0, 1, -1, 0, 1, 1, 0, -1, -1, 0, -1,
        0, 1, 1, 0, -1, 1, 0, 1, -1, 0, -1, -1
    };

    // fBm (Fractional Brownian Motion) parameters
    private const float Scale = 0.05f;
    private const int Octaves = 4;
    private const float Persistence = 0.5f;

    public float[,] GenerateNoise(int width, int height, int seed)
    {
        InitializePermutation(seed);
        var noise = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noise[x, y] = SampleFBm(x, y);
            }
        }

        return noise;
    }

    private float SampleFBm(float x, float y)
    {
        float value = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        float maxValue = 0f;

        for (int i = 0; i < Octaves; i++)
        {
            value += Sample(x * frequency * Scale, y * frequency * Scale) * amplitude;
            maxValue += amplitude;

            amplitude *= Persistence;
            frequency *= 2f;
        }

        return value / maxValue;
    }

    private void InitializePermutation(int seed)
    {
        var random = new Random(seed);
        var basePermutation = new int[256];

        for (int i = 0; i < 256; i++)
        {
            basePermutation[i] = i;
        }

        // Fisher-Yates shuffle
        for (int i = 255; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (basePermutation[i], basePermutation[j]) = (basePermutation[j], basePermutation[i]);
        }

        // Duplicate the permutation table
        for (int i = 0; i < 256; i++)
        {
            Permutation[i] = basePermutation[i];
            Permutation[i + 256] = basePermutation[i];
            PermutationMod12[i] = Permutation[i] % 12;
            PermutationMod12[i + 256] = Permutation[i + 256] % 12;
        }
    }

    private float Sample(float x, float y)
    {
        // Skew the input space
        float s = (x + y) * 0.5f * (MathF.Sqrt(3f) - 1f);
        int i = FastFloor(x + s);
        int j = FastFloor(y + s);

        // Unskew back to (x, y) space
        float t = (i + j) * (3f - MathF.Sqrt(3f)) / 6f;
        float x0 = x - (i - t);
        float y0 = y - (j - t);

        // Determine which simplex we're in
        int i1, j1;
        if (x0 > y0)
        {
            i1 = 1;
            j1 = 0;
        }
        else
        {
            i1 = 0;
            j1 = 1;
        }

        // Offsets for second corner
        float x1 = x0 - i1 + (3f - MathF.Sqrt(3f)) / 6f;
        float y1 = y0 - j1 + (3f - MathF.Sqrt(3f)) / 6f;

        // Offsets for third corner
        float x2 = x0 - 1 + 2f * (3f - MathF.Sqrt(3f)) / 6f;
        float y2 = y0 - 1 + 2f * (3f - MathF.Sqrt(3f)) / 6f;

        // Get gradient indices
        int gi0 = PermutationMod12[Permutation[i & 255] + (j & 255)];
        int gi1 = PermutationMod12[Permutation[(i + i1) & 255] + ((j + j1) & 255)];
        int gi2 = PermutationMod12[Permutation[(i + 1) & 255] + ((j + 1) & 255)];

        // Calculate contribution from each corner
        float n0 = ContributionGradient(gi0, x0, y0);
        float n1 = ContributionGradient(gi1, x1, y1);
        float n2 = ContributionGradient(gi2, x2, y2);

        float result = n0 + n1 + n2;

        // Normalize from [-0.42, 0.42] to [0, 1]
        return (result + 0.42f) / 0.84f;
    }

    private float ContributionGradient(int gi, float x, float y)
    {
        float t = 0.5f - x * x - y * y;
        if (t < 0)
            return 0;

        t *= t;
        return t * t * DotGradient(gi, x, y);
    }

    private float DotGradient(int gi, float x, float y)
    {
        return Gradient3[gi] * x + Gradient3[gi + 1] * y;
    }

    private static int FastFloor(float x)
    {
        int xi = (int)x;
        return x < xi ? xi - 1 : xi;
    }
}
