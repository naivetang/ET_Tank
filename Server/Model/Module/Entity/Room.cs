using System;
using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    [ObjectSystem]
    public class RoomEntityAwakeSystem : AwakeSystem<Room>
    {
        public override void Awake(Room self)
        {
            self.Awake();
        }
    }
    public enum BigModel
    {
        None = 0,
        Round = 1,
        Time = 2,
    }
    public sealed partial class Room : Entity
    {

        public int PeopleNum { get; set; }

        public int MapTableId { get; set; }

        public BigModel BigModel { get; set; }

        public int SmallMode { get; set; }

        public string RoomName { get; set; }

        // 房主
        public long OwnerId { get; set; }

        private int m_state = 1;

        /// <summary>
        /// 当前状态 1:准备中 2:游戏中
        /// </summary>
        public int State
        {
            get => this.m_state;
            set
            {
                this.m_state = value;

                if(value == 2)
                    foreach (Player player in this.idPlayers.Values)
                    {
                        player.PlayerState = PlayerState.InBattle;
                    }
                else if (value == 1)
                {
                    foreach (RoomOnePeople leftCampValue in this.LeftCamp.Values)
                    {
                        leftCampValue.State = false;
                    }
                    foreach (RoomOnePeople rightCampValue in this.RightCamp.Values)
                    {
                        rightCampValue.State = false;
                    }

                    foreach (Player player in this.idPlayers.Values)
                    {
                        player.PlayerState = PlayerState.InRoom;
                    }
                }
                    
            }
        }

        // 房间的序列号 1 开始
        public int SerialNumber { get; set; }

        private Dictionary<long,RoomOnePeople> LeftCamp { get; set; } = new Dictionary<long, RoomOnePeople>();

        private int LeftCount => this.LeftCamp.Count;

        private Dictionary<long,RoomOnePeople> RightCamp { get; set; } = new Dictionary<long, RoomOnePeople>();

        private int RightCount => this.RightCamp.Count;

        private readonly Dictionary<long, Player> idPlayers = new Dictionary<long, Player>();

        private long m_latestPlayerId;

        public void Awake()
        {

        }

        private RoomOnePeople AddLeftCamp(Player player)
        {
            RoomOnePeople one = new RoomOnePeople();
            one.Id = player.Id;
            one.Level = player.UserDB.GetComponent<UserBaseComponent>().Level;
            one.Name = player.UserDB.Name;
            one.State = false;
            one.Camp = 1;
            this.LeftCamp.Add(player.Id, one);
            return one;
        }

        

        private RoomOnePeople AddRightCamp(Player player)
        {
            RoomOnePeople one = new RoomOnePeople();
            one.Id = player.Id;
            one.Level = player.UserDB.GetComponent<UserBaseComponent>().Level;
            one.Name = player.UserDB.Name;
            one.State = false;
            one.Camp = 2;
            this.RightCamp.Add(player.Id, one);
            return one;
        }

        

        public RoomOnePeople Add(Player player)
        {
            if (this.Count >= this.PeopleNum * 2)
                return null;

            if (GetPlayer(player.Id) != null)
                return null;

            this.idPlayers.Add(player.Id, player);

            m_latestPlayerId = player.Id;

            RoomOnePeople ret = this.LeftCount <= this.RightCount? this.AddLeftCamp(player) : this.AddRightCamp(player);

            player.RoomId = this.Id;



            return ret;
        }

        public Player GetPlayer(long id)
        {
            if(this.idPlayers.TryGetValue(id, out Player gamer))
                return gamer;
            Log.Error($"未找到Player id = {id}");
            return null;
        }

        public RoomOnePeople GetPlayerRoomInfo(long id)
        {
            if (this.LeftCamp.TryGetValue(id, out RoomOnePeople left))
            {
                return left;
            }
            else if (this.RightCamp.TryGetValue(id, out RoomOnePeople right))
            {
                return right;
            }

            return null;
        }

        public bool ChangeCamp(long id, int targetCamp)
        {
            if (this.LeftCamp.TryGetValue(id, out RoomOnePeople left))
            {
                if (targetCamp == 1)
                    return false;

                if (this.RightCamp.Count >= this.PeopleNum)
                    return false;

                this.LeftCamp.Remove(id);

                left.Camp = 2;

                this.RightCamp.Add(id, left);
            }
            else if (this.RightCamp.TryGetValue(id, out RoomOnePeople right))
            {
                if (targetCamp == 2)
                    return false;

                if (this.LeftCamp.Count >= this.PeopleNum)
                    return false;

                this.RightCamp.Remove(id);

                right.Camp = 1;

                this.LeftCamp.Add(id, right);
            }
            else
            {
                Log.Error($"不存在玩家 {id}");
                return false;
            }
            return true;
        }

        public bool Remove(long id)
        {
            if (!this.idPlayers.TryGetValue(id, out Player player))
            {
                return false;
            }

            player.RoomId = 0;

            this.idPlayers.Remove(id);

            if (this.LeftCamp.ContainsKey(id))
            {
                this.LeftCamp.Remove(id);
            }
            else if (this.RightCamp.ContainsKey(id))
            {
                this.RightCamp.Remove(id);
            }



            if (id == OwnerId)
            {
                this.OwnerId = this.GetOwnerId();
            }

            this.m_latestPlayerId = id;

            return true;
        }

        private long GetOwnerId()
        {
            if (this.LeftCamp.Count > 0)
                return this.GetLeftCamp()[0].Id;
            else if (this.RightCamp.Count > 0)
                return this.GetRightCamp()[0].Id;
            else
            {
                return 0;
            }
        }

        public int Count
        {
            get
            {
                return this.idPlayers.Count;
            }
        }

        public Player[] GetAll()
        {
            return this.idPlayers.Values.ToArray();
        }

        public RoomOnePeople[] GetLeftCamp()
        {
            return this.LeftCamp.Values.ToArray();
        }

        public RoomOnePeople[] GetRightCamp()
        {
            return this.RightCamp.Values.ToArray();
        }

        public bool CanStartGame(ref int errCode)
        {
            if (this.LeftCamp.Count + this.RightCamp.Count != this.PeopleNum * 2)
            {
                errCode = 1045;
                return false;
            }

            foreach (RoomOnePeople roomOnePeople in this.LeftCamp.Values)
            {
                if (!roomOnePeople.State && roomOnePeople.Id != this.OwnerId)
                {
                    errCode = 1046;
                    return false;
                }
            }

            foreach (RoomOnePeople roomOnePeople in this.RightCamp.Values)
            {
                if (!roomOnePeople.State && roomOnePeople.Id != this.OwnerId)
                {
                    errCode = 1046;
                    return false;
                }
            }

            return true;
        }


        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.PeopleNum = 0;

            this.MapTableId = 0;

            this.BigModel = BigModel.None;

            this.SmallMode = 0;

            this.RoomName = string.Empty;

            this.State = 1;

            this.idPlayers.Clear();
        }
    }
}
