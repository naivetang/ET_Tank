using ETModel;
using PF;
using Vector3 = UnityEngine.Vector3;

namespace ETHotfix
{
    [MessageHandler]
    class B2C_BattleEndHandler:AMHandler<B2C_BattleEnd>
    {
        protected override void Run(ETModel.Session session, B2C_BattleEnd message)
        {
            RunAsync(session,message).NoAwait();
        }

        protected async ETVoid RunAsync(ETModel.Session session, B2C_BattleEnd message)
        {
            ETModel.Game.Scene.GetComponent<TankComponent>().RemoveAll();

            // 加载Unit资源
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();

            // 加载场景资源
            await resourcesComponent.LoadBundleAsync("start.unity3d");
            using (SceneChangeComponent sceneChangeComponent = Game.Scene.AddComponent<SceneChangeComponent>())
            {
                await sceneChangeComponent.ChangeSceneAsync(SceneType.Start);
            }

            await FUIFactory.Create<CombatSettlementViewComponent, B2C_BattleEnd>(FUIType.CombatSettlement, message);
        }
    }
}
