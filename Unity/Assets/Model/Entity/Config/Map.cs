namespace ETModel
{
	[Config(AppType.ClientH |  AppType.AllServer)]
	public partial class MapCategory : ACategory<Map>
	{
	}

	public partial class Map: IConfig
	{
		public long Id { get; set; }
		public string ChineseMapName;
		public string EnglishMapName;
	}
}
