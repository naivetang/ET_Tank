using System;
using System.Net;
using ETModel;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_EnterRoomHandler : AMRpcHandler<C2G_EnterRoom,G2C_EnterRoom>
    {
        protected override void Run(Session session, C2G_EnterRoom message, Action<G2C_EnterRoom> reply)
        {
            RunAsync(session, message, reply).NoAwait();
        }

        protected async ETVoid RunAsync(Session session, C2G_EnterRoom message, Action<G2C_EnterRoom> reply)
        {
            G2C_EnterRoom response = new G2C_EnterRoom();

            try
            {
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();

                Player player = session.GetComponent<SessionPlayerComponent>().Player;

                Room room = roomComponent.Get(message.RoomId);

                if (room == null)
                {
                    Log.Error($"不存在房间{message.RoomId}");

                    response.Error = ErrorCode.ERR_RpcFail;
                }

                if (room.Add(player) == null)
                {
                    Log.Error($"房间已满，或者玩家{player.UserDB.Name}已经存在房间{room.RoomName}中");

                    response.Error = ErrorCode.ERR_RpcFail;
                }

                room.BroadcastRoomDetailInfo();

                BroadcastMessage.Send_G2C_Rooms();

                await ETTask.CompletedTask;

                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }


    }
}
