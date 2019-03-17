using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    [ObjectSystem]
    public class TankComponentSystem : AwakeSystem<TankComponent>
    {
        public override void Awake(TankComponent self)
        {
            self.Awake();
        }
    }

    public class TankComponent : Component
    {
        public static TankComponent Instance { get; private set; }

        public Tank MyTank;

        private readonly Dictionary<long, Tank> idTanks = new Dictionary<long, Tank>();

        public void Awake()
        {
            Instance = this;
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            foreach (Tank Tank in this.idTanks.Values)
            {
                Tank.Dispose();
            }

            this.idTanks.Clear();

            Instance = null;
        }

        public void Add(Tank Tank)
        {
            this.idTanks.Add(Tank.Id, Tank);
        }

        public Tank Get(long id)
        {
            Tank Tank;
            this.idTanks.TryGetValue(id, out Tank);
            return Tank;
        }

        public void Remove(long id)
        {
            Tank Tank;
            this.idTanks.TryGetValue(id, out Tank);
            this.idTanks.Remove(id);
            Tank?.Dispose();
        }

        public void RemoveNoDispose(long id)
        {
            this.idTanks.Remove(id);
        }

        public int Count
        {
            get
            {
                return this.idTanks.Count;
            }
        }

        public Tank[] GetAll()
        {
            return this.idTanks.Values.ToArray();
        }
    }
}