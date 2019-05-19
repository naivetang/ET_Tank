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

            entity.BroadcastExceptSelf(b2CShoot);

        }
    }
}
