using System.Numerics;
using MMO.Client.Entities;
using MMO.Client.Game;
using Raylib_cs;

namespace MMO.Client.Render;

public class Renderer
{
    public const int WorldWidth = 320;
    public const int WorldHeight = 180;

    private const float PlayerSize = 2.2f;

    private readonly WorldCamera _camera;

    public Renderer(WorldCamera camera)
    {
        _camera = camera;
    }

    public void DrawWorld(GameState state)
    {
        Raylib.ClearBackground(new Color(20, 24, 30, 255));

        DrawGrid();
        DrawWorldBorder();

        foreach (var (_, p) in state.RemotePlayers)
            DrawPlayer(p, PlayerColors.ForUsername(p.Username));

        DrawPlayer(state.Self, PlayerColors.Self, highlight: true);
    }

    private void DrawGrid()
    {
        var gridColor = new Color(45, 50, 60, 255);
        var cell = 16;

        var minWorldX = _camera.FollowX - (_camera.ScreenWidth  * 0.5f) / _camera.Zoom - cell;
        var maxWorldX = _camera.FollowX + (_camera.ScreenWidth  * 0.5f) / _camera.Zoom + cell;
        var minWorldY = _camera.FollowY - (_camera.ScreenHeight * 0.5f) / _camera.Zoom - cell;
        var maxWorldY = _camera.FollowY + (_camera.ScreenHeight * 0.5f) / _camera.Zoom + cell;

        var startX = ((int)MathF.Floor(minWorldX / cell)) * cell;
        var endX   = ((int)MathF.Ceiling (maxWorldX / cell)) * cell;
        var startY = ((int)MathF.Floor(minWorldY / cell)) * cell;
        var endY   = ((int)MathF.Ceiling (maxWorldY / cell)) * cell;

        for (int x = startX; x <= endX; x += cell)
        {
            var top    = _camera.WorldToScreen(x, startY);
            var bottom = _camera.WorldToScreen(x, endY);
            Raylib.DrawLine((int)top.X, (int)top.Y, (int)bottom.X, (int)bottom.Y, gridColor);
        }

        for (int y = startY; y <= endY; y += cell)
        {
            var left  = _camera.WorldToScreen(startX, y);
            var right = _camera.WorldToScreen(endX,   y);
            Raylib.DrawLine((int)left.X, (int)left.Y, (int)right.X, (int)right.Y, gridColor);
        }
    }

    private void DrawWorldBorder()
    {
        var tl = _camera.WorldToScreen(0, WorldHeight);
        var br = _camera.WorldToScreen(WorldWidth, 0);
        Raylib.DrawRectangleLines(
            (int)tl.X, (int)tl.Y,
            (int)(br.X - tl.X), (int)(br.Y - tl.Y),
            new Color(120, 120, 140, 255));
    }

    private void DrawPlayer(Player p, Color color, bool highlight = false)
    {
        var pos = _camera.WorldToScreen(p.X, p.Y);
        var size = PlayerSize * _camera.Zoom;

        var rect = new Rectangle(
            pos.X - size / 2f,
            pos.Y - size / 2f,
            size,
            size);
        Raylib.DrawRectangleRec(rect, color);

        if (highlight)
        {
            Raylib.DrawRectangleLinesEx(rect, 2, new Color(255, 255, 255, 220));
        }

        var name = string.IsNullOrEmpty(p.Username) ? "?" : p.Username;
        var textSize = Raylib.MeasureText(name, 14);
        Raylib.DrawText(
            name,
            (int)(pos.X - textSize / 2),
            (int)(pos.Y - size / 2f - 18),
            14,
            new Color(230, 230, 230, 255));
    }
}
