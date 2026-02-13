using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MMO.WorldServer.Database.Models;

public class WorldEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public Vector2 Position { get; set; }
}