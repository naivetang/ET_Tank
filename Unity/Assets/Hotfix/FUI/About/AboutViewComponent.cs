using ETModel;
using FairyGUI;

namespace ETHotfix
{
    [ObjectSystem]
    public class AboutViewAwakeSystem : AwakeSystem<AboutViewComponent>
    {
        public override void Awake(AboutViewComponent self)
        {
            self.Awake();
        }
    }

    public class AboutViewComponent : FUIBase
    {

        private GTextField m_author;
        private GTextField m_version;
        private GTextField m_contact;
        private GTextField m_tanks;

        private GButton m_sureBtn;


        public void Awake()
        {
            this.FUIComponent = this.GetParent<FUI>();

            this.StartFUI();
        }
        protected override void StartFUI()
        {
            this.m_author = this.FUIComponent.Get("n8").GObject.asTextField;
            this.m_version = this.FUIComponent.Get("n9").GObject.asTextField;
            this.m_contact = this.FUIComponent.Get("n10").GObject.asTextField;
            this.m_tanks = this.FUIComponent.Get("n11").GObject.asTextField;
            this.m_sureBtn = this.FUIComponent.Get("n5").GObject.asButton;


            this.UI();
        }

        private void UI()
        {
            this.m_sureBtn.onClick.Set(this.SureBtn_OnClick);

            this.Lanaguage();
        }

        private void Lanaguage()
        {
            this.m_author.text = Message.Get(1068);
            this.m_version.text = Message.Get(1069);
            this.m_contact.text = Message.Get(1070);
            this.m_tanks.text = Message.Get(1071);

            this.m_sureBtn.text = Message.Get(1026);
        }

        private void SureBtn_OnClick()
        {
            this.OnClose();
        }
    }
}
