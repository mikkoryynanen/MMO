using System.ComponentModel.DataAnnotations;

namespace MMO.WorldServer.Database.Models;

public class WorldEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public float PositionX { get; set; }
    public float PositionY { get; set; }
}