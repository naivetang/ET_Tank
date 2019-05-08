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
        //轮轴
        private List<AxleInfo> axleInfos;

        //轮子
        private Transform wheels;

        //履带
        private Transform tracks;

        //音效
        private AudioSource motorAudioSource;

        private AudioClip motorClip;



        // 上一次的位置和旋转角度
        private Vector3 m_lPos;
        private Vector3 m_lRot;
        // 本次接受到的位置和旋转角度
        private Vector3 m_nPos;
        private Vector3 m_nRot;
        // 预测下一次的位置和旋转角度
        private Vector3 m_fPos;
        private Vector3 m_fRot;

        /// <summary>
        /// 这次接到信息和上一次时间间隔
        /// 单位 秒
        /// </summary>
        private float m_delta = 1f;

        // 上一次接受位置信息时间
        private long m_lastRecvInfoTime = long.MinValue;


        private GameObject m_tank;

        public void Awake()
        {
            // 初始化
            this.m_tank = this.GetParent<Tank>().GameObject;
            InitPhysical();

            this.m_lPos = this.m_fPos = this.m_tank.transform.position;
            this.m_lRot = this.m_fRot = this.m_tank.transform.eulerAngles;


            //Rigidbody rg = this.m_tank.GetComponent<Rigidbody>();

            //rg.constraints = RigidbodyConstraints.FreezeAll;
        }

        private void InitPhysical()
        {
            wheels = m_tank.FindComponentInChildren<Transform>("wheels");

            tracks = m_tank.FindComponentInChildren<Transform>("tracks");


            motorAudioSource = m_tank.GetComponent<AudioSource>();
            if (motorAudioSource == null)
            {
                motorAudioSource = m_tank.AddComponent<AudioSource>();
            }

            
            motorAudioSource.spatialBlend = 1;

            motorAudioSource.volume = GameSettingInfo.AudioVolume();

            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();

            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle($"Unit.unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset("Unit.unity3d", "Unit");
            Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"Unit.unity3d");
            motorClip = bundleGameObject.Get<AudioClip>("motor");

            motorAudioSource.loop = true;

            motorAudioSource.clip = this.motorClip;

            axleInfos = new List<AxleInfo>(2);

            // 前轮 转向轮
            AxleInfo axleInfo = new AxleInfo();
            axleInfo.leftWheel = m_tank.FindComponentInChildren<WheelCollider>("PhysicalBody/wheelL1");
            axleInfo.rightWheel = m_tank.FindComponentInChildren<WheelCollider>("PhysicalBody/wheelR1");
            axleInfo.montor = false;
            axleInfo.steering = true;
            this.axleInfos.Add(axleInfo);
            // 后轮 动力轮
            axleInfo = new AxleInfo();
            axleInfo.leftWheel = m_tank.FindComponentInChildren<WheelCollider>("PhysicalBody/wheelL2");
            axleInfo.rightWheel = m_tank.FindComponentInChildren<WheelCollider>("PhysicalBody/wheelR2");
            axleInfo.montor = true;
            axleInfo.steering = false;
            this.axleInfos.Add(axleInfo);
        }

        public void LateUpdate()
        {

            // 时间间隔
            this.m_delta = (TimeHelper.NowMilliSecond() - this.m_lastRecvInfoTime) / 1000f;
            UpdatePos();
            WheelsRotation();
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

                if (TimeHelper.NowMilliSecond() - this.m_lastRecvInfoTime > 300f)
                {
                    this.m_fPos = this.m_nPos;
                    this.m_fRot = this.m_nRot;
                }

                // 时间间隔
                //this.m_delta = (TimeHelper.NowMilliSecond() - this.m_lastRecvInfoTime) / 1000f;

                //Log.Info($"{this.m_delta}");

                // 更新
                this.m_lPos = this.m_nPos;
                this.m_lRot = this.m_nRot;
                this.m_lastRecvInfoTime = TimeHelper.NowMilliSecond();
            }
            catch (Exception e)
            {
                Log.Error("NetForecastInfo" + e);
            }

            //UpdatePos();
        }

        private void UpdatePos()
        {
            try
            {
                Vector3 pos = this.m_tank.transform.position;

                Vector3 rot = this.m_tank.transform.eulerAngles;

                float distance = (pos - this.m_nPos).magnitude;

                // 如果服务器位置与本地位置相差不到0.1f，就不进行移动
                if (Mathf.Abs(distance) < 0.1f)
                    return;

                // 从当前位置向预测位置移动
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

        private void WheelsRotation()
        {
            float z = this.m_tank.transform.InverseTransformPoint(this.m_fPos).z;

            // 判断是否在移动
            if (Mathf.Abs(z) < 0.1f)
            {
                this.motorAudioSource.Pause();
                return;
            }

            // 轮子转动
            foreach (Transform wheel in this.wheels)
            {
                //try
                {
                    if(this.m_delta < 0.001f)
                        continue;
                    Vector3 eulerAngle = wheel.localEulerAngles;
                    eulerAngle += new Vector3(360 * z / this.m_delta, 0, 0);
                    eulerAngle /= 360;
                    wheel.localEulerAngles = eulerAngle;
                }
                // catch (Exception e)
                // {
                //     Log.Error(e);
                // }
                
            }

            // 履带滚动
            float offset = -this.wheels.GetChild(0).localEulerAngles.x / 90f;

            foreach (Transform track in this.tracks)
            {
                MeshRenderer mr = track.gameObject.GetComponent<MeshRenderer>();

                if (mr == null)
                    continue;

                Material mtl = mr.material;

                mtl.mainTextureOffset = new UnityEngine.Vector2(0,offset);
            }

            // 音效
            if (!this.motorAudioSource.isPlaying)
            {
                this.motorAudioSource.Play();
            }
        }
    }
}
