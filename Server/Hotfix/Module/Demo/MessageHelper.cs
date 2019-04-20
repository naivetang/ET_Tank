using System;
using ETModel;

namespace ETHotfix
{
	public static class MessageHelper
	{
        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="message"></param>
		public static void Broadcast(IActorMessage message)
		{
			Unit[] units = Game.Scene.GetComponent<UnitComponent>().GetAll();
			ActorMessageSenderComponent actorLocationSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
			foreach (Unit unit in units)
			{
				UnitGateComponent unitGateComponent = unit.GetComponent<UnitGateComponent>();
				if (unitGateComponent.IsDisconnect)
				{
					continue;
				}

				ActorMessageSender actorMessageSender = actorLocationSenderComponent.Get(unitGateComponent.GateSessionActorId);
				actorMessageSender.Send(message);
			}
		}

        public static void BroadcastExceptSelf(this Tank self,IActorMessage message)
        {
            Tank[] tanks = self.Battle.GetAll();
            ActorMessageSenderComponent actorLocationSenderComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
            foreach (Tank tank in tanks)
            {
                if (self.Id == tank.Id)
                    continue;
                TankGateComponent tankGateComponent = tank.GetComponent<TankGateComponent>();
                if (tankGateComponent.IsDisconnect)
                {
                    continue;
                }

                ActorMessageSender actorMessageSender = actorLocationSenderComponent.Get(tankGateComponent.GateSessionActorId);


                actorMessageSender.Send(message);
            }
        }

        public static void BroadcastExceptSelf(this Player self,IMessage message)
        {
            Player[] players = Game.Scene.GetComponent<PlayerComponent>().GetAll();

            foreach (Player player in players)
            {
                if(player == self)
                    continue;

                player.Session.Send(message);
            }
        }

        public static void BroadcastPlayer(IMessage message)
        {
            Player[] players = Game.Scene.GetComponent<PlayerComponent>().GetAll();

            foreach (Player player in players)
            {
                player.Session.Send(message);
            }
        }
    }
}
