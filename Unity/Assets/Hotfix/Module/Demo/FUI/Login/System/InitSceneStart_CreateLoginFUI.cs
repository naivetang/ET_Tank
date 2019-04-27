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
			// 使用工厂创建一个Login UI
            await FUIFactory.Create<LoginViewComponent>(FUIType.Login);
        }
	}
}
