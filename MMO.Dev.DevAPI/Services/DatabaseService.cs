using Microsoft.EntityFrameworkCore;
using MMO.Database.Shared;
using MMO.Dev.WorldAPI.Models;
using MMO.WorldServer.Database.Models;

namespace MMO.Dev.WorldAPI.Services;

public class DatabaseService
{
    private readonly WorldDbContext _context;
    // private readonly Mapper _mapper;

    public DatabaseService(WorldDbContext context)
    {
        _context = context;
        // _mapper = new Mapper();
    }
    
    public async Task CreateWorldEntities(PostEntitiesBody body)
    {
        try
        {
            foreach (var entity in body.Entities)
            {
                var foundEntity = await _context.WorldEntities.FirstOrDefaultAsync(e => e.Id == entity.Id);
                // Only CREATE non-existing entities
                if (foundEntity == null)
                {
                    await _context.WorldEntities.AddAsync(new WorldEntity
                    {
                        Id =  entity.Id,
                        PositionX = entity.PositionX,
                        PositionY = entity.PositionY
                    });
                }
                // UPDATE existing entities
                else
                {
                    foundEntity.PositionX = entity.PositionX;
                    foundEntity.PositionY = entity.PositionY;
                }
            }
            
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}