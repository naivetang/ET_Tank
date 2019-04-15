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

        public void Awake()
        {
            Instance = this;
        }

        public void Add(Room room)
        {
            this.idRooms.Add(room.Id, room);
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

            Instance = null;
        }
    }
}
