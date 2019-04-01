using System;
using PF;
using UnityEngine;
using Mathf = UnityEngine.Mathf;
using Vector3 = UnityEngine.Vector3;

namespace ETModel
{
	[ObjectSystem]
	public class CameraComponentAwakeSystem : AwakeSystem<CameraComponent>
	{
		public override void Awake(CameraComponent self)
		{
			self.Awake();
		}
	}

	[ObjectSystem]
	public class CameraComponentLateUpdateSystem : LateUpdateSystem<CameraComponent>
	{
		public override void LateUpdate(CameraComponent self)
		{
			self.LateUpdate();
		}
	}

	public class CameraComponent : Component
	{
		private Camera mainCamera;

		private Tank tank;

        //距离
        private float distance = 10f;

        //横向角度
        private float rot = 0;

        // 纵向角度范围
        private float maxRoll = PF.Mathf.Deg2Rad(30);  //60f * Mathf.PI * 2 / 360;
        private float minRoll = PF.Mathf.Deg2Rad(-10);//10f * Mathf.PI * 2 / 360;

        //纵向角度
        private float roll = 30f * Mathf.PI * 2 / 360;

        //横向旋转角度
        private float rotSpeed = 0.2f;

        // 距离范围
        private float maxDistance = 20f;
        private float minDistance = 5f;

        //距离变化的速度
        private float zoomSpeed = 1f;

		public Camera MainCamera
		{
			get
			{
				return this.mainCamera;
			}
		}

		public void Awake()
		{
			this.mainCamera = Camera.main;
            this.tank = this.GetParent<Tank>();
        }

		public void LateUpdate()
		{
			// 摄像机每帧更新位置
			UpdatePosition();

            Rotate();

            Zoom();
        }

        /// <summary>
        /// 更新相机位置
        /// </summary>
		private void UpdatePosition()
        {
            Vector3 targetPos = this.tank.Point;

            Vector3 cameraPos;

            float d = this.distance * Mathf.Cos(roll);

            float height = this.distance * Mathf.Sin(roll);

            cameraPos.x = targetPos.x + d * Mathf.Cos(rot);

            cameraPos.y = targetPos.y + height;

            cameraPos.z = targetPos.z + d * Mathf.Sin(rot);

            this.MainCamera.transform.position = cameraPos;

            this.MainCamera.transform.LookAt(this.tank.Point);


            return;

			//Vector3 cameraPos = this.mainCamera.transform.position;
			this.mainCamera.transform.position = new Vector3(this.tank.Position.x , this.tank.Position.y + 5, this.tank.Position.z - 6);
            this.mainCamera.transform.LookAt(this.tank.Position);
			
			//this.mainCamera.transform.eulerAngles = new Vector3(30,0,0);
            
        }

        /// <summary>
        /// 鼠标控制相机旋转
        /// </summary>
        private void Rotate()
        {
            float x = Input.GetAxis("Mouse X") * this.rotSpeed;

            this.rot -= x;



            float y = Input.GetAxis("Mouse Y") * this.rotSpeed;

            this.roll -= y;

            this.roll = PF.Mathf.Clamp(this.roll, this.minRoll, this.maxRoll);

            //Log.Info($"相机纵向角度 = {PF.Mathf.Rad2Deg(roll)}" );
        }


        /// <summary>
        /// 缩放
        /// </summary>
        private void Zoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Math.Abs(scroll) > 0.01f)
            {
                this.distance -= this.zoomSpeed * PF.Mathf.Sign(scroll);
                this.distance = PF.Mathf.Clamp(this.distance, this.minDistance, this.maxDistance);
            }
        }
    }
}
