using MMO.Auth.Models;
using MMO.Auth.Services;

namespace MMO.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");
        group.MapPost("/login", Login);
        group.MapPost("/create", Create);
    }
    
    private static async Task<IResult> Create(CreateAccountRequestBody createBody, DatabaseService db)
    {
        // TODO
        // Get get request data (e.g., username and password)
        // Create user in database
        // Return success or failure response

        var account = await db.CreateAccountAsync(createBody);
        if (account != null)
        {
            return Results.Ok(account);
        }
        
        // TODO Add Failure reason here? Username in use? email in use?
        return Results.NoContent();
    }

    private static async Task<IResult> Login(LoginRequestBody loginRequest, DatabaseService db)
    {
        // TODO
        // Get get request data (e.g., username and password)
        
        var (account, sessionToken) = await db.GetAccount(loginRequest.Username, loginRequest.Password);
        if (account != null)
        {
            // TODO Save the session token to redis cache, using database for now

            return Results.Ok(new { SessionToken = sessionToken });
        }
        
        return Results.NoContent();
    }
}