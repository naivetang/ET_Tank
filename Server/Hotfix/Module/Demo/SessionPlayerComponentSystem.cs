using ETModel;

namespace ETHotfix
{
	[ObjectSystem]
	public class SessionPlayerComponentDestroySystem : DestroySystem<SessionPlayerComponent>
	{
		public override void Destroy(SessionPlayerComponent self)
		{
			// 发送断线消息
            long id = self.Player.UnitId == 0L? self.Player.TankId : self.Player.UnitId;
			ActorLocationSender actorLocationSender = Game.Scene.GetComponent<ActorLocationSenderComponent>().Get(id);
			//actorLocationSender.Send(new G2M_SessionDisconnect());
			actorLocationSender.Send(new G2B_SessionDisconnect());
			Game.Scene.GetComponent<PlayerComponent>()?.Remove(self.Player.Id);
		}
	}
}