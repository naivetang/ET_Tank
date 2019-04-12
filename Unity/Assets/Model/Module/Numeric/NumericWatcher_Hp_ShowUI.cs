namespace ETModel
{
	/// <summary>
	/// 监视hp数值变化，改变血条值
	/// </summary>
	[NumericWatcher(NumericType.Hp)]
	public class NumericWatcher_Hp_ShowUI : INumericWatcher
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"> 变化的基础部分 比如血量的HpBase，HpAdd部分 </param>
		public void Run(long id, int changePart)
		{
            Tank tank = Game.Scene.GetComponent<TankComponent>().Get(id);

            int nowHp = tank.GetComponent<NumericComponent>()[NumericType.Hp];

            // if (nowHp < 0)
            // {
            //     tank.GetComponent<NumericComponent>()[NumericType.HpBase] = 0;
            //     return;
            // }
            //
            // if (nowHp == 0)
            // {
            //     tank.Died = true;
            // }

            if (nowHp <= 0)
            {
                nowHp = 0;
            }

            if (tank.TankType == TankType.Local)
            {
                tank.LocalTankHpUIChange(tank.GetComponent<NumericComponent>()[NumericType.MaxHp], nowHp);
            }
            else if (tank.TankType == TankType.Remote)
            {
                tank.RemoteTankHpUIChange(tank.GetComponent<NumericComponent>()[NumericType.MaxHp], nowHp);
            }

            if (nowHp == 0)
                tank.Died = true;

            Log.Warning($"当前血量 = {nowHp}");
        }
	}
}
