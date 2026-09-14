using Raylib_cs;

namespace MMO.Client.Render;

public static class PlayerColors
{
    private static readonly Color[] Palette =
    {
        new(231,  76,  60, 255),
        new( 46, 204, 113, 255),
        new( 52, 152, 219, 255),
        new(241, 196,  15, 255),
        new(155,  89, 182, 255),
        new( 26, 188, 156, 255),
        new(230, 126,  34, 255),
        new( 41, 128, 185, 255),
    };

    public static Color ForUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
            return Palette[0];
        var hash = 0u;
        foreach (var c in username) hash = hash * 31u + c;
        return Palette[hash % (uint)Palette.Length];
    }

    public static Color Self { get; } = new(255, 255, 255, 255);
}
