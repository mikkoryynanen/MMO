using System.ComponentModel.DataAnnotations;

namespace MMO.WorldServer.Database.Models;

public class WorldEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public int PositionX { get; set; }
    public int PositionY { get; set; }
}