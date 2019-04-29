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
        private GTextField m_title;

        private GButton m_createButton;
        private GButton m_mallButton;
        private GButton m_cangKuButton;
        private GButton m_friendButton;
        private GButton m_settingBtn;
        private GButton m_exitButton;

        private GList m_list;

        private GComponent m_listTitle;


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
            m_title = this.FUIComponent.Get("n9").GObject.asTextField;

            m_createButton = this.FUIComponent.Get("CreateRoom").GObject.asButton;
            this.m_mallButton = this.FUIComponent.Get("n12").GObject.asButton;
            this.m_cangKuButton = this.FUIComponent.Get("n13").GObject.asButton;
            this.m_friendButton = this.FUIComponent.Get("n14").GObject.asButton;
            m_settingBtn = this.FUIComponent.Get("n15").GObject.asButton;
            this.m_exitButton = this.FUIComponent.Get("n16").GObject.asButton;

            m_list = this.FUIComponent.Get("n41").GObject.asCom.GetChild("n0").asList;

            m_listTitle = this.FUIComponent.Get("n41").GObject.asCom.GetChild("n3").asCom;

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

            Lanaguage();
        }

        private void Lanaguage()
        {

            this.m_title.text = Message.Get(1006);

            this.m_createButton.text = Message.Get(1007);
            this.m_mallButton.text = Message.Get(1008);
            this.m_cangKuButton.text = Message.Get(1009);
            this.m_friendButton.text = Message.Get(1010);
            this.m_settingBtn.text = Message.Get(1011);
            this.m_exitButton.text = Message.Get(1012);

            m_listTitle.GetChild("n3").asRichTextField.text = Message.Get(1013);
            m_listTitle.GetChild("n4").asRichTextField.text = Message.Get(1014);
            m_listTitle.GetChild("n5").asRichTextField.text = Message.Get(1016);
            m_listTitle.GetChild("n6").asRichTextField.text = Message.Get(1017);
            m_listTitle.GetChild("n7").asRichTextField.text = Message.Get(1015);
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

            go.asCom.GetChild("n6").asRichTextField.text = roomSimpleInfo.State == 1 ? Message.Get(1004) : Message.Get(1005);

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

            G2C_EnterRoom response = (G2C_EnterRoom) await ETModel.SessionComponent.Instance.Session.Call(msg);

            if(response.Error == ErrorCode.ERR_Success)
                FUIFactory.Create<RoomViewComponent>(FUIType.Room).NoAwait();

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
