using ETModel;

namespace ETHotfix
{
	[Config(AppType.ClientH |  AppType.AllServer)]
	public partial class PropCategory : ACategory<Prop>
	{
	}

	public partial class Prop: IConfig
	{
		public long Id { get; set; }
		public int Price;
		public string Icon;
		public string Chinese;
		public string English;
		public int Experience;
		public int Gold;
		public int TotleTimes;
	}
}
