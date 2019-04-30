using System;
using System.Net;
using ETModel;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_CreateRoomHandler : AMRpcHandler<C2G_CreateRoom,G2C_CreateRoom>
    {
        protected override void Run(Session session, C2G_CreateRoom message, Action<G2C_CreateRoom> reply)
        {
            RunAsync(session, message, reply).NoAwait();
        }

        protected async ETVoid RunAsync(Session session, C2G_CreateRoom message, Action<G2C_CreateRoom> reply)
        {
            G2C_CreateRoom response = new G2C_CreateRoom();
            try
            {

                Player player = session.GetComponent<SessionPlayerComponent>().Player;

                if (!this.CheckRoomName(message.RoomNam))
                {
                    response.Error = ErrorCode.ERR_RpcFail;

                    response.Message = Message.Get(player, 1044);

                    reply(response);

                    A2C_PopMessage msg = new A2C_PopMessage();

                    msg.Text = response.Message;

                    msg.Type = PopMessageType.Float;

                    session.Send(msg);

                    return;
                }

                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
                

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

                room.BroadcastRoomDetailInfo();

                //player.RoomId = room.Id;

                roomComponent.Add(room);

                reply(response);

                BroadcastMessage.Send_G2C_Rooms();

                await ETTask.CompletedTask;
            }
            catch (Exception e)
            {
                Log.Error(e);
                ReplyError(response, e, reply);
            }
        }

        private bool CheckRoomName(string roomName)
        {
            if (string.IsNullOrWhiteSpace(roomName))
                return false;

            if (roomName.Length < 3 || roomName.Length > 14)
                return false;

            return true;
        }
    }
}
