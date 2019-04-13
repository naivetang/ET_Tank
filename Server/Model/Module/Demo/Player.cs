namespace ETModel
{
	[ObjectSystem]
	public class PlayerSystem : AwakeSystem<Player, string>
	{
		public override void Awake(Player self, string a)
		{
			self.Awake(a);
		}
	}

    [ObjectSystem]
    public class Player2System : AwakeSystem<Player, UserDB>
    {
        public override void Awake(Player self, UserDB a)
        {
            self.Awake(a);
        }
    }

    public static class PlayerHelper
    {
        public static Tank Tank(this Player self)
        {
            return Game.Scene.GetComponent<TankComponent>().Get(self.TankId);
        }
    }

    public sealed class Player : Entity
	{
		public string Account { get; private set; }

        public UserDB UserDB { get; private set; }

        public long TankId { get; set; } = 0L;

        public long UnitId { get; set; } = 0L;

		public void Awake(string account)
		{
			this.Account = account;
		}

        public void Awake(UserDB userDb)
        {
            this.UserDB = userDb;
        }
		
		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();
		}
	}
}