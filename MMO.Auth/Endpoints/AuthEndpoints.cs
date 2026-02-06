namespace MMO.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");
        group.MapGet("/login", Login);
        group.MapGet("/create", Create);
    }

    private static Task Create(HttpContext context)
    {
        // TODO
        // Get get request data (e.g., username and password)
        // Create user in database
        // Return success or failure response
        
        throw new NotImplementedException();
    }

    private static Task Login(HttpContext context)
    {
        // TODO
        // Get get request data (e.g., username and password)
        
        return Task.CompletedTask;
    }
}