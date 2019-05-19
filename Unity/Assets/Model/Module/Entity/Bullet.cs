using System;
using PF;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace ETModel
{
    [ObjectSystem]
    public class BulletAwake: AwakeSystem<Bullet,Tank>
    {
        public override void Awake(Bullet self, Tank _tank)
        {
            self.Awake(_tank);
        }
    }

    public sealed class Bullet : Entity
    {
        private GameObject m_gameObject;

        private Tank m_tank;

        private int m_attackPower = 40;

        private float m_instanceTime = 0f;

        public int AttackPower
        {
            get
            {
                // 飞行时间越长，炮弹威力越小

                float realAtt = m_attackPower - (Time.time - this.m_instanceTime) * 30;

                return Convert.ToInt32(realAtt);
            }
            set => this.m_attackPower = value;
        }

        public GameObject GameObject
        {
            set
            {
                this.m_gameObject = value;
            }
            get
            {
                return m_gameObject;
            }
        }

        public Tank Tank
        {
            get
            {
                return this.m_tank;
            }
        }

        public void Awake(Tank _tank)
        {
            this.m_tank = _tank;
            this.m_instanceTime = Time.time;
        }

        public Vector3 Position
        {
            get
            {
                return GameObject.transform.position;
            }
            set
            {
                GameObject.transform.position = value;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return GameObject.transform.rotation;
            }
            set
            {
                GameObject.transform.rotation = value;
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();

            // 回收GameObject
            resourcesComponent.RecycleObj(PrefabType.Bullet,this.m_gameObject, (obj) =>
            {
                // 移除碰撞组件
                //UnityEngine.Object.Destroy(obj.GetComponent<BulletCollision>());
            });

            // 从子弹所属坦克的子弹系统中移除这颗子弹，但是不Dispose，因为现在就在子弹的Dispose过程中。
            this.m_tank.GetComponent<BulletComponent>().RemoveNoDispose(this.InstanceId);

            
        }
    }
}
