using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    class C2G_ReadyHandler : AMHandler<C2G_Ready>
    {
        protected override void Run(Session session, C2G_Ready message)
        {
            Room room = ETModel.Game.Scene.GetComponent<RoomComponent>().Get(message.RoomId);

            long playerId = session.GetComponent<SessionPlayerComponent>().Player.Id;

            RoomOnePeople roomOnePeople =  room.GetPlayerRoomInfo(playerId);

            if (message.Opt == Ready_OPT.Ready)
            {
                roomOnePeople.State = true;
            }
            else
            {
                roomOnePeople.State = false;
            }

            room.BroadcastRoomDetailInfo();

        }
    }
}
