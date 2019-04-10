using System;
using System.Collections.Generic;
using System.Linq;
using PF;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace ETModel
{
    [ObjectSystem]
    public class TankShootAwakeComponent:AwakeSystem<TankShootComponent>
    {
        public override void Awake(TankShootComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class TankShootUpdateComponent : UpdateSystem<TankShootComponent>
    {
        public override void Update(TankShootComponent self)
        {
            self.Update();
        }
    }

    /// <summary>
    /// 子弹发射、碰撞组件，绑定到Tank上
    /// </summary>
    public class TankShootComponent: Component
    {
        private Tank m_tank;

        // 上一次开炮时间
        private float lastShootTime = 0f;

        // 开炮时间间隔
        private float shootInterval = 0.5f;


        public void Awake()
        {
            this.m_tank = this.GetParent<Tank>();
        }

        public void Update()
        {
            if (this.m_tank.m_tankType != TankType.Local)
                return;
            if (Input.GetMouseButtonDown(0))
            {
                this.LocalShoot();
            }
        }

        public void NetShoot(IntVector3 intPos, IntVector3 intAngles)
        {

            Vector3 pos = new Vector3(intPos.x * 1f / Tank.m_coefficient, intPos.y * 1f / Tank.m_coefficient, intPos.z * 1f / Tank.m_coefficient);

            Vector3 eulerAngles = new Vector3(intAngles.x*1f/Tank.m_coefficient, intAngles.y * 1f / Tank.m_coefficient,intAngles.z * 1f / Tank.m_coefficient);

            RemoteShoot(pos,eulerAngles);
        }

        private void LocalShoot()
        {
            if (this.m_tank.m_tankType != TankType.Local)
                return;

            if (Time.time - this.lastShootTime < this.shootInterval)
                return;

            // this.m_tank.BeAttacked(30f);

            Vector3 pos = this.m_tank.Gun.transform.position + this.m_tank.Gun.transform.forward * 2;

            //
            Bullet bullet = BulletFactory.Create(this.m_tank);

            bullet.GameObject.transform.position = pos;

            bullet.GameObject.transform.rotation = this.m_tank.Gun.transform.rotation;

            //UnityEditor.EditorApplication.isPaused = true;


            //UnityEditor.EditorApplication.isPaused = true;

            Send_C2B_Shoot(pos, bullet.GameObject.transform.eulerAngles);

            this.lastShootTime = Time.time;
        }

        private void Send_C2B_Shoot(Vector3 pos, Vector3 eulerAngles)
        {
            C2B_Shoot c2BShoot = new C2B_Shoot();

            c2BShoot.PX = Convert.ToInt32(pos.x * Tank.m_coefficient);
            c2BShoot.PY = Convert.ToInt32(pos.y * Tank.m_coefficient);
            c2BShoot.PZ = Convert.ToInt32(pos.z * Tank.m_coefficient);

            c2BShoot.RX = Convert.ToInt32(eulerAngles.x * Tank.m_coefficient);
            c2BShoot.RY = Convert.ToInt32(eulerAngles.y * Tank.m_coefficient);
            c2BShoot.RZ = Convert.ToInt32(eulerAngles.z * Tank.m_coefficient);

            SessionComponent.Instance.Session.Send(c2BShoot);
        }

        private void RemoteShoot(Vector3 pos, Vector3 eulerAngles)
        {
            if (this.m_tank.m_tankType != TankType.Remote)
                return;

            Bullet bullet = BulletFactory.Create(this.m_tank);

            bullet.GameObject.transform.position = pos;

            bullet.GameObject.transform.eulerAngles = eulerAngles;

        }

    }
}
