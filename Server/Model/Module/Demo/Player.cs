using System;
using System.Net;
using Google.Protobuf.Collections;

namespace ETModel
{
    [ObjectSystem]
    public class PlayerSystem : AwakeSystem<Player, string>
    {
        public override void Awake(Player self, string a)
        {
            self.Awake(a);
        }
    }

    [ObjectSystem]
    public class Player2System : AwakeSystem<Player, Session, UserDB>
    {
        public override void Awake(Player self, Session a, UserDB b)
        {
            self.Awake(a, b);
        }
    }

    [ObjectSystem]
    public class PlayerStartSystem : StartSystem<Player>
    {
        public override void Start(Player self)
        {
            self.Start();
        }
    }

    public static class PlayerHelper
    {
        public static Tank Tank(this Player self)
        {
            return Game.Scene.GetComponent<TankComponent>().Get(self.TankId);
        }
    }

    public enum PlayerState
    {
        Free,
        InRoom,
        InBattle,
    }

    public sealed class Player : Entity
    {
        public string Account { get; private set; }

        public UserDB UserDB { get; private set; }

        public long TankId { get; set; } = 0L;

        public long UnitId { get; set; } = 0L;

        private long m_roomId = 0L;

        public long RoomId
        {
            get => this.m_roomId;
            set
            {
                this.m_roomId = value;
                this.PlayerState = this.m_roomId == 0? PlayerState.Free : PlayerState.InRoom;
            }
        } 

        public PlayerState PlayerState { get; set; } = PlayerState.Free;

        public Session Session { get; set; }

        public void Awake(string account)
        {
            this.Account = account;
        }

        public void Awake(Session session, UserDB userDb)
        {
            this.UserDB = userDb;
            this.Session = session;

        }

        public void Start()
        {
            this.AfterSuccLogin();
        }

        /// <summary>
        ///  在回复登陆成功之后发送
        /// </summary>
        private void AfterSuccLogin()
        {
            this.Send_G2C_UserBaseInfo();
            this.Send_G2C_SettingInfo();
            this.Send_G2C_Rooms();
        }

        private void Send_G2C_UserBaseInfo()
        {
            // 下发等级，名字
            G2C_UserBaseInfo msg = new G2C_UserBaseInfo();

            UserBaseComponent userBase = this.UserDB.GetComponent<UserBaseComponent>();

            msg.Level = userBase.Level;

            msg.Experience = userBase.Experience;

            msg.Name = this.UserDB.GetComponent<UserBaseComponent>().UserName;

            msg.UserDBID = (long)this.UserDB.PhoneNum;

            msg.Gold = userBase.Gold;

            this.Session.Send(msg);
        }

        private void Send_G2C_SettingInfo()
        {
            SettingInfoComponent settingInfo = this.UserDB.GetComponent<SettingInfoComponent>();

            G2C_SettingInfo msg = new G2C_SettingInfo();

            msg.Language = settingInfo.Language;

            msg.Volume = settingInfo.Volume;

            msg.BinarySwitch = settingInfo.BinarySwitch;

            msg.RotSpeed = settingInfo.RotSpeed;

            this.Session.Send(msg);

            //msg.Language = this.UserDB;
        }

        public void Send_PopMessage(string str, PopMessageType type = PopMessageType.Float)
        {
            A2C_PopMessage msg = new A2C_PopMessage();

            msg.Text = str;

            msg.Type = type;

            this.Session.Send(msg);
        }

        public void Send_PopMessage(int messageID, PopMessageType type = PopMessageType.Float)
        {
            A2C_PopMessage msg = new A2C_PopMessage();

            msg.Text = Message.Get(this, messageID);

            msg.Type = type;

            this.Session.Send(msg);
        }

        private void Send_G2C_Rooms()
        {
            G2C_Rooms msg = new G2C_Rooms();

            msg.RoomSimpleInfo = new RepeatedField<RoomSimpleInfo>();

            Room[] rooms = Game.Scene.GetComponent<RoomComponent>().GetAll;

            foreach (Room room in rooms)
            {
                RoomSimpleInfo simpleInfo = new RoomSimpleInfo();

                simpleInfo.RoomId = room.Id;

                simpleInfo.PeopleNum = room.PeopleNum;

                simpleInfo.MapId = room.MapTableId;

                simpleInfo.BigModel = (int)room.BigModel;

                simpleInfo.SmallModel = room.SmallMode;

                simpleInfo.RoomName = room.RoomName;

                simpleInfo.State = room.State;

                simpleInfo.SerialNumber = room.SerialNumber;

                simpleInfo.ExistNum = room.Count;

                msg.RoomSimpleInfo.Add(simpleInfo);
            }

            this.Session.Send(msg);
        }


        public override void Dispose()
        {
            this.DisposeAsync().NoAwait();
        }

        private async ETVoid DisposeAsync()
        {
            if (this.IsDisposed)
            {
                return;
            }

            await this.UpdateUserDB();


            // 在房间中，未开始游戏，就退出房间
            if (this.PlayerState == PlayerState.InRoom || this.PlayerState == PlayerState.InBattle)
            {
                Room room = Game.Scene.GetComponent<RoomComponent>().Get(this.m_roomId);

                if (room.Remove(this.Id))
                {
                    if (room.Count == 0)
                    {
                        Game.Scene.GetComponent<RoomComponent>().Remove(room.Id);

                    }
                    else
                    {
                        room.BroadcastRoomDetailInfo();
                    }

                    BroadcastMessage.Send_G2C_Rooms();
                }
            }

            if (this.PlayerState == PlayerState.InBattle)
            {
                IPEndPoint mapAddress = StartConfigComponent.Instance.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;

                Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);

                G2B_Disconnect msg = new G2B_Disconnect();

                msg.PlayerId = this.Id;

                msg.BattleId = this.RoomId;

                mapSession.Send(msg);
            }

            base.Dispose();

            this.Account = string.Empty;

            this.UserDB = null;

            this.TankId = 0L;

            this.UnitId = 0L;

            this.RoomId = 0L;

            this.PlayerState = PlayerState.Free;

            this.Session = null;
        }


        public async ETTask UpdateUserDB()
        {

            DBProxyComponent db = Game.Scene.GetComponent<DBProxyComponent>();

            await db.Save(this.UserDB);


        }
    }
}