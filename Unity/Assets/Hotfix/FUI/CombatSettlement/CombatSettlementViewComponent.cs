using System.Collections.Generic;
using ETModel;
using FairyGUI;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [ObjectSystem]
    public class CombatSettlementViewAwakeComponent : AwakeSystem<CombatSettlementViewComponent, B2C_BattleEnd>
    {
        public override void Awake(CombatSettlementViewComponent self, B2C_BattleEnd a)
        {
            self.Awake(a);
        }
    }
    public class CombatSettlementViewComponent : FUIBase
    {

        private GButton m_againBtn;

        private GList m_leftList;

        private GList m_rightList;

        private GLoader m_leftSucOrFail;

        private GLoader m_rightSucOrFail;

        private GRichTextField m_leftCampName;

        private GRichTextField m_rightCampName;

        private GComponent m_leftTopTitle;

        private GComponent m_rightTopTitle;

        private B2C_BattleEnd BattleInfo;

        public void Awake(B2C_BattleEnd msg)
        {
            this.FUIComponent = this.GetParent<FUI>();

            BattleInfo = msg;

            this.StartFUI();
        }


        protected override void StartFUI()
        {
            m_againBtn = this.FUIComponent.Get("n10").GObject.asButton;
            m_leftList = this.FUIComponent.Get("n32").Get("n1").GObject.asList;
            m_rightList = this.FUIComponent.Get("n33").Get("n1").GObject.asList;
            m_leftSucOrFail = this.FUIComponent.Get("n28").GObject.asLoader;
            m_rightSucOrFail = this.FUIComponent.Get("n29").GObject.asLoader;
            m_leftCampName = this.FUIComponent.Get("n24").GObject.asRichTextField;
            m_rightCampName = this.FUIComponent.Get("n25").GObject.asRichTextField;

            m_leftTopTitle = this.FUIComponent.Get("n32").GObject.asCom.GetChild("n2").asCom;

            m_rightTopTitle = this.FUIComponent.Get("n33").GObject.asCom.GetChild("n2").asCom;

            m_againBtn.onClick.Set(this.AgainBtn_OnClick);

            this.m_leftList.itemRenderer = ItemRendererLeft;
            this.m_rightList.itemRenderer = ItemRendererRight;
            this.UI();
        }

        private void UI()
        {
            if (this.BattleInfo.WinCamp == 1)
            {
                this.m_leftSucOrFail.url = "ui://CombatSettlement/sheng";
                this.m_rightSucOrFail.url = "ui://CombatSettlement/bai";
            }
            else if (this.BattleInfo.WinCamp == 2)
            {
                this.m_leftSucOrFail.url = "ui://CombatSettlement/bai";
                this.m_rightSucOrFail.url = "ui://CombatSettlement/sheng";
            }

            {
                GComponent com = this.m_leftTopTitle;

                com.GetChild("ping").asTextField.text = Message.Get(1050);

                com.GetChild("name").asTextField.text = Message.Get(1051);

                com.GetChild("rank").asTextField.text = Message.Get(1052);

                com.GetChild("kills").asTextField.text = Message.Get(1053);

                com.GetChild("damage").asTextField.text = Message.Get(1054);

                com.GetChild("death").asTextField.text = Message.Get(1055);
            }

            {
                GComponent com = this.m_rightTopTitle;

                com.GetChild("ping").asTextField.text = Message.Get(1050);

                com.GetChild("name").asTextField.text = Message.Get(1051);

                com.GetChild("rank").asTextField.text = Message.Get(1052);

                com.GetChild("kills").asTextField.text = Message.Get(1053);

                com.GetChild("damage").asTextField.text = Message.Get(1054);

                com.GetChild("death").asTextField.text = Message.Get(1055);
            }

            m_againBtn.text = Message.Get(1056);

            this.m_leftCampName.text = $"{Message.Get(1048)}   {this.BattleInfo.LeftCamp.count}";
            this.m_rightCampName.text = $"{Message.Get(1049)}   {this.BattleInfo.RightCamp.count}";

            this.m_leftList.numItems = this.BattleInfo.LeftCamp.count;

            this.m_rightList.numItems = this.BattleInfo.RightCamp.count;
        }

        private void ItemRendererLeft(int index, GObject gObject)
        {
            GComponent com = gObject as GComponent;

            PersonBattleData personBattleData = this.BattleInfo.LeftCamp[index];

            com.GetChild("ping").asLoader.url = FUIHelper.GetPingUrl(personBattleData.Ping);

            com.GetChild("name").asTextField.text = personBattleData.Name;

            com.GetChild("rank").asLoader.url =FUIHelper.GetLevelUrl(personBattleData.Level);

            com.GetChild("kills").asTextField.text = personBattleData.Kills.ToString();

            com.GetChild("damage").asTextField.text = personBattleData.Damage.ToString();

            com.GetChild("death").asTextField.text = personBattleData.Deaths.ToString();

        }

        private void ItemRendererRight(int index, GObject gObject)
        {
            GComponent com = gObject as GComponent;

            PersonBattleData personBattleData = this.BattleInfo.RightCamp[index];

            com.GetChild("ping").asLoader.url = FUIHelper.GetPingUrl(personBattleData.Ping);

            com.GetChild("name").asTextField.text = personBattleData.Name;

            com.GetChild("rank").asLoader.url = FUIHelper.GetLevelUrl(personBattleData.Level);

            com.GetChild("kills").asTextField.text = personBattleData.Kills.ToString();

            com.GetChild("damage").asTextField.text = personBattleData.Damage.ToString();

            com.GetChild("death").asTextField.text = personBattleData.Deaths.ToString();
        }

        private void AgainBtn_OnClick()
        {
            FUIFactory.Create<RoomViewComponent>(FUIType.Room).NoAwait();

            this.OnClose();
        }

        private void Send_C2G_AgainGame()
        {


            C2G_AgainGame msg = new C2G_AgainGame();

            msg.RoomId = this.BattleInfo.BattleId;

            ETModel.SessionComponent.Instance.Session.Send(msg);
        }
    }
}
