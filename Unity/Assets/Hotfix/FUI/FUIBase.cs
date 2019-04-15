using ETModel;
using FairyGUI;

namespace ETHotfix
{
    public abstract class FUIBase : Component
    {
        public FUI FUIComponent;

        public virtual void OnClose()
        {
            Game.Scene.GetComponent<FUIComponent>().Remove(this.FUIComponent.Name);
        }
    }
}
