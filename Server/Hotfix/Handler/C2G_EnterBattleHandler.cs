using System;
using System.Net;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_EnterBattleHandler : AMRpcHandler<C2G_EnterBattle,G2C_EnterBattle>
    {
        protected override void Run(Session session, C2G_EnterBattle message, Action<G2C_EnterBattle> reply)
        {
            RunAsync(session,message,reply).NoAwait();
        }

        private async ETVoid RunAsync(Session session, C2G_EnterBattle message, Action<G2C_EnterBattle> reply)
        {
            G2C_EnterBattle response = new G2C_EnterBattle();
            try
            {
                // 这个组件是在登陆成功的时候挂上的
                Player player = session.GetComponent<SessionPlayerComponent>().Player;
                // 在Battle服务器创建战斗
                IPEndPoint mapAddress = StartConfigComponent.Instance.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
                Session battleSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);
                B2G_CreateTank createTank = (B2G_CreateTank)await battleSession.Call(new G2B_CreateTank() { PlayerId = player.Id, GateSessionId = session.InstanceId });
                player.TankId = createTank.TankId;
                response.TankId = createTank.TankId;
                reply(response);

                // 广播创建的Tank

                B2C_CreateTanks createTanks = new B2C_CreateTanks();

                Tank[] tanks = Game.Scene.GetComponent<TankComponent>().GetAll();

                foreach (Tank t in tanks)
                {
                    TankInfo tankInfo = new TankInfo();
                    tankInfo.TankId = t.Id;
                    tankInfo.PX = t.Position.x;
                    tankInfo.PY = t.Position.y;
                    tankInfo.PZ = t.Position.z;
                    tankInfo.RX = t.Rotation.x;
                    tankInfo.RY = t.Rotation.y;
                    tankInfo.RZ = t.Rotation.z;
                    tankInfo.TurretRY = t.TurretRY;
                    tankInfo.GunRX = t.GunRX;
                    createTanks.Tanks.Add(tankInfo);
                }

                Log.Info("广播坦克");
                MessageHelper.BroadcastTank(createTanks);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
