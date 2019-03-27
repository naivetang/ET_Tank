using ETModel;

namespace ETHotfix
{
    public class FUILoginComponent: Component
	{
		// 缓存只需要缓存FUI类型即可
		public FUI AccountInput;

	    public FUI PasswordInput;

	    public FUI RegistBtn;

	    public FUI LoginBtn;

        // 错误提示
	    public FUI ErrorPrompt;

	}



    [ObjectSystem]
    public class FUILoginComponentSystem: AwakeSystem<FUILoginComponent>
    {
        public override void Awake(FUILoginComponent self)
        {
            FUI FGUICompunt = self.GetParent<FUI>();

            self.AccountInput = FGUICompunt.Get("AccountInput");

            self.PasswordInput = FGUICompunt.Get("PasswordInput");

            self.ErrorPrompt = FGUICompunt.Get("ErrorPrompt");

            self.RegistBtn = FGUICompunt.Get("RegistBtn");

            self.LoginBtn = FGUICompunt.Get("LoginBtn");

            self.LoginBtn.GObject.asButton.onClick.Set(() => { LoginBtnOnClick(self); });

            self.RegistBtn.GObject.asButton.onClick.Set(()=>{RigistBtnOnClick(self);});

            //login.Get("LoginBtn").GObject.asButton.onClick.Add(() => LoginBtnOnClick(self));
        }

        public static void LoginBtnOnClick(FUILoginComponent self)
        {
            string account = self.AccountInput.Get("title").GObject.asTextInput.text;  
            
            string password = self.PasswordInput.Get("title").GObject.asTextInput.text;

            SetErrorPrompt(self, "");

            Login(self, account, password).NoAwait();
        }

        public static async ETVoid Login(FUILoginComponent self, string account, string password)
        {

            #region 客户端给服务器发送登陆信息
            // 创建一个ETModel层的Session
            ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalConfigComponent.Instance.GlobalProto.Address);

            Log.Info("服务器地址 : " + GlobalConfigComponent.Instance.GlobalProto.Address);
            // 创建一个ETHotfix层的Session, ETHotfix的Session会通过ETModel层的Session发送消息
            Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);
            R2C_Login r2CLogin = (R2C_Login) await realmSession.Call(new C2R_Login() { Account = account, Password = password });
            realmSession.Dispose();

            #endregion

            if (r2CLogin.Error != ErrorCode.ERR_Success)
            {
                SetErrorPrompt(self,r2CLogin.Message);
                return;
            }
            

            #region 客户端根据服务器下发的网关地址连接网关
            
            Log.Info("服务器下发的网关地址 ： " + r2CLogin.Address);
            Log.Info("服务器下发的网关验证码 ： " + r2CLogin.Key);
            // 创建一个ETModel层的Session,并且保存到ETModel.SessionComponent中
            ETModel.Session gateSession = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(r2CLogin.Address);
            ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>().Session = gateSession;

            // 创建一个ETHotfix层的Session, 并且保存到ETHotfix.SessionComponent中
            Game.Scene.AddComponent<SessionComponent>().Session = ComponentFactory.Create<Session, ETModel.Session>(gateSession);

            G2C_LoginGate g2CLoginGate = (G2C_LoginGate) await SessionComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });

            Log.Info("登陆gate成功!");

            #endregion

            // 创建Player
            Player player = ETModel.ComponentFactory.CreateWithId<Player>(g2CLoginGate.PlayerId);
            PlayerComponent playerComponent = ETModel.Game.Scene.GetComponent<PlayerComponent>();
            playerComponent.MyPlayer = player;

            // 测试消息有成员是class类型
            G2C_PlayerInfo g2CPlayerInfo = (G2C_PlayerInfo) await SessionComponent.Instance.Session.Call(new C2G_PlayerInfo());

            // 逻辑层不应该去调用UI，逻辑层只关心逻辑并且抛出事件，由UI层自己去订阅事件，而且注意事件名字
            // 很多人容易把这个事件取名成LoginFinishiCreateLobbyUI，这是不对的，事件抛出去不可能知道谁订阅了这个事件，
            // 也不会知道别人订阅这个事件是干什么的,这里只知道我Login Finish
            Game.EventSystem.Run(EventIdType.LoginFinish);
        }

        public static void RigistBtnOnClick(FUILoginComponent self)
        {
            string account = self.AccountInput.Get("Input").GObject.asTextInput.text;

            string password = self.PasswordInput.Get("Input").GObject.asTextInput.text;


            SetErrorPrompt(self, "");

            Rigist(self, account, password).NoAwait();
        }

        public static async ETVoid Rigist(FUILoginComponent self, string account, string password)
        {

            // 创建一个ETModel层的Session
            ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalConfigComponent.Instance.GlobalProto.Address);

            //Log.Info("服务器地址 : " + GlobalConfigComponent.Instance.GlobalProto.Address);
            // 创建一个ETHotfix层的Session, ETHotfix的Session会通过ETModel层的Session发送消息
            Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);

             var r2cRegist = (R2C_Regist) await realmSession.Call(new C2R_Regist()
            {
                    Account = account,
                    Password = password
            });

            if (r2cRegist.Error == ErrorCode.ERR_Success)
            {
                Log.Info("注册成功");

                Game.EventSystem.Run(EventIdType.LoginFinish);
            }
            else
            {
                Log.Info(r2cRegist.Message);

                SetErrorPrompt(self, r2cRegist.Message);
            }
            
        }

        private static void SetErrorPrompt(FUILoginComponent self, string Prompt)
        {
            self.ErrorPrompt.GObject.asTextField.text = Prompt;
        }

    }
}
