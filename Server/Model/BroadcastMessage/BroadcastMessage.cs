
using Google.Protobuf.Collections;

namespace ETModel
{
    public static class BroadcastMessage
    {
        public static void Send_G2C_Rooms()
        {
            // 下发所有房间信息
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

            MessageHelper.BroadcastPlayer(msg);
        }
    }
}
