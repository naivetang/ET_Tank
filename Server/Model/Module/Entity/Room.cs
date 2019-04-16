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
    public sealed class Room : Entity
    {
        public int PeopleNum { get; set; }

        public int MapTableId { get; set; }

        public BigModel BigModel { get; set; }

        public int SmallMode { get; set; }

        public string RoomName { get; set; }

        // 房主
        public long OwnerId { get; set; }

        //当前状态 1:准备中 2:游戏中
        public int State { get; set; } = 1;

        // 房间的序列号 1 开始
        public int SerialNumber { get; set; }

        private List<RoomOnePeople> LeftCamp { get; set; } = new List<RoomOnePeople>();
        private List<RoomOnePeople> RightCamp { get; set; } = new List<RoomOnePeople>();

        private readonly Dictionary<long, Player> idPlayers = new Dictionary<long, Player>();

        public void Awake()
        {

        }

        public RoomOnePeople AddLeftCamp(Player player)
        {
            Add(player);
            RoomOnePeople one = new RoomOnePeople();
            one.Id = player.Id;
            one.Level = player.UserDB.Level;
            one.Name = player.UserDB.Name;
            one.State = false;
            this.LeftCamp.Add(one);
            return one;
        }

        public RoomOnePeople AddRightCamp(Player player)
        {
            Add(player);
            RoomOnePeople one = new RoomOnePeople();
            one.Id = player.Id;
            one.Level = player.UserDB.Level;
            one.Name = player.UserDB.Name;
            one.State = false;
            this.RightCamp.Add(one);
            return one;
        }

        private void Add(Player player)
        {
            this.idPlayers.Add(player.Id, player);
        }

        public Player Get(long id)
        {
            if(this.idPlayers.TryGetValue(id, out Player gamer))
                return gamer;
            Log.Error($"未找到Player id = {id}");
            return null;
        }

        public void Remove(long id)
        {
            this.idPlayers.Remove(id);
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
