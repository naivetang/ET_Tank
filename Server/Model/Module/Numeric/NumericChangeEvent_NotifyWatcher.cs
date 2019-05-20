namespace ETModel
{
	// 分发数值监听
	[Event(EventIdType.NumbericChange)]
	public class NumericChangeEvent_NotifyWatcher: AEvent<NumericType, long, long, NumericType>
	{

        public override void Run(NumericType a, long b, long c, NumericType d)
        {
            Game.Scene.GetComponent<NumericWatcherComponent>().Run(a, b, c,d);
        }
    }
}
