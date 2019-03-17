using System;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class FUILobbyComponentSystem : AwakeSystem<FUILobbyComponent>
    {
        public override void Awake(FUILobbyComponent self)
        {
            FUI lobby = self.GetParent<FUI>();
            lobby.Get("EnterMapBtn").GObject.asButton.onClick.Add(() => EnterMapBtnOnClick(self));
            lobby.Get("ShopBtn").GObject.asButton.onClick.Add(() => ShopBtnOnClick(self));
        }

        public static void EnterMapBtnOnClick(FUILobbyComponent self)
        {
            EnterMapAsync().NoAwait();
        }
        
        public static void ShopBtnOnClick(FUILobbyComponent self)
        {
            Game.EventSystem.Run(EventIdType.ShopBtnOnClick);
        }

        private static async ETVoid EnterMapAsync()
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