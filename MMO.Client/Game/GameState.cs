using MMO.Client.Entities;

namespace MMO.Client.Game;

public class GameState
{
    public Player Self { get; set; } = new();

    public Guid? SelfId { get; set; }

    public Dictionary<Guid, Player> RemotePlayers { get; } = new();

    public string? LastEvent { get; set; }

    public void ApplySelfPosition(float x, float y)
    {
        Self.X = x;
        Self.Y = y;
    }

    public void ApplyRemotePosition(Guid id, float x, float y)
    {
        // Self-join hasn't arrived yet — server will re-broadcast next tick, and
        // the next position echo will arrive after our SelfId is known. Drop for now.
        if (SelfId is null)
            return;

        if (id == SelfId)
        {
            Self.X = x;
            Self.Y = y;
            return;
        }

        if (!RemotePlayers.TryGetValue(id, out var p))
        {
            p = new Player { Id = id, Username = $"Player-{id.ToString()[..8]}" };
            RemotePlayers[id] = p;
        }
        p.X = x;
        p.Y = y;
    }

    public void ApplyRemoteJoin(Guid id, string username)
    {
        if (SelfId is null)
        {
            // First PlayerJoinedPacket we see is the server's self-join handshake —
            // server includes the new session in the join broadcast so we know our Id.
            SelfId = id;
            if (!string.IsNullOrEmpty(username))
                Self.Username = username;
            LastEvent = "Connected";
            return;
        }

        if (id == SelfId)
        {
            if (!string.IsNullOrEmpty(username))
                Self.Username = username;
            return;
        }

        if (!RemotePlayers.TryGetValue(id, out var p))
        {
            p = new Player { Id = id, Username = username };
            RemotePlayers[id] = p;
        }
        else if (!string.IsNullOrEmpty(username))
        {
            p.Username = username;
        }
        LastEvent = $"{username} joined";
    }
}
