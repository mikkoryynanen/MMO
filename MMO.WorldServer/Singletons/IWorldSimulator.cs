using MMO.Shared.Packets;

namespace MMO.WorldServer.Singletons;

public interface IWorldSimulator
{
   void Start();
   void Stop();
   void AddPlayer(Session session);
   void EnqueueInput(PlayerInput input);
   public bool HasInput(InputPacket input);
}