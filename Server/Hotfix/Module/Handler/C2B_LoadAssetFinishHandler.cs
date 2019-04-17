using System;
using ETModel;
using PF;

namespace ETHotfix
{
    [ActorMessageHandler(AppType.Battle)]
    class C2B_LoadAssetFinishHandler:AMActorLocationHandler<Tank, C2B_LoadAssetFinish>
    {
        protected override void Run(Tank entity, C2B_LoadAssetFinish message)
        {
            throw new NotImplementedException();
        }
    }
}
