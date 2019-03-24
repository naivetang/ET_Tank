
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class BulletFlyAwakeSystem : AwakeSystem<BulletFlyComponent>
    {
        public override void Awake(BulletFlyComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class BulletFlyStartSystem : StartSystem<BulletFlyComponent>
    {
        public override void Start(BulletFlyComponent self)
        {
            self.Start();
        }
    }

    [ObjectSystem]
    public class BulletFlyUpdateSystem : UpdateSystem<BulletFlyComponent>
    {
        public override void Update(BulletFlyComponent self)
        {
            self.Update();
        }
    }

    public class BulletFlyComponent : Component
    {
        private Bullet m_bullet;

        private float speed = 100f;

        private float maxLiftTime = 2f;

        private float instantiateTime = 0f;

        public void Awake()
        {
            this.m_bullet = this.GetParent<Bullet>();
            this.instantiateTime = Time.time;
        }

        public void Start()
        {


        }

        public void Update()
        {
            this.m_bullet.GameObject.transform.position += this.m_bullet.GameObject.transform.forward * this.speed * Time.deltaTime;

             if(Time.time - this.instantiateTime > this.maxLiftTime)
                 this.m_bullet.Dispose();
        }

    }
}
