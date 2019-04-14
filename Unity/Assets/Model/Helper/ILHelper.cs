using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FairyGUI;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Generated;
using ILRuntime.Runtime.Intepreter;

namespace ETModel
{
	public static class ILHelper
	{
		public static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
		{
			// 注册重定向函数

			// 注册委托
            appdomain.DelegateManager.RegisterMethodDelegate<int, int>();
            appdomain.DelegateManager.RegisterMethodDelegate<float, float>();
            appdomain.DelegateManager.RegisterMethodDelegate<float, float,float>();
			appdomain.DelegateManager.RegisterMethodDelegate<List<object>>();
			appdomain.DelegateManager.RegisterMethodDelegate<AChannel, System.Net.Sockets.SocketError>();
			appdomain.DelegateManager.RegisterMethodDelegate<byte[], int, int>();
			appdomain.DelegateManager.RegisterMethodDelegate<IResponse>();
			appdomain.DelegateManager.RegisterMethodDelegate<Session, object>();
			appdomain.DelegateManager.RegisterMethodDelegate<Session, byte, ushort, MemoryStream>();
			appdomain.DelegateManager.RegisterMethodDelegate<Session>();
			appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
			appdomain.DelegateManager.RegisterMethodDelegate<EventContext>();
			appdomain.DelegateManager.RegisterFunctionDelegate<Google.Protobuf.Adapt_IMessage.Adaptor>();
			appdomain.DelegateManager.RegisterMethodDelegate<Google.Protobuf.Adapt_IMessage.Adaptor>();
			appdomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.EventCallback0>((act) =>
			{
				return new FairyGUI.EventCallback0(() =>
				{
					((Action)act)();
				});
			});
            appdomain.DelegateManager.RegisterDelegateConvertor<FairyGUI.EventCallback1>((act) =>
            {
                return new FairyGUI.EventCallback1((context) =>
                {
                    ((Action<FairyGUI.EventContext>)act)(context);
                });
            });


            CLRBindings.Initialize(appdomain);

			// 注册适配器
			Assembly assembly = typeof(Init).Assembly;
			foreach (Type type in assembly.GetTypes())
			{
				object[] attrs = type.GetCustomAttributes(typeof(ILAdapterAttribute), false);
				if (attrs.Length == 0)
				{
					continue;
				}
				object obj = Activator.CreateInstance(type);
				CrossBindingAdaptor adaptor = obj as CrossBindingAdaptor;
				if (adaptor == null)
				{
					continue;
				}
				appdomain.RegisterCrossBindingAdaptor(adaptor);
			}

			LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);
		}
	}
}