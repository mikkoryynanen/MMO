namespace MMO.Client.Entities;

public class Player
{
    public string Username { get; set; } = string.Empty;
    public float X { get; set; }
    public float Y { get; set; }

    public Player Clone() => new()
    {
        Username = Username,
        X = X,
        Y = Y,
    };
}
