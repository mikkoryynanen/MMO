using System.ComponentModel.DataAnnotations;

namespace MMO.Database.Shared.Models;

public class SessionToken
{
    [Key] 
    public string Token { get; set; } = string.Empty;

    [Required] 
    public string AccountId { get; set; } = string.Empty;
    
    [Required] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required] 
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;
}