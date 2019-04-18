using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.LoginHasFinish)]
    class LoginHasFinish_CreateHallUI : AEvent
    {
        public override void Run()
        {
            Game.Scene.GetComponent<FUIComponent>().Remove(FUIType.Login);

            // 卸载包
            ETModel.Game.Scene.GetComponent<FUIPackageComponent>().RemovePackage(FUIType.Login);

        }
       
    }
}
