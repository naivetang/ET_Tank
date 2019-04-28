namespace ETModel
{
	public static class Game
    {

		private static Scene scene;

		public static Scene Scene
		{
			get
			{
				if (scene != null)
				{
					return scene;
				}
				scene = new Scene();
				scene.AddComponent<TimerComponent>();
				return scene;
			}
		}

		private static EventSystem eventSystem;

		public static EventSystem EventSystem
		{
			get
			{
				return eventSystem ?? (eventSystem = new EventSystem());
			}
		}

		private static ComObjectPool comObjectPool;

		public static ComObjectPool ComObjectPool
		{
			get
			{
				return comObjectPool ?? (comObjectPool = new ComObjectPool());
			}
		}

		public static void Close()
		{
			scene.Dispose();
			scene = null;
			eventSystem = null;
			comObjectPool = null;
		}
	}
}