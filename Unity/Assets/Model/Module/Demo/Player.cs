namespace ETModel
{
	public sealed class Player : Entity
	{
		public long UnitId { get; set; }

        public long TankId { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public int Experience { get; set; }

        public long DbID { get; set; }

        public int Gold { get; set; }


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