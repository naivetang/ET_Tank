using ETModel;
using FairyGUI;

namespace ETHotfix
{
    [ObjectSystem]
    public class WarehouseAwakeSystem:AwakeSystem<WarehouseViewComponent>
    {
        public override void Awake(WarehouseViewComponent self)
        {
            self.Awake();
        }
    }

    [Event(EventIdType.GoldChange)]
    class GoldChange_WarehouseGold: AEvent
    {
        public override void Run()
        {
            FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Warehouse);
            if (fui != null)
            {
                fui.GetComponent<WarehouseViewComponent>().GoldChange();
            }
        }
    }

    public class WarehouseViewComponent : FUIBase
    {
        private GButton m_tank;

        private GButton m_bullet;

        private GButton m_prop;

        private GTextField m_gold;

        private GButton m_closeBtn;

        private GList m_itemList;

        private Controller m_controller;

        private static G2C_Warehouse m_data = null;

        public static G2C_Warehouse Data
        {
            get => m_data;
            set
            {
                m_data = value;
                FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Warehouse);
                if (fui != null)
                {
                    fui.GetComponent<WarehouseViewComponent>().RefreshData();
                }
            }

        }



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

            this.TankItemRender();
        }

        private void RefreshData()
        {
            this.GoldChange();
            CtlChange();
        }
        private void UI()
        {
            this.m_gold.text = PlayerComponent.Instance.MyPlayer.Gold.ToString();

            this.Lanaguage();
        }

        private void Lanaguage()
        {
            this.m_tank.text = Message.Get(1076);
            this.m_bullet.text = Message.Get(1077);
            this.m_prop.text = Message.Get(1078);
        }

        private void CtlChange()
        {
            switch (this.m_controller.selectedPage)
            {
                case "tank":
                    TankItemRender();
                    break;
                case "bullet":
                    BulletItemRender();
                    break;
                case "props":
                    PropItemRender();
                    break;
                default:
                    Log.Error($"不存在的页{this.m_controller.selectedPage}");
                    break;
            }

        }

        public void GoldChange()
        {
            this.m_gold.text = PlayerComponent.Instance.MyPlayer.Gold.ToString();
        }

        private void SetBtnCanClick(GComponent com)
        {
            com.touchable = true;
            com.GetController("c1").selectedIndex = 0;
        }

        private void SetBtnCanNotClick(GComponent com)
        {
            com.touchable = false;
            com.onClick.Set(() => { });
            com.GetController("c1").selectedIndex = 1;
        }

        private void TankItemRender()
        {
            //this.m_itemList.RemoveChildrenToPool();
            this.m_itemList.numItems = m_data.Tanks.count;

            for (int i = 0; i < m_data.Tanks.count; i++)
            {
                TankCfg tankInfo = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(TankCfg), m_data.Tanks[i]) as TankCfg;

                GComponent com = this.m_itemList.GetChildAt(i).asCom;

                com.GetChild("n11").text = tankInfo.Name();

                //com.GetChild("n7").text = tankInfo.Price.ToString();

                com.GetChild("n15").asLoader.url = tankInfo.Icon;
                if (m_data.TankId == tankInfo.Id)
                {
                    com.GetChild("n12").text = Message.Get(1082);
                    SetBtnCanNotClick(com.GetChild("n12").asCom);
                }
                else
                {
                    com.GetChild("n12").text = Message.Get(1079);
                    com.GetChild("n12").onClick.Set(() =>
                    {
                        this.Send_C2G_OptGood(GoodType.Tank,(int)tankInfo.Id);
                    });
                    SetBtnCanClick(com.GetChild("n12").asCom);
                }

                com.data = tankInfo;
            }
        }



        private void BulletItemRender()
        {
            //this.m_itemList.RemoveChildrenToPool();
            this.m_itemList.numItems = m_data.Bullets.count;

            for (int i = 0; i < m_data.Bullets.count; i++)
            {
                BulletCfg bulletInfo = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(BulletCfg), m_data.Bullets[i]) as BulletCfg;

                GComponent com = this.m_itemList.GetChildAt(i).asCom;

                com.GetChild("n11").text = bulletInfo.Name();

                //com.GetChild("n7").text = tankInfo.Price.ToString();

                com.GetChild("n15").asLoader.url = bulletInfo.Icon;

                if (m_data.BulletId == bulletInfo.Id)
                {
                    com.GetChild("n12").text = Message.Get(1082);
                    SetBtnCanNotClick(com.GetChild("n12").asCom);

                }
                else
                {
                    com.GetChild("n12").text = Message.Get(1080);
                    com.GetChild("n12").onClick.Set(() =>
                    {
                        this.Send_C2G_OptGood(GoodType.Bullet, (int)bulletInfo.Id);
                    });
                    SetBtnCanClick(com.GetChild("n12").asCom);
                }

                com.data = bulletInfo;
            }
        }
        private void PropItemRender()
        {
            //this.m_itemList.RemoveChildrenToPool();

            this.m_itemList.numItems = m_data.Props.count;

            for (int i = 0; i < m_data.Props.count; i++)
            {
                Prop propInfo = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Prop), m_data.Props[i].TableId) as Prop;

                GComponent com = this.m_itemList.GetChildAt(i).asCom;

                com.GetChild("n11").text = propInfo.Name();

                //com.GetChild("n7").text = tankInfo.Price.ToString();

                com.GetChild("n15").asLoader.url = propInfo.Icon;

                if (m_data.Props[i].PropState == PropState.InUser)
                {
                    com.GetChild("n12").text = Message.Get(1082);
                    SetBtnCanNotClick(com.GetChild("n12").asCom);
                }
                else
                {
                    com.GetChild("n12").text = Message.Get(1081);
                    com.onClick.Set(() =>
                    {
                        this.Send_C2G_OptGood(GoodType.Prop, (int)propInfo.Id);
                    });
                    SetBtnCanClick(com.GetChild("n12").asCom);
                }

                com.data = propInfo;
            }
        }

        private void Send_C2G_OptGood(GoodType type, int id)
        {
            C2G_OptGood msg = new C2G_OptGood();

            msg.GoodType = type;

            msg.TableId = id;

            msg.GoodOpt = GoodOpt.Use;

            ETModel.SessionComponent.Instance.Session.Send(msg);
        }

    }
}
