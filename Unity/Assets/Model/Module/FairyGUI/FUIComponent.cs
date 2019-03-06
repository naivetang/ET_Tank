using FairyGUI;

namespace ETModel
{
	[ObjectSystem]
	public class FUIComponentAwakeSystem : AwakeSystem<FUIComponent>
	{
		public override void Awake(FUIComponent self)
		{
			GRoot.inst.SetContentScaleFactor(1280, 720, UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);
			self.Root = ComponentFactory.Create<FUI, GObject>(GRoot.inst);
		}
	}

	/// <summary>
	/// 管理所有顶层UI, 顶层UI都是GRoot的孩子
	/// </summary>
	public class FUIComponent: Component
	{
		public FUI Root;
		
		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();
			
			this.Root.RemoveChildren();
		}

		public void Add(FUI ui)
		{
			this.Root.Add(ui);
		}
		
		public void Remove(string name)
		{
			this.Root.Remove(name);
		}
		
		public FUI Get(string name)
		{
			FUI ui = this.Root.Get(name);
			return ui;
		}
	}
}