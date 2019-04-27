using ETModel;

namespace ETHotfix
{
    [Event(EventIdType.LanguageChange)]
    class LanguageChange_RefreshHallView : AEvent
    {
        public override void Run()
        {
            FUI fui = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Hall);

            if (fui == null)
                return;

            HallViewComponent hallView = fui.GetComponent<HallViewComponent>();

            hallView.RefreshData();
        }
    }
}
