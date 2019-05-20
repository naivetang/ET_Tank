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

        private readonly Dictionary<long,long> m_instaceid2ID = new Dictionary<long, long>();

        private readonly Dictionary<long, Tank> m_idTanks = new Dictionary<long, Tank>();


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

            foreach (Tank Tank in this.m_idTanks.Values)
            {
                Tank.Dispose();
            }

       

            this.m_idTanks.Clear();


            Instance = null;
        }

        public void RemoveAll()
        {
            foreach (Tank tank in this.m_idTanks.Values)
            {
                tank.Dispose();

                ETModel.Game.Scene.GetComponent<ResourcesComponent>().RecycleObj(PrefabType.Tank, tank.GameObject);

                tank.GameObject = null;
            }

            m_instaceid2ID.Clear();

            m_idTanks.Clear();

            this.MyTank = null;
        }

        public void Add(Tank Tank)
        {
            
            this.m_instaceid2ID.Add(Tank.GameObject.GetInstanceID(),Tank.Id);
            this.m_idTanks.Add(Tank.Id, Tank);
        }


        public Tank Get(long id)
        {

            if (this.m_instaceid2ID.TryGetValue(id, out long tankId))
            {
                if (this.m_idTanks.TryGetValue(tankId, out Tank tank))
                {
                    return tank;
                }
            }

            this.m_idTanks.TryGetValue(id, out Tank Tank);
            return Tank;
        }

        public void Remove(long id)
        {
            if (this.m_instaceid2ID.TryGetValue(id, out long tankId))
            {
                this.m_instaceid2ID.Remove(id);
                if (this.m_idTanks.TryGetValue(tankId, out Tank tank))
                {
                    this.m_idTanks.Remove(tankId);
                    tank?.Dispose();
                    return;
                }
            }

            // Tank Tank;
            // this.m_idTanks.TryGetValue(id, out Tank);
            // this.m_idTanks.Remove(id);
            // Tank?.Dispose();
        }

        public void RemoveNoDispose(long id)
        {
            if (this.m_instaceid2ID.TryGetValue(id, out long tankId))
            {
                this.m_instaceid2ID.Remove(id);
                if (this.m_idTanks.TryGetValue(tankId, out Tank tank))
                {
                    this.m_idTanks.Remove(tankId);
                }
            }

        }

        public int Count
        {
            get
            {
                return this.m_idTanks.Count;
            }
        }

        public Tank[] GetAll()
        {
            return this.m_idTanks.Values.ToArray();
        }
    }
}