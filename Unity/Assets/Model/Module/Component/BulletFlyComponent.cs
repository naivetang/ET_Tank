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

    [ObjectSystem]
    public class BulletFlyFixUpdateSystem : FixedUpdateSystem<BulletFlyComponent>
    {
        public override void FixedUpdate(BulletFlyComponent self)
        {
            self.FixUpdate();
        }
    }

    public class BulletFlyComponent : Component
    {
        private Bullet m_bullet;

        private float speed = 500f;

        private float maxLiftTime = 5f;

        private float instantiateTime = 0f;

        private SphereCollider m_collider;


        public void Awake()
        {
            Log.Warning($"Awake时间:{TimeHelper.NowMilliSecond()}");

            this.m_bullet = this.GetParent<Bullet>();
            this.instantiateTime = Time.time;
            this.m_collider = this.m_bullet.GameObject.GetComponent<SphereCollider>();
        }

        public void Start()
        {

            Log.Warning($"Start时间:{TimeHelper.NowMilliSecond()}");

            OnTrigger();

        }

        private Tank Tank => this.m_bullet.Tank;

        public void Update()
        {
            //this.m_bullet.GameObject.transform.position += this.m_bullet.GameObject.transform.forward * this.speed * Time.deltaTime;



            
        }

        public void FixUpdate()
        {
            //this.m_bullet.GameObject.transform.position += this.m_bullet.GameObject.transform.forward * this.speed * Time.deltaTime;
            //this.m_bullet.GameObject.GetComponent<Rigidbody>().AddForce(this.m_bullet.GameObject.transform.forward * 100000);
            //this.m_bullet.GameObject.transform.Translate(this.m_bullet.GameObject.transform.position + this.m_bullet.GameObject.transform.forward * this.speed * Time.deltaTime);
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


        public void OnTrigger()
        {
            Vector3 pos = this.Tank.Gun.transform.position;

            Ray ray = new Ray(pos, this.Tank.Gun.transform.forward);

            RaycastHit raycastHit;

            Vector3 hitPoint = Vector3.zero;

            LayerMask layerMask = ~(1 << 9);

            // 过滤自己
            hitPoint = Physics.Raycast(ray, out raycastHit, 3000f, layerMask) ? raycastHit.point : ray.GetPoint(3000f);

            this.m_bullet.Position = hitPoint;

            if (raycastHit.collider != null)
            {
                OnTriggerEnter(raycastHit.collider);
            }

            else
            {
                
            }
        }

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
                //if (beAttackTank.TankCamp == this.Tank.TankCamp)
                //    return;

                int damage = this.m_bullet.AttackPower + this.Tank.GetComponent<NumericComponent>()[NumericType.Atk];

                // beAttackTank.BeAttacked(this.Tank, damage);

                // 发送炮弹的玩家才向服务器通知
                if(this.m_bullet.Tank.TankType == TankType.Local)
                    Send_C2B_AttackTank(beAttackTank.Id, damage).NoAwait();
            }

            this.m_bullet.Dispose();
        }

        /// <summary>
        /// 如果是自己打中了别的坦克才发送
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="damage"></param>
        private async ETVoid Send_C2B_AttackTank(long targetId, int damage)
        {
            C2B_AttackTankRequest msg = new C2B_AttackTankRequest();

            msg.SourceTankId = PlayerComponent.Instance.MyPlayer.TankId;

            msg.TargetTankId = targetId;

            msg.Damage = damage;

            B2C_AttackTankResponse response = (B2C_AttackTankResponse) await SessionComponent.Instance.Session.Call(msg);

            if(response.SourceTankId != msg.SourceTankId || response.TargetTankId != msg.TargetTankId)
                Log.Error("回包错误");

            ETModel.Tank tank = Game.Scene.GetComponent<TankComponent>().Get(targetId);

            tank.GetComponent<NumericComponent>().Set(NumericType.HpBase,response.CurrentHp); 
        }


    }
}
