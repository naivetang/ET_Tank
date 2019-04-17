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

            IPEndPoint mapAddress = StartConfigComponent.Instance.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;

            Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);

            await mapSession.Call(new G2B_CreateBattle() { RoomId = message.RoomId });

            room.BroadCastStartGame();
        }
    }
}
