using System;
using System.Runtime.CompilerServices;
using PF;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace ETModel
{
    public enum TankType
    {
        None,
        Local,
        Remote
    }


    public sealed class Tank : Entity
    {

        public static int m_coefficient = 1000000;

        // 最大血量，当前血量
        public static Action<int, int> m_hpChange;

        public string Name { get; set; }

        private TankType m_backups = TankType.None;


        private GameObject m_boomEffectGO;

        // 坦克类型：本地还是其他
        private TankType m_tankType;

        public TankType TankType
        {
            get => this.m_tankType;
            set
            {
                this.m_tankType = value;
                if(m_backups == TankType.None)
                    this.m_backups = value;
            }
        }

        // 坦克阵营：蓝方还是红方
        private TankCamp m_tankCamp = TankCamp.None;

        public TankCamp TankCamp
        {
            get => this.m_tankCamp;
            set
            {
                this.m_tankCamp = value;
            }
        }

        //private float m_maxHp = 100f;

        //private float m_hp = 100f;

        private GameObject m_gameObject;

        private GameObject m_gun;

        private GameObject m_turret;

        private bool m_died = false;


        public GameObject GameObject
        {
            set
            {
                this.m_gameObject = value;
                this.m_gun = this.m_gameObject.FindChildObjectByPath("turret/gun");

                this.m_turret = this.m_gameObject.FindChildObjectByPath("turret");
            }
            get
            {
                return this.m_gameObject;
            }
        }

        public bool Died
        {
            get=> m_died;
            set
            {
                m_died = value;

                if (value)
                {
                    this.TankType = TankType.None;

                    this.DiedAfter();
                }
                
                //this.ClearComponents();
            }
        }

        public GameObject Gun => this.m_gun;

        public GameObject Turret => this.m_turret;

        /// <summary>
        /// 开始下一局需要重置数据
        /// </summary>
        public void Reset()
        {
            this.TankType = this.m_backups;

            this.Died = false;

            this.GetComponent<TankMoveComponent>()?.Stop();

            this.GetComponent<NumericComponent>()[NumericType.HpBase] = this.GetComponent<NumericComponent>()[NumericType.MaxHpBase];

            if(this.m_boomEffectGO != null)
            {
                ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();

                resourcesComponent.RecycleObj(PrefabType.TankBoom, m_boomEffectGO);

                m_boomEffectGO = null;
            }
    }

        /// <summary>
        /// 受到攻击
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="att">伤害值，正值</param>
        public void BeAttacked(Tank attacker, int att)
        {
            //this.GetComponent<NumericComponent>()[NumericType.Hp]
            if (this.Died)
                return;

            this.GetComponent<NumericComponent>().Change(NumericType.HpBase, -att);
            
        }

        /// <summary>
        /// 左下角血量条变化
        /// </summary>
        /// <param name="maxHp"></param>
        /// <param name="hp"></param>
        public void LocalTankHpUIChange(int maxHp, int hp)
        {
            m_hpChange?.Invoke(maxHp,hp);
        }

        /// <summary>
        /// 头顶血量条变化
        /// </summary>
        /// <param name="maxHp"></param>
        /// <param name="hp"></param>
        public void RemoteTankHpUIChange(int maxHp, int hp)
        {
            this.GetComponent<OverHeadComponent>().HpChange(maxHp, hp);
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

        public Vector3 Point
        {
            get
            {
                GameObject cameraPoint = this.GameObject.FindChildObjectByPath("cameraPoint");
                return cameraPoint != null? cameraPoint.transform.position : this.Position;
            }
        }

        private void DiedAfter()
        {
            this.m_boomEffectGO = TankFactory.CreateTankBoomEffect(this);

            if(this.TankType == TankType.Local)
                this.GetComponent<TankMoveComponent>()?.Stop();
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            //TankComponent tankComponent = Game.Scene.GetComponent<TankComponent>();

            // 从坦克管理器里面删除此坦克
            //tankComponent.RemoveNoDispose(this.m_gameObject.GetInstanceID());



            //ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();

            // 回收GameObject
            //resourcesComponent.RecycleObj(PrefabType.Tank, this.m_gameObject);

            //Log.Info("删除坦克");

            base.Dispose();

            m_backups = TankType.None;
        }
    }
}