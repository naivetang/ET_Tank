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

                Send_B2C_TankFrameInfos();
                //
                // tankFrameInfo.TankFrameInfo.TankId = tank.Id;
                //
                // tankFrameInfo.TankFrameInfo.PX = tank.Position.x;
                // tankFrameInfo.TankFrameInfo.PY = tank.Position.y;
                // tankFrameInfo.TankFrameInfo.PZ = tank.Position.z;
                //
                // tankFrameInfo.TankFrameInfo.RX = tank.GameObject.transform.eulerAngles.x;
                // tankFrameInfo.TankFrameInfo.RY = tank.GameObject.transform.eulerAngles.y;
                // tankFrameInfo.TankFrameInfo.RZ = tank.GameObject.transform.eulerAngles.z;
                //
                // tankFrameInfo.TankFrameInfo.TurretRY = tank.Turret.transform.eulerAngles.y;
                // tankFrameInfo.TankFrameInfo.GunRX = tank.Gun.transform.eulerAngles.x;
                //
                // //Log.Warning("发送成功");
                //
                // SessionComponent.Instance.Session.Send(tankFrameInfo);

            }
        }

        private void Send_B2C_TankFrameInfos()
        {
            B2C_TankFrameInfos tankInfos = new B2C_TankFrameInfos();
            tankInfos.TankFrameInfos = new RepeatedField<TankFrameInfo>();

            Tank[] tanks = Game.Scene.GetComponent<TankComponent>().GetAll();

            foreach (Tank tank in tanks)
            {
                TankFrameInfo tankFrameInfo = new TankFrameInfo();
                tankFrameInfo.TankId = tank.Id;

                tankFrameInfo.PX = tank.PX;
                tankFrameInfo.PY = tank.PY;
                tankFrameInfo.PZ = tank.PZ;

                tankFrameInfo.RX = tank.RX;
                tankFrameInfo.RY = tank.RY;
                tankFrameInfo.RZ = tank.RZ;

                tankFrameInfo.TurretRY = tank.TurretRY;
                tankFrameInfo.GunRX = tank.GunRX;

                tankInfos.TankFrameInfos.Add(tankFrameInfo);
            }

            MessageHelper.BroadcastTank(tankInfos);
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