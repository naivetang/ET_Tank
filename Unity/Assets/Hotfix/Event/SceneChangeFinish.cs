using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.ChangeSceneFinish)]
    class SceneChangeFinish : AEvent<string>
    {
        public override void Run(string a)
        {
            switch (a)
            {
                case SceneType.Battle:
                    Game.EventSystem.Run(EventIdType.EnterBattleFinish);
                    break;
                default:
                    Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Loading).GetComponent<LoadingViewComponent>().CanClose = true;
                    break;
            }
        }
    }

    [Event(EventIdType.CreateTanksFinish)]
    class CreateTanksFinish : AEvent
    {
        public override void Run()
        {
            Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Loading).GetComponent<LoadingViewComponent>().CanClose = true;
        }
    }


}
