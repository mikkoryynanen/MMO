using Raylib_cs;

namespace MMO.Client.Game;

public class InputState
{
    public bool Up { get; private set; }
    public bool Down { get; private set; }
    public bool Left { get; private set; }
    public bool Right { get; private set; }

    public void Poll()
    {
        Up    = Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.Up);
        Down  = Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down);
        Left  = Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left);
        Right = Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right);
    }

    public (sbyte x, sbyte y) Vector()
    {
        sbyte x = (sbyte)((Right ? 1 : 0) - (Left ? 1 : 0));
        sbyte y = (sbyte)((Up    ? 1 : 0) - (Down ? 1 : 0));
        return (x, y);
    }

    public bool Any() => Up || Down || Left || Right;
}
