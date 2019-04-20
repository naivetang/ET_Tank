using System;
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

        private bool m_died = false;

        public int Level { get; set; }

        public bool Died
        {
            get => m_died;
            set
            {
                this.m_died = value;

                if (this.m_died)
                {
                    if (this.Battle.BigMode == BigModel.Time)
                    {
                        if (this.TankCamp == TankCamp.Left)
                            ++this.Battle.TimeLeftDiedNum;
                        else
                        {
                            ++this.Battle.TimeRightDiedNum;
                        }
                    }
                    else
                    {
                        if (this.TankCamp == TankCamp.Left)
                            ++this.Battle.RoundLeftDiedNum;
                        else
                        {
                            ++this.Battle.RoundRightDiedNum;
                        }
                    }

                }
                
            }
        }

        

        public string Name { get; set; }


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

        public void Reset()
        {
            this.GetComponent<NumericComponent>()[NumericType.HpBase] = this.GetComponent<NumericComponent>()[NumericType.MaxHpBase];

            this.Died = false;
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
