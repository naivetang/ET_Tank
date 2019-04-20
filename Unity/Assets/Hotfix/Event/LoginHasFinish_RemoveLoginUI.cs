using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.LoginHasFinish)]
    class LoginHasFinish_RemoveLoginUI : AEvent
    {
        public override void Run()
        {
            if (Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Login) == null)

                return;

            Game.Scene.GetComponent<FUIComponent>().Remove(FUIType.Login);

            // 卸载包
            ETModel.Game.Scene.GetComponent<FUIPackageComponent>().RemovePackage(FUIType.Login);
        }
    }
}
