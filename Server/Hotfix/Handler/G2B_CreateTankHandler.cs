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
                // 随机生成一辆坦克，id和instanceId不相等
                Tank tank = ComponentFactory.CreateWithId<Tank>(IdGenerater.GenerateId());

                await tank.AddComponent<MailBoxComponent>().AddLocation();

                tank.AddComponent<TankGateComponent, long>(message.GateSessionId);

                Game.Scene.GetComponent<TankComponent>().Add(tank);

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
