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
		public string LeftPos1;
		public string LeftRot1;
		public string LeftPos2;
		public string LeftRot2;
		public string LeftPos3;
		public string LeftRot3;
		public string LeftPos4;
		public string LeftRot4;
		public string LeftPos5;
		public string LeftRot5;
		public string RightPos1;
		public string RightRot1;
		public string RightPos2;
		public string RightRot2;
		public string RightPos3;
		public string RightRot3;
		public string RightPos4;
		public string RightRot4;
		public string RightPos5;
		public string RightRot5;
	}
}
