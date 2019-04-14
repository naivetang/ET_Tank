using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.LoginHasFinish)]
    class LoginHasFinish_CreateHallUI : AEvent
    {
        public override void Run()
        {
            RunAsync().NoAwait();
        }
        public async ETVoid RunAsync()
        {
            FUI fui = await FUIFactory.Create<HallViewComponent>(FUIType.Hall);
        }

       
    }
}
