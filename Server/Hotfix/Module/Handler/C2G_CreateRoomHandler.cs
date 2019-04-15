using System;
using System.Net;
using ETModel;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_CreateRoomHandler : AMRpcHandler<C2G_CreateRoom,G2C_RoomDetailInfo>
    {
        protected override void Run(Session session, C2G_CreateRoom message, Action<G2C_RoomDetailInfo> reply)
        {
            RunAsync(session,message,reply).NoAwait();
        }

        protected async ETVoid RunAsync(Session session, C2G_CreateRoom message, Action<G2C_RoomDetailInfo> reply)
        {
            G2C_RoomDetailInfo response = new G2C_RoomDetailInfo();
            try
            {
                RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();

                Player player = session.GetComponent<SessionPlayerComponent>().Player;

                Room room = ComponentFactory.CreateWithId<Room>(player.Id);

                room.PeopleNum = message.PeopleNum;

                room.MapName = message.MapName;

                room.BigModel = (BigModel)message.BigModel;

                room.SmallMode = message.SmallModel;

                room.RoomName = message.RoomNam;

                //room.AddComponent<PlayerComponent>().Add(player);

                RoomOnePeople onePeople = room.AddLeftCamp(player);

                roomComponent.Add(room);

                response.Error = ErrorCode.ERR_Success;

                response.RoomId = room.Id;

                response.LeftCamp = new RepeatedField<RoomOnePeople>();

                RoomOnePeople one = new RoomOnePeople(onePeople);

                response.LeftCamp.Add(one);

                {
                    response.RoomSimpleInfo = new RoomSimpleInfo();
                    RoomSimpleInfo roomSimpleInfo = response.RoomSimpleInfo;
                    roomSimpleInfo.RoomId = room.Id;
                    roomSimpleInfo.PeopleNum = room.PeopleNum;
                    roomSimpleInfo.MapName = room.MapName;
                    roomSimpleInfo.BigModel = (int)room.BigModel;
                    roomSimpleInfo.SmallModel = room.SmallMode;
                    room.RoomName = room.RoomName;
                    roomSimpleInfo.State = 1;
                }

                reply(response);

                await ETTask.CompletedTask;
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
}
