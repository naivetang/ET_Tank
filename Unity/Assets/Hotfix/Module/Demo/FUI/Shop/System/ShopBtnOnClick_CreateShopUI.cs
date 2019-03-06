using ETModel;

namespace ETHotfix
{
	[Event(EventIdType.ShopBtnOnClick)]
	public class ShopBtnOnClick_CreateShopUI: AEvent
	{
		public override void Run()
		{
			RunAsync().NoAwait();
		}

		public async ETVoid RunAsync()
		{
			await FUIShopFactory.Create();
		}
	}
}
