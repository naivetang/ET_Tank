using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class FUIShopComponentSystem : AwakeSystem<FUIShopComponent>
    {
        public override void Awake(FUIShopComponent self)
        {
            FUI fui = self.GetParent<FUI>();
            fui.GetComponent<FUIWindowComponent>().Window.closeButton.onClick.Add(() =>
            {
                fui.Dispose();
                ETModel.Game.Scene.GetComponent<FUIPackageComponent>().RemovePackage(FUIType.Shop);
            });
        }
    }
}