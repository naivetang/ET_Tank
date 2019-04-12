
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

        private float speed = 40f;

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

        private Tank Tank => this.m_bullet.Tank;

        public void Update()
        {
            this.m_bullet.GameObject.transform.position += this.m_bullet.GameObject.transform.forward * this.speed * Time.deltaTime;

            if (Time.time - this.instantiateTime > this.maxLiftTime)
                this.m_bullet.Dispose();
        }

        // public void OnCollisionEnter(Collision collision)
        // {
        //     Log.Info("进入OnCollisionEnter");
        //     // 创建爆炸效果
        //
        //     ExplosionEffectFactory.Create(this.m_bullet.Position);
        //
        //
        //     if (collision.rigidbody != null && collision.rigidbody.tag == Tag.Tank)
        //     {
        //         // 给坦克造成伤害
        //
        //         TankComponent tankComponent = Game.Scene.GetComponent<TankComponent>();
        //
        //         long objInstanceId = collision.rigidbody.gameObject.GetInstanceID();
        //
        //         Tank beAttackTank = tankComponent.Get(objInstanceId);
        //
        //         beAttackTank.BeAttacked(this.m_bullet.AttackPower);
        //
        //     }
        //
        //     this.m_bullet.Dispose();
        // }

        public void OnTriggerEnter(Collider other)
        {
            // Log.Info("OnTriggerEnter");

            // layer = 9 是自己
            if (other.gameObject != null && other.gameObject.layer == 9)
            {
                return;
            }


            // 创建爆炸效果

            ExplosionEffectFactory.Create(this.m_bullet.Position);


            if (other.attachedRigidbody != null && other.attachedRigidbody.tag == Tag.Tank)
            {
                // 给坦克造成伤害

                TankComponent tankComponent = Game.Scene.GetComponent<TankComponent>();

                long objInstanceId = other.attachedRigidbody.gameObject.GetInstanceID();

                Tank beAttackTank = tankComponent.Get(objInstanceId);

                // 如果自己阵营，不造成伤害
                if (beAttackTank.TankCamp == this.Tank.TankCamp)
                    return;

                int damage = this.m_bullet.AttackPower + this.Tank.GetComponent<NumericComponent>()[NumericType.Atk];

                beAttackTank.BeAttacked(this.Tank, damage);

                Send_C2B_AttackTank(beAttackTank.Id, damage);
            }

            this.m_bullet.Dispose();
        }


        private void Send_C2B_AttackTank(long targetId, int damage)
        {
            C2B_AttackTank msg = new C2B_AttackTank();

            msg.TargetTankId = targetId;

            msg.Damage = damage;

            SessionComponent.Instance.Session.Send(msg);
        }


    }
}
