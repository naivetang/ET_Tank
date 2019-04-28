using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.ShowPopMessage)]
    class ShowPopMessage:AEvent<string,PopMessageType>
    {
        public override void Run(string a, PopMessageType b)
        {
            FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.PopMessage);

            fui.GetComponent<PopMessageViewComponent>().AddEmitTip(a, b);
        }
    }
}
