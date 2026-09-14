namespace MMO.Dev.WorldAPI.Models;

public class PostEntity
{
    public string Id { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
}

public class PostEntitiesBody
{
    public PostEntity[] Entities { get; set; }
}