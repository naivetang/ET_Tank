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
            
        }

       
    }
}
