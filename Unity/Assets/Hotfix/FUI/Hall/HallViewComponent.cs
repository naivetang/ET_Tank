using System.Collections.Generic;
using ETModel;
using FairyGUI;
using Google.Protobuf.Collections;
using UnityEngine;

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

        private GList m_list;

        private GButton m_settingBtn;

        private List<RoomSimpleInfo> m_rooms = new List<RoomSimpleInfo>();

        private static G2C_Rooms m_data;

        public static G2C_Rooms Data
        {
            get => m_data;
            set
            {
                m_data = value;
                FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Hall);
                if (fui != null)
                {
                    fui.GetComponent<HallViewComponent>().RefreshData();
                }
            }

        } 

        public void Awake()
        {
            this.FUIComponent = this.GetParent<FUI>();

            this.StartFUI();
        }

        public void RefreshData()
        {
            RepeatedFieldToList(m_data.RoomSimpleInfo, m_rooms);
            this.UI();
        }

        private void RepeatedFieldToList(RepeatedField<RoomSimpleInfo> a, List<RoomSimpleInfo> b)
        {

            if (a == null)
                return;

            b.Clear();

            foreach (RoomSimpleInfo roomOnePeople in a)
            {
                b.Add(roomOnePeople);
            }
        }

        protected override void StartFUI()
        {
            m_createButton = this.FUIComponent.Get("CreateRoom").GObject.asButton;

            m_list = this.FUIComponent.Get("n41").GObject.asCom.GetChild("n0").asList;

            m_settingBtn = this.FUIComponent.Get("n15").GObject.asButton;

            m_settingBtn.onClick.Set(this.SettingBtn_OnClick);

            m_list.itemRenderer = ItemRenderer;

            m_list.onClickItem.Add(this.ListItemClick);

            this.m_createButton.onClick.Set(this.CreateBtn_OnClick);

            if(m_data != null)
                RepeatedFieldToList(m_data.RoomSimpleInfo, m_rooms);

            this.UI();
        }

        private void UI()
        {
            this.m_list.numItems = this.m_rooms.Count;
        }

        private void SettingBtn_OnClick()
        {
            this.CreateSetAsync().NoAwait();
        }

        private async ETVoid CreateSetAsync()
        {
            FUI fui = await FUIFactory.Create<GameSettingsViewComponent>(FUIType.GameSettings);
        }

        private void ItemRenderer(int i, GObject go)
        {
            RoomSimpleInfo roomSimpleInfo = this.m_rooms[i];

            // 序号
            go.asCom.GetChild("n3").asRichTextField.text = roomSimpleInfo.SerialNumber.ToString();

            go.asCom.GetChild("n4").asRichTextField.text = roomSimpleInfo.RoomName;

            go.asCom.GetChild("n5").asRichTextField.text = $"{roomSimpleInfo.ExistNum}/{roomSimpleInfo.PeopleNum*2}";

            go.asCom.GetChild("n6").asRichTextField.text = roomSimpleInfo.State == 1 ? Message.Get(4) : Message.Get(5);

            go.asCom.GetChild("n7").asRichTextField.text = Map.Get(roomSimpleInfo.MapId);



            if (roomSimpleInfo.State == 1)
            {
                go.asCom.GetChild("n3").asRichTextField.color = Color.white;
                go.asCom.GetChild("n4").asRichTextField.color = Color.white;
                go.asCom.GetChild("n5").asRichTextField.color = Color.white;
                go.asCom.GetChild("n6").asRichTextField.color = Color.white;
                go.asCom.GetChild("n7").asRichTextField.color = Color.white;
            }
            else
            {
                go.asCom.GetChild("n3").asRichTextField.color = Color.red;
                go.asCom.GetChild("n4").asRichTextField.color = Color.red;
                go.asCom.GetChild("n5").asRichTextField.color = Color.red;
                go.asCom.GetChild("n6").asRichTextField.color = Color.red;
                go.asCom.GetChild("n7").asRichTextField.color = Color.red;
            }

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

           await ETModel.SessionComponent.Instance.Session.Call(msg);

            // if (Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Room) != null)
            //     return;
            //
            // await FUIFactory.Create<RoomViewComponent, G2C_RoomDetailInfo>(FUIType.Room, response);
        }

        private void CreateBtn_OnClick()
        {
            CreateRoomAsync().NoAwait();
        }

        private async ETVoid CreateRoomAsync()
        {
            FUI fui = await FUIFactory.Create<CreateRoomViewComponent>(FUIType.CreateRoom);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
