using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
	public static class Init
	{
		public static void Start()
		{
			try
			{
				Game.Scene.ModelScene = ETModel.Game.Scene;

				// 注册热更层回调
				ETModel.Game.Hotfix.Update = () => { Update(); };
				ETModel.Game.Hotfix.LateUpdate = () => { LateUpdate(); };
				ETModel.Game.Hotfix.OnApplicationQuit = () => { OnApplicationQuit(); };
				
				Game.Scene.AddComponent<FUIComponent>();
				Game.Scene.AddComponent<OpcodeTypeComponent>();
				Game.Scene.AddComponent<MessageDispatcherComponent>();

				// 加载热更配置
				ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
                // 加载表格数据
				Game.Scene.AddComponent<ConfigComponent>();
				ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");

				UnitConfig unitConfig = (UnitConfig)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(UnitConfig), 1001);
				Log.Debug($"config {JsonHelper.ToJson(unitConfig)}");

				Game.EventSystem.Run(EventIdType.InitSceneStart);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public static void Update()
		{
			try
			{
				Game.EventSystem.Update();

			    if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
			    {
			        Game.EventSystem.Run(EventIdType.ClickEnter);
			    }
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public static void LateUpdate()
		{
			try
			{
				Game.EventSystem.LateUpdate();
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public static void OnApplicationQuit()
		{
			Game.Close();
		}
	}
}