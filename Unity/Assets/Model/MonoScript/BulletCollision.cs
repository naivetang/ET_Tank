using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// 碰撞事件分发
    /// </summary>
    public class BulletCollision : MonoBehaviour
    {
        private BulletFlyComponent m_bulletFly;

        public BulletFlyComponent BulletFly
        {
            set
            {
                this.m_bulletFly = value;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            this.m_bulletFly.OnCollisionEnter(collision);
        }
    }
}
