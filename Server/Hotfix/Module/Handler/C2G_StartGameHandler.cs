using System.Collections.Generic;
using System.Net;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_StartGameHandler : AMHandler<C2G_StartGame>
    {
        protected override void Run(Session session, C2G_StartGame message)
        {

            RunAsync(session,message).NoAwait();

            
        }

        protected async ETVoid RunAsync(Session session, C2G_StartGame message)
        {
            long roomId = message.RoomId;

            Room room = Game.Scene.GetComponent<RoomComponent>().Get(roomId);

            room.State = 2;

            IPEndPoint mapAddress = StartConfigComponent.Instance.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;

            Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);

            G2B_CreateBattle msg = new G2B_CreateBattle();

            msg.RoomId = roomId;

            msg.LeftCamp = new List<RoomOnePeople>();

            msg.RightCamp = new List<RoomOnePeople>();

            foreach (RoomOnePeople onePeople in room.GetLeftCamp())
            {
                RoomOnePeople tmp = new RoomOnePeople(onePeople);

                msg.LeftCamp.Add(tmp);
            }

            foreach (RoomOnePeople onePeople in room.GetRightCamp())
            {
                RoomOnePeople tmp = new RoomOnePeople(onePeople);

                msg.RightCamp.Add(tmp);
            }

            if (true)
            {
                msg.RoomSimpleInfo = new RoomSimpleInfo();

                RoomSimpleInfo roomSimpleInfo = msg.RoomSimpleInfo;

                roomSimpleInfo.RoomId = room.Id;

                roomSimpleInfo.PeopleNum = room.PeopleNum;

                roomSimpleInfo.MapId = room.MapTableId;

                roomSimpleInfo.BigModel = (int)room.BigModel;

                roomSimpleInfo.SmallModel = room.SmallMode;

                roomSimpleInfo.RoomName = room.RoomName;

                roomSimpleInfo.State = room.State;

                roomSimpleInfo.SerialNumber = room.SerialNumber;

                roomSimpleInfo.RoomOwnerId = room.OwnerId;
            }
            
            await mapSession.Call(msg);

            room.BroadCastStartGame();
        }
    }
}
