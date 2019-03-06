using ETModel;

namespace ETHotfix
{
	[Event(EventIdType.CreateLobbyUIFinish)]
	public class CreateLobbyUIFinish_RemoveLoginUI: AEvent
	{
		public override void Run()
		{
			Game.Scene.GetComponent<FUIComponent>().Remove(FUIType.Login);
			
			// 卸载包
			ETModel.Game.Scene.GetComponent<FUIPackageComponent>().RemovePackage(FUIType.Login);
		}
	}
}
