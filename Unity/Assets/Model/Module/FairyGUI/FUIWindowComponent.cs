namespace ETModel
{
	[ObjectSystem]
	public class FUIWindowComponentAwakeSystem : AwakeSystem<FUIWindowComponent>
	{
		public override void Awake(FUIWindowComponent self)
		{
			FUI fui = self.GetParent<FUI>();
			self.Window = new GWindow();
			self.Window.contentPane = fui.GObject.asCom;
		}
	}

	/// <summary>
	/// 挂上这个组件，就成为了一个窗口
	/// </summary>
	public class FUIWindowComponent: Component
	{
		public GWindow Window;
		
		public void Show()
		{
			this.Window.Show();
		}

		public void Hide()
		{
			this.Window.Hide();
		}

		public bool IsShowing
		{
			get
			{
				return this.Window.isShowing;
			}
		}

		public bool Modal
		{
			get
			{
				return this.Window.modal;
			}
			set
			{
				this.Window.modal = value;
			}
		}
	}
}