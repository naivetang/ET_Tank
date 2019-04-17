using System;
using System.Net;
using ETModel;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_EnterRoomHandler : AMHandler<C2G_EnterRoom>
    {
        protected override void Run(Session session, C2G_EnterRoom message)
        {
            RunAsync(session, message).NoAwait();
        }

        protected async ETVoid RunAsync(Session session, C2G_EnterRoom message)
        {
            try
            {
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();

                Player player = session.GetComponent<SessionPlayerComponent>().Player;

                Room room = roomComponent.Get(message.RoomId);

                if (room == null)
                {
                    Log.Error($"不存在房间{message.RoomId}");
                }

                if (room.Add(player) == null)
                {
                    Log.Error($"房间已满，或者玩家{player.UserDB.Name}已经存在房间{room.RoomName}中");
                }

                await ETTask.CompletedTask;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
