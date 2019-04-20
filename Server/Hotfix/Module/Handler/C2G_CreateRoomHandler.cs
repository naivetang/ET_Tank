using System;
using System.Net;
using ETModel;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_CreateRoomHandler : AMHandler<C2G_CreateRoom>
    {
        protected override void Run(Session session, C2G_CreateRoom message)
        {
            RunAsync(session,message).NoAwait();
        }

        protected async ETVoid RunAsync(Session session, C2G_CreateRoom message)
        {
            try
            {
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();

                Player player = session.GetComponent<SessionPlayerComponent>().Player;

                Room room = ComponentFactory.CreateWithId<Room>(IdGenerater.GenerateId());

                room.SerialNumber = roomComponent.GetSerialNum();

                room.PeopleNum = message.PeopleNum;

                room.MapTableId = message.MapId;

                room.BigModel = (BigModel)message.BigModel;

                room.SmallMode = message.SmallModel;

                room.RoomName = message.RoomNam;

                room.OwnerId = player.Id;

                //room.AddComponent<PlayerComponent>().Add(player);

                room.Add(player);

                roomComponent.Add(room);

                this.Send_G2C_Rooms();

                await ETTask.CompletedTask;
            }
            catch (Exception e)
            {
                Log.Error(e);
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
