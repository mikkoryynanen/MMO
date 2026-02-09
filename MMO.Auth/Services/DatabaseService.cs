using Dapper;
using MMO.Auth.Models;
using MySqlConnector;

namespace MMO.Auth.Services;

public class DatabaseService
{
    private readonly MySqlConnection _conn;

    public DatabaseService(IConfiguration config)
    {
        _conn = new(config.GetConnectionString("AuthDb"));
    }
    
    public async Task<bool> CreateAccountAsync(CreateAccountRequestBody body)
    {
        try
        {
            await _conn.ExecuteAsync(@"INSERT INTO Accounts(username, password_hash) VALUES (@Username, PasswordHash)",
                new
                {
                    Username = body.Username,
                    PasswordHash = body.PasswordHash
                });
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return false;
    }
    
    public async Task GetAccount(string accountId)
    {
        // await _conn.ExecuteAsync(@"INSERT INTO SessionTokens(token, account_id, created_at, expires_at)
        //     VALUES (@Token, @AccountId, @CreatedAt, @ExpiresAt)",
        //     new
        //     {
        //         Token = sessionToken,
        //         AccountId = accountId,
        //         CreatedAt = DateTime.UtcNow.Ticks,
        //         ExpiresAt = DateTime.UtcNow.AddSeconds(60).Ticks
        //     });
    }
}