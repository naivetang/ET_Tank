namespace ETModel
{
	public interface INumericWatcher
	{
		void Run(long battleId,long tankId, NumericType changeType);
	}
}
