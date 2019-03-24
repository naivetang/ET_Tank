using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    /// 子弹发射、碰撞组件，绑定到Bullet上
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
            if (Input.GetMouseButtonDown(0))
            {
                this.Shoot();
            }
        }

        private void Shoot()
        {
            if (Time.time - this.lastShootTime < this.shootInterval)
                return;

            Vector3 pos = this.m_tank.Gun.transform.position + this.m_tank.Gun.transform.forward * 5;

            //
            Bullet bullet = BulletFactory.Create(this.m_tank);

            bullet.GameObject.transform.position = pos;

            bullet.GameObject.transform.rotation = this.m_tank.Gun.transform.rotation;

            //UnityEditor.EditorApplication.isPaused = true;

            this.lastShootTime = Time.time;
        }

    }
}
