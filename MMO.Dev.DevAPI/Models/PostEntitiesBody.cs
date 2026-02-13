namespace MMO.Dev.WorldAPI.Models;

public class PostEntity
{
    public string Id { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
}

public class PostEntitiesBody
{
    public PostEntity[] Entities { get; set; }
}