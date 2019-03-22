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
        public GameObject GameObject;

        public void Awake()
        {
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

        public Transform Transform
        {
            get
            {
                return this.GameObject.transform;
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