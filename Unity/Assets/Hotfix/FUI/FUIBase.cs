using ETModel;
using FairyGUI;

namespace ETHotfix
{
    public abstract class FUIBase : Component
    {
        public FUI FUIComponent;

        public virtual void OnClose()
        {
            if(Game.Scene.GetComponent<FUIComponent>().Get(this.FUIComponent.Name) != null)
                Game.Scene.GetComponent<FUIComponent>().Remove(this.FUIComponent.Name);

            this.Dispose();
        }
    }
}
