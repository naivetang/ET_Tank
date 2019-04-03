

using System;
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

        public static int m_coefficient = 1000000;

        private Tank m_tank;

        public void Awake()
        {
            m_tank = this.GetParent<Tank>();
            this.HeartBeat30ms1().NoAwait();
        }


        private async ETVoid HeartBeat30ms1()
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();

            while (true)
            {
                await timerComponent.WaitAsync(30);

                C2B_TankInfo tankInfo = new C2B_TankInfo();
                tankInfo.TankInfo = new TankInfo();


                tankInfo.TankInfo.TankId = m_tank.Id;

                tankInfo.TankInfo.PX = Convert.ToInt32(m_tank.Position.x * m_coefficient);
                tankInfo.TankInfo.PY = Convert.ToInt32(this.m_tank.Position.y* m_coefficient);
                tankInfo.TankInfo.PZ = Convert.ToInt32(m_tank.Position.z* m_coefficient);

                tankInfo.TankInfo.RX = Convert.ToInt32(m_tank.GameObject.transform.eulerAngles.x* m_coefficient);
                tankInfo.TankInfo.RY = Convert.ToInt32(m_tank.GameObject.transform.eulerAngles.y* m_coefficient);
                tankInfo.TankInfo.RZ = Convert.ToInt32(m_tank.GameObject.transform.eulerAngles.z* m_coefficient);

                tankInfo.TankInfo.TurretRY = Convert.ToInt32(m_tank.Turret.transform.eulerAngles.y* m_coefficient);
                tankInfo.TankInfo.GunRX = Convert.ToInt32(m_tank.Gun.transform.eulerAngles.x* m_coefficient);

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
        }
    }
}
