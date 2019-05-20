using ETModel;
using PF;
using Vector3 = UnityEngine.Vector3;

namespace ETHotfix
{
    [MessageHandler]
    class B2C_RoundEndHandler : AMHandler<B2C_RoundEnd>
    {
        protected override void Run(ETModel.Session session, B2C_RoundEnd message)
        {
            Log.Warning($"胜利方{message.WinCamp}");

            FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.MainInterface);

            if (fui != null)
            {
                if(message.WinCamp == 1)
                    fui.GetComponent<MainItfViewComponent>().LeftWin();
                else if(message.WinCamp == 2)
                    fui.GetComponent<MainItfViewComponent>().RightWin();
                else
                {
                    Log.Error($"winCamp {message.WinCamp} 有误");
                }
            }

            //ETModel.Game.Scene.GetComponent<TankComponent>().MyTank.GetComponent<LocalTankComponent>().StopMove = true;
        }
    }
}
