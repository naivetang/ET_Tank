using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.EnterBattleFinish)]
    public class EnterBattleFinish_UnloadPag : AEvent
    {
        public override void Run()
        {
            FUIComponent fuiComponent = Game.Scene.GetComponent<FUIComponent>();

            //fuiComponent.Remove(FUIType.Lobby);

            fuiComponent.Remove(FUIType.Room);


            fuiComponent.Remove(FUIType.Hall);

            

            // 卸载包
            ETModel.Game.Scene.GetComponent<FUIPackageComponent>().RemovePackage(FUIType.Room);

            ETModel.Game.Scene.GetComponent<FUIPackageComponent>().RemovePackage(FUIType.Hall);
        }
    }
}
