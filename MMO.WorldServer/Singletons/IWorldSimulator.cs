using MMO.Shared.Packets;

namespace MMO.WorldServer.Singletons;

public interface IWorldSimulator
{
   void Start();
   void Stop();
   void AddPlayer(Session session);
   public void RemovePlayer(Guid sessionId);
   void EnqueueInput(PlayerInput input);
   // public bool HasInput(InputPacket input);
}