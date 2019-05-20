using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.ClickEnter)]
    public class ClickEnter_OpenGM : AEvent
    {
        public override void Run()
        {
            RunAsync().NoAwait();
        }

        public async ETVoid RunAsync()
        {
            FUIComponent fuiComponent = Game.Scene.GetComponent<FUIComponent>();
            if (fuiComponent.Get(FUIType.GM) != null)
                return;
            FUI ui = await FUIGMFactory.Create();
            fuiComponent.Add(ui);
        }
    }
}
