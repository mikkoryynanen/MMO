using MMO.Client.Entities;

namespace MMO.Client.Game;

public class GameState
{
    public Player Self { get; set; } = new();

    public Dictionary<string, Player> RemotePlayers { get; } = new();

    public string? LastEvent { get; set; }

    public void ApplySelfPosition(float x, float y)
    {
        Self.X = x;
        Self.Y = y;
    }

    public void ApplyRemotePosition(string username, float x, float y)
    {
        if (!RemotePlayers.TryGetValue(username, out var p))
        {
            p = new Player { Username = username };
            RemotePlayers[username] = p;
        }
        p.X = x;
        p.Y = y;
    }

    public void ApplyRemoteJoin(string username, float x = 0, float y = 0)
    {
        if (!RemotePlayers.ContainsKey(username))
        {
            RemotePlayers[username] = new Player { Username = username, X = x, Y = y };
            LastEvent = $"{username} joined";
        }
    }
}
