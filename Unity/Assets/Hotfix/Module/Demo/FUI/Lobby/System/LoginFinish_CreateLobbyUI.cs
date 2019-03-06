using ETModel;

namespace ETHotfix
{
	[Event(EventIdType.LoginFinish)]
	public class LoginFinish_CreateLobbyUI: AEvent
	{
		public override void Run()
		{
			RunAsync().NoAwait();
		}

		public async ETVoid RunAsync()
		{
			FUIComponent fuiComponent = Game.Scene.GetComponent<FUIComponent>();
			
			// 使用工厂创建一个Lobby UI
			FUI ui = await FUILobbyFactory.Create();
			fuiComponent.Add(ui);
			
			// 创建Lobby UI完成 
			Game.EventSystem.Run(EventIdType.CreateLobbyUIFinish);
		}
	}
}
