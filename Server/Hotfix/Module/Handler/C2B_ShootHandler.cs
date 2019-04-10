using System;
using ETModel;
using PF;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Battle)]
    class C2B_ShootHandler : AMActorLocationHandler<Tank, C2B_Shoot>
    {
        protected override void Run(Tank entity, C2B_Shoot message)
        {
            B2C_Shoot b2CShoot = new B2C_Shoot();

            b2CShoot.TankId = entity.Id;

            b2CShoot.PX = message.PX;
            b2CShoot.PY = message.PY;
            b2CShoot.PZ = message.PZ;

            b2CShoot.RX = message.RX;
            b2CShoot.RY = message.RY;
            b2CShoot.RZ = message.RZ;

            entity.BroadcastExceptSelf(b2CShoot);

        }
    }
}
