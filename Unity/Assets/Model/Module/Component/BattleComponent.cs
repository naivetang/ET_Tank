

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
            this.HeartBeat30ms1().NoAwait();
        }


        private async ETVoid HeartBeat30ms1()
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();
            Tank tank = TankComponent.Instance.MyTank;
            
            

            while (true)
            {
                await timerComponent.WaitAsync(30);

                if (tank == null)
                {
                    tank = TankComponent.Instance.MyTank;
                    continue;
                }

                HeartBeat30ms?.Invoke();
                C2B_TankInfo tankInfo = new C2B_TankInfo();
                tankInfo.TankInfo = new TankInfo();


                tankInfo.TankInfo.TankId = tank.Id;

                tankInfo.TankInfo.PX = tank.Position.x;
                tankInfo.TankInfo.PY = tank.Position.y;
                tankInfo.TankInfo.PZ = tank.Position.z;

                tankInfo.TankInfo.RX = tank.GameObject.transform.eulerAngles.x;
                tankInfo.TankInfo.RY = tank.GameObject.transform.eulerAngles.y;
                tankInfo.TankInfo.RZ = tank.GameObject.transform.eulerAngles.z;

                tankInfo.TankInfo.TurretRY = tank.Turret.transform.eulerAngles.y;
                tankInfo.TankInfo.GunRX = tank.Gun.transform.eulerAngles.x;
                
                //Log.Warning("发送成功");

                SessionComponent.Instance.Session.Send(tankInfo);

            }
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
