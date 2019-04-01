using System;
using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.GMEnterBattle)]
    public class GM_EnterBattle : AEvent
    {
        public override void Run()
        {
            EnterBattleAsync().NoAwait();
        }

        private static async ETVoid EnterBattleAsync()
        {
            try
            {
                // 加载Unit资源
                ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();

                // 加载场景资源
                await resourcesComponent.LoadBundleAsync("map.unity3d");
                // 切换到Battle场景
                using (SceneChangeComponent sceneChangeComponent = ETModel.Game.Scene.AddComponent<SceneChangeComponent>())
                {
                    await sceneChangeComponent.ChangeSceneAsync(SceneType.Battle);
                }

                // G2C_EnterMap g2CEnterMap = await ETModel.SessionComponent.Instance.Session.Call(new C2G_EnterMap()) as G2C_EnterMap;
                // PlayerComponent.Instance.MyPlayer.UnitId = g2CEnterMap.UnitId;
                //
                // Game.Scene.AddComponent<OperaComponent>();
                //
                // // 逻辑层不应该去调用UI，逻辑层只关心逻辑并且抛出事件，由UI层自己去订阅事件
                // Game.EventSystem.Run(EventIdType.EnterMapFinish);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
