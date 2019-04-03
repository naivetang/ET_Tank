using ETModel;
using System;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [ObjectSystem]
    public class BattleAwakeSystem: AwakeSystem<BattleComponent>
    {
        public override void Awake(BattleComponent self)
        {
            self.Awake();
        }
    }

    public class BattleComponent: Component
    {

        public static Action HeartBeat30ms;

        public void Awake()
        {
            this.HeartBeat30ms1().NoAwait();
        }


        private async ETVoid HeartBeat30ms1()
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();



            while (true)
            {
                await timerComponent.WaitAsync(30);

                HeartBeat30ms?.Invoke();
                B2C_TankInfos tankInfos = new B2C_TankInfos();
                tankInfos.TankInfos = new RepeatedField<TankInfo>();

                Tank[] tanks = Game.Scene.GetComponent<TankComponent>().GetAll();

                foreach (Tank tank in tanks)
                {
                    TankInfo tankInfo = new TankInfo();
                    tankInfo.TankId = tank.Id;

                    tankInfo.PX = tank.PX;
                    tankInfo.PY = tank.PY;
                    tankInfo.PZ = tank.PZ;

                    tankInfo.RX = tank.RX;
                    tankInfo.RY = tank.RY;
                    tankInfo.RZ = tank.RZ;

                    tankInfo.TurretRY = tank.TurretRY;
                    tankInfo.GunRX = tank.GunRX;
                    
                    tankInfos.TankInfos.Add(tankInfo);
                }

                MessageHelper.BroadcastTank(tankInfos);

                //
                // tankInfo.TankInfo.TankId = tank.Id;
                //
                // tankInfo.TankInfo.PX = tank.Position.x;
                // tankInfo.TankInfo.PY = tank.Position.y;
                // tankInfo.TankInfo.PZ = tank.Position.z;
                //
                // tankInfo.TankInfo.RX = tank.GameObject.transform.eulerAngles.x;
                // tankInfo.TankInfo.RY = tank.GameObject.transform.eulerAngles.y;
                // tankInfo.TankInfo.RZ = tank.GameObject.transform.eulerAngles.z;
                //
                // tankInfo.TankInfo.TurretRY = tank.Turret.transform.eulerAngles.y;
                // tankInfo.TankInfo.GunRX = tank.Gun.transform.eulerAngles.x;
                //
                // //Log.Warning("发送成功");
                //
                // SessionComponent.Instance.Session.Send(tankInfo);

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