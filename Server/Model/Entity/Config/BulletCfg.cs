namespace ETModel
{
	[Config(AppType.ClientH |  AppType.AllServer)]
	public partial class BulletCfgCategory : ACategory<BulletCfg>
	{
	}

	public partial class BulletCfg: IConfig
	{
		public long Id { get; set; }
		public int Price;
		public string Icon;
		public string Chinese;
		public string English;
		public int Attack;
		public int SunderArmor;
		public int LoadingSpeed;
		public int CanBuy;
	}
}
