using System;
using System.Net;
using ETModel;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    class C2G_ExitRoomHandler : AMRpcHandler<C2G_ExitRoom,G2C_ExitRoom>
    {
        protected override void Run(Session session, C2G_ExitRoom message, Action<G2C_ExitRoom> reply)
        {
            G2C_ExitRoom response = new G2C_ExitRoom();
            try
            {
                Player player = session.GetComponent<SessionPlayerComponent>().Player;

                long roomId = message.Id;

                Room room = Game.Scene.GetComponent<RoomComponent>().Get(roomId);

                room.Remove(player.Id);

                if (room.Count == 0)
                {
                    Game.Scene.GetComponent<RoomComponent>().Remove(roomId);

                    this.Send_G2C_Rooms();
                }

                reply(response);
            }
            catch (Exception e)
            {
               ReplyError(response,e,reply);
            }
            
        }

        private void Send_G2C_Rooms()
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

                msg.RoomSimpleInfo.Add(simpleInfo);
            }

            MessageHelper.BroadcastPlayer(msg);
        }

    }
}
