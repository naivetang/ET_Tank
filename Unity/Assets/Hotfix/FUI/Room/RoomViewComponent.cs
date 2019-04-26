using ETModel;
using FairyGUI;
using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [ObjectSystem]
    public class RoomViewAwakeComponent : AwakeSystem<RoomViewComponent, G2C_RoomDetailInfo>
    {
        public override void Awake(RoomViewComponent self, G2C_RoomDetailInfo msg)
        {
            self.Awake(msg);
        }
    }

    public class RoomViewComponent : FUIBase
    {
        private bool m_isOwners;

        private long m_roomOwnerId;

        // 有效人数
        private int m_eftvPeopleNum;

        private readonly List<RoomOnePeople> m_leftItems = new List<RoomOnePeople>();

        private readonly List<RoomOnePeople> m_rightItems = new List<RoomOnePeople>();

        private GButton m_startGame;

        private GButton m_exit;

        private GList m_leftList;

        private GList m_rightList;

        private long id;

        public void Awake(G2C_RoomDetailInfo msg)
        {
            this.FUIComponent = this.GetParent<FUI>();
            this.RepeatedFieldToList(msg.LeftCamp,this.m_leftItems);
            this.RepeatedFieldToList(msg.RightCamp,this.m_rightItems);
            this.m_eftvPeopleNum = msg.RoomSimpleInfo.PeopleNum;
            this.m_roomOwnerId = msg.RoomSimpleInfo.RoomOwnerId;
            this.m_isOwners = this.m_roomOwnerId == PlayerComponent.Instance.MyPlayer.Id;
            this.id = msg.RoomId;
            this.StartFUI();
        }

        public void RefreshData(G2C_RoomDetailInfo msg)
        {
            this.RepeatedFieldToList(msg.LeftCamp, this.m_leftItems);

            this.RepeatedFieldToList(msg.RightCamp, this.m_rightItems);

            this.m_roomOwnerId = msg.RoomSimpleInfo.RoomOwnerId;

            this.m_isOwners = this.m_roomOwnerId == PlayerComponent.Instance.MyPlayer.Id;

            this.UI();
        }

        private void RepeatedFieldToList(RepeatedField<RoomOnePeople> a, List<RoomOnePeople> b)
        {
            if (a == null)
                return;

            b.Clear();

            foreach (RoomOnePeople roomOnePeople in a)
            {
                b.Add(roomOnePeople);
            }
        }


        protected override void StartFUI()
        {
            this.m_startGame = this.FUIComponent.Get("n1").GObject.asButton;

            this.m_exit = this.FUIComponent.Get("n2").GObject.asButton;

            this.m_exit.onClick.Set(this.ExitBtn_OnClick);

            this.m_leftList = this.FUIComponent.Get("n9").GObject.asCom.GetChild("n0").asList;

            this.m_rightList = this.FUIComponent.Get("n10").GObject.asCom.GetChild("n0").asList;

            this.m_leftList.numItems = 5;
            
            this.m_rightList.numItems = 5;

            this.UI();
        }

        private void UI()
        {
            UpdateList(this.m_leftItems, this.m_leftList);

            UpdateList(this.m_rightItems, this.m_rightList);


            if (this.m_isOwners)
            {
                Message message = (Message)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Message), 1);
                this.m_startGame.text = message.English;
                this.m_startGame.onClick.Set(this.StartGame);
            }
            else
            {
                Message message = (Message)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Message), 2);
                this.m_startGame.text = message.English;
                this.m_startGame.onClick.Set(this.Ready);
            }
        }

        private void StartGame()
        {
            this.Send_C2G_StartGame();
        }

        private void Send_C2G_StartGame()
        {
            C2G_StartGame msg = new C2G_StartGame();

            msg.RoomId = this.id;

            ETModel.SessionComponent.Instance.Session.Send(msg);
        }

        private void Ready()
        {

        }

        private void UpdateList(List<RoomOnePeople> items, GList list)
        {
            if (items.Count > this.m_eftvPeopleNum)
            {
                Log.Error("填充数量大于五条");
                return;
            }

            int i = 0;

            // 在房间内的玩家
            for (; i < items.Count; i++)
            {
                RoomOnePeople item = items[i];
                GComponent com = list.GetChildAt(i).asCom;
                Controller ctl = com.GetController("state");
                ctl.selectedIndex = i % 2 == 1? 0 : 1;

                com.GetChild("touxian").asLoader.url = $"ui://Common/{item.Level}";

                com.GetChild("n6").asTextField.text = item.Name;

                com.GetChild("n12").asImage.visible = item.Id == this.m_roomOwnerId;

                com.GetChild("n7").asImage.visible = (item.Id != this.m_roomOwnerId) && (item.State? true : false);

            }

            // 还可进入的玩家数量
            for (; i < this.m_eftvPeopleNum; i++)
            {
                GComponent com = list.GetChildAt(i).asCom;
                Controller ctl = com.GetController("state");
                ctl.selectedIndex = 2;
            }

            // 不可进入的玩家数量
            for (;i < list.numItems; i++)
            {
                GComponent com = list.GetChildAt(i).asCom;
                Controller ctl = com.GetController("state");
                ctl.selectedIndex = 3;
            }
        }

        private void ExitBtn_OnClick()
        {
            this.Send_C2G_ExitRoom().NoAwait();

        }

        private async ETVoid Send_C2G_ExitRoom()
        {
            C2G_ExitRoom msg = new C2G_ExitRoom();

            msg.Id = this.id;

            G2C_ExitRoom response = (G2C_ExitRoom) await ETModel.SessionComponent.Instance.Session.Call(msg);

            if (response.Error == ErrorCode.ERR_Success)
            {
                this.OnClose();
            }

        }

    }
}
