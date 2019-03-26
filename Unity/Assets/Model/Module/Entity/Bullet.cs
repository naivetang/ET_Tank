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

            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();

            // 回收GameObject
            resourcesComponent.RecycleObj(PrefabType.Bullet,this.m_gameObject, (obj) =>
            {
                // 移除碰撞组件
                UnityEngine.Object.Destroy(obj.GetComponent<BulletCollision>());
            });

            // 从子弹所属坦克的子弹系统中移除这颗子弹，但是不Dispose，因为现在就在子弹的Dispose过程中。
            this.m_tank.GetComponent<BulletComponent>().RemoveNoDispose(this.InstanceId);

            base.Dispose();
        }
    }
}
