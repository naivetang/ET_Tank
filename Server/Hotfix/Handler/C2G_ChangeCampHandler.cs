using ETModel;
namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    class C2G_ChangeCampHandler : AMHandler<C2G_ChangeCamp>
    {
        protected override void Run(Session session, C2G_ChangeCamp message)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;

            Room room = Game.Scene.GetComponent<RoomComponent>().Get(message.RoomId);

            if (room.ChangeCamp(player.Id, message.TargetCamp))
            {
                room.BroadcastRoomDetailInfo();
            }
        }
    }
}
