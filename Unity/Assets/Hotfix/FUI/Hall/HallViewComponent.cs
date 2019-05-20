using System;
using System.Collections.Generic;
using ETModel;
using FairyGUI;
using Google.Protobuf.Collections;
using UnityEngine;

namespace ETHotfix
{
    [Event(EventIdType.RoomViewClose)]
    class RoomViewClose_ClearChat:AEvent
    {
        public override void Run()
        {
            FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Hall);

            if (fui != null)
            {
                fui.GetComponent<HallViewComponent>().ClearChat();
            }
        }
    }

    [ObjectSystem]
    public class HallViewAwakeComponent : AwakeSystem<HallViewComponent>
    {
        public override void Awake(HallViewComponent self)
        {
            self.Awake();
        }
    }

    public class ChatView
    {
        private ChatType m_chatType;

        private GComponent FUIComponent;

        private GTextInput m_input;

        private GList m_chatList;

        public ChatView(GComponent com, ChatType chatType)
        {
            this.FUIComponent = com;
            this.m_chatType = chatType;
            this.StartUp();
        }

        private void StartUp()
        {
            this.m_input = this.FUIComponent.GetChild("n38").asLabel.GetChild("title").asTextInput;

            this.m_input.onSubmit.Set(this.OnSubmit);

            this.m_chatList = this.FUIComponent.GetChild("n40").asList;

            this.m_chatList.numItems = 0;
        }

        private void OnSubmit()
        {
            // 自己的黄色
            //AddChatData($"[color=#FFCC00]{PlayerComponent.Instance.MyPlayer.Name}[/color]:{this.m_input.text}");

            this.Send_C2G_ChatMessage(this.m_input.text);

            this.m_input.text = "";

        }

        public void UpdateChatInfo(G2C_ChatMessage msg)
        {
            AddChatData($"[color=#FFCC00]{msg.SourceName}[/color]:{msg.ChatStr}");
        }

        public void ClearChat()
        {
            this.m_chatList.numItems = 0;
        }

        private void AddChatData(string chatStr)
        {
             GComponent textCom = this.m_chatList.GetFromPool(String.Empty).asCom;

             textCom.GetChild("title").text = chatStr;

             this.m_chatList.AddChildAt(textCom, this.m_chatList.numItems);

             this.m_chatList.ScrollToView(this.m_chatList.numItems - 1);
        }

        private void Send_C2G_ChatMessage(string chatStr)
        {
            C2G_ChatMessage msg = new C2G_ChatMessage();

            msg.ChatStr = chatStr;

            msg.ChatType = this.m_chatType;

            ETModel.SessionComponent.Instance.Session.Send(msg);
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

        private ChatView m_chatView;


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
            Lanaguage();
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

        public void UpdateChatInfo(G2C_ChatMessage msg)
        {
            if(this.m_chatView != null)
                this.m_chatView.UpdateChatInfo(msg);
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

            m_exitButton.onClick.Set(this.ExitBtn_OnClick);

            m_list = this.FUIComponent.Get("n41").GObject.asCom.GetChild("n0").asList;

            m_listTitle = this.FUIComponent.Get("n41").GObject.asCom.GetChild("n3").asCom;

            m_settingBtn.onClick.Set(this.SettingBtn_OnClick);

            this.m_mallButton.onClick.Set(this.MallBtn_OnClick);

            m_list.itemRenderer = ItemRenderer;

            m_list.onClickItem.Add(this.ListItemClick);

            this.m_createButton.onClick.Set(this.CreateBtn_OnClick);

            this.m_cangKuButton.onClick.Set(this.Warehouse_OnClick);

            if(m_data != null)
                RepeatedFieldToList(m_data.RoomSimpleInfo, m_rooms);

            m_chatView = new ChatView(this.FUIComponent.Get("n29").GObject.asCom,ChatType.Hall);

            Lanaguage();

            this.UI();
        }

        private void UI()
        {
            this.m_list.numItems = this.m_rooms.Count;

            
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
            m_listTitle.GetChild("n13").asRichTextField.text = Message.Get(1072);
        }

        private void SettingBtn_OnClick()
        {
            this.CreateSetAsync().NoAwait();
        }

        private void ExitBtn_OnClick()
        {
            Application.Quit();
        }

        private void MallBtn_OnClick()
        {
            FUIFactory.Create<MallViewComponent>(FUIType.Mall).NoAwait();
        }

        private void Warehouse_OnClick()
        {
            this.Send_C2G_Warehouse().NoAwait();
        }

        private async ETVoid Send_C2G_Warehouse()
        {
            C2G_Warehouse msg = new C2G_Warehouse();

            G2C_Warehouse response = (G2C_Warehouse) await ETModel.SessionComponent.Instance.Session.Call(msg);

            WarehouseViewComponent.Data = response;

            FUIFactory.Create<WarehouseViewComponent>(FUIType.Warehouse).NoAwait();
        }

        private async ETVoid CreateSetAsync()
        {
            FUI fui = await FUIFactory.Create<GameSettingsViewComponent>(FUIType.GameSettings);
        }

        private string ModelName(RoomSimpleInfo roomSimple)
        {
            string str;

            // 回合制
            if (roomSimple.BigModel == 1)
            {
                str = $"{roomSimple.SmallModel} {Message.Get(1074)}";
            }

            //时间制
            else
            {
                str = $"{roomSimple.SmallModel} {Message.Get(1073)}";
            }

            return str;
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

            go.asCom.GetChild("n13").asRichTextField.text = ModelName(roomSimpleInfo);



            if (roomSimpleInfo.State == 1)
            {
                go.asCom.GetChild("n3").asRichTextField.color = Color.white;
                go.asCom.GetChild("n4").asRichTextField.color = Color.white;
                go.asCom.GetChild("n5").asRichTextField.color = Color.white;
                go.asCom.GetChild("n6").asRichTextField.color = Color.white;
                go.asCom.GetChild("n7").asRichTextField.color = Color.white;
                go.asCom.GetChild("n13").asRichTextField.color = Color.white;
            }
            else
            {
                go.asCom.GetChild("n3").asRichTextField.color = Color.red;
                go.asCom.GetChild("n4").asRichTextField.color = Color.red;
                go.asCom.GetChild("n5").asRichTextField.color = Color.red;
                go.asCom.GetChild("n6").asRichTextField.color = Color.red;
                go.asCom.GetChild("n7").asRichTextField.color = Color.red;
                go.asCom.GetChild("n13").asRichTextField.color = Color.red;
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

        public void ClearChat()
        {
            if(this.m_chatView != null)
                this.m_chatView.ClearChat();
        }
        

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
