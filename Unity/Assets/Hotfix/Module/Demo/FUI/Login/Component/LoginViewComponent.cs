using System;
using ETModel;
using FairyGUI;

namespace ETHotfix
{
    public class LoginViewComponent: FUIBase
	{
		// 缓存只需要缓存FUI类型即可
		public FUI AccountInput;

	    public FUI PasswordInput;

	    public FUI RegistBtn;

	    public FUI LoginBtn;

        public GTextField m_title;

        public GTextField m_label;

        public GButton m_aboutBtn;

        public GButton m_exitBtn;

        public void Awake()
        {
            this.StartFUI();
        }

        protected override void StartFUI()
        {
            this.UI();
        }

        public void UI()
        {
            this.Lanaguage();
        }

        public  void Lanaguage( )
        {
            this.m_title.text = Message.Get(1006);

            this.m_label.text = Message.Get(1018);

            RegistBtn.GObject.text = Message.Get(1019);

            this.LoginBtn.GObject.text = Message.Get(1020);

            this.m_aboutBtn.text = Message.Get(1021);

            this.m_exitBtn.text = Message.Get(1012);

            this.AccountInput.GObject.asLabel.GetTextField().asTextInput.promptText = Message.Get(1022);
            this.PasswordInput.GObject.asLabel.GetTextField().asTextInput.promptText = Message.Get(1023);
        }

        public  void RefreshData()
        {
        }
    }



    [ObjectSystem]
    public class LoginViewAwakeSystem: AwakeSystem<LoginViewComponent>
    {
        public override void Awake(LoginViewComponent self)
        {
            self.FUIComponent = self.GetParent<FUI>();

            self.AccountInput = self.FUIComponent.Get("AccountInput");

            self.PasswordInput = self.FUIComponent.Get("PasswordInput");

            self.RegistBtn = self.FUIComponent.Get("RegistBtn");

            self.LoginBtn = self.FUIComponent.Get("LoginBtn");

            self.m_title = self.FUIComponent.Get("n21").GObject.asTextField;

            self.m_label = self.FUIComponent.Get("n22").GObject.asTextField;

            self.m_aboutBtn = self.FUIComponent.Get("n25").GObject.asButton;

            self.m_exitBtn = self.FUIComponent.Get("ExitBtn").GObject.asButton;

            //if(Game.Scene.GetComponent<FUIPackageComponent>().get)

            self.LoginBtn.GObject.asButton.onClick.Set(() => { LoginBtnOnClick(self); });

            self.RegistBtn.GObject.asButton.onClick.Set(() => { RigistBtnOnClick(self); });

            self.Awake();

            //login.Get("LoginBtn").GObject.asButton.onClick.Add(() => LoginBtnOnClick(self));
        }

        public static void LoginBtnOnClick(LoginViewComponent self)
        {
            string account = self.AccountInput.Get("title").GObject.asTextInput.text;  
            
            string password = self.PasswordInput.Get("title").GObject.asTextInput.text;

            Login(self, account, password).NoAwait();
            
        }

        public static async ETVoid Login(LoginViewComponent self, string account, string password)
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


            // 加载Unit资源
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();

            // 加载场景资源
            await resourcesComponent.LoadBundleAsync("start.unity3d");
            // 切换到Battle场景
            using (SceneChangeComponent sceneChangeComponent = Game.Scene.AddComponent<SceneChangeComponent>())
            {
                

                await sceneChangeComponent.ChangeSceneAsync(SceneType.Start);
            }

            // 逻辑层不应该去调用UI，逻辑层只关心逻辑并且抛出事件，由UI层自己去订阅事件，而且注意事件名字
            // 很多人容易把这个事件取名成LoginFinishiCreateLobbyUI，这是不对的，事件抛出去不可能知道谁订阅了这个事件，
            // 也不会知道别人订阅这个事件是干什么的,这里只知道我Login Finish
            //Game.EventSystem.Run(EventIdType.LoginFinish);
            Game.EventSystem.Run(EventIdType.LoginHasFinish);
        }

        public static void RigistBtnOnClick(LoginViewComponent self)
        {
            string account = self.AccountInput.Get("title").GObject.asTextInput.text;

            string password = self.PasswordInput.Get("title").GObject.asTextInput.text;


            Rigist(self, account, password).NoAwait();
        }

        public static async ETVoid Rigist(LoginViewComponent self, string account, string password)
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

        private static void SetErrorPrompt(LoginViewComponent self, string Prompt)
        {
            Game.EventSystem.Run(EventIdType.ShowPopMessage,Prompt,PopMessageType.Float);
        }

    }
}
