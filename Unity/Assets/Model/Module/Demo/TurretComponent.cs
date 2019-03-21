using System;
using NPOI.OpenXmlFormats.Wordprocessing;
using PF;
using UnityEngine;
using Mathf = UnityEngine.Mathf;
using Vector3 = UnityEngine.Vector3;

namespace ETModel
{
    [ObjectSystem]
    public class TurretComponentAwakeSystem : AwakeSystem<TurretComponent>
    {
        public override void Awake(TurretComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class TurretComponentLateUpdateSystem : LateUpdateSystem<TurretComponent>
    {
        public override void LateUpdate(TurretComponent self)
        {
            self.LateUpdate();
        }
    }


    /// <summary>
    /// 炮塔旋转组件
    /// </summary>
    public class TurretComponent : Component
    {
        private Transform transform;

        // 炮塔旋转速度
        private float rotSpeed = 0.5f;

        // 炮塔目标角度
        private float rotTarget = 0f;


        public void Awake()
        {
            this.transform = this.GetParent<Tank>().Transform.Find("turret");
        }

        public void LateUpdate()
        {
            // 炮塔角度
            rotTarget = Camera.main.transform.eulerAngles.y;
        }


    }
}
