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

        public int PX { get; set; } = 0;
        public int PY { get; set; } = 0;
        public int PZ { get; set; } = 0;
        public int RX { get; set; } = 0;
        public int RY { get; set; } = 0;
        public int RZ { get; set; } = 0;

        public Vector3 Position { get; set; } = Vector3.zero;

        public Vector3 Rotation { get; set; } = Vector3.zero;

        public int TurretRY { get; set; } = 0;
        public int GunRX { get; set; } = 0;

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
