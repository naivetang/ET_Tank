using ETModel;

namespace ETHotfix
{
	[Event(EventIdType.EnterMapFinish)]
	public class EnterMapFinish_RemoveLobbyUI: AEvent
	{
		public override void Run()
		{
			Game.Scene.GetComponent<FUIComponent>().Remove(FUIType.Lobby);
			ETModel.Game.Scene.GetComponent<FUIPackageComponent>().RemovePackage(FUIType.Lobby);
		}
	}
}
