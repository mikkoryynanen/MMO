using System.ComponentModel.DataAnnotations;

namespace MMO.Database.Migrations.Models;

public class PlayerAccount
{
    [Key]
    public string AccountId { get; set; } = Guid.NewGuid().ToString();

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsBanned { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}