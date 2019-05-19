using System.Threading;
using ETModel;
using FairyGUI;
using UnityEngine;

namespace ETHotfix
{
    public class MainItfViewComponent : Component
    {
        struct Round
        {
            public GTextField m_text;
            public int m_num;

            public int Num
            {
                set
                {
                    this.m_num = value;
                    this.m_text.text = this.m_num.ToString();
                }
                get => this.m_num;
            }
        }

        public static G2C_StartGame BattleInfo;

        private CancellationTokenSource CancellationTokenSource = null;

        public FUI m_point;

        public FUI m_BoomPoint;

        public FUI m_HP;

        private FUI FUIComponent;

        private Round m_totalRound;

        private GTextField m_time;

        private Round m_leftRound;

        private Round m_rightRound;

        private GTextField m_leftCampName;

        private GTextField m_rightCampName;

        public void Awake()
        {
            this.FUIComponent = this.GetParent<FUI>();
            this.StartUp();
        }

        private void StartUp()
        {
            this.m_totalRound.m_text = this.FUIComponent.Get("n15").GObject.asTextField;
            this.m_time = this.FUIComponent.Get("n18").GObject.asTextField;
            this.m_leftRound.m_text = this.FUIComponent.Get("n19").GObject.asTextField;
            this.m_rightRound.m_text = this.FUIComponent.Get("n20").GObject.asTextField;
            this.m_leftCampName = this.FUIComponent.Get("n12").GObject.asTextField;
            this.m_rightCampName = this.FUIComponent.Get("n13").GObject.asTextField;

            this.m_totalRound.Num = 0;

            this.m_leftRound.Num = 0;

            this.m_rightRound.Num = 0;

            this.Lanaguage();

            // 回合制
            if (BattleInfo.BigModel == 1)
            {
                TimeStartFromZero();

                this.AddTotalRound();
            }
            // 时间制
            else
            {
                TimeStartFromTime(BattleInfo.SmallModel);

                // 时间制始终显示一回合
                this.AddTotalRound();
            }
        }

        public void LeftWin()
        {
            this.m_leftRound.Num += 1;

        }

        public void RightWin()
        {
            this.m_rightRound.Num += 1;
            
        }

        public void AddTotalRound()
        {
            this.m_totalRound.Num += 1;


        }

        private void TimeStartFromZero()
        {
            this.m_time.text = "00:00";

            if (CancellationTokenSource != null)
            {
                if (!CancellationTokenSource.IsCancellationRequested)
                {
                    this.CancellationTokenSource.Cancel();
                }

                this.CancellationTokenSource.Dispose();

                this.CancellationTokenSource = null;
            }

            this.CancellationTokenSource = new CancellationTokenSource();

            this.AddOneSecond().NoAwait();
        }

        private async ETVoid AddOneSecond()
        {
            TimerComponent timer = ETModel.Game.Scene.GetComponent<TimerComponent>();

            int min = 0;

            int sec = 0;

            while (true)
            {
                await timer.WaitAsync(1000, CancellationTokenSource.Token);

                sec += 1;

                if (sec == 60)
                {
                    sec = 0;
                    min += 1;
                }

                this.m_time.text = $"{min.Int2TwoChar()}:{sec.Int2TwoChar()}";
            }

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">分钟</param>
        private void TimeStartFromTime(int time)
        {
            this.m_time.text = $"{time.Int2TwoChar()}:00";

            if (CancellationTokenSource != null)
            {
                if (!CancellationTokenSource.IsCancellationRequested)
                {
                    this.CancellationTokenSource.Cancel();
                }

                this.CancellationTokenSource.Dispose();

                this.CancellationTokenSource = null;
            }

            this.CancellationTokenSource = new CancellationTokenSource();

            this.ReduceOneSecond(time).NoAwait();
        }

        private async ETVoid ReduceOneSecond(int time)
        {
            TimerComponent timer = ETModel.Game.Scene.GetComponent<TimerComponent>();

            int min = time;

            int sec = 0;

            while (true)
            {
                await timer.WaitAsync(1000, CancellationTokenSource.Token);

                sec -= 1;

                if (sec == -1)
                {
                    sec = 59;
                    min -= 1;
                }

                if (min <= 0 && sec <= 0)
                {
                    this.m_time.text = $"00:00";

                    CancellationTokenSource.Cancel();
                }


                this.m_time.text = $"{min.Int2TwoChar()}:{sec.Int2TwoChar()}";
            }


        }

        private void Lanaguage()
        {
            this.m_leftCampName.text = Message.Get(1048);
            this.m_rightCampName.text = Message.Get(1049);
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            if (CancellationTokenSource != null)
            {

                if (!CancellationTokenSource.IsCancellationRequested)
                {
                    this.CancellationTokenSource.Cancel();
                }

                this.CancellationTokenSource.Dispose();

                this.CancellationTokenSource = null;
            }
        }

        public void UpdateBoomPoint( float x,float y)
        {
            m_BoomPoint.GObject.SetXY(x, y);
        }

        public void HpChange(int max,int current)
        {
            GProgressBar pb = this.m_HP.GObject.asProgress;

            pb.max = max;

            pb.value = current;
        }

    }

    

    [ObjectSystem]
    public class MainIntAwakeSystem: AwakeSystem<MainItfViewComponent>
    {
        public override void Awake(MainItfViewComponent self)
        {
            FUI FGUICompunt = self.GetParent<FUI>();

            self.m_point = FGUICompunt.Get("point");

            self.m_BoomPoint = FGUICompunt.Get("BoomPoint");

            self.m_HP = FGUICompunt.Get("hp");

            TurretComponent.UpdatePos += self.UpdateBoomPoint;

            Tank.m_hpChange += self.HpChange;

            self.Awake();
        }
        
    }

}
