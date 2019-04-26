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

                    BroadcastMessage.Send_G2C_Rooms();
                }

                reply(response);

                BroadcastMessage.Send_G2C_Rooms();
            }
            catch (Exception e)
            {
               ReplyError(response,e,reply);
            }
            
        }

      

    }
}
