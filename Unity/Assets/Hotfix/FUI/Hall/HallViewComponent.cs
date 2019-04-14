using ETModel;
using FairyGUI;

namespace ETHotfix
{
    [ObjectSystem]
    public class HallViewAwakeComponent : AwakeSystem<HallViewComponent>
    {
        public override void Awake(HallViewComponent self)
        {
            self.Awake();
        }
    }

    public class HallViewComponent : FUIBase
    {
        private GButton m_createButton;

        public void Awake()
        {
            this.FUIComponent = this.GetParent<FUI>();
            this.StartFUI();
        }

        private void StartFUI()
        {
            m_createButton = this.FUIComponent.Get("CreateRoom").GObject.asButton;

            this.m_createButton.onClick.Set(this.CreateBtn_OnClick);
        }


        private void CreateBtn_OnClick()
        {
            CreateRoomAsync().NoAwait();
        }

        private async ETVoid CreateRoomAsync()
        {
            FUI fui = await FUIFactory.Create<CreateRoomViewComponent>(FUIType.CreateRoom);
        }
    }
}
