using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    class B2C_TankDieHandler : AMHandler<B2C_TankDie>
    {
        protected override void Run(ETModel.Session session, B2C_TankDie message)
        {
            FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.MainInterface);

            if (fui != null)
            {
                Tank tank = ETModel.Game.Scene.GetComponent<TankComponent>().Get(message.DieTandkId);

                if (tank == null)
                {
                    Log.Error($"不存在坦克{message.DieTandkId}");
                    return;
                }
                if(tank.TankCamp == TankCamp.Left)
                    fui.GetComponent<MainItfViewComponent>().LeftWin();
                else
                {
                    fui.GetComponent<MainItfViewComponent>().RightWin();
                }
            }
        }
    }
}
