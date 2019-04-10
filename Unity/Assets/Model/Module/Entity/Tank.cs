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

        // 最大血量，当前血量，上一次扣掉/增加的血量
        public static Action<float, float, float> m_hpChange;

        public TankType m_tankType = TankType.None;

        private float m_maxHp = 100f;

        private float m_hp = 100f;

        private GameObject m_gameObject;

        private GameObject m_gun;

        private GameObject m_turret;


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

        public GameObject Gun => this.m_gun;

        public GameObject Turret => this.m_turret;

        public void BeAttacked(Tank attacker, float att)
        {
            if (m_hp <= 0)
                return;

            if (this.m_hp > 0)
            {
                this.m_hp -= att;

                if (this.m_tankType == TankType.Local)
                {
                    // 通知血量变化

                    m_hpChange?.Invoke(this.m_maxHp, this.m_hp, -att);
                }



                //Game.EventSystem.Run(Game.Hotfix.);

            }

            if (this.m_hp <= 0)
            {

                TankFactory.CreateTankBoomEffect(this);

            }
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
        }
    }
}