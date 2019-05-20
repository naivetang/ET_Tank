using System;
using System.Collections.Generic;

namespace ETModel
{
	public class GateSessionKeyComponent : Component
	{
		private readonly Dictionary<long, UInt64> sessionKey = new Dictionary<long, UInt64>();
		
		public void Add(long key, UInt64 phoneNum)
		{
			this.sessionKey.Add(key, phoneNum);
			this.TimeoutRemoveKey(key).NoAwait();
		}

		public UInt64 Get(long key)
		{
            UInt64 account = 0;
			this.sessionKey.TryGetValue(key, out account);
			return account;
		}

		public void Remove(long key)
		{
			this.sessionKey.Remove(key);
		}

		private async ETVoid TimeoutRemoveKey(long key)
		{
			await Game.Scene.GetComponent<TimerComponent>().WaitAsync(20000);
			this.sessionKey.Remove(key);
		}
	}
}
