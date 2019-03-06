using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class FUILoginComponentSystem : AwakeSystem<FUILoginComponent>
    {
        public override void Awake(FUILoginComponent self)
        {
            FUI login = self.GetParent<FUI>();

            self.AccountInput = login.Get("AccountInput");
            
            login.Get("LoginBtn").GObject.asButton.onClick.Add(() => LoginBtnOnClick(self));
        }

        public static void LoginBtnOnClick(FUILoginComponent self)
        {
            LogicHelper.Login(self.AccountInput.Get("Input").GObject.asTextInput.text).NoAwait();
        }
    }
}