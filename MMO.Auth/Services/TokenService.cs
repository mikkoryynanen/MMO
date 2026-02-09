namespace MMO.Auth.Services;

public class TokenService : ITokenService
{
    public string GenerateToken()
    {
        return Guid.NewGuid().ToString("N");
    }
}