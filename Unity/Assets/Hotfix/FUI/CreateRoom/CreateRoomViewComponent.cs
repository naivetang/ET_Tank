using ETModel;
using FairyGUI;

namespace ETHotfix
{
    [ObjectSystem]
    public class CreateRoomAwakeComponent : AwakeSystem<CreateRoomViewComponent>
    {
        public override void Awake(CreateRoomViewComponent self)
        {
            self.Awake();
        }
    }

    public class CreateRoomViewComponent : FUIBase
    {

        private GButton m_closeBtn;
        private GButton m_enterBtn;

        private GComboBox m_peopleNum;

        private GComboBox m_map;

        private GComboBox m_bigModel;

        private GComboBox m_smallModel;

        private string[] m_nums = new string[] { "1v1", "2v2", "3v3", "4v4", "5v5" };

        private string[] m_maps = new string[] { "沙漠一灰" };

        private string[] m_models = new string[] { "回合制","时间制" };

        private string[] m_rounds = new[] {"1回合" ,"5回合", "10回合", "12回合" };

        private string[] m_times = new[] { "1min","5min", "10min", "12min" };


        public void Awake()
        {
            this.FUIComponent = this.GetParent<FUI>();
            this.StartFUI();
        }

        private void StartFUI()
        {
            this.m_closeBtn = this.FUIComponent.Get("Close").GObject.asButton;

            this.m_closeBtn.onClick.Set(this.Close);

            this.m_peopleNum = this.FUIComponent.Get("n2").GObject.asComboBox;

            this.m_peopleNum.items = m_nums;

            this.m_peopleNum.values = m_nums;

            this.m_peopleNum.selectedIndex = 0;

            this.m_map = this.FUIComponent.Get("n6").GObject.asComboBox;

            this.m_map.items = this.m_map.values = m_maps;

            this.m_map.selectedIndex = 0;

            this.m_bigModel = this.FUIComponent.Get("n10").GObject.asComboBox;

            this.m_bigModel.items = this.m_bigModel.values = this.m_models;

            this.m_smallModel = this.FUIComponent.Get("n11").GObject.asComboBox;

            this.m_bigModel.onChanged.Set(BigModel_OnChange);

            this.m_bigModel.value =this.m_models[0];

            this.m_smallModel.items = this.m_smallModel.values = this.m_rounds;

            this.m_smallModel.selectedIndex = 0;
            //this.m_smallModel.items = this.m_smallModel.values = this.
        }

        private void BigModel_OnChange(EventContext context)
        {
            if (this.m_bigModel.selectedIndex == 0)
            {
                this.m_smallModel.items = this.m_smallModel.values = this.m_rounds;
            }
            else
            {
                this.m_smallModel.items = this.m_smallModel.values = this.m_times;
            }
            this.m_smallModel.selectedIndex = 0;
        }
    }
}
