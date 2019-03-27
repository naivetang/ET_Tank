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
        Owener,
        Enemy
    }

    public sealed class Tank : Entity
    {
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

        public void BeAttacked(float att)
        {
            if (m_hp <= 0)
                return;

            if (this.m_hp > 0)
                this.m_hp -= att;

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