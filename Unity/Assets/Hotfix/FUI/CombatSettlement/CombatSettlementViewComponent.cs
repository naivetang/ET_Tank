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

        public void Awake(B2C_BattleEnd msg)
        {
            this.FUIComponent = this.GetParent<FUI>();

            this.StartFUI();


        }

        private void StartFUI()
        {
            m_againBtn = this.FUIComponent.Get("n10").GObject.asButton;
            m_leftList = this.FUIComponent.Get("n32").Get("n2").GObject.asList;
            m_rightList = this.FUIComponent.Get("n33").Get("n2").GObject.asList;
            m_leftSucOrFail = this.FUIComponent.Get("n28").GObject.asLoader;
            m_rightSucOrFail = this.FUIComponent.Get("n29").GObject.asLoader;
            m_leftCampName = this.FUIComponent.Get("n24").GObject.asRichTextField;
            m_rightCampName = this.FUIComponent.Get("n25").GObject.asRichTextField;
        }

        private void UI()
        {

        }
    }
}
