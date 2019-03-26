
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

        private SphereCollider m_collider;


        public void Awake()
        {
            this.m_bullet = this.GetParent<Bullet>();
            this.instantiateTime = Time.time;
            this.m_collider = this.m_bullet.GameObject.GetComponent<SphereCollider>();
        }

        public void Start()
        {


        }

        public void Update()
        {
            this.m_bullet.GameObject.transform.position += this.m_bullet.GameObject.transform.forward * this.speed * Time.deltaTime;

            if (Time.time - this.instantiateTime > this.maxLiftTime)
                this.m_bullet.Dispose();
        }

        public void OnCollisionEnter(Collision collision)
        {
            //Log.Info($"撞到了{collision.collider.transform.gameObject.name}");
            //Log.Info($"销毁子弹");


            // 创建爆炸效果

            ExplosionEffectFactory.Create(this.m_bullet.Position);


            /*if (collision.rigidbody.tag == Tag.Tank)
            {
                Log.Info("撞到坦克");
            }*/

            if (collision.rigidbody != null && collision.rigidbody.tag == Tag.Tank)
            {
                // 给坦克造成伤害

                TankComponent tankComponent = Game.Scene.GetComponent<TankComponent>();

                tankComponent.Remove(collision.rigidbody.gameObject.GetInstanceID());

            }

            this.m_bullet.Dispose();
        }
    }
}
