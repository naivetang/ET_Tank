using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    class B2G_BattleEndHandler:AMHandler<B2G_BattleEnd>
    {
        protected override void Run(Session session, B2G_BattleEnd message)
        {
            Room room = Game.Scene.GetComponent<RoomComponent>().Get(message.RoomId);

            room.State = 1;

            // 房间内信息
            room.BroadcastRoomDetailInfo();

            // 房间外状态
            BroadcastMessage.Send_G2C_Rooms();
        }
    }
}
