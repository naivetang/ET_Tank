using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class OverHeadAwakeSystem: AwakeSystem<OverHeadComponent>
    {
        public override void Awake(OverHeadComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class OverHeadLateUpdateSystem : LateUpdateSystem<OverHeadComponent>
    {
        public override void LateUpdate(OverHeadComponent self)
        {
            self.LateUpdate();
        }
    }

    public class OverHeadComponent : Component
    {
        private Tank m_tank;

        private GameObject m_overHeadGO;

        public void Awake()
        {
            this.m_tank = this.GetParent<Tank>();

            this.m_overHeadGO = this.m_tank.GameObject.FindChildObjectByPath("OverHead");

            this.m_overHeadGO.SetActive(true);


        }

        public void LateUpdate()
        {
            this.m_overHeadGO.transform.LookAt(Camera.main.transform.position);
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
