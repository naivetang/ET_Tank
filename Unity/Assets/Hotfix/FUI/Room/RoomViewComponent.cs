using ETModel;
using FairyGUI;
using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [ObjectSystem]
    public class RoomViewAwakeComponent : AwakeSystem<RoomViewComponent>
    {
        public override void Awake(RoomViewComponent self)
        {
            self.Awake();
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

        private long roomId;

        private static G2C_RoomDetailInfo m_data = null;

        public static G2C_RoomDetailInfo Data
        {
            get => m_data;
            set
            {
                m_data = value;

                if (m_data == null)
                    return;

                FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Room);

                if(fui != null)
                    fui.GetComponent<RoomViewComponent>().RefreshData(m_data);
            }
        }


        public void Awake()
        {
            this.FUIComponent = this.GetParent<FUI>();

            if (Data == null)
            {
                Log.Error("数据出错");
                return;
            }

            this.RepeatedFieldToList(Data.LeftCamp, this.m_leftItems);
            this.RepeatedFieldToList(Data.RightCamp, this.m_rightItems);
            this.m_eftvPeopleNum = Data.RoomSimpleInfo.PeopleNum;
            this.m_roomOwnerId = Data.RoomSimpleInfo.RoomOwnerId;
            this.m_isOwners = this.m_roomOwnerId == PlayerComponent.Instance.MyPlayer.Id;
            this.roomId = Data.RoomId;
            this.StartFUI();
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




        private RoomOnePeople OwnInfo()
        {
            foreach (RoomOnePeople roomOnePeople in this.m_leftItems)
            {
                if (roomOnePeople.Id == PlayerComponent.Instance.MyPlayer.Id)
                    return roomOnePeople;
            }
            foreach (RoomOnePeople roomOnePeople in this.m_rightItems)
            {
                if (roomOnePeople.Id == PlayerComponent.Instance.MyPlayer.Id)
                    return roomOnePeople;
            }

            return null;
        }

        private void UI()
        {



            UpdateList(this.m_leftItems, this.m_leftList, true);

            UpdateList(this.m_rightItems, this.m_rightList, false);

            this.Lanaguage();

            if (this.m_isOwners)
            {
                this.m_startGame.onClick.Set(this.StartGame);
            }
            else
            {
                this.m_startGame.onClick.Set(this.Ready);
            }
        }

        private void Lanaguage()
        {
            if(this.m_isOwners)
                this.m_startGame.text = Message.Get(1001);
            else
            {
                if (this.OwnInfo().State)
                {
                    this.m_startGame.text = Message.Get(1047);
                }
                else
                {
                    this.m_startGame.text = Message.Get(1002);
                }
            }
                
            this.m_exit.text = Message.Get(1024);
        }

        private void StartGame()
        {
            this.Send_C2G_StartGame();
        }

        private void Send_C2G_StartGame()
        {
            C2G_StartGame msg = new C2G_StartGame();

            msg.RoomId = this.roomId;

            ETModel.SessionComponent.Instance.Session.Send(msg);
        }

        private void Ready()
        {
            Send();
        }

        private void Send()
        {
            C2G_Ready msg = new C2G_Ready();

            msg.RoomId = this.roomId;

            msg.Opt = this.OwnInfo().State? Ready_OPT.CancleReady : Ready_OPT.Ready;

            ETModel.SessionComponent.Instance.Session.Send(msg);
        }

        private void UpdateList(List<RoomOnePeople> items, GList list, bool left)
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

                com.onClick.Set(()=>{});

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


                if (left)
                {
                    com.onClick.Set(this.ChangeToLeftCamp);
                }
                else
                {
                    com.onClick.Set(this.ChangeToRightCamp);
                }

                ctl.selectedIndex = 2;
            }

            // 不可进入的玩家数量
            for (;i < list.numItems; i++)
            {
                GComponent com = list.GetChildAt(i).asCom;

                com.onClick.Set(() => { });

                Controller ctl = com.GetController("state");

                ctl.selectedIndex = 3;
            }
        }

        private void ChangeToLeftCamp()
        {
            C2G_ChangeCamp msg = new C2G_ChangeCamp();

            msg.RoomId = this.roomId;

            msg.TargetCamp = 1;

            ETModel.SessionComponent.Instance.Session.Send(msg);
        }

        private void ChangeToRightCamp()
        {
            C2G_ChangeCamp msg = new C2G_ChangeCamp();

            msg.RoomId = this.roomId;

            msg.TargetCamp = 2;

            ETModel.SessionComponent.Instance.Session.Send(msg);
        }

        private void ExitBtn_OnClick()
        {
            this.Send_C2G_ExitRoom().NoAwait();
        }

        private async ETVoid Send_C2G_ExitRoom()
        {
            C2G_ExitRoom msg = new C2G_ExitRoom();

            msg.Id = this.roomId;

            G2C_ExitRoom response = (G2C_ExitRoom) await ETModel.SessionComponent.Instance.Session.Call(msg);

            if (response.Error == ErrorCode.ERR_Success)
            {
                if (Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Hall) == null)
                {
                    await FUIFactory.Create<HallViewComponent>(FUIType.Hall);
                }

                this.OnClose();
            }

        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;

            base.Dispose();

            m_data = null;

        }
    }
}
