using System;
using ETModel;

namespace ETHotfix
{
    public static class LogicHelper
    {
        public static async ETVoid Login(string account)
        {
            // 创建一个ETModel层的Session
            ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalConfigComponent.Instance.GlobalProto.Address);
				
            // 创建一个ETHotfix层的Session, ETHotfix的Session会通过ETModel层的Session发送消息
            Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);
            R2C_Login r2CLogin = (R2C_Login) await realmSession.Call(new C2R_Login() { Account = account, Password = "111111" });
            realmSession.Dispose();

            // 创建一个ETModel层的Session,并且保存到ETModel.SessionComponent中
            ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(r2CLogin.Address);
            ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>().Session = gateSession;
				
            // 创建一个ETHotfix层的Session, 并且保存到ETHotfix.SessionComponent中
            Game.Scene.AddComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);
				
            G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });

            Log.Info("登陆gate成功!");

            // 创建Player
            Player player = ETModel.ComponentFactory.CreateWithId<Player>(g2CLoginGate.PlayerId);
            PlayerComponent playerComponent = ETModel.Game.Scene.GetComponent<PlayerComponent>();
            playerComponent.MyPlayer = player;
            
            // 测试消息有成员是class类型
            G2C_PlayerInfo g2CPlayerInfo = (G2C_PlayerInfo) await SessionComponent.Instance.Session.Call(new C2G_PlayerInfo());
            
            // 逻辑层不应该去调用UI，逻辑层只关心逻辑并且抛出事件，由UI层自己去订阅事件，而且注意事件名字
            // 很多人容易把这个事件取名成LoginFinishiCreateLobbyUI，这是不对的，事件抛出去不可能知道谁订阅了这个事件，
            // 也不会知道别人订阅这个事件是干什么的,这里只知道我Login Finish
            Game.EventSystem.Run(EventIdType.LoginFinish);
        }

        public static async ETVoid EnterMapAsync()
        {
            try
            {
                // 加载Unit资源
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                await resourcesComponent.LoadBundleAsync($"unit.unity3d");

                // 加载场景资源
                await ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadBundleAsync("map.unity3d");
                // 切换到map场景
                using (SceneChangeComponent sceneChangeComponent = ETModel.Game.Scene.AddComponent<SceneChangeComponent>())
                {
                    await sceneChangeComponent.ChangeSceneAsync(SceneType.Map);
                }
				
                G2C_EnterMap g2CEnterMap = await ETModel.SessionComponent.Instance.Session.Call(new C2G_EnterMap()) as G2C_EnterMap;
                PlayerComponent.Instance.MyPlayer.UnitId = g2CEnterMap.UnitId;
				
                Game.Scene.AddComponent<OperaComponent>();
                
                // 逻辑层不应该去调用UI，逻辑层只关心逻辑并且抛出事件，由UI层自己去订阅事件
                Game.EventSystem.Run(EventIdType.EnterMapFinish);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }	
        }
    }
}