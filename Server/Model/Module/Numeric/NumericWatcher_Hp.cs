
using System;
using ETModel;

namespace ETModel
{
    [NumericWatcher(NumericType.Hp)]
    public class NumericWatcher_Hp  : INumericWatcher
    {
        public void Run(long battleId, long tankId, NumericType changeType)
        {
            try
            {
                Tank tank = Game.Scene.GetComponent<BattleComponent>().Get(battleId).Get(tankId);
                if (tank == null)
                    return;

                NumericComponent numericComponent = tank.GetComponent<NumericComponent>();

                if (numericComponent == null)
                {
                    Log.Error($"未添加NumericComponent组件");
                    return;
                }

                int nowHp = numericComponent[NumericType.Hp];

                if (nowHp <= 0 && !tank.Died)
                {
                    tank.Died = true;

                    if (tank.Battle.BigMode == BigModel.Time)
                    {
                        Send_B2C_TankDie(tank);

                        tank.Battle.TimeResetTank(tank).NoAwait();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            
        }

        private void Send_B2C_TankDie(Tank tank)
        {
            B2C_TankDie msg = new B2C_TankDie();

            msg.DieTandkId = tank.Id;

            tank.Broadcast(msg);
        }
    }

    
}
