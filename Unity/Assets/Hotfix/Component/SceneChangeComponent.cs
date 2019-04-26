using UnityEngine;
using ETModel;
using UnityEngine.SceneManagement;

namespace ETHotfix
{

    [ObjectSystem]
    public class SceneChangeComponentAwakeSystem : AwakeSystem<SceneChangeComponent>
    {
        public override void Awake(SceneChangeComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
	public class SceneChangeComponentUpdateSystem: UpdateSystem<SceneChangeComponent>
	{
		public override void Update(SceneChangeComponent self)
		{
			if (self.loadMapOperation.isDone)
			{
                self.tcs.SetResult();
			}
		}
	}

	public class SceneChangeComponent: Component, System.IDisposable
    {
		public AsyncOperation loadMapOperation;
		public ETTaskCompletionSource tcs;
	    public float deltaTime;
	    public int lastProgress = 0;

        public void Awake()
        {
            Game.EventSystem.Run(EventIdType.ChangeScene);
        }

		public ETTask ChangeSceneAsync(string sceneName)
		{
            this.tcs = new ETTaskCompletionSource();
			// 加载map
			this.loadMapOperation = SceneManager.LoadSceneAsync(sceneName);

            return this.tcs.Task;
		}

		public int Process
		{
			get
			{
				if (this.loadMapOperation == null)
				{
					return 0;
				}
				return (int)(this.loadMapOperation.progress * 100);
			}
		}

		public void Finish()
		{
			this.tcs.SetResult();
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
			base.Dispose();

			if (this.Entity.IsDisposed)
			{
				return;
			}
			
			this.Entity.RemoveComponent<SceneChangeComponent>();

            Game.EventSystem.Run(EventIdType.ChangeSceneFinish, SceneManager.GetActiveScene().name);
        }

    }
}