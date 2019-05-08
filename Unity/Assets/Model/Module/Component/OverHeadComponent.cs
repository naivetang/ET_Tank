using FairyGUI;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class OverHeadAwakeSystem: AwakeSystem<OverHeadComponent>
    {
        public override void Awake(OverHeadComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class OverHeadLateUpdateSystem : LateUpdateSystem<OverHeadComponent>
    {
        public override void LateUpdate(OverHeadComponent self)
        {
            self.LateUpdate();
        }
    }

    public class OverHeadComponent : Component
    {
        private Tank m_tank;

        private GameObject m_overHeadGO;

        private UIPanel m_uiPanel;

        private GRichTextField m_name;

        private FairyGUI.GProgressBar m_redBar;

        private FairyGUI.GProgressBar m_whiteBar;

        private NumericComponent m_numericComponent;

        public void Awake()
        {

            Game.Scene.GetComponent<FUIPackageComponent>().AddPackage( "OverHead");

            this.m_tank = this.GetParent<Tank>();

            this.m_overHeadGO = this.m_tank.GameObject.FindChildObjectByPath("OverHead");

            this.m_overHeadGO.SetActive(true);

            this.m_uiPanel = this.m_overHeadGO.GetComponent<UIPanel>();

            this.m_name = this.m_uiPanel.ui.GetChild("name").asRichTextField;

            this.m_redBar = this.m_uiPanel.ui.GetChild("n0").asProgress;

            this.m_whiteBar = this.m_uiPanel.ui.GetChild("n1").asProgress;

            this.m_name.text = this.m_tank.Name;

            m_numericComponent = this.m_tank.GetComponent<NumericComponent>();

            this.m_redBar.max = m_numericComponent[NumericType.MaxHp];

            this.m_redBar.value = m_numericComponent[NumericType.Hp];

            this.m_whiteBar.max = 100;

            this.m_whiteBar.value = 100;

            if (this.m_tank.TankCamp != TankComponent.Instance.MyTank.TankCamp)
            {
                this.m_name.color = Color.red;
            }

            this.GameSetting();
        }

        private void GameSetting()
        {
            this.m_name.visible = GameSettingInfo.NameVisible();

            this.m_uiPanel.ui.GetChild("bloodGroup").visible = GameSettingInfo.HpVisible();
        }

        public void HpChange(int maxHp,int nowHp)
        {
            this.m_redBar.max = maxHp;
            this.m_redBar.value = nowHp;
        }

        public void LateUpdate()
        {
            this.m_overHeadGO.transform.LookAt(Camera.main.transform.position);
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
        }
    }
}
