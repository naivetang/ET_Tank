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
        private Transform turretTransform;

        // 炮塔旋转速度
        private float rotSpeed = 0.5f;

        // 炮塔目标角度
        private float rotTarget = 0f;
        private float rollTarget = 0f;

        //炮管
        private Transform gunTransform;

        //炮管旋转角度范围
        private float maxRoll = 60f - 40f;
        private float minRoll = 10f - 40f;

        public void Awake()
        {
            this.turretTransform = this.GetParent<Tank>().GameObject.FindChildObjectByPath("turret").transform;
            this.gunTransform = this.GetParent<Tank>().GameObject.FindChildObjectByPath("turret/gun").transform;
        }

        public void LateUpdate()
        {
            this.TurretRotate();
            this.GunRotate();
        }

        //炮塔旋转
        private void TurretRotate()
        {
            // 炮塔角度
            rotTarget = Camera.main.transform.eulerAngles.y;

            float angle = this.turretTransform.eulerAngles.y - this.rotTarget;
            if (angle < 0)
                angle += 360;
            if (angle > this.rotSpeed && angle < 180)
                this.turretTransform.Rotate(0f, -this.rotSpeed, 0f);
            else if (angle > 180 && angle < 360 - this.rotSpeed)
                this.turretTransform.Rotate(0f, this.rotSpeed, 0f);

        }


        /// <summary>
        /// 炮管旋转
        /// </summary>
        private void GunRotate()
        {
            this.rollTarget = Camera.main.transform.eulerAngles.x;
            this.rollTarget= this.rollTarget * 2f / 3f - 10f;

            //Log.Info($"rollTarget = {this.rollTarget}");

            //获取角度
            Vector3 worldEuler = this.gunTransform.eulerAngles;
            Vector3 localEuler = this.gunTransform.localEulerAngles;

            //世界坐标系角度计算
            worldEuler.x = this.rollTarget;
            this.gunTransform.eulerAngles = worldEuler;
            //本地坐标系角度限制
            Vector3 euler = this.gunTransform.localEulerAngles;
            if (euler.x > 180)
                euler.x -= 360;

            //euler.x = PF.Mathf.Clamp(euler.x, this.minRoll, this.maxRoll);

            this.gunTransform.localEulerAngles = new Vector3(euler.x,localEuler.y,localEuler.z);

            //Log.Info($"localEulerAngles = {this.gunTransform.localEulerAngles}");
        }

    }
}
