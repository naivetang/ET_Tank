using ETModel;
using PF;
using Vector3 = UnityEngine.Vector3;

namespace ETHotfix
{
    [MessageHandler]
    public class B2C_CreateTanksHandler : AMHandler<B2C_CreateTanks>
    {
        protected override void Run(ETModel.Session session, B2C_CreateTanks message)
        {
            Log.Warning("收到消息 B2C_CreateTanks");

            Log.Warning("进入战场，坦克数量=" + message.Tanks.Count);

            TankComponent tankComponent = ETModel.Game.Scene.GetComponent<TankComponent>();

            foreach (TankInfo tankInfo in message.Tanks)
            {
                if (tankComponent.Get(tankInfo.TankId) != null)
                {
                    continue;
                }

                Tank tank = TankFactory.Create(tankInfo.TankId);

                tank.TankCamp = tankInfo.TankCamp;

                tank.Position = new Vector3(tankInfo.PX * 1f / Tank.m_coefficient, tankInfo.PY * 1f / Tank.m_coefficient,
                        tankInfo.PZ * 1f / Tank.m_coefficient);
            }
        }
    }
}
