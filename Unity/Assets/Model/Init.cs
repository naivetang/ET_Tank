using System;
using System.Threading;
using UnityEngine;

namespace ETModel
{
	public class Init : MonoBehaviour
	{
		private void Start()
		{
			this.StartAsync().NoAwait();
		}
		
		private async ETVoid StartAsync()
		{
			try
			{
				SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

				DontDestroyOnLoad(gameObject);
				Game.EventSystem.Add(DLLType.Model, typeof(Init).Assembly);

                // 全局配置  GlobalProto.txt 
                // {"AssetBundleServerUrl":"http://127.0.0.1:8080/","Address":"127.0.0.1:10002"}
                Game.Scene.AddComponent<GlobalConfigComponent>();

                // 网络组件连接组件（跟服务器通信）
				Game.Scene.AddComponent<NetOuterComponent>();

                // 资源管理组件（热更新资源的管理，AB包）
				Game.Scene.AddComponent<ResourcesComponent>();

				Game.Scene.AddComponent<PlayerComponent>();

				Game.Scene.AddComponent<UnitComponent>();

                // FGUI包管理
				Game.Scene.AddComponent<FUIPackageComponent>();

                // FGUI组件管理
                Game.Scene.AddComponent<FUIComponent>();

				// 下载ab包
				await BundleHelper.DownloadBundle();

                // 加载热更新
				Game.Hotfix.LoadHotfixAssembly();

				// 加载配置
				Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");

                //
				Game.Scene.AddComponent<ConfigComponent>();
				Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");

                // 协议类型
				Game.Scene.AddComponent<OpcodeTypeComponent>();

                // 消息分发 （客户端跟服务器通讯，进行消息的分发传递）
				Game.Scene.AddComponent<MessageDispatcherComponent>();

                // 执行热更层Hotfix的入口
				Game.Hotfix.GotoHotfix();

				Game.EventSystem.Run(EventIdType.TestHotfixSubscribMonoEvent, "TestHotfixSubscribMonoEvent");
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		private void Update()
		{
			OneThreadSynchronizationContext.Instance.Update();
			Game.Hotfix.Update?.Invoke();
			Game.EventSystem.Update();
		}

		private void LateUpdate()
		{
			Game.Hotfix.LateUpdate?.Invoke();
			Game.EventSystem.LateUpdate();
		}

		private void OnApplicationQuit()
		{
			Game.Hotfix.OnApplicationQuit?.Invoke();
			Game.Close();
		}
	}
}