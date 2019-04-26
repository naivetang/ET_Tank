using ETModel;

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
            await FUIFactory.Create<LoadingViewComponent>(FUIType.Loading);
        }
    }
}
