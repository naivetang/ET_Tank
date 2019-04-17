using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    [ObjectSystem]
    public class RoomAwakeSystem : AwakeSystem<RoomComponent>
    {
        public override void Awake(RoomComponent self)
        {
            self.Awake();
        }
    }

    public class RoomComponent : Component
    {
        public static RoomComponent Instance { get; private set; }

        private readonly Dictionary<long,Room> idRooms = new Dictionary<long, Room>();

        // 序列化id
        private readonly Dictionary<int, bool> serialNumber = new Dictionary<int, bool>();

        public int GetSerialNum()
        {
            if (this.serialNumber.Count == this.idRooms.Count)
            {
                this.serialNumber[this.serialNumber.Count + 1] = false;
                return this.serialNumber.Count;
            }
            else
            {
                for (int i = 1; i <= this.serialNumber.Count; i++)
                {
                    if (this.serialNumber[i] == false)
                        return i;
                }       
            }

            return 0;
        }

        public void Awake()
        {
            Instance = this;
        }

        public void Add(Room room)
        {
            this.idRooms.Add(room.Id, room);

            this.serialNumber[room.SerialNumber] = true;
        }

        public void Remove(long id)
        {
            
            if (this.idRooms.TryGetValue(id, out Room room))
            {
                this.idRooms.Remove(id);

                this.serialNumber[room.SerialNumber] = false;

                return;
            }

            Log.Error($"不存在房间id {id}");
        }

        public Room Get(long id)
        {
            if (this.idRooms.TryGetValue(id, out Room room))
            {
                return room;
            }

            Log.Error($"不存在房间id {id}");
            return null;
        }

        public int Count => this.idRooms.Count;


        public Room[] GetAll => this.idRooms.Values.ToArray();

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            foreach (Room room in this.idRooms.Values)
            {
                room.Dispose();
            }

            this.idRooms.Clear();

            this.serialNumber.Clear();

            Instance = null;
        }
    }
}
