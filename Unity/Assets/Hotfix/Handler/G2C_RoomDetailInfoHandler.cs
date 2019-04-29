using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_RoomDetailInfoHandler : AMHandler<G2C_RoomDetailInfo>
    {
        protected override void Run(ETModel.Session session, G2C_RoomDetailInfo message)
        {
            RunAsync(session,message).NoAwait();

            
        }

        protected async ETVoid RunAsync(ETModel.Session session, G2C_RoomDetailInfo message)
        {
            RoomViewComponent.Data = message;


            await ETTask.CompletedTask;
            // FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Room);
            // if (fui == null)
            // {
            //     await FUIFactory.Create<RoomViewComponent, G2C_RoomDetailInfo>(FUIType.Room, message);
            //     return;
            // }
            // else
            // {
            //     RoomViewComponent roomView = fui.GetComponent<RoomViewComponent>();
            //
            //     roomView.RefreshData(message);
            // }
        }
    }
}
