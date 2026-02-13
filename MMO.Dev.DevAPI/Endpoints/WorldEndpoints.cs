using MMO.Dev.WorldAPI.Models;
using MMO.Dev.WorldAPI.Services;

namespace MMO.Dev.WorldAPI.Endpoints;

public static class WorldEndpoints
{
    public static void MapWorldEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/world");
        group.MapPost("/entities", PostEntities);
        // TODO Could just update 1 entity in the future, doing everything for now
        // group.MapPatch("/entity", UpdateEntity);
    }

    private static async Task<IResult> PostEntities(PostEntitiesBody body, DatabaseService db)
    {
        await db.CreateWorldEntities(body);

        return Results.Ok();
    }
}