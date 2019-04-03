using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
//using Vector3 = PF.Vector3;

namespace ETModel
{
    [ObjectSystem]
    public class RemoteTankAwakeSystem : AwakeSystem<RemoteTankComponent>
    {
        public override void Awake(RemoteTankComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class RemoteTankUpdateSystem : LateUpdateSystem<RemoteTankComponent>
    {
        public override void LateUpdate(RemoteTankComponent self)
        {
            self.LateUpdate();
        }
    }

    public class RemoteTankComponent : Component
    {
        // 上一次的位置和旋转角度
        private Vector3 m_lPos;
        private Vector3 m_lRot;
        // 本次接受到的位置和旋转角度
        private Vector3 m_nPos;
        private Vector3 m_nRot;
        // 预测下一次的位置和旋转角度
        private Vector3 m_fPos;
        private Vector3 m_fRot;

        // 这次接到信息和上一次时间间隔
        private float m_delta = 1f;

        // 上一次接受位置信息时间
        private long m_lastRecvInfoTime = long.MinValue;


        private GameObject m_tank;

        public void Awake()
        {
            this.m_tank = this.GetParent<Tank>().GameObject;

            this.m_lPos = this.m_fPos = this.m_tank.transform.position;
            this.m_lRot = this.m_fRot = this.m_tank.transform.eulerAngles;

            //Rigidbody rg = this.m_tank.GetComponent<Rigidbody>();

            //rg.constraints = RigidbodyConstraints.FreezeAll;
        }

        public void LateUpdate()
        {
            
            
        }

        public void NetForecastInfo(Vector3 nPos, Vector3 nRot)
        {
            this.m_nPos = nPos;
            this.m_nRot = nRot;

            try
            {
                // 预测的位置
                this.m_fPos = this.m_lPos + (this.m_nPos - this.m_lPos) * 2;
                this.m_fRot = this.m_lRot + (this.m_nRot - this.m_lRot) * 2;

                if (TimeHelper.Now() - this.m_lastRecvInfoTime > 300f)
                {
                    this.m_fPos = this.m_nPos;
                    this.m_fRot = this.m_nRot;
                }

                // 时间间隔
                this.m_delta = (TimeHelper.Now() - this.m_lastRecvInfoTime) / 1000f;

                //Log.Info($"{this.m_delta}");

                // 更新
                this.m_lPos = this.m_nPos;
                this.m_lRot = this.m_nRot;
                this.m_lastRecvInfoTime = TimeHelper.Now();
            }
            catch (Exception e)
            {
                Log.Error("NetForecastInfo" + e);
            }

            UpdatePos();
        }

        private void UpdatePos()
        {
            try
            {
                Vector3 pos = this.m_tank.transform.position;
                Vector3 rot = this.m_tank.transform.eulerAngles;
                float distance = (pos - this.m_nPos).magnitude;

                if (Mathf.Abs(distance) < 0.1f)
                    return;

                if (this.m_delta > 0)
                {
                    this.m_tank.transform.position = Vector3.Lerp(pos, this.m_fPos, this.m_delta);
                    this.m_tank.transform.rotation = Quaternion.Lerp(Quaternion.Euler(rot), Quaternion.Euler(this.m_fRot), this.m_delta);
                }
            }
            catch (Exception e)
            {
                Log.Error("Update" + e);
            }
        }

        private void UpdateEu()
        {

        }
    }
}
