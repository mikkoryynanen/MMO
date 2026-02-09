using System.ComponentModel.DataAnnotations.Schema;

namespace MMO.Auth.Models;

public class CreateAccountRequestBody
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
}