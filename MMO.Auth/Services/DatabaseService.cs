using Microsoft.EntityFrameworkCore;
using MMO.Auth.Models;
using MMO.Database.Shared;
using MMO.Database.Shared.Models;

namespace MMO.Auth.Services;

public class DatabaseService
{
    private readonly AccountDbContext _context;
    private readonly Mapper _mapper;

    public DatabaseService(AccountDbContext context)
    {
        _context = context;
        _mapper = new Mapper();
    }
    
    public async Task<PlayerAccountDTO?> CreateAccountAsync(CreateAccountRequestBody body)
    {
        try
        {
            var userExists = await _context.Accounts.AnyAsync(a => a.Username == body.Username);
            if (!userExists)
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(body.Password);
                var t = await _context.Accounts.AddAsync(new PlayerAccount()
                {
                    AccountId = Guid.NewGuid().ToString("N"),
                    Username =  body.Username,
                    PasswordHash =  passwordHash
                });
                await _context.SaveChangesAsync();
            
                return _mapper.PlayerAccountToPlayerAccountDTO(t.Entity);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return null;
    }
    
    public async Task<(PlayerAccountDTO?, SessionTokenDTO)> GetAccount(string username, string password)
    {
        try
        {

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == username);
            if (account != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, account.PasswordHash))
                {
                    var existingToken = await _context.SessionTokens.FirstOrDefaultAsync(t => t.AccountId == account.AccountId);
                    if (existingToken != null)
                    {
                        if (DateTime.UtcNow < existingToken.ExpiresAt)
                        {
                            return (_mapper.PlayerAccountToPlayerAccountDTO(account),
                                _mapper.SessionTokenToSessionTokenDTO(existingToken));
                        }
                        // Remove old one
                        _context.SessionTokens.Remove(existingToken);
                        await _context.SaveChangesAsync();
                    }
                    
                    // Generate session token
                    var token = await _context.SessionTokens.AddAsync(new SessionToken
                    {
                        Token = Guid.NewGuid().ToString("N"),
                        AccountId = account.AccountId,
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddSeconds(60)
                    });
                    await _context.SaveChangesAsync();

                    return (_mapper.PlayerAccountToPlayerAccountDTO(account), 
                        _mapper.SessionTokenToSessionTokenDTO(token.Entity));
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return (null, null);
    }
}