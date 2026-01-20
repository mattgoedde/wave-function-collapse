namespace PerlinNoise.Core;

public class MapDimensions
{
    public int Width { get; }
    public int Height { get; }

    public MapDimensions(int width, int height)
    {
        if (width <= 0)
            throw new ArgumentException("Width must be positive", nameof(width));
        if (height <= 0)
            throw new ArgumentException("Height must be positive", nameof(height));

        Width = width;
        Height = height;
    }
}
