using ETModel;

namespace ETHotfix
{
	[Config(AppType.ClientH |  AppType.AllServer)]
	public partial class MessageCategory : ACategory<Message>
	{
	}

	public partial class Message: IConfig
	{
		public long Id { get; set; }
		public string Chinese;
		public string English;
	}
}
