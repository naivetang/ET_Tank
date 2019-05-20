using ETModel;
using UnityEngine;

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

                    Cursor.visible = false;//隐藏指针

                    Cursor.lockState = CursorLockMode.Confined;
                    break;
                default:
                    Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Loading).GetComponent<LoadingViewComponent>().CanClose = true;

                    Cursor.visible = true;//隐藏指针

                    Cursor.lockState = CursorLockMode.None;
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
