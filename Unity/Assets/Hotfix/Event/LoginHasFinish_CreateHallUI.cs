using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.LoginHasFinish)]
    class LoginHasFinish_CreateHallUI : AEvent
    {
        public override void Run()
        {
            FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Hall);

            if (fui == null)
            {
                FUIFactory.Create<HallViewComponent>(FUIType.Hall).NoAwait();
            }
        }
       
    }
}
