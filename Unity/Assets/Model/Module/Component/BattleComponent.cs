

using System;

namespace ETModel
{
    [ObjectSystem]
    public class BattleAwakeSystem : AwakeSystem<BattleComponent>
    {
        public override void Awake(BattleComponent self)
        {
            self.Awake();
        }
    }

    public class BattleComponent : Component
    {

        public Action HeartBeat30ms;

        public void Awake()
        {
            //this.HeartBeat30ms1().NoAwait();
        }


        private async ETVoid HeartBeat30ms1()
        {

            await ETTask.CompletedTask;

        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            HeartBeat30ms = null;

            base.Dispose();
        }
    }
}
