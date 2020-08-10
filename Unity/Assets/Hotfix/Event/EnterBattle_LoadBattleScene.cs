using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.EnterBattle)]
    public class EnterBattle_LoadBattleScene : AEvent
    {
        public override void Run()
        {
            this.RunAsync().NoAwait();
        }

        public async ETVoid RunAsync()
        {
            // 加载Unit资源
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();

            // 加载场景资源

            await resourcesComponent.LoadBundleAsync(AssetBundleName.Unit);

            await resourcesComponent.LoadBundleAsync(AssetBundleName.Battle);

            

            // 切换到Battle场景
            using (SceneChangeComponent sceneChangeComponent = Game.Scene.AddComponent<SceneChangeComponent>())
            {

                await sceneChangeComponent.ChangeSceneAsync(SceneType.Battle);
            }


            

            C2B_LoadAssetFinish msg = new C2B_LoadAssetFinish();

            ETModel.SessionComponent.Instance.Session.Send(msg);

        }
    }
}
