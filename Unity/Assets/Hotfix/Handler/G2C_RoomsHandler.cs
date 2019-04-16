using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_RoomsHandler : AMHandler<G2C_Rooms>
    {
        protected override void Run(ETModel.Session session, G2C_Rooms message)
        {
            RunAsync(session,message).NoAwait();
        }

        protected async ETVoid RunAsync(ETModel.Session session, G2C_Rooms message)
        {
            Game.EventSystem.Run(EventIdType.LoginHasFinish);

            FUI fui = await FUIFactory.Create<HallViewComponent, G2C_Rooms>(FUIType.Hall, message);

            if (fui == null)
            {
                Log.Error("还未创建大厅界面");
                return;
            }

        }
    }
}
