using ETModel;
using PF;
using Vector3 = UnityEngine.Vector3;

namespace ETHotfix
{
    [MessageHandler]
    public class B2C_TankFrameInfosHandler : AMHandler<B2C_TankFrameInfos>
    {
        protected override void Run(ETModel.Session session, B2C_TankFrameInfos message)
        {
            TankFrameInfo[] tankFrameInfo = message.TankFrameInfos.array;

            //Log.Warning($"坦克数量={message.TankInfos.Count}");

            foreach (TankFrameInfo info in message.TankFrameInfos)
            {
                if(TankComponent.Instance.MyTank.Id == info.TankId)
                    continue;

                Tank tank = TankComponent.Instance.Get(info.TankId);

                if (tank == null)
                {
                    Log.Error($"不存在坦克{info.TankId}");
                    continue;
                }

                int coefficient = Tank.m_coefficient;


                tank.GetComponent<RemoteTankComponent>().NetForecastInfo(
                        new PF.Vector3((info.PX * 1f) / coefficient, (info.PY * 1f) / coefficient, (info.PZ * 1f) / coefficient),
                        new PF.Vector3((info.RX * 1f) / coefficient, (info.RY * 1f) / coefficient, (info.RZ * 1f) / coefficient));

                tank.GetComponent<TurretComponent>().NetUpdate(info.GunRX * 1f / coefficient, info.TurretRY * 1f / coefficient);
                //tank.Position = new Vector3(info.PX,info.PY,info.PZ);
            }
        }
    }
}
