using MMO.Auth.Models;
using MMO.Auth.Services;

namespace MMO.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");
        group.MapGet("/login", Login);
        group.MapGet("/create", Create);
    }
    
    private static async Task<IResult> Create(CreateAccountRequestBody createBody, DatabaseService db)
    {
        // TODO
        // Get get request data (e.g., username and password)
        // Create user in database
        // Return success or failure response

        await db.CreateAccountAsync(createBody);
        
        return Results.NoContent();
    }

    private static async Task<IResult> Login(HttpContext context, DatabaseService db)
    {
        // TODO
        // Get get request data (e.g., username and password)

        var sessionToken = Guid.NewGuid().ToString("N");
       
        // Save token to auth table

        return Results.Ok(new { SessionToken = sessionToken });
    }
}