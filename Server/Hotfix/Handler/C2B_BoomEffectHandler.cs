using System;
using ETModel;
using PF;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Battle)]
    class C2B_BoomEffectHandler : AMActorLocationHandler<Tank, C2B_BoomEffect>
    {
        protected override void Run(Tank entity, C2B_BoomEffect message)
        {
            B2C_BoomEffect msg = new B2C_BoomEffect();

            msg.PX = message.PX;
            msg.PY = message.PY;
            msg.PZ = message.PZ;

            entity.BroadcastExceptSelf(msg);
        }
    }
}
