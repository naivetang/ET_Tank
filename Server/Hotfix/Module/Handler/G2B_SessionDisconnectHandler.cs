using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Battle)]
    public class G2B_SessionDisconnectHandler:AMActorLocationHandler<Tank,G2B_SessionDisconnect>
    {
        protected override void Run(Tank entity, G2B_SessionDisconnect message)
        {
            // 断开链接
            entity.GetComponent<TankGateComponent>().IsDisconnect = true;
        }
    }
}
