using MMO.WorldServer.Database.Models;

namespace MMO.WorldServer.Map;

public class WorldMap
{
   // TODO Size of the screen for now
   private const int WORLD_HEIGHT = 180;
   private const int WORLD_WIDTH = 320;

   private WorldEntity?[,] map;
   
   public void Populate(WorldEntity[] entities)
   {
      map = new WorldEntity[WORLD_WIDTH, WORLD_HEIGHT];
      
      // TODO Load world context
      for (int x = 0; x < WORLD_WIDTH; x++)
      {
         for (int y = 0; y < WORLD_HEIGHT; y++)
         {
            map[x, y] = GetCellEntity(entities, x, y);
         } 
      }
   }

   public bool IsColliding(float requestedPosX, float requestedPosY)
   {
      int ix = (int)requestedPosX;
      int iy = (int)requestedPosY;
      if (ix < 0 || iy < 0 || ix >= WORLD_WIDTH || iy >= WORLD_HEIGHT)
         return true; // out-of-bounds = wall

      var cell = map[ix, iy];
      return cell is not null;
   }

   private WorldEntity? GetCellEntity(WorldEntity[] entities, int x, int y)
   {
      foreach (var entity in entities)
      {
         if (entity.PositionX == x && entity.PositionY == y)
         {
            return entity;
         }
      }

      return null;
   }
}