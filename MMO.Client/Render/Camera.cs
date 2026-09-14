using System.Numerics;
using MMO.Client.Entities;
using Raylib_cs;

namespace MMO.Client.Render;

public class WorldCamera
{
    public float ScreenWidth { get; set; }
    public float ScreenHeight { get; set; }
    public float Zoom { get; set; } = 20f;

    public float FollowX { get; private set; }
    public float FollowY { get; private set; }

    public void Follow(Player self)
    {
        FollowX = self.X;
        FollowY = self.Y;
    }

    public Vector2 WorldToScreen(float worldX, float worldY)
    {
        return new Vector2(
            ScreenWidth  * 0.5f + (worldX - FollowX) * Zoom,
            ScreenHeight * 0.5f - (worldY - FollowY) * Zoom);
    }
}
