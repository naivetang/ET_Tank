using System;
using ETModel;
using PF;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Battle)]
    public class C2B_TankFrameInfoHandler: AMActorLocationHandler<Tank, C2B_TankFrameInfo>
    {
        protected override void Run(Tank tank, C2B_TankFrameInfo message)
        {

            TankFrameInfo tankFrameInfo = message.TankFrameInfo;
            //tank.Position = new Vector3(tankFrameInfo.PX,tankFrameInfo.PY,tankFrameInfo.PZ);
            //tank.Rotation = new Vector3(tankFrameInfo.RX,tankFrameInfo.RY,tankFrameInfo.RZ);
            tank.PX = tankFrameInfo.PX;
            tank.PY = tankFrameInfo.PY;
            tank.PZ = tankFrameInfo.PZ;
            tank.RX = tankFrameInfo.RX;
            tank.RY = tankFrameInfo.RY;
            tank.RZ = tankFrameInfo.RZ;
            tank.GunRX = tankFrameInfo.GunRX;
            tank.TurretRY = tankFrameInfo.TurretRY;

            // Console.WriteLine($"坦克id = {entity.Id}");
            // Console.WriteLine($"坦克instanceId = {entity.InstanceId}");
            //
            // Console.WriteLine($"消息中坦克id = {message.TankFrameInfo.TankId}");
            //
            // Console.WriteLine($"坦克位置 = ({message.TankFrameInfo.PX},{message.TankFrameInfo.PY},{message.TankFrameInfo.PZ})");

        }
    }
}
