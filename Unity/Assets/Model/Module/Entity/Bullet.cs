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

            UnityEngine.Object.Destroy(this.m_gameObject);

            this.m_tank.GetComponent<BulletComponent>().RemoveNoDispose(this.InstanceId);

            base.Dispose();
        }
    }
}
