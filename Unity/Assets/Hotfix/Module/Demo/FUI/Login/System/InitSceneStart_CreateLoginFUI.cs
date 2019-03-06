using ETModel;

namespace ETHotfix
{
	[Event(EventIdType.InitSceneStart)]
	public class InitSceneStart_CreateLoginFUI: AEvent
	{
		public override void Run()
		{
			RunAsync().NoAwait();
		}

		public async ETVoid RunAsync()
		{
			FUIComponent fuiComponent = Game.Scene.GetComponent<FUIComponent>();
			// 使用工厂创建一个Login UI
			FUI ui = await FUILoginFactory.Create();
			fuiComponent.Add(ui);
		}
	}
}
