using ETModel;
using PF;
using Vector3 = UnityEngine.Vector3;

namespace ETHotfix
{
    [MessageHandler]
    public class B2C_TankInfosHandler : AMHandler<B2C_TankInfos>
    {
        protected override void Run(ETModel.Session session, B2C_TankInfos message)
        {
            TankInfo[] tankInfo = message.TankInfos.array;

            Log.Warning($"坦克数量={message.TankInfos.Count}");

            foreach (TankInfo info in message.TankInfos)
            {
                if(TankComponent.Instance.MyTank.Id == info.TankId)
                    continue;
                Tank tank = TankComponent.Instance.Get(info.TankId);
                if (tank == null)
                {
                    Log.Error($"不存在坦克{info.TankId}");
                    continue;
                }

                tank.GetComponent<RemoteTankComponent>().NetForecastInfo(new PF.Vector3(info.PX,info.PY,info.PZ),new PF.Vector3(info.RX
                ,info.RY,info.RZ) );

                //tank.Position = new Vector3(info.PX,info.PY,info.PZ);
            }
        }
    }
}
