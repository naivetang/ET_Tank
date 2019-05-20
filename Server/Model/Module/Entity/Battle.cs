using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
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
            public int Index;
            // 玩家id
            public long Id;

            public Vector3 InitPos;
            public Vector3 InitRot;
        }


        private CancellationTokenSource CancellationTokenSource;

        private readonly Dictionary<long, Tank> idTanks = new Dictionary<long, Tank>();

        private int m_peopleNum;

        private int m_hasLoadSceneFinishNum;

        public bool StopGetPosInfo { get; set; } = false;

        public BigModel BigMode
        {
            get => this.m_bigModel;
            set => this.m_bigModel = value;
        }

        /// <summary>
        /// 1 回合制，2时间制
        /// </summary>
        private BigModel m_bigModel;

        /// <summary>
        /// 时间制的话表示是几分钟，回合制的话表示是多少回合
        /// </summary>
        private int m_smallModel;

        public int TimeLeftDiedNum { get; set; }

        public int TimeRightDiedNum { get; set; }



        private int m_roundLeftDiedNum;
        public int RoundLeftDiedNum
        {
            get => m_roundLeftDiedNum;
            set
            {
                m_roundLeftDiedNum = value;
                if (this.m_roundLeftDiedNum == this.m_peopleNum)
                {
                    //TODO::本回合结束，右方阵营获胜
                    ++this.RoundRightWinNum;

                    if (this.RoundRightWinNum != this.m_smallModel)
                    {
                        ResetOneRound(TankCamp.Right).NoAwait();
                    }
                }
            }
        }

        private int m_roundRightDiedNum;

        public int RoundRightDiedNum
        {
            get => m_roundRightDiedNum;
            set
            {
                m_roundRightDiedNum = value;
                if (this.m_roundRightDiedNum == this.m_peopleNum)
                {
                    //TODO::本回合结束，左方阵营获胜
                    ++this.RoundLeftWinNum;

                    if (this.RoundLeftWinNum != this.m_smallModel)
                    {
                        ResetOneRound(TankCamp.Left).NoAwait();
                    }
                }
            }
        }

        private int m_roundLeftWinNum;

        private int RoundLeftWinNum
        {
            get => m_roundLeftWinNum;
            set
            {
                m_roundLeftWinNum = value;
                if (m_roundLeftWinNum == this.m_smallModel)
                {
                    // TODO:: 游戏结束，左方阵营获胜,到结束页面 

                    BattleEnd(TankCamp.Left).NoAwait();
                }
            }
        }

        private int m_roundRightWinNum;

        private int RoundRightWinNum
        {
            get => m_roundRightWinNum;
            set
            {
                m_roundRightWinNum = value;
                if (m_roundRightWinNum == this.m_smallModel)
                {
                    // TODO:: 游戏结束，右方阵营，到结束页面

                    BattleEnd(TankCamp.Right).NoAwait();
                }
            }
        }



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
                    this.InitTanks();

                    this.BroadcastCreateTank();

                    this.HeartBeat30ms().NoAwait();

                    if(this.BigMode == BigModel.Time)
                        this.AwaitTimeEnd().NoAwait();
                }
            }
        }

        private void Clear()
        {
            m_peopleNum = 0;
            this.m_hasLoadSceneFinishNum = 0;
            this.m_bigModel = BigModel.None;
            m_smallModel = 0;
            TimeLeftDiedNum = 0;
            TimeRightDiedNum = 0;
            m_roundLeftDiedNum = 0;
            m_roundRightDiedNum = 0;
            m_roundLeftWinNum = 0;
            m_roundRightWinNum = 0;
            StopGetPosInfo = false;
        }

        private readonly Dictionary<long,BattleOnePeople> m_LeftCamp = new Dictionary<long, BattleOnePeople>();
        private readonly Dictionary<long,BattleOnePeople> m_RightCamp = new Dictionary<long, BattleOnePeople>();

        public void Awake(G2B_CreateBattle msg)
        {
            CancellationTokenSource = new CancellationTokenSource();

            this.m_peopleNum = msg.RoomSimpleInfo.PeopleNum;

            this.m_bigModel = (BigModel)msg.RoomSimpleInfo.BigModel;

            this.m_smallModel = msg.RoomSimpleInfo.SmallModel;

            foreach (RoomOnePeople value in msg.LeftCamp)
            {
                BattleOnePeople one = new BattleOnePeople();

                one.Id = value.Id;

                one.RoomInfo = new RoomOnePeople(value);

                this.m_LeftCamp.Add(one.Id, one);
            }

            foreach (RoomOnePeople value in msg.RightCamp)
            {
                BattleOnePeople one = new BattleOnePeople();

                one.Id = value.Id;

                one.RoomInfo = new RoomOnePeople(value);

                this.m_RightCamp.Add(one.Id, one);
            }

            HasLoadSceneFinishNum = 0;
        }

        /// <summary>
        /// 重新开始新的一局
        /// </summary>
        private async ETVoid ResetOneRound(TankCamp winCamp)
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();

            Send_B2C_RoundEnd(winCamp);

            await timerComponent.WaitAsync(3000);

            CancellationTokenSource.Cancel();

            CancellationTokenSource.Dispose();

            await timerComponent.WaitAsync(40);

            this.StopGetPosInfo = true;

            this.InitTanks();

            this.Send_B2C_StartNextRound();

            m_roundLeftDiedNum = 0;

            m_roundRightDiedNum = 0;

            foreach (Tank tank in this.idTanks.Values)
            {
                tank.Reset();
            }

            CancellationTokenSource = new CancellationTokenSource();

            this.HeartBeat30ms().NoAwait();

            this.StopGetPosInfo = false;
        }

        private void Send_B2C_StartNextRound()
        {
            B2C_StartNextRound msg = new B2C_StartNextRound();

            msg.TankFrameInfos = new RepeatedField<TankFrameInfo>();

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

                msg.TankFrameInfos.Add(tankFrameInfo);
            }

            this.Broadcast(msg);
        }

        private void Send_B2C_RoundEnd(TankCamp winCamp)
        {
            B2C_RoundEnd msg = new B2C_RoundEnd();

            msg.WinCamp =(int)winCamp;

            this.Broadcast(msg);
        }

        private async ETVoid BattleEnd(TankCamp winCamp)
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();

            Send_B2C_RoundEnd(winCamp);

            await timerComponent.WaitAsync(3000);

            this.CancellationTokenSource?.Cancel();
            this.CancellationTokenSource?.Dispose();

            // 结算
           this.GameOver();
        }

        public async ETVoid TimeResetTank(Tank tank)
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();

            await timerComponent.WaitAsync(3000);

            Map mapInfo = (Map)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Map), 1001);
            if(this.m_LeftCamp.TryGetValue(tank.PlayerId,out BattleOnePeople onePeople))
            {
                InitOneTank(tank,onePeople.Index);
                
            }
            else if(this.m_RightCamp.TryGetValue(tank.PlayerId, out  onePeople))
            {
                InitOneTank(tank, onePeople.Index);
                
            }

            tank.Reset();

            Send_B2C_TankReset(tank);
        }

        private void Send_B2C_TankReset(Tank tank)
        {
            B2C_TankReset msg = new B2C_TankReset();

            msg.TankFrameInfo = new TankFrameInfo();

            msg.TankFrameInfo.TankId = tank.Id;

            msg.TankFrameInfo.PX = tank.PX;
            msg.TankFrameInfo.PY = tank.PY;
            msg.TankFrameInfo.PZ = tank.PZ;

            msg.TankFrameInfo.RX = tank.RX;
            msg.TankFrameInfo.RY = tank.RY;
            msg.TankFrameInfo.RZ = tank.RZ;

            msg.TankFrameInfo.TurretRY = tank.TurretRY;
            msg.TankFrameInfo.GunRX = tank.GunRX;

            this.Broadcast(msg);

        }

        private void GameOver()
        {
            this.Send_B2G_BattleEnd();

            Send_B2C_BattleEnd();

            Game.Scene.GetComponent<BattleComponent>().Remove(this.Id);

            this.Dispose();
        }

        private void Send_B2C_BattleEnd()
        {
            B2C_BattleEnd msg = new B2C_BattleEnd();

            msg.BattleId = this.Id;

            msg.WinCamp = this.RoundLeftWinNum > this.RoundRightWinNum? 1 : 2;

            msg.BigModel = (int)this.BigMode;

            msg.SmallModel = this.m_smallModel;

            if (this.BigMode == BigModel.Round)
            {
                msg.LeftCampWinNum = this.RoundLeftWinNum;

                msg.RightCampWinNum = this.RoundRightWinNum;
            }
            else
            {
                msg.LeftCampWinNum = this.TimeRightDiedNum;

                msg.RightCampWinNum = this.TimeLeftDiedNum;
            }

            msg.LeftCamp = new RepeatedField<PersonBattleData>();
            foreach (BattleOnePeople battleOnePeople in this.m_LeftCamp.Values)
            {
                PersonBattleData personBattleData = new PersonBattleData();

                personBattleData.PlayerId = battleOnePeople.Id;

                NumericComponent numeric = battleOnePeople.Tank.GetComponent<NumericComponent>();

                personBattleData.Kills = numeric[NumericType.Kills];

                personBattleData.Damage = numeric[NumericType.Damage];

                personBattleData.Deaths = numeric[NumericType.Deaths];

                personBattleData.Name = battleOnePeople.Tank.Name;

                personBattleData.Level = battleOnePeople.Tank.Level;

                personBattleData.Ping = 2;

                msg.LeftCamp.Add(personBattleData);
            }

            msg.RightCamp = new RepeatedField<PersonBattleData>();
            foreach (BattleOnePeople battleOnePeople in this.m_RightCamp.Values)
            {
                PersonBattleData personBattleData = new PersonBattleData();

                personBattleData.PlayerId = battleOnePeople.Id;

                NumericComponent numeric = battleOnePeople.Tank.GetComponent<NumericComponent>();

                personBattleData.Kills = numeric[NumericType.Kills];

                personBattleData.Damage = numeric[NumericType.Damage];

                personBattleData.Deaths = numeric[NumericType.Deaths];

                personBattleData.Name = battleOnePeople.Tank.Name;

                personBattleData.Level = battleOnePeople.Tank.Level;

                personBattleData.Ping = 2;

                msg.RightCamp.Add(personBattleData);
            }



            this.Broadcast(msg);
        }


        private void Send_B2G_BattleEnd()
        {
            IPEndPoint gateAddress = StartConfigComponent.Instance.GateConfigs[0].GetComponent<InnerConfig>().IPEndPoint;

            Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gateAddress);

            B2G_BattleEnd msg =  new B2G_BattleEnd();

            msg.RoomId = this.Id;

            gateSession.Send(msg);

        }


        private void InitTanks()
        {
            Map mapInfo = (Map)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Map), 1001);

            int index = 1;

            foreach (BattleOnePeople battleOnePeople in this.m_LeftCamp.Values)
            {
                Tank tank = battleOnePeople.Tank;

                battleOnePeople.Index = index;

                InitOneTank(tank, index);

            }

            index = 1;

            foreach (BattleOnePeople battleOnePeople in this.m_RightCamp.Values)
            {
                Tank tank = battleOnePeople.Tank;

                battleOnePeople.Index = index;

                InitOneTank(tank, index);

            }
        }

        private void InitOneTank(Tank tank, int index)
        {
            Map mapInfo = (Map)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(Map), 1001);
            Vector3 Pos;
            Vector3 Rot;
            if (tank.TankCamp == TankCamp.Left)
            {
                switch (index)
                {
                    case 1:
                        Pos =  ConfigHelper.String2Vector3(mapInfo.LeftPos1);
                        Rot =  ConfigHelper.String2Vector3(mapInfo.LeftRot1);
                        break;
                    case 2:
                        Pos = ConfigHelper.String2Vector3(mapInfo.LeftPos2);
                        Rot =  ConfigHelper.String2Vector3(mapInfo.LeftRot2);
                        break;
                    case 3:
                        Pos = ConfigHelper.String2Vector3(mapInfo.LeftPos3);
                        Rot =  ConfigHelper.String2Vector3(mapInfo.LeftRot3);
                        break;
                    case 4:
                        Pos = ConfigHelper.String2Vector3(mapInfo.LeftPos4);
                        Rot =  ConfigHelper.String2Vector3(mapInfo.LeftRot4);
                        break;
                    case 5:
                        Pos = ConfigHelper.String2Vector3(mapInfo.LeftPos5);
                        Rot =  ConfigHelper.String2Vector3(mapInfo.LeftRot5);
                        break;
                    default:
                        Log.Error($"{index}不能大于5");
                        Pos = Vector3.zero;
                        Rot = Vector3.zero;
                        break;
                }
            }
            else
            {
                switch (index)
                {
                    case 1:
                        Pos = ConfigHelper.String2Vector3(mapInfo.RightPos1);
                        Rot =  ConfigHelper.String2Vector3(mapInfo.RightRot1);
                        break;
                    case 2:
                        Pos = ConfigHelper.String2Vector3(mapInfo.RightPos2);
                        Rot =  ConfigHelper.String2Vector3(mapInfo.RightRot2);
                        break;
                    case 3:
                        Pos = ConfigHelper.String2Vector3(mapInfo.RightPos3);
                        Rot =  ConfigHelper.String2Vector3(mapInfo.RightRot3);
                        break;
                    case 4:
                        Pos = ConfigHelper.String2Vector3(mapInfo.RightPos4);
                        Rot =  ConfigHelper.String2Vector3(mapInfo.RightRot4);
                        break;
                    case 5:
                        Pos = ConfigHelper.String2Vector3(mapInfo.RightPos5);
                        Rot =  ConfigHelper.String2Vector3(mapInfo.RightRot5);
                        break;
                    default:
                        Log.Error($"{index}不能大于5");
                        Pos = Vector3.zero;
                        Rot = Vector3.zero;
                        break;
                }

            }
            tank.PX = (int)(Pos.x * Tank.m_coefficient);
            tank.PY = (int)(Pos.y * Tank.m_coefficient);
            tank.PZ = (int)(Pos.z * Tank.m_coefficient);

            tank.RX = (int) (Rot.x * Tank.m_coefficient);
            tank.RY = (int) (Rot.y * Tank.m_coefficient);
            tank.RZ = (int) (Rot.z * Tank.m_coefficient);
        }
        
        private async ETVoid HeartBeat30ms()
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();

            while (true)
            {
                await timerComponent.WaitAsync(30, CancellationTokenSource.Token);

                Send_B2C_TankFrameInfos();

            }
        }

        private async ETVoid AwaitTimeEnd()
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();

            await timerComponent.WaitAsync(this.m_smallModel * 60 * 1000);

            CancellationTokenSource?.Cancel();

            CancellationTokenSource?.Dispose();

            this.GameOver();
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

            this.Clear();

            foreach (Tank tank in this.idTanks.Values)
            {
                tank.Dispose();
            }

            this.idTanks.Clear();

            this.m_LeftCamp.Clear();

            this.m_RightCamp.Clear();

            this.CancellationTokenSource?.Dispose();
        }
    }
}
