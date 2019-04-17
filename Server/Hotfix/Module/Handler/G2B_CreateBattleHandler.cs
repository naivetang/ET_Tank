using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Battle)]
    public class G2B_CreateBattleHandler : AMRpcHandler<G2B_CreateBattle, B2G_CreateBattle>
    {
        protected override void Run(Session session, G2B_CreateBattle message, Action<B2G_CreateBattle> reply)
        {
            B2G_CreateBattle response = new B2G_CreateBattle();

            BattleComponent battleComponent = Game.Scene.GetComponent<BattleComponent>();

            Battle battle = ComponentFactory.CreateWithId<Battle>(message.RoomId);

            battleComponent.Add(battle);

            reply(response);
        }
    }
}
