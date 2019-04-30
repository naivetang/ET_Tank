using System;
using ETModel;
using FairyGUI;
using UnityEngine;

namespace ETHotfix
{
    public class LoginViewComponent: FUIBase
	{
        private class RegisterView
        {
            private GComponent FUIComponent;

            private GLabel m_phoneNum;
            private GLabel m_name;
            private GLabel m_passWord;
            private GButton m_registerBtn;

            private GButton m_closeBtn;

            public void Awake(GComponent com)
            {
                this.FUIComponent = com;

                this.FUIComponent.visible = false;

                this.StartUp();
            }

            private void StartUp()
            {
                this.m_phoneNum = this.FUIComponent.GetChild("n33").asLabel;
                this.m_name = this.FUIComponent.GetChild("n34").asLabel;
                this.m_passWord = this.FUIComponent.GetChild("n35").asLabel;
                this.m_registerBtn = this.FUIComponent.GetChild("n36").asButton;
                this.m_closeBtn = this.FUIComponent.GetChild("n37").asButton;

                this.m_registerBtn.onClick.Set(RegistBtn_OnClick);

                this.m_closeBtn.onClick.Set(this.OnClose);

                this.UI();
            }

            private void UI()
            {
                this.Lanaguage();
            }

            private void Lanaguage()
            {
                this.m_phoneNum.GetTextField().asTextInput.promptText = Message.Get(1058);
                this.m_name.GetTextField().asTextInput.promptText = Message.Get(1059);
                this.m_passWord.GetTextField().asTextInput.promptText = Message.Get(1060);
                this.m_registerBtn.text = Message.Get(1019);


            }

            private void RegistBtn_OnClick()
            {
                this.Register(this.m_phoneNum.text, this.m_name.text, this.m_passWord.text).NoAwait();
            }

            

            private async ETVoid Register(string phoneNum, string userName, string password)
            {

                // 创建一个ETModel层的Session
                ETModel.Session session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalConfigComponent.Instance.GlobalProto.Address);

                //Log.Info("服务器地址 : " + GlobalConfigComponent.Instance.GlobalProto.Address);
                // 创建一个ETHotfix层的Session, ETHotfix的Session会通过ETModel层的Session发送消息
                Session realmSession = ComponentFactory.Create<Session, ETModel.Session>(session);

                var r2cRegist = (R2C_Regist)await realmSession.Call(new C2R_Regist()
                {
                        PhoneNum = phoneNum,
                        UserName = userName,
                        Password = password
                });

                if (r2cRegist.ErrorMessagId == 0)
                {
                    Log.Info("注册成功");

                    this.OnClose();
                }
                else
                {
                    Game.EventSystem.Run(EventIdType.ShowPopMessage, Message.Get(r2cRegist.ErrorMessagId), PopMessageType.Float);
                }

            }
            

            public bool visible
            {
                get => this.FUIComponent.visible;
                set => this.FUIComponent.visible = value;
            }

            private void OnClose()
            {
                m_phoneNum.text = "";
                m_name.text = "";
                m_passWord.text = "";
                this.FUIComponent.visible = false;
            }

        }

		// 缓存只需要缓存FUI类型即可
		public FUI AccountInput;

	    public FUI PasswordInput;

	    public FUI RegistBtn;

	    public FUI LoginBtn;

        public GTextField m_title;

        public GTextField m_label;

        public GButton m_aboutBtn;

        public GButton m_exitBtn;

        public GButton m_rememberBtn;

        public GTextField m_rememberText;

        public GComponent m_registGoup;

        private RegisterView m_registerView = new RegisterView();

        public void Awake()
        {
            this.StartFUI();

            m_registerView.Awake(this.m_registGoup);
        }

        protected override void StartFUI()
        {
            this.UI();
        }

        public void UI()
        {
            RegistBtn.GObject.asButton.onClick.Set(RegistBtn_OnClick);

            string account = PlayerPrefs.GetString("Account");

            if (!string.IsNullOrWhiteSpace(account))
            {
                AccountInput.GObject.asLabel.text = account;
            }

            string password = PlayerPrefs.GetString("Password");

            if (!string.IsNullOrWhiteSpace(password))
            {
                PasswordInput.GObject.asLabel.text = password;
            }

            this.Lanaguage();
        }

        private void RegistBtn_OnClick()
        {
            m_registerView.visible = true;
        }

        private  void Lanaguage( )
        {
            this.m_title.text = Message.Get(1006);

            this.m_label.text = Message.Get(1018);

            RegistBtn.GObject.text = Message.Get(1019);

            this.LoginBtn.GObject.text = Message.Get(1020);

            this.m_aboutBtn.text = Message.Get(1021);

            this.m_exitBtn.text = Message.Get(1012);

            this.AccountInput.GObject.asLabel.GetTextField().asTextInput.promptText = Message.Get(1022);
            this.PasswordInput.GObject.asLabel.GetTextField().asTextInput.promptText = Message.Get(1023);

            this.m_rememberText.text = Message.Get(1057);

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


            self.m_rememberBtn = self.FUIComponent.Get("n27").GObject.asButton;

            self.m_rememberBtn.selected = !string.IsNullOrWhiteSpace(PlayerPrefs.GetString("Remember"));

            self.m_rememberText = self.FUIComponent.Get("n28").GObject.asTextField;

            self.m_registGoup = self.FUIComponent.Get("n31").GObject.asCom;

            self.m_registGoup.visible = false;

            self.Awake();

            //login.Get("LoginBtn").GObject.asButton.onClick.Add(() => LoginBtnOnClick(self));
        }

        public static void LoginBtnOnClick(LoginViewComponent self)
        {
            string account = self.AccountInput.Get("title").GObject.asTextInput.text;

            string password = self.PasswordInput.Get("title").GObject.asTextInput.text;

            StorageSet(self);

            Login(self, account, password).NoAwait();
            
        }

        private static void StorageSet(LoginViewComponent self)
        {
            string account = self.AccountInput.Get("title").GObject.asTextInput.text;

            string password = self.PasswordInput.Get("title").GObject.asTextInput.text;

            PlayerPrefs.SetString("Account", account);

            PlayerPrefs.SetString("Password", self.m_rememberBtn.selected ? password : "");

            PlayerPrefs.SetString("Remember", self.m_rememberBtn.selected ? "Remember" : "");
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
                SetErrorPrompt(self,Message.Get(r2CLogin.ErrorMessageId));
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


        

        private static void SetErrorPrompt(LoginViewComponent self, string Prompt)
        {
            Game.EventSystem.Run(EventIdType.ShowPopMessage,Prompt,PopMessageType.Float);
        }

    }
}
