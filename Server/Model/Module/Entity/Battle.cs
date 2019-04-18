using System.Collections.Generic;
using System.Linq;
using ETModel;
using Google.Protobuf.Collections;
using MongoDB.Driver.Core.Operations;
using PF;

namespace ETModel
{
    [ObjectSystem]
    public class BattleEntityAwakeSystem : AwakeSystem<Battle, G2B_CreateBattle>
    {
        public override void Awake(Battle self, G2B_CreateBattle a)
        {
            self.Awake(a);
        }
    }
    public sealed class Battle : Entity
    {
        class BattleOnePeople
        {
            public RoomOnePeople RoomInfo;
            public Tank Tank;
            // 玩家id
            public long Id;
        }

        private readonly Dictionary<long, Tank> idTanks = new Dictionary<long, Tank>();

        private int m_peopleNum;

        private int m_hasLoadSceneFinishNum;

        public int HasLoadSceneFinishNum
        {
            get => this.m_hasLoadSceneFinishNum;
            set
            {
                this.m_hasLoadSceneFinishNum = value;

                // 所有客户端加载完成
                if (this.m_hasLoadSceneFinishNum == this.m_peopleNum * 2 && this.m_peopleNum != 0)
                {

                    //TODO:初始化所有tank，开始心跳
                    this.InitTanksPos();

                    this.BroadcastCreateTank();

                    this.HeartBeat30ms1().NoAwait();
                }
            }
        }

        private readonly Dictionary<long,BattleOnePeople> m_LeftCamp = new Dictionary<long, BattleOnePeople>();
        private readonly Dictionary<long,BattleOnePeople> m_RightCamp = new Dictionary<long, BattleOnePeople>();

        public void Awake(G2B_CreateBattle msg)
        {
            this.m_peopleNum = msg.RoomSimpleInfo.PeopleNum;

            foreach (RoomOnePeople value in msg.LeftCamp)
            {
                BattleOnePeople one = new BattleOnePeople();

                one.Id = value.Id;

                one.RoomInfo = new RoomOnePeople(value);

                if (value.Camp == 1)
                    this.m_LeftCamp.Add(one.Id, one);
                else
                    this.m_RightCamp.Add(one.Id, one);

                HasLoadSceneFinishNum = 0;
            }
        }

        private void InitTanksPos()
        {
            Map mapInfo = (Map)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Map), 1001);

            int index = 1;

            foreach (BattleOnePeople battleOnePeople in this.m_LeftCamp.Values)
            {
                Tank tank = battleOnePeople.Tank;

                Vector3 vec = GetPos(mapInfo, true, index++);
                tank.PX = (int)(vec.x * Tank.m_coefficient);
                tank.PY = (int)(vec.y * Tank.m_coefficient);
                tank.PZ = (int)(vec.z * Tank.m_coefficient);
            }

            index = 1;

            foreach (BattleOnePeople battleOnePeople in this.m_RightCamp.Values)
            {
                Tank tank = battleOnePeople.Tank;

                Vector3 vec = GetPos(mapInfo, false, index++);
                tank.PX = (int)(vec.x * Tank.m_coefficient);
                tank.PY = (int)(vec.y * Tank.m_coefficient);
                tank.PZ = (int)(vec.z * Tank.m_coefficient);
            }
        }

        public Vector3 GetPos(Map mapInfo, bool left, int index)
        {
            if (left)
            {
                switch (index)
                {
                    case 1:
                        return ConfigHelper.String2Vector3(mapInfo.LeftPos1);
                    case 2:
                        return ConfigHelper.String2Vector3(mapInfo.LeftPos2);
                    case 3:
                        return ConfigHelper.String2Vector3(mapInfo.LeftPos3);
                    case 4:
                        return ConfigHelper.String2Vector3(mapInfo.LeftPos4);
                    case 5:
                        return ConfigHelper.String2Vector3(mapInfo.LeftPos5);
                    default:
                        Log.Error($"{index}不能大于5");
                        return Vector3.zero;
                }
            }
            else
            {
                switch (index)
                {
                    case 1:
                        return ConfigHelper.String2Vector3(mapInfo.RightPos1);
                    case 2:
                        return ConfigHelper.String2Vector3(mapInfo.RightPos2);
                    case 3:
                        return ConfigHelper.String2Vector3(mapInfo.RightPos3);
                    case 4:
                        return ConfigHelper.String2Vector3(mapInfo.RightPos4);
                    case 5:
                        return ConfigHelper.String2Vector3(mapInfo.RightPos5);
                    default:
                        Log.Error($"{index}不能大于5");
                        return Vector3.zero;
                }

            }
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

            this.Broadcast(tankInfos);
        }

        


        public void Add(Tank tank)
        {
            this.idTanks.Add(tank.Id, tank);

            long playerId = tank.PlayerId;

            if (this.m_LeftCamp.TryGetValue(playerId,out BattleOnePeople one))
            {
                one.Tank = tank;
            }
            else if (this.m_RightCamp.TryGetValue(playerId,out one))
            {
                one.Tank = tank;
            }
            else
            {
                Log.Error("创建的坦克不应该不在此战场");
            }
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

        private void BroadcastCreateTank()
        {
            // 广播创建的Tank

             B2C_CreateTanks createTanks = new B2C_CreateTanks();
            
             Tank[] tanks = this.idTanks.Values.ToArray();
            
             foreach (Tank t in tanks)
             {
            
                 TankFrameInfo tankFrameInfo = new TankFrameInfo();
            
                 tankFrameInfo.TankId = t.Id;
                 tankFrameInfo.PX = t.PX;
                 tankFrameInfo.PY = t.PY;
                 tankFrameInfo.PZ = t.PZ;
                 tankFrameInfo.RX = t.RX;
                 tankFrameInfo.RY = t.RY;
                 tankFrameInfo.RZ = t.RZ;
                 tankFrameInfo.TurretRY = t.TurretRY;
                 tankFrameInfo.GunRX = t.GunRX;
            
                 TankInfoFirstEnter tankInfoFirstEnter = new TankInfoFirstEnter();
                 tankInfoFirstEnter.TankFrameInfo = tankFrameInfo;
            
                 tankInfoFirstEnter.TankCamp = t.TankCamp;
            
                 tankInfoFirstEnter.MaxHpBase = t.GetComponent<NumericComponent>()[NumericType.MaxHpBase];
            
                 tankInfoFirstEnter.HpBase = t.GetComponent<NumericComponent>()[NumericType.HpBase];
            
                 tankInfoFirstEnter.AtkBase = t.GetComponent<NumericComponent>()[NumericType.AtkBase];
            
                 tankInfoFirstEnter.Name = t.Name;
            
            
                 createTanks.Tanks.Add(tankInfoFirstEnter);
             }

            this.Broadcast(createTanks);
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
