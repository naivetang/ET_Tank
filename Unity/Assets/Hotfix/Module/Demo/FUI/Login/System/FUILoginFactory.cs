using ETModel;
using FairyGUI;

namespace ETHotfix
{
    public static class FUILoginFactory
    {
        public static async ETTask<FUI> Create()
        {
	        await ETTask.CompletedTask;
	        
	        // 可以同步或者异步加载,异步加载需要搞个转圈圈,这里为了简单使用同步加载
	        // await ETModel.Game.Scene.GetComponent<FUIPackageComponent>().AddPackageAsync(FUIType.Login);
	        ETModel.Game.Scene.GetComponent<FUIPackageComponent>().AddPackage(FUIType.Login);
	        
	        FUI fui = ComponentFactory.Create<FUI, GObject>(UIPackage.CreateObject(FUIType.Login, FUIType.Login));
	        fui.Name = FUIType.Login;
	        
	        // 这里可以根据UI逻辑的复杂度关联性，拆分成多个小组件来写逻辑,这里逻辑比较简单就只使用一个组件了
	        fui.AddComponent<FUILoginComponent>();
	        
	        return fui;
        }
    }
}