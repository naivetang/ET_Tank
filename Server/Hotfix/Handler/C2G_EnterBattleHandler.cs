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

                Room room = Game.Scene.GetComponent<RoomComponent>().Get(message.BattleId);

                RoomOnePeople info = room.GetPlayerRoomInfo(player.Id);

                G2B_CreateTank msg = new G2B_CreateTank();

                msg.BattleId = message.BattleId;

                msg.PlayerId = player.Id;

                msg.GateSessionId = session.InstanceId;

                msg.Camp = info.Camp;

                B2G_CreateTank createTank = (B2G_CreateTank)await battleSession.Call(msg);
                player.TankId = createTank.TankId;
                response.TankId = createTank.TankId;
                reply(response);

                // 广播创建的Tank

                // B2C_CreateTanks createTanks = new B2C_CreateTanks();
                //
                // Tank[] tanks = Game.Scene.GetComponent<TankComponent>().GetAll();
                //
                // foreach (Tank t in tanks)
                // {
                //     Player p = t.Player;
                //
                //     TankFrameInfo tankFrameInfo = new TankFrameInfo();
                //
                //     tankFrameInfo.TankId = t.Id;
                //     tankFrameInfo.PX = t.PX;
                //     tankFrameInfo.PY = t.PY;
                //     tankFrameInfo.PZ = t.PZ;
                //     tankFrameInfo.RX = t.RX;
                //     tankFrameInfo.RY = t.RY;
                //     tankFrameInfo.RZ = t.RZ;
                //     tankFrameInfo.TurretRY = t.TurretRY;
                //     tankFrameInfo.GunRX = t.GunRX;
                //
                //     TankInfoFirstEnter tankInfoFirstEnter = new TankInfoFirstEnter();
                //     tankInfoFirstEnter.TankFrameInfo = tankFrameInfo;
                //
                //     tankInfoFirstEnter.TankCamp = t.TankCamp;
                //
                //     tankInfoFirstEnter.MaxHpBase = t.GetComponent<NumericComponent>()[NumericType.MaxHpBase];
                //
                //     tankInfoFirstEnter.HpBase = t.GetComponent<NumericComponent>()[NumericType.HpBase];
                //
                //     tankInfoFirstEnter.AtkBase = t.GetComponent<NumericComponent>()[NumericType.AtkBase];
                //
                //     tankInfoFirstEnter.Name = p.UserDB.Name;
                //
                //
                //     createTanks.Tanks.Add(tankInfoFirstEnter);
                // }

                //Log.Info("广播坦克");
                //MessageHelper.BroadcastBattle(createTanks);

                // if (Game.Scene.GetComponent<BattleComponent>() == null)
                //     Game.Scene.AddComponent<BattleComponent>();
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }

        // private async ETVoid RunAsync(Session session, C2G_EnterBattle message, Action<G2C_EnterBattle> reply)
        // {
        //     G2C_EnterBattle response = new G2C_EnterBattle();
        //     try
        //     {
        //         // 这个组件是在登陆成功的时候挂上的
        //         Player player = session.GetComponent<SessionPlayerComponent>().Player;
        //         // 在Battle服务器创建战斗
        //         IPEndPoint mapAddress = StartConfigComponent.Instance.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
        //         Session battleSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);
        //         B2G_CreateTank createTank = (B2G_CreateTank)await battleSession.Call(new G2B_CreateTank() { PlayerId = player.Id, GateSessionId = session.InstanceId });
        //         player.TankId = createTank.TankId;
        //         response.TankId = createTank.TankId;
        //         reply(response);
        //
        //         // 广播创建的Tank
        //
        //         B2C_CreateTanks createTanks = new B2C_CreateTanks();
        //
        //         //Player[] players = Game.Scene.GetComponent<PlayerComponent>().GetAll();
        //
        //         Tank[] tanks = Game.Scene.GetComponent<TankComponent>().GetAll();
        //
        //         foreach (Tank t in tanks)
        //         {
        //             Player p = t.Player;
        //
        //             TankFrameInfo tankFrameInfo = new TankFrameInfo();
        //             
        //             tankFrameInfo.TankId = t.Id;
        //             tankFrameInfo.PX = t.PX;
        //             tankFrameInfo.PY = t.PY;
        //             tankFrameInfo.PZ = t.PZ;
        //             tankFrameInfo.RX = t.RX;
        //             tankFrameInfo.RY = t.RY;
        //             tankFrameInfo.RZ = t.RZ;
        //             tankFrameInfo.TurretRY = t.TurretRY;
        //             tankFrameInfo.GunRX = t.GunRX;
        //
        //             TankInfoFirstEnter tankInfoFirstEnter = new TankInfoFirstEnter();
        //             tankInfoFirstEnter.TankFrameInfo = tankFrameInfo;
        //
        //             tankInfoFirstEnter.TankCamp = t.TankCamp;
        //
        //             tankInfoFirstEnter.MaxHpBase = t.GetComponent<NumericComponent>()[NumericType.MaxHpBase];
        //
        //             tankInfoFirstEnter.HpBase = t.GetComponent<NumericComponent>()[NumericType.HpBase];
        //
        //             tankInfoFirstEnter.AtkBase = t.GetComponent<NumericComponent>()[NumericType.AtkBase];
        //
        //             tankInfoFirstEnter.Name = p.UserDB.Name;
        //
        //
        //             createTanks.Tanks.Add(tankInfoFirstEnter);
        //         }
        //
        //         Log.Info("广播坦克");
        //         MessageHelper.BroadcastBattle(createTanks);
        //
        //         if(Game.Scene.GetComponent<BattleComponent>()==null)
        //             Game.Scene.AddComponent<BattleComponent>();
        //     }
        //     catch (Exception e)
        //     {
        //         ReplyError(response, e, reply);
        //     }
        // }
    }
}
