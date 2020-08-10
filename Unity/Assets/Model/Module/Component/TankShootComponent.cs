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

        public static Action<float> ShootNoticeHtf;

        // 上一次开炮时间
        private float lastShootTime = 0f;

        // 开炮时间间隔
        private float shootInterval = 2f;


        public void Awake()
        {
            this.m_tank = this.GetParent<Tank>();
        }

        public void Update()
        {
            if (this.m_tank.TankType != TankType.Local)
                return;
            if (Input.GetMouseButtonDown(0))
            {
                this.LocalShoot();
            }
        }

        public void NetShoot()
        {

           this.ShowShootEffect();
        }


        private void ShootNotice()
        {
            ShootNoticeHtf?.Invoke(this.shootInterval);
        }

        /// <summary>
        /// 本地坦克发射炮弹
        /// </summary>
        private void LocalShoot()
        {
            if (this.m_tank.TankType != TankType.Local)
                return;

            if (Time.time - this.lastShootTime < this.shootInterval)
                return;

            // this.m_tank.BeAttacked(30f);

            this.ShootNotice();

            Vector3 pos = this.m_tank.Gun.transform.position + this.m_tank.Gun.transform.forward * 2;

            //
            //Bullet bullet = BulletFactory.Create(this.m_tank);

            //bullet.GameObject.transform.position = pos;

            //bullet.GameObject.transform.rotation = this.m_tank.Gun.transform.rotation;

            //UnityEditor.EditorApplication.isPaused = true;


            //UnityEditor.EditorApplication.isPaused = true;

            this.ShowShootEffect();

            Send_C2B_Shoot();

            this.RayDetection();
            

            this.ShootAudio();

            this.lastShootTime = Time.time;
        }

        /// <summary>
        /// 显示发射特效
        /// </summary>
        private void ShowShootEffect()
        {
            this.m_tank.m_shootEffect.SetActive(false);
            this.m_tank.m_shootEffect.SetActive(true);
        }

        /// <summary>
        /// 射线监测
        /// </summary>
        public void RayDetection()
        {
            Vector3 pos = this.m_tank.Gun.transform.position;

            Ray ray = new Ray(pos, this.m_tank.Gun.transform.forward);

            RaycastHit raycastHit;

            Vector3 hitPoint = Vector3.zero;

            LayerMask layerMask = ~(1 << 9);

            // 过滤自己
            hitPoint = Physics.Raycast(ray, out raycastHit, 3000f, layerMask) ? raycastHit.point : ray.GetPoint(3000f);

            //this.m_bullet.Position = hitPoint;

            if (raycastHit.collider != null)
            {
                OnTriggerEnter(raycastHit.collider, hitPoint);
            }

            else
            {

            }
        }


        /// <summary>
        /// 创建爆炸特效
        /// </summary>
        /// <param name="boomPos"></param>
        public static void CreateBoomEffect(Vector3 boomPos)
        {
            ExplosionEffectFactory.Create(boomPos);
        }

        /// <summary>
        /// 爆炸效果、伤害上传
        /// </summary>
        /// <param name="other"></param>
        /// <param name="boomPos"></param>
        public void OnTriggerEnter(Collider other,Vector3 boomPos)
        {
            // Log.Info("OnTriggerEnter");

            // layer = 9 是自己
            if (other.gameObject != null && other.gameObject.layer == 9)
            {
                return;
            }

            // 创建爆炸效果

            CreateBoomEffect(boomPos);

            Send_C2B_BoomEffect(boomPos);


            if (other.attachedRigidbody != null && other.attachedRigidbody.tag == Tag.Tank)
            {
                // 给坦克造成伤害

                TankComponent tankComponent = Game.Scene.GetComponent<TankComponent>();

                long objInstanceId = other.attachedRigidbody.gameObject.GetInstanceID();

                Tank beAttackTank = tankComponent.Get(objInstanceId);

                // 如果自己阵营，不造成伤害
                //if (beAttackTank.TankCamp == this.Tank.TankCamp)
                //    return;


                // 暂时只计算坦克自己造成的伤害，不计算炮弹造成的伤害
                int damage = this.m_tank.GetComponent<NumericComponent>()[NumericType.Atk];

                // beAttackTank.BeAttacked(this.Tank, damage);

                // 发送炮弹的玩家才向服务器通知
                if (this.m_tank.TankType == TankType.Local)
                    Send_C2B_AttackTank(beAttackTank.Id, damage).NoAwait();
            }

            //this.m_bullet.Dispose();
        }

        private void Send_C2B_BoomEffect(Vector3 boomPos)
        {
            C2B_BoomEffect msg = new C2B_BoomEffect();

            msg.PX = (int)(boomPos.x * Tank.m_coefficient);
            msg.PY = (int)(boomPos.y * Tank.m_coefficient);
            msg.PZ = (int)(boomPos.z * Tank.m_coefficient);

            ETModel.SessionComponent.Instance.Session.Send(msg);
        }

        private async ETVoid Send_C2B_AttackTank(long targetId, int damage)
        {
            C2B_AttackTankRequest msg = new C2B_AttackTankRequest();

            msg.SourceTankId = PlayerComponent.Instance.MyPlayer.TankId;

            msg.TargetTankId = targetId;

            msg.Damage = damage;

            B2C_AttackTankResponse response = (B2C_AttackTankResponse)await SessionComponent.Instance.Session.Call(msg);

            if (response.SourceTankId != msg.SourceTankId || response.TargetTankId != msg.TargetTankId)
                Log.Error("回包错误");

            ETModel.Tank tank = Game.Scene.GetComponent<TankComponent>().Get(targetId);

            tank.GetComponent<NumericComponent>().Set(NumericType.HpBase, response.CurrentHp);
        }

        /// <summary>
        /// 发射炮弹音效
        /// </summary>
        private void ShootAudio()
        {

        }

        private void Send_C2B_Shoot()
        {
            C2B_Shoot c2BShoot = new C2B_Shoot();
            SessionComponent.Instance.Session.Send(c2BShoot);
        }

        private void RemoteShoot(Vector3 pos, Vector3 eulerAngles)
        {
            if (this.m_tank.TankType != TankType.Remote)
                return;

            this.m_tank.m_shootEffect.SetActive(false);
            this.m_tank.m_shootEffect.SetActive(true);

            //Bullet bullet = BulletFactory.Create(this.m_tank);

            //bullet.GameObject.transform.position = pos;

            //bullet.GameObject.transform.eulerAngles = eulerAngles;

            this.ShootAudio();

        }

    }
}
