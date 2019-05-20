using ETModel;
using UnityEngine.SceneManagement;

namespace ETHotfix
{
    [Event(EventIdType.ChangeScene)]
    class SceneChange:AEvent
    {
        public override void Run()
        {
            this.RunAsync().NoAwait();
        }

        public async ETVoid RunAsync()
        {
            if (SceneManager.GetActiveScene().name.Equals(SceneType.Battle))
            {
                ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle(AssetBundleName.Unit);
            }

            await FUIFactory.Create<LoadingViewComponent>(FUIType.Loading);
        }
    }
}
