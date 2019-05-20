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

        private GComponent m_infoTip;

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

            this.m_infoTip = this.FUIComponent.Get("InfoTip").GObject.asCom;


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

                com.GetChild("n15").asLoader.onClick.Set(this.TankIcon_OnClick);

                com.GetChild("n12").text = Message.Get(1075);

                com.GetChild("n12").asButton.onClick.Set(() =>
                {
                    Send_C2G_OptGood(GoodType.Tank, (int)tankInfo.Id);
                });

                com.data = tankInfo;
            }
        }

        private void TankIcon_OnClick(EventContext context)
        {
            this.m_infoTip.visible = true;

            this.m_infoTip.GetChild("bg").onClick.Set(() => { this.m_infoTip.visible = false; });

            GComponent tip = this.m_infoTip.GetChild("InfoTip").asCom;

            tip.x = context.inputEvent.x;

            tip.y = context.inputEvent.y;

            TankCfg tankInfo = ((context.sender as GLoader).parent.data) as TankCfg;

            tip.GetChild("n2").asLoader.url = tankInfo.Icon;

            tip.GetChild("n4").asTextField.text = tankInfo.Name();

            tip.GetChild("n6").asTextField.text = tankInfo.Type();

            tip.GetChild("n8").asTextField.text = Message.Get(1088);

            GList attrList = tip.GetChild("n7").asList;

            attrList.numItems = 3;

            attrList.GetChildAt(0).asCom.GetChild("n0").asTextField.text = $"{Message.Get(1089)}   +{tankInfo.Attack}";

            attrList.GetChildAt(1).asCom.GetChild("n0").asTextField.text = $"{Message.Get(1090)}   +{tankInfo.Defence}";

            attrList.GetChildAt(2).asCom.GetChild("n0").asTextField.text = $"{Message.Get(1091)}  +{tankInfo.SunderArmor}";

            attrList.ResizeToFit(3);
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

                com.GetChild("n15").asLoader.onClick.Set(this.BulletIcon_OnClick);

                com.GetChild("n12").text = Message.Get(1075);

                com.GetChild("n12").asButton.onClick.Set(() =>
                {
                    Send_C2G_OptGood(GoodType.Bullet, (int)bulletInfo.Id);
                });

                com.data = bulletInfo;
            }
        }

        private void BulletIcon_OnClick(EventContext context)
        {
            this.m_infoTip.visible = true;

            this.m_infoTip.GetChild("bg").onClick.Set(() => { this.m_infoTip.visible = false; });

            GComponent tip = this.m_infoTip.GetChild("InfoTip").asCom;

            tip.x = context.inputEvent.x;

            tip.y = context.inputEvent.y;

            BulletCfg bulletInfo = ((context.sender as GLoader).parent.data) as BulletCfg;

            tip.GetChild("n2").asLoader.url = bulletInfo.Icon;

            tip.GetChild("n4").asTextField.text = bulletInfo.Name();

            tip.GetChild("n6").asTextField.text = bulletInfo.Type();

            tip.GetChild("n8").asTextField.text = Message.Get(1088);

            GList attrList = tip.GetChild("n7").asList;

            attrList.numItems = 3;

            attrList.GetChildAt(0).asCom.GetChild("n0").asTextField.text = $"{Message.Get(1089)}   +{bulletInfo.Attack}";

            attrList.GetChildAt(1).asCom.GetChild("n0").asTextField.text = $"{Message.Get(1091)}  +{bulletInfo.SunderArmor}";

            attrList.GetChildAt(2).asCom.GetChild("n0").asTextField.text = $"{Message.Get(1092)}  {bulletInfo.LoadingSpeed}";

            attrList.ResizeToFit(3);
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

                com.GetChild("n15").asLoader.onClick.Set(this.PropIcon_OnClick);

                com.GetChild("n12").text = Message.Get(1075);

                com.GetChild("n12").asButton.onClick.Set(() =>
                {
                    Send_C2G_OptGood(GoodType.Prop, (int)propInfo.Id);
                });

                com.data = propInfo;
            }
        }

        private void PropIcon_OnClick(EventContext context)
        {
            this.m_infoTip.visible = true;

            this.m_infoTip.GetChild("bg").onClick.Set(() => { this.m_infoTip.visible = false; });

            GComponent tip = this.m_infoTip.GetChild("InfoTip").asCom;

            tip.x = context.inputEvent.x;

            tip.y = context.inputEvent.y;

            Prop propInfo = ((context.sender as GLoader).parent.data) as Prop;

            tip.GetChild("n2").asLoader.url = propInfo.Icon;

            tip.GetChild("n4").asTextField.text = propInfo.Name();

            tip.GetChild("n6").asTextField.text = propInfo.Type();

            tip.GetChild("n8").asTextField.text = Message.Get(1088);

            GList attrList = tip.GetChild("n7").asList;

            attrList.numItems = 2;

            // 经验加成卡
            if (propInfo.Class == 1)
            {
                attrList.GetChildAt(0).asCom.GetChild("n0").asTextField.text = $"{Message.Get(1093)}   +{propInfo.Experience}%";

                attrList.GetChildAt(1).asCom.GetChild("n0").asTextField.text = $"{Message.Get(1095)}  +{propInfo.TotleTimes}";
            }
            // 金币加成卡
            else if (propInfo.Class == 2)
            {
                attrList.GetChildAt(0).asCom.GetChild("n0").asTextField.text = $"{Message.Get(1094)}   +{propInfo.Gold}%";

                attrList.GetChildAt(1).asCom.GetChild("n0").asTextField.text = $"{Message.Get(1095)}  +{propInfo.TotleTimes}";
            }
            else
            {
                Log.Error($"不存在的类别{propInfo.Class}");
            }


            attrList.ResizeToFit(2);
            //attrList.GetChildAt(1)


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
