using System;
using System.Threading;
using ETModel;
using NLog;

namespace App
{
	internal static class Program
	{
        //--appId=1 --appType=AllServer --config=../Config/StartConfig/LocalAllServer.txt

        /*{ "_t" : "StartConfig", "_id" : NumberLong("98547768819754"),
         "C" : [{ "_t" : "OuterConfig", "Address" : "127.0.0.1:10002", "Address2" : "127.0.0.1:10002" }, 
         { "_t" : "InnerConfig", "Address" : "127.0.0.1:20000" }, 
         { "_t" : "HttpConfig", "Url" : "http://*:8080/", "AppId" : 0, "AppKey" : "", "ManagerSystemUrl" : "" }, 
         { "_t" : "DBConfig", "ConnectionString" : "mongodb://localhost:27017", "DBName" : "ET" }],
         "AppId" : 1, "AppType" : "AllServer", "ServerIP" : "*" }
        */
        private static void Main(string[] args)
		{
			// 异步方法全部会回掉到主线程
			SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);
			
			try
			{			
				Game.EventSystem.Add(DLLType.Model, typeof(Game).Assembly);
				Game.EventSystem.Add(DLLType.Hotfix, DllHelper.GetHotfixAssembly());

				Options options = Game.Scene.AddComponent<OptionComponent, string[]>(args).Options;
				StartConfig startConfig = Game.Scene.AddComponent<StartConfigComponent, string, int>(options.Config, options.AppId).StartConfig;

				if (!options.AppType.Is(startConfig.AppType))
				{
					Log.Error("命令行参数apptype与配置不一致");
					return;
				}

				IdGenerater.AppId = options.AppId;

				LogManager.Configuration.Variables["appType"] = startConfig.AppType.ToString();
				LogManager.Configuration.Variables["appId"] = startConfig.AppId.ToString();
				LogManager.Configuration.Variables["appTypeFormat"] = $"{startConfig.AppType,-8}";
				LogManager.Configuration.Variables["appIdFormat"] = $"{startConfig.AppId:D3}";

				Log.Info($"server start........................ {startConfig.AppId} {startConfig.AppType}");

				Game.Scene.AddComponent<OpcodeTypeComponent>();
				Game.Scene.AddComponent<MessageDispatcherComponent>();

				// 根据不同的AppType添加不同的组件
				OuterConfig outerConfig = startConfig.GetComponent<OuterConfig>();
				InnerConfig innerConfig = startConfig.GetComponent<InnerConfig>();
				ClientConfig clientConfig = startConfig.GetComponent<ClientConfig>();
				
				switch (startConfig.AppType)
				{
					case AppType.Manager:
						Game.Scene.AddComponent<AppManagerComponent>();
						Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
						Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
						break;
					case AppType.Realm:
						Game.Scene.AddComponent<MailboxDispatcherComponent>();
						Game.Scene.AddComponent<ActorMessageDispatcherComponent>();
						Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
						Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
						Game.Scene.AddComponent<LocationProxyComponent>();
						Game.Scene.AddComponent<RealmGateAddressComponent>();
						break;
					case AppType.Gate:
						Game.Scene.AddComponent<PlayerComponent>();
						Game.Scene.AddComponent<MailboxDispatcherComponent>();
						Game.Scene.AddComponent<ActorMessageDispatcherComponent>();
						Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
						Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
						Game.Scene.AddComponent<LocationProxyComponent>();
						Game.Scene.AddComponent<ActorMessageSenderComponent>();
						Game.Scene.AddComponent<ActorLocationSenderComponent>();
						Game.Scene.AddComponent<GateSessionKeyComponent>();
						break;
					case AppType.Location:
						Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
						Game.Scene.AddComponent<LocationComponent>();
						break;
					case AppType.Map:
						Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
						Game.Scene.AddComponent<UnitComponent>();
						Game.Scene.AddComponent<LocationProxyComponent>();
						Game.Scene.AddComponent<ActorMessageSenderComponent>();
						Game.Scene.AddComponent<ActorLocationSenderComponent>();
						Game.Scene.AddComponent<MailboxDispatcherComponent>();
						Game.Scene.AddComponent<ActorMessageDispatcherComponent>();
						Game.Scene.AddComponent<PathfindingComponent>();
						break;
					case AppType.AllServer:
						// 发送普通actor消息
						Game.Scene.AddComponent<ActorMessageSenderComponent>();
						
						// 发送location actor消息
						Game.Scene.AddComponent<ActorLocationSenderComponent>();
						
                        // 数据库管理组件（管理数据库连接地址，数据库名称等）
						Game.Scene.AddComponent<DBComponent>();
                        // 数据库调用组件（调用DB数据库的组件  添加、查询、修改、删除等操作）
						Game.Scene.AddComponent<DBProxyComponent>();
						
						// location server需要的组件
						Game.Scene.AddComponent<LocationComponent>();
						
						// 访问location server的组件
						Game.Scene.AddComponent<LocationProxyComponent>();
						
						// 这两个组件是处理actor消息使用的
						Game.Scene.AddComponent<MailboxDispatcherComponent>();
						Game.Scene.AddComponent<ActorMessageDispatcherComponent>();
						
						// 内网消息组件
						Game.Scene.AddComponent<NetInnerComponent, string>(innerConfig.Address);
						
						// 外网消息组件
						Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
						
						// manager server组件，用来管理其它进程使用
						Game.Scene.AddComponent<AppManagerComponent>();
						Game.Scene.AddComponent<RealmGateAddressComponent>();
						Game.Scene.AddComponent<GateSessionKeyComponent>();
						
						// 配置管理
						Game.Scene.AddComponent<ConfigComponent>();

                        // 数值变化检测组件
                        Game.Scene.AddComponent<NumericWatcherComponent>();

                        // recast寻路组件
                        Game.Scene.AddComponent<PathfindingComponent>();
						
                        // 登陆之后保护的玩家
						Game.Scene.AddComponent<PlayerComponent>();
						Game.Scene.AddComponent<UnitComponent>();

                        // 进入战场后的战场组件
                        Game.Scene.AddComponent<TankComponent>();

                        // 房间系统组件
                        Game.Scene.AddComponent<RoomComponent>();

						Game.Scene.AddComponent<ConsoleComponent>();
						// Game.Scene.AddComponent<HttpComponent>();
						break;
					case AppType.Benchmark:
						Game.Scene.AddComponent<NetOuterComponent>();
						Game.Scene.AddComponent<BenchmarkComponent, string>(clientConfig.Address);
						break;
					case AppType.BenchmarkWebsocketServer:
						Game.Scene.AddComponent<NetOuterComponent, string>(outerConfig.Address);
						break;
					case AppType.BenchmarkWebsocketClient:
						Game.Scene.AddComponent<NetOuterComponent>();
						Game.Scene.AddComponent<WebSocketBenchmarkComponent, string>(clientConfig.Address);
						break;
					default:
						throw new Exception($"命令行参数没有设置正确的AppType: {startConfig.AppType}");
				}

                long fixedUpdateInterval = (long)(EventSystem.FixedUpdateTime * 1000);
                long timing = TimeHelper.ClientNow();
                while (true)
                {
                    try
                    {
                        Thread.Sleep(1);
                        OneThreadSynchronizationContext.Instance.Update();
                        Game.EventSystem.Update();
                        if (TimeHelper.ClientNow() - timing >= fixedUpdateInterval)
                        {
                            timing += fixedUpdateInterval;
                            Game.EventSystem.FixedUpdate();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}
	}
}
