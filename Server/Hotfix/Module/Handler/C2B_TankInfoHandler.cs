using System;
using ETModel;
using PF;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Battle)]
    public class C2B_TankInfoHandler: AMActorLocationHandler<Tank, C2B_TankInfo>
    {
        protected override void Run(Tank tank, C2B_TankInfo message)
        {

            TankInfo tankInfo = message.TankInfo;
            tank.Position = new Vector3(tankInfo.PX,tankInfo.PY,tankInfo.PZ);
            tank.Rotation = new Vector3(tankInfo.RX,tankInfo.RY,tankInfo.RZ);
            tank.GunRX = tankInfo.GunRX;
            tank.TurretRY = tankInfo.TurretRY;

            // Console.WriteLine($"坦克id = {entity.Id}");
            // Console.WriteLine($"坦克instanceId = {entity.InstanceId}");
            //
            // Console.WriteLine($"消息中坦克id = {message.TankInfo.TankId}");
            //
            // Console.WriteLine($"坦克位置 = ({message.TankInfo.PX},{message.TankInfo.PY},{message.TankInfo.PZ})");

        }
    }
}
