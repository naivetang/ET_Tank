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
                GameObject cameraPoint = this.GameObject.FindChildObject("cameraPoint");
                return cameraPoint != null? cameraPoint.transform.position : this.Position;
            }
        }
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
        }
    }
}