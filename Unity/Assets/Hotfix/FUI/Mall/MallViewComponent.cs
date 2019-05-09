using ETModel;
using FairyGUI;

namespace ETHotfix
{
    [ObjectSystem]
    public class MallAwakeSystem : AwakeSystem<MallViewComponent>
    {
        public override void Awake(MallViewComponent self)
        {
            self.Awake();
        }
    }

    [Event(EventIdType.GoldChange)]
    class GoldChange_MallGold : AEvent
    {
        public override void Run()
        {
            FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Mall);
            if (fui != null)
            {
                fui.GetComponent<MallViewComponent>().GoldChange();
            }
        }
    }

    public class MallViewComponent:FUIBase
    {
        private GButton m_tank;

        private GButton m_bullet;

        private GButton m_prop;

        private GTextField m_gold;

        private GButton m_closeBtn;

        private GList m_itemList;

        private Controller m_controller;

        public void Awake()
        {
            this.FUIComponent = this.GetParent<FUI>();

            this.StartFUI();
        }
        protected override void StartFUI()
        {
            this.m_tank = this.FUIComponent.Get("n18").GObject.asButton;
            this.m_bullet = this.FUIComponent.Get("n19").GObject.asButton;
            this.m_prop = this.FUIComponent.Get("n20").GObject.asButton;
            this.m_closeBtn = this.FUIComponent.Get("n16").GObject.asButton;
            this.m_gold = this.FUIComponent.Get("n10").GObject.asTextField;
            this.m_itemList = this.FUIComponent.Get("n15").GObject.asList;
            this.m_controller = this.FUIComponent.GObject.asCom.GetController("c1");


            this.m_closeBtn.onClick.Set(this.OnClose);

            this.m_controller.onChanged.Set(this.CtlChange);

            this.UI();
        }

        private void UI()
        {
            this.m_gold.text = PlayerComponent.Instance.MyPlayer.Gold.ToString();

            this.TankItemRender();

            this.Lanaguage();
        }

        private void CtlChange()
        {
            switch (this.m_controller.selectedPage)
            {
                case "tank":
                    this.TankItemRender();
                    break;
                case "bullet":
                    this.BulletItemRender();
                    break;
                case "props":
                    this.PropItemRender();
                    break;
                default:
                    Log.Error($"不存在的页{this.m_controller.selectedPage}");
                    break;
            }
            
        }

        private void TankItemRender()
        {
            IConfig[] configs = Game.Scene.GetComponent<ConfigComponent>().GetAll(typeof(TankCfg));

            this.m_itemList.RemoveChildrenToPool();

            for (int i = 0; i < configs.Length; i++)
            {
                TankCfg tankInfo = configs[i] as TankCfg;

                if(tankInfo.CanBuy == 0)
                    continue;
                

                GComponent com = this.m_itemList.AddItemFromPool().asCom; 
                
                com.GetChild("n11").text = tankInfo.Name();

                com.GetChild("n7").text = tankInfo.Price.ToString();

                com.GetChild("n15").asLoader.url = tankInfo.Icon;

                com.GetChild("n12").text = Message.Get(1075);

                com.GetChild("n12").asButton.onClick.Set(() =>
                {
                    Send_C2G_OptGood(GoodType.Tank, (int)tankInfo.Id);
                });

                com.data = tankInfo;
            }
        }

        private void BulletItemRender()
        {
            IConfig[] configs = Game.Scene.GetComponent<ConfigComponent>().GetAll(typeof(BulletCfg));

            this.m_itemList.RemoveChildrenToPool();

            for (int i = 0; i < configs.Length; i++)
            {
                BulletCfg bulletInfo = configs[i] as BulletCfg;

                if (bulletInfo.CanBuy == 0)
                    continue;

                GComponent com = this.m_itemList.AddItemFromPool().asCom;

                com.GetChild("n11").text = bulletInfo.Name();

                com.GetChild("n7").text = bulletInfo.Price.ToString();

                com.GetChild("n15").asLoader.url = bulletInfo.Icon;

                com.GetChild("n12").text = Message.Get(1075);

                com.GetChild("n12").asButton.onClick.Set(() =>
                {
                    Send_C2G_OptGood(GoodType.Bullet, (int)bulletInfo.Id);
                });

                com.data = bulletInfo;
            }
        }

        private void PropItemRender()
        {
            IConfig[] configs = Game.Scene.GetComponent<ConfigComponent>().GetAll(typeof(Prop));

            this.m_itemList.numItems = configs.Length;

            for (int i = 0; i < configs.Length; i++)
            {
                Prop propInfo = configs[i] as Prop;

                GComponent com = this.m_itemList.GetChildAt(i).asCom;

                com.GetChild("n11").text = propInfo.Name();

                com.GetChild("n7").text = propInfo.Price.ToString();

                com.GetChild("n15").asLoader.url = propInfo.Icon;

                com.GetChild("n12").text = Message.Get(1075);

                com.GetChild("n12").asButton.onClick.Set(() =>
                {
                    Send_C2G_OptGood(GoodType.Prop, (int)propInfo.Id);
                });

                com.data = propInfo;
            }
        }

        private void Lanaguage()
        {
            this.m_tank.text = Message.Get(1076);
            this.m_bullet.text = Message.Get(1077);
            this.m_prop.text = Message.Get(1078);
        }

        private void Send_C2G_OptGood(GoodType type, int id)
        {
            C2G_OptGood msg = new C2G_OptGood();

            msg.GoodType = type;

            msg.TableId = id;

            msg.GoodOpt = GoodOpt.Buy;

            ETModel.SessionComponent.Instance.Session.Send(msg);

        }

        public void GoldChange()
        {
            this.m_gold.text = PlayerComponent.Instance.MyPlayer.Gold.ToString();
        }
    }
}
