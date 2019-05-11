using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    class C2G_ChatMessageHandler : AMHandler<C2G_ChatMessage>
    {
        protected override void Run(Session session, C2G_ChatMessage message)
        {
            G2C_ChatMessage msg = new G2C_ChatMessage();

            msg.ChatStr = message.ChatStr;

            msg.ChatType = message.ChatType;

            Player player = session.GetComponent<SessionPlayerComponent>().Player;

            msg.SourceId = player.Id;

            msg.SourceName = player.UserDB.GetComponent<UserBaseComponent>().UserName;

            if (message.ChatType == ChatType.Room)
            {

                long roomId = session.GetComponent<SessionPlayerComponent>().Player.RoomId;

                Room room = Game.Scene.GetComponent<RoomComponent>().Get(roomId);

                msg.TankCamp = (TankCamp)room.GetPlayerRoomInfo(player.Id).Camp;

                room.BroadCast(msg);

            }
            else
            {
                MessageHelper.BroadcastPlayer(msg);
            }
        }
    }
}
