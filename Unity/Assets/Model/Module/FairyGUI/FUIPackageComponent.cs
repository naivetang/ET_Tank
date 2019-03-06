using System.Collections.Generic;
using FairyGUI;
#if !UNITY_EDITOR
using UnityEngine;
#endif

namespace ETModel
{
	/// <summary>
	/// 管理所有UI Package
	/// </summary>
	public class FUIPackageComponent: Component
	{
		public const string FUI_PACKAGE_DIR = "Assets/Res/FairyGUI";
		
		private readonly Dictionary<string, UIPackage> packages = new Dictionary<string, UIPackage>();
		
		
		public void AddPackage(string type)
		{
#if UNITY_EDITOR
			UIPackage uiPackage = UIPackage.AddPackage($"{FUI_PACKAGE_DIR}/{type}");
#else
            string uiBundleName = type.StringToAB();
            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle(uiBundleName);
	        
            AssetBundle assetBundle = resourcesComponent.GetAssetBundle(uiBundleName);
            UIPackage uiPackage = UIPackage.AddPackage(assetBundle);
#endif
			this.packages.Add(type, uiPackage);
		}
        
		public async ETTask AddPackageAsync(string type)
		{
#if UNITY_EDITOR
			await ETTask.CompletedTask;
            
			UIPackage uiPackage = UIPackage.AddPackage($"{FUI_PACKAGE_DIR}/{type}");
#else
            string uiBundleName = type.StringToAB();
            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            await resourcesComponent.LoadBundleAsync(uiBundleName);
	        
            AssetBundle assetBundle = resourcesComponent.GetAssetBundle(uiBundleName);
            UIPackage uiPackage = UIPackage.AddPackage(assetBundle);
#endif
			this.packages.Add(type, uiPackage);
		}

		public void RemovePackage(string type)
		{
			this.packages.TryGetValue(type, out UIPackage package);
			UIPackage.RemovePackage(package.name);
			this.packages.Remove(package.name);
#if !UNITY_EDITOR
			ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle(type.StringToAB());
#endif
		}
	}
}