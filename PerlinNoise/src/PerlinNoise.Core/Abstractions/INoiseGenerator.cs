namespace PerlinNoise.Core.Abstractions;

public interface INoiseGenerator
{
    float[,] GenerateNoise(int width, int height, int seed);
}
