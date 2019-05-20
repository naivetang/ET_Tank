using ETModel;
using PF;
using Vector3 = UnityEngine.Vector3;

namespace ETHotfix
{
    [MessageHandler]
    class B2C_TankResetHandler : AMHandler<B2C_TankReset>
    {
        protected override void Run(ETModel.Session session, B2C_TankReset message)
        {
            TankFrameInfo tankInfo = message.TankFrameInfo;

            Tank tank = ETModel.Game.Scene.GetComponent<TankComponent>().Get(tankInfo.TankId);

            tank.Position = new Vector3(tankInfo.PX * 1f / Tank.m_coefficient, tankInfo.PY * 1f / Tank.m_coefficient,
                    tankInfo.PZ * 1f / Tank.m_coefficient);

            tank.Reset();

        }
    }
}
