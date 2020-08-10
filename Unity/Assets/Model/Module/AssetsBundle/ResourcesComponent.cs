﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ETModel
{

    public enum PrefabType
    {
        Tank,                   //坦克
        Bullet,                 //炮弹
        BulletBoom,             //子弹爆炸特效
        TankBoom,               //坦克焚烧
    }

	public class ABInfo : Component
	{
		private int refCount;
		public string Name { get; }

		public int RefCount
		{
			get
			{
				return this.refCount;
			}
			set
			{
				//Log.Debug($"{this.Name} refcount: {value}");
				this.refCount = value;
			}
		}

		public AssetBundle AssetBundle { get; private set; }

		public ABInfo(string name, AssetBundle ab)
		{
			this.Name = name;
			this.AssetBundle = ab;
			this.RefCount = 1;
			Log.Debug($"load assetbundle: {this.Name} {this.InstanceId}");
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
			
			Log.Debug($"desdroy assetbundle: {this.Name} {this.InstanceId}");

			base.Dispose();

			if (this.AssetBundle != null)
			{
				this.AssetBundle.Unload(true);
			}

			this.AssetBundle = null;
		}
	}
	
	// 用于字符串转换，减少GC
	public static class AssetBundleHelper
	{
		public static readonly Dictionary<int, string> IntToStringDict = new Dictionary<int, string>();
		
		public static readonly Dictionary<string, string> StringToABDict = new Dictionary<string, string>();

		public static readonly Dictionary<string, string> BundleNameToLowerDict = new Dictionary<string, string>() 
		{
			{ "StreamingAssets", "StreamingAssets" }
		};
		
		// 缓存包依赖，不用每次计算
		public static Dictionary<string, string[]> DependenciesCache = new Dictionary<string, string[]>();

		public static string IntToString(this int value)
		{
			string result;
			if (IntToStringDict.TryGetValue(value, out result))
			{
				return result;
			}

			result = value.ToString();
			IntToStringDict[value] = result;
			return result;
		}
		
		public static string StringToAB(this string value)
		{
			string result;
			if (StringToABDict.TryGetValue(value, out result))
			{
				return result;
			}

			result = value.ToLower() + ".unity3d";
			StringToABDict[value] = result;
			return result;
		}

		public static string IntToAB(this int value)
		{
			return value.IntToString().StringToAB();
		}
		
		public static string BundleNameToLower(this string value)
		{
			string result;
			if (BundleNameToLowerDict.TryGetValue(value, out result))
			{
				return result;
			}

			result = value.ToLower();
			BundleNameToLowerDict[value] = result;
			return result;
		}
		
		public static string[] GetDependencies(string assetBundleName)
		{
			string[] dependencies = new string[0];
			if (DependenciesCache.TryGetValue(assetBundleName,out dependencies))
			{
				return dependencies;
			}
			if (!Define.IsAsync)
			{
#if UNITY_EDITOR
				dependencies = AssetDatabase.GetAssetBundleDependencies(assetBundleName, true);
#endif
			}
			else
			{
				dependencies = ResourcesComponent.AssetBundleManifestObject.GetAllDependencies(assetBundleName);
			}
			DependenciesCache.Add(assetBundleName, dependencies);
			return dependencies;
		}

		public static string[] GetSortedDependencies(string assetBundleName)
		{
			Dictionary<string, int> info = new Dictionary<string, int>();
			List<string> parents = new List<string>();
			CollectDependencies(parents, assetBundleName, info);
			string[] ss = info.OrderBy(x => x.Value).Select(x => x.Key).ToArray();
			return ss;
		}

		public static void CollectDependencies(List<string> parents, string assetBundleName, Dictionary<string, int> info)
		{
			parents.Add(assetBundleName);
			string[] deps = GetDependencies(assetBundleName);
			foreach (string parent in parents)
			{
				if (!info.ContainsKey(parent))
				{
					info[parent] = 0;
				}
				info[parent] += deps.Length;
			}


			foreach (string dep in deps)
			{
				if (parents.Contains(dep))
				{
					throw new Exception($"包有循环依赖，请重新标记: {assetBundleName} {dep}");
				}
				CollectDependencies(parents, dep, info);
			}
			parents.RemoveAt(parents.Count - 1);
		}
	}
	

	public class ResourcesComponent : Component
	{
		public static AssetBundleManifest AssetBundleManifestObject { get; set; }

		private readonly Dictionary<string, Dictionary<string, UnityEngine.Object>> resourceCache = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();

		private readonly Dictionary<string, ABInfo> bundles = new Dictionary<string, ABInfo>();

        private readonly Dictionary<PrefabType, Queue<GameObject>> m_prefabs = new Dictionary<PrefabType, Queue<GameObject>>();

        private GameObject m_poolRoot;

        private GameObject PoolRoot
        {
            get
            {
                if(this.m_poolRoot == null)
                    this.m_poolRoot = GameObject.Find("ObjPool");
                return this.m_poolRoot;
            }
        }

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

			foreach (var abInfo in this.bundles)
			{
				abInfo.Value?.AssetBundle?.Unload(true);
			}

			this.bundles.Clear();
			this.resourceCache.Clear();
		}
        /// <summary>
        /// 从Bundle中获取某个Prefab，需要实例化
        /// </summary>
        /// <param name="bundleName">不区分大小写</param>
        /// <param name="prefab">区分大小写</param>
        /// <returns></returns>
		public UnityEngine.Object GetAsset(string bundleName, string prefab)
		{
			Dictionary<string, UnityEngine.Object> dict;
			if (!this.resourceCache.TryGetValue(bundleName.BundleNameToLower(), out dict))
			{
				throw new Exception($"not found asset: {bundleName} {prefab}");
			}

			UnityEngine.Object resource = null;
			if (!dict.TryGetValue(prefab, out resource))
			{
				throw new Exception($"not found asset: {bundleName} {prefab}");
			}

			return resource;
		}

		public AssetBundle GetAssetBundle(string abName)
		{
			ABInfo abInfo;
			if (!this.bundles.TryGetValue(abName, out abInfo))
			{
				throw new Exception($"not found bundle: {abName}");
			}
			return abInfo.AssetBundle;
		}

		public void UnloadBundle(string assetBundleName)
		{
			assetBundleName = assetBundleName.ToLower();

			string[] dependencies = AssetBundleHelper.GetSortedDependencies(assetBundleName);

			//Log.Debug($"-----------dep unload {assetBundleName} dep: {dependencies.ToList().ListToString()}");
			foreach (string dependency in dependencies)
			{
				this.UnloadOneBundle(dependency);
			}
		}

		private void UnloadOneBundle(string assetBundleName)
		{
			assetBundleName = assetBundleName.ToLower();

			ABInfo abInfo;
			if (!this.bundles.TryGetValue(assetBundleName, out abInfo))
			{
				throw new Exception($"not found assetBundle: {assetBundleName}");
			}
			
			//Log.Debug($"---------- unload one bundle {assetBundleName} refcount: {abInfo.RefCount - 1}");

			--abInfo.RefCount;
            
			if (abInfo.RefCount > 0)
			{
				return;
			}


			this.bundles.Remove(assetBundleName);
			abInfo.Dispose();
			//Log.Debug($"cache count: {this.cacheDictionary.Count}");
		}

		/// <summary>
		/// 同步加载assetbundle
		/// </summary>
		/// <param name="assetBundleName"></param>
		/// <returns></returns>
		public void LoadBundle(string assetBundleName)
		{
			assetBundleName = assetBundleName.ToLower();
			string[] dependencies = AssetBundleHelper.GetSortedDependencies(assetBundleName);
			//Log.Debug($"-----------dep load {assetBundleName} dep: {dependencies.ToList().ListToString()}");
			foreach (string dependency in dependencies)
			{
				if (string.IsNullOrEmpty(dependency))
				{
					continue;
				}
				this.LoadOneBundle(dependency);
			}
        }

		public void AddResource(string bundleName, string assetName, UnityEngine.Object resource)
		{
			Dictionary<string, UnityEngine.Object> dict;
			if (!this.resourceCache.TryGetValue(bundleName.BundleNameToLower(), out dict))
			{
				dict = new Dictionary<string, UnityEngine.Object>();
				this.resourceCache[bundleName] = dict;
			}

			dict[assetName] = resource;
		}

		public void LoadOneBundle(string assetBundleName)
		{
			//Log.Debug($"---------------load one bundle {assetBundleName}");
			ABInfo abInfo;
			if (this.bundles.TryGetValue(assetBundleName, out abInfo))
			{
				++abInfo.RefCount;
				return;
			}

			if (!Define.IsAsync)
			{
				string[] realPath = null;
#if UNITY_EDITOR
				realPath = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
				foreach (string s in realPath)
				{
					string assetName = Path.GetFileNameWithoutExtension(s);
					UnityEngine.Object resource = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(s);
					AddResource(assetBundleName, assetName, resource);
				}

				this.bundles[assetBundleName] = new ABInfo(assetBundleName, null);
#endif
				return;
			}

			string p = Path.Combine(PathHelper.AppHotfixResPath, assetBundleName);
			AssetBundle assetBundle = null;
			if (File.Exists(p))
			{
				assetBundle = AssetBundle.LoadFromFile(p);
			}
			else
			{
				p = Path.Combine(PathHelper.AppResPath, assetBundleName);
				assetBundle = AssetBundle.LoadFromFile(p);
			}

			if (assetBundle == null)
			{
				throw new Exception($"assets bundle not found: {assetBundleName}");
			}

			if (!assetBundle.isStreamedSceneAssetBundle)
			{
				// 异步load资源到内存cache住
				UnityEngine.Object[] assets = assetBundle.LoadAllAssets();
				foreach (UnityEngine.Object asset in assets)
				{
					AddResource(assetBundleName, asset.name, asset);
				}
			}

			this.bundles[assetBundleName] = new ABInfo(assetBundleName, assetBundle);
		}

		/// <summary>
		/// 异步加载assetbundle
		/// </summary>
		/// <param name="assetBundleName"></param>
		/// <returns></returns>
		public async ETTask LoadBundleAsync(string assetBundleName)
		{
            assetBundleName = assetBundleName.ToLower();
			string[] dependencies = AssetBundleHelper.GetSortedDependencies(assetBundleName);
            // Log.Debug($"-----------dep load {assetBundleName} dep: {dependencies.ToList().ListToString()}");
            foreach (string dependency in dependencies)
			{
				if (string.IsNullOrEmpty(dependency))
				{
					continue;
				}
				await this.LoadOneBundleAsync(dependency);
			}
        }

		public async ETTask LoadOneBundleAsync(string assetBundleName)
		{
			ABInfo abInfo;
			if (this.bundles.TryGetValue(assetBundleName, out abInfo))
			{
				++abInfo.RefCount;
				return;
			}

            //Log.Debug($"---------------load one bundle {assetBundleName}");
            if (!Define.IsAsync)
			{
				string[] realPath = null;
#if UNITY_EDITOR
				realPath = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
				foreach (string s in realPath)
				{
					string assetName = Path.GetFileNameWithoutExtension(s);
					UnityEngine.Object resource = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(s);
					AddResource(assetBundleName, assetName, resource);
				}

				this.bundles[assetBundleName] = new ABInfo(assetBundleName, null);
#endif
				return;
			}

			string p = Path.Combine(PathHelper.AppHotfixResPath, assetBundleName);
			AssetBundle assetBundle = null;
			if (!File.Exists(p))
			{
				p = Path.Combine(PathHelper.AppResPath, assetBundleName);
			}
			
			using (AssetsBundleLoaderAsync assetsBundleLoaderAsync = ComponentFactory.Create<AssetsBundleLoaderAsync>())
			{
				assetBundle = await assetsBundleLoaderAsync.LoadAsync(p);
			}

			if (assetBundle == null)
			{
				throw new Exception($"assets bundle not found: {assetBundleName}");
			}

			if (!assetBundle.isStreamedSceneAssetBundle)
			{
				// 异步load资源到内存cache住
				UnityEngine.Object[] assets;
				using (AssetsLoaderAsync assetsLoaderAsync = ComponentFactory.Create<AssetsLoaderAsync, AssetBundle>(assetBundle))
				{
					assets = await assetsLoaderAsync.LoadAllAssetsAsync();
				}
				foreach (UnityEngine.Object asset in assets)
				{
					AddResource(assetBundleName, asset.name, asset);
				}
			}

			this.bundles[assetBundleName] = new ABInfo(assetBundleName, assetBundle);
		}

		public string DebugString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (ABInfo abInfo in this.bundles.Values)
			{
				sb.Append($"{abInfo.Name}:{abInfo.RefCount}\n");
			}
			return sb.ToString();
		}

        public GameObject NewObj(PrefabType type, GameObject prefab)
        {
            if (!this.m_prefabs.TryGetValue(type, out Queue<GameObject> gameObjects))
            {
                this.m_prefabs.Add(type, new Queue<GameObject>());
            }

            if (this.m_prefabs[type].Count == 0)
            {
                return UnityEngine.Object.Instantiate(prefab);
            }

            GameObject old = this.m_prefabs[type].Dequeue();

            old.SetActive(true);

            return old;
        }

        public void RecycleObj(PrefabType type, GameObject prefab, Action<GameObject> callBack = null)
        {
            if (prefab == null)
                return;

            if (!this.m_prefabs.TryGetValue(type, out Queue<GameObject> gameObjects))
            {
                this.m_prefabs.Add(type,new Queue<GameObject>());
            }

            // 如果需要移除一些组件
            if (callBack != null)
            {
                callBack(prefab);
            }

            prefab.transform.SetParent(this.PoolRoot.transform);

            prefab.SetActive(false);

            this.m_prefabs[type].Enqueue(prefab);


        }
	}
}