

using System;
using System.Threading;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class LocalTankAwakeSystem : AwakeSystem<LocalTankComponent>
    {
        public override void Awake(LocalTankComponent self)
        {
            self.Awake();
        }
    }

    public class LocalTankComponent : Component
    {
        private Tank m_tank;

        private CancellationTokenSource CancellationTokenSource;

        public void Awake()
        {
            m_tank = this.GetParent<Tank>();
            CancellationTokenSource = new CancellationTokenSource();
            this.HeartBeat30ms().NoAwait();
        }


        private async ETVoid HeartBeat30ms()
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();

            while (true)
            {
                await timerComponent.WaitAsync(30, CancellationTokenSource.Token);

                if (m_tank.Died)

                    continue;
                

                C2B_TankFrameInfo tankInfo = new C2B_TankFrameInfo();

                tankInfo.TankFrameInfo = new TankFrameInfo();


                tankInfo.TankFrameInfo.TankId = m_tank.Id;

                int m_coefficient = Tank.m_coefficient;

                tankInfo.TankFrameInfo.PX = Convert.ToInt32(m_tank.Position.x * m_coefficient);
                tankInfo.TankFrameInfo.PY = Convert.ToInt32(this.m_tank.Position.y* m_coefficient);
                tankInfo.TankFrameInfo.PZ = Convert.ToInt32(m_tank.Position.z* m_coefficient);

                tankInfo.TankFrameInfo.RX = Convert.ToInt32(m_tank.GameObject.transform.eulerAngles.x* m_coefficient);
                tankInfo.TankFrameInfo.RY = Convert.ToInt32(m_tank.GameObject.transform.eulerAngles.y* m_coefficient);
                tankInfo.TankFrameInfo.RZ = Convert.ToInt32(m_tank.GameObject.transform.eulerAngles.z* m_coefficient);

                TurretComponent turretComponent = m_tank.GetComponent<TurretComponent>();
                

                tankInfo.TankFrameInfo.TurretRY = Convert.ToInt32(turretComponent.RotTarget* m_coefficient);
                tankInfo.TankFrameInfo.GunRX = Convert.ToInt32(turretComponent .RollTarget* m_coefficient);

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

            base.Dispose();

            CancellationTokenSource?.Cancel();
            this.CancellationTokenSource?.Dispose();
        }
    }
}
