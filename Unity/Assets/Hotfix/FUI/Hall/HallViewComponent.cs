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

        private void RepeatedFieldToList(RepeatedField<RoomSimpleInfo> a, List<RoomSimpleInfo> b)
        {

            if (a == null)
                return;

            foreach (RoomSimpleInfo roomOnePeople in a)
            {
                b.Add(roomOnePeople);
            }
        }

        private void StartFUI()
        {
            m_createButton = this.FUIComponent.Get("CreateRoom").GObject.asButton;

            m_list = this.FUIComponent.Get("n41").GObject.asCom.GetChild("n0").asList;

            m_list.itemRenderer = ItemRenderer;

            m_list.onClickItem.Add(this.ListItemClick);

            this.m_list.numItems = this.m_rooms.Count;

            this.m_createButton.onClick.Set(this.CreateBtn_OnClick);
        }

        private void ItemRenderer(int i, GObject go)
        {
            RoomSimpleInfo roomSimpleInfo = this.m_rooms[i];

            // 序号
            go.asCom.GetChild("n3").asRichTextField.text = roomSimpleInfo.SerialNumber.ToString();

            go.asCom.GetChild("n4").asRichTextField.text = roomSimpleInfo.RoomName;

            go.asCom.GetChild("n5").asRichTextField.text = $"{roomSimpleInfo.PeopleNum}V{roomSimpleInfo.PeopleNum}";

            go.asCom.GetChild("n6").asRichTextField.text = roomSimpleInfo.State == 1 ? Message.Get(4) : Message.Get(5);

            go.asCom.GetChild("n7").asRichTextField.text = Map.Get(roomSimpleInfo.MapId);

            go.data = roomSimpleInfo;
        }

        private void ListItemClick(EventContext context)
        {
            if (!context.inputEvent.isDoubleClick)
                return;

            Send_C2G_EnterRoom((context.data as GButton).data as RoomSimpleInfo).NoAwait();
        }

        private async ETVoid Send_C2G_EnterRoom(RoomSimpleInfo roomInfo)
        {
            C2G_EnterRoom msg = new C2G_EnterRoom();

            msg.RoomId = roomInfo.RoomId;

            G2C_RoomDetailInfo response = (G2C_RoomDetailInfo) await ETModel.SessionComponent.Instance.Session.Call(msg);

            if (Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Room) != null)
                return;

            await FUIFactory.Create<RoomViewComponent, G2C_RoomDetailInfo>(FUIType.Room, response);
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
