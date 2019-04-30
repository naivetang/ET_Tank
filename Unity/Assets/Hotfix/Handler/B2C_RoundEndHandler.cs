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

            //ETModel.Game.Scene.GetComponent<TankComponent>().MyTank.GetComponent<LocalTankComponent>().StopMove = true;
        }
    }
}
