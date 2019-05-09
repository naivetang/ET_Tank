using ETModel;

namespace ETHotfix
{
	[Config(AppType.ClientH |  AppType.AllServer)]
	public partial class TankCfgCategory : ACategory<TankCfg>
	{
	}

	public partial class TankCfg: IConfig
	{
		public long Id { get; set; }
		public int Price;
		public string Chinese;
		public string English;
		public string Icon;
		public int Attack;
		public int Defence;
		public int SunderArmor;
		public int CanBuy;
	}
}
