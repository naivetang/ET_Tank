using ETModel;
using FairyGUI;

namespace ETHotfix
{

    public static class MainIntViewFactory
    {
        public static async ETTask<FUI> Create()
        {
            await ETTask.CompletedTask;

            // 可以同步或者异步加载,异步加载需要搞个转圈圈,这里为了简单使用同步加载
            // await ETModel.Game.Scene.GetComponent<FUIPackageComponent>().AddPackageAsync(FUIType.Login);
            ETModel.Game.Scene.GetComponent<FUIPackageComponent>().AddPackage(FUIType.MainInterface);

            FUI fui = ComponentFactory.Create<FUI, GObject>(UIPackage.CreateObject(FUIType.MainInterface, FUIType.MainInterface));

            fui.Name = FUIType.MainInterface;

            // 这里可以根据UI逻辑的复杂度关联性，拆分成多个小组件来写逻辑,这里逻辑比较简单就只使用一个组件了
            fui.AddComponent<FUIMainItfComponent>();


            // FUI uilong = Game.Scene.GetComponent<FUIComponent>().Get(FUIType.Login);
            //
            // uilong.GetComponent<LoginViewComponent>();

            return fui;
        }
    }


    [Event(EventIdType.EnterBattlefieldFinish),Event(EventIdType.EnterBattleFinish)]
    public class CreateMainIntView : AEvent
    {
        public override void Run()
        {
            this.RunAsync().NoAwait();
        }

        public async ETVoid RunAsync()
        {
            FUIComponent fuiComponent = Game.Scene.GetComponent<FUIComponent>();
            // 使用工厂创建一个Login UI
            FUI ui = await MainIntViewFactory.Create();
            fuiComponent.Add(ui);
        }
    }
}
