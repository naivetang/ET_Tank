using System.Collections.Generic;
using System.Linq;
using ETModel;
using Google.Protobuf.Collections;

namespace ETModel
{
    [ObjectSystem]
    public class BattleEntityAwakeSystem : AwakeSystem<Battle>
    {
        public override void Awake(Battle self)
        {
            self.Awake();
        }
    }
    public sealed class Battle : Entity
    {

        private readonly Dictionary<long, Tank> idTanks = new Dictionary<long, Tank>();

        private long m_latestPlayerId;

        public void Awake()
        {
            this.HeartBeat30ms1().NoAwait();
        }


        private async ETVoid HeartBeat30ms1()
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();

            while (true)
            {
                await timerComponent.WaitAsync(30);

                Send_B2C_TankFrameInfos();

            }
        }

        private void Send_B2C_TankFrameInfos()
        {
            B2C_TankFrameInfos tankInfos = new B2C_TankFrameInfos();

            tankInfos.TankFrameInfos = new RepeatedField<TankFrameInfo>();

            foreach (Tank tank in idTanks.Values)
            {
                TankFrameInfo tankFrameInfo = new TankFrameInfo();
                tankFrameInfo.TankId = tank.Id;

                tankFrameInfo.PX = tank.PX;
                tankFrameInfo.PY = tank.PY;
                tankFrameInfo.PZ = tank.PZ;

                tankFrameInfo.RX = tank.RX;
                tankFrameInfo.RY = tank.RY;
                tankFrameInfo.RZ = tank.RZ;

                tankFrameInfo.TurretRY = tank.TurretRY;
                tankFrameInfo.GunRX = tank.GunRX;

                tankInfos.TankFrameInfos.Add(tankFrameInfo);
            }

            MessageHelper.BroadcastBattle(this, tankInfos);
        }

        


        public void Add(Tank tank)
        {
            this.idTanks.Add(tank.Id, tank);
        }

        public Tank Get(long id)
        {
            if (this.idTanks.TryGetValue(id, out Tank tank))
                return tank;
            Log.Error($"未找到tank id = {id}");
            return null;
        }

        public void Remove(long id)
        {
            this.idTanks.Remove(id);

            this.m_latestPlayerId = id;

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

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            foreach (Tank tank in this.idTanks.Values)
            {
                tank.Dispose();
            }

            this.idTanks.Clear();
        }
    }
}
