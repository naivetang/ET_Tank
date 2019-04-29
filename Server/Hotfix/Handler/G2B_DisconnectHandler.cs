using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Battle)]
    class G2B_DisconnectHandler : AMHandler<G2B_Disconnect>
    {
        protected override void Run(Session session, G2B_Disconnect message)
        {

            Battle battle = Game.Scene.GetComponent<BattleComponent>().Get(message.BattleId);

            //message.PlayerId;

        }
    }
}
