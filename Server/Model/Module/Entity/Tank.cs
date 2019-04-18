using PF;

namespace ETModel
{
    public enum TankType
    {
        Player,
        Computer,
    }

    // 坦克阵营

    [ObjectSystem]
    public class TankAwakeSystem : AwakeSystem<Tank>
    {
        public override void Awake(Tank self)
        {
            self.Awake();
        }
    }
    public class Tank : Entity
    {
        public static int m_coefficient = 1000000;

        public long PlayerId { get; set; }

        public Battle Battle { get; set; }

        public bool Died { get; set; } = false;

        public string Name { get; set; }

        public TankType TankType { get; private set; }

        public TankCamp TankCamp { get; set; } = TankCamp.None;

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

        public void Awake( )
        {
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
