using System.Collections.Generic;
using ETModel;
using FairyGUI;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [ObjectSystem]
    public class HallViewAwakeComponent : AwakeSystem<HallViewComponent, G2C_Rooms>
    {
        public override void Awake(HallViewComponent self, G2C_Rooms a)
        {
            self.Awake(a);
        }
    }

    public class HallViewComponent : FUIBase
    {
        private GButton m_createButton;

        private GList m_list;

        private List<RoomSimpleInfo> m_rooms = new List<RoomSimpleInfo>();

        public void Awake(G2C_Rooms rooms)
        {
            this.FUIComponent = this.GetParent<FUI>();

            RepeatedFieldToList(rooms.RoomSimpleInfo,m_rooms);

            this.StartFUI();
        }

        private void StartFUI()
        {
            m_createButton = this.FUIComponent.Get("CreateRoom").GObject.asButton;

            m_list = this.FUIComponent.Get("n41").GObject.asCom.GetChild("n0").asList;

            this.m_list.numItems = 1;

            this.m_createButton.onClick.Set(this.CreateBtn_OnClick);
        }
        private void RepeatedFieldToList(RepeatedField<RoomSimpleInfo> a, List<RoomSimpleInfo> b)
        {
            
            if (a == null)
                return;

            foreach (RoomSimpleInfo roomOnePeople in a)
            {
                b.Add(roomOnePeople);
            }
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
