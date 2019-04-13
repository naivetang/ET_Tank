
using ETModel;

namespace ETModel
{
    [NumericWatcher(NumericType.Hp)]
    public class NumericWatcher_Hp  : INumericWatcher
    {
        public void Run(long id, int value)
        {
            Tank tank = Game.Scene.GetComponent<TankComponent>().Get(id);

             if (tank == null)
                 return;

             NumericComponent numericComponent = tank.GetComponent<NumericComponent>();

             if (numericComponent == null)
                 return;

            int nowHp = numericComponent[NumericType.Hp];

            if (nowHp <= 0)
                tank.Died = true;
        }
    }
}
