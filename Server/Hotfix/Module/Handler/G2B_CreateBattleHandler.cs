using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Battle)]
    public class G2B_CreateBattleHandler : AMRpcHandler<G2B_CreateBattle, B2G_CreateBattle>
    {
        protected override void Run(Session session, G2B_CreateBattle message, Action<B2G_CreateBattle> reply)
        {
            B2G_CreateBattle response = new B2G_CreateBattle();
            try
            {
               

                BattleComponent battleComponent = Game.Scene.GetComponent<BattleComponent>();

                Battle battle = ComponentFactory.CreateWithId<Battle, G2B_CreateBattle>(message.RoomId, message);

                battleComponent.Add(battle);

                reply(response);
            }
            catch (Exception e)
            {
                Log.Error(e);
                ReplyError(response, e, reply);
            }
            

            
        }
    }
}
