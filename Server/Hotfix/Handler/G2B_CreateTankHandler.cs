using System;
using PF;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Battle)]
    public class G2B_CreateTankHandler : AMRpcHandler<G2B_CreateTank,B2G_CreateTank>
    {
        protected override void Run(Session session, G2B_CreateTank message, Action<B2G_CreateTank> reply)
        {
            RunAsync(session,message,reply).NoAwait();
        }

        private async ETVoid RunAsync(Session session, G2B_CreateTank message, Action<B2G_CreateTank> reply)
        {
            B2G_CreateTank response = new B2G_CreateTank();
            try
            {

                Battle battle = Game.Scene.GetComponent<BattleComponent>().Get(message.BattleId);
                
                // 随机生成一辆坦克，id和instanceId不相等
                Tank tank = ComponentFactory.CreateWithId<Tank>(IdGenerater.GenerateId());

                tank.PlayerId = message.PlayerId;

                tank.Battle = battle;

                tank.Name = message.Name;

                tank.AddComponent<NumericComponent>();

                battle.Add(tank);

                tank.TankCamp = message.Camp == 1? TankCamp.Blue : TankCamp.Red;

                await tank.AddComponent<MailBoxComponent>().AddLocation();

                tank.AddComponent<TankGateComponent, long>(message.GateSessionId);

                response.TankId = tank.Id;

                reply(response);

            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
