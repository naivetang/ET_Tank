using System;
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
    public class PlayerStartSystem:StartSystem<Player>
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

    public sealed class Player : Entity
	{
		public string Account { get; private set; }

        public UserDB UserDB { get; private set; }

        public long TankId { get; set; } = 0L;

        public long UnitId { get; set; } = 0L;

        public Session Session { get; set; }

		public void Awake(string account)
		{
			this.Account = account;
		}

        public void Awake(Session session,UserDB userDb)
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
            this.Send_G2C_Rooms();
        }

        private void Send_G2C_UserBaseInfo()
        {
            // 下发等级，名字
            G2C_UserBaseInfo msg = new G2C_UserBaseInfo();

            msg.Level = this.UserDB.Level;

            msg.Experience = this.UserDB.Experience;

            msg.Name = this.UserDB.Name;

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

                msg.RoomSimpleInfo.Add(simpleInfo);
            }

            this.Session.Send(msg);
        }


        public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

            this.Account = string.Empty;

            this.UserDB = null;

            this.TankId = 0L;

            this.UnitId = 0L;
        }
	}
}