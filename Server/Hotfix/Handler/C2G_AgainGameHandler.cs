
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    class C2G_AgainGameHandler : AMHandler<C2G_AgainGame>
    {
        protected override void Run(Session session, C2G_AgainGame message)
        {
            Player player =  session.GetComponent<SessionPlayerComponent>().Player;

            Room room = Game.Scene.GetComponent<RoomComponent>().Get(message.RoomId);

            

            G2C_RoomDetailInfo msg = new G2C_RoomDetailInfo();

        }
    }
}
