namespace PerlinNoise.Console;

using PerlinNoise.Core;
using Spectre.Console;

public class MapRenderer
{
    public void Render(Tile[,] tileMap)
    {
        if (tileMap == null)
            throw new ArgumentNullException(nameof(tileMap));

        int height = tileMap.GetLength(0);
        int width = tileMap.GetLength(1);

        var canvas = new Canvas(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Tile tile = tileMap[y, x];
                var color = ParseHexColor(tile.Color);
                canvas.SetPixel(x, y, color);
            }
        }

        AnsiConsole.Write(canvas);
    }

    private static Color ParseHexColor(string hexColor)
    {
        // Remove # if present
        hexColor = hexColor.TrimStart('#');

        if (hexColor.Length != 6)
            throw new ArgumentException($"Invalid hex color format: {hexColor}");

        byte r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color(r, g, b);
    }
}
