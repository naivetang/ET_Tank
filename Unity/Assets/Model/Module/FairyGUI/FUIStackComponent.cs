using System.Collections.Generic;

namespace ETModel
{
	[ObjectSystem]
	public class FUIStackComponentAwakeSystem : AwakeSystem<FUIStackComponent>
	{
		public override void Awake(FUIStackComponent self)
		{
		}
	}

	/// <summary>
	/// UI栈
	/// </summary>
	public class FUIStackComponent: Component
	{
		private readonly Stack<FUI> uis = new Stack<FUI>();

		public int Count
		{
			get
			{
				return this.uis.Count;
			}
		}

		public void Push(FUI fui)
		{
			this.uis.Peek().Visible = false;
			this.uis.Push(fui);
		}

		public void Pop()
		{
			FUI fui = this.uis.Pop();
			fui.Dispose();
			if (this.uis.Count > 0)
			{
				this.uis.Peek().Visible = true;
			}
		}

		public void Clear()
		{
			while (this.uis.Count > 0)
			{
				FUI fui = this.uis.Pop();
				fui.Dispose();
			}
		}
	}
}