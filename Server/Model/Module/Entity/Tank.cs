using PF;

namespace ETModel
{
    public enum TankType
    {
        Hero,
        Npc
    }

    [ObjectSystem]
    public class TankAwakeSystem : AwakeSystem<Tank, TankType>
    {
        public override void Awake(Tank self, TankType a)
        {
            self.Awake(a);
        }
    }
    public class Tank : Entity
    {
        public TankType TankType { get; private set; }

        public Vector3 Position { get; set; } = Vector3.zero;

        public Vector3 Rotation { get; set; } = Vector3.zero;

        public float TurretRY { get; set; } = 0f;
        public float GunRX { get; set; } = 0f;

        public void Awake(TankType tankType)
        {
            this.TankType = tankType;
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
