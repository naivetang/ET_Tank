using System;
using UnityEngine;

namespace ETModel
{
	public static class GameObjectHelper
	{
		public static T Get<T>(this GameObject gameObject, string key) where T : class
		{
			try
			{
				return gameObject.GetComponent<ReferenceCollector>().Get<T>(key);
			}
			catch (Exception e)
			{
				throw new Exception($"获取{gameObject.name}的ReferenceCollector key失败, key: {key}", e);
			}
		}

        public static GameObject FindChildObject(this GameObject fromGameObject, string withName)
        {
            Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < ts.Length; ++i)
            {
                Transform t = ts[i];
                if (t.gameObject.name == withName)
                {
                    return t.gameObject;
                }
            }
            return null;
        }

        public static GameObject FindChildObjectByPath(this GameObject gameObject, string name)
        {
            if (name.IndexOf('/') == -1)
            {
                Transform child = gameObject.transform.Find(name);
                if (null == child)
                {
                    return null;
                }
                return child.gameObject;
            }
            else
            {
                string[] path = name.Split('/');
                Transform child = gameObject.transform;
                for (int i = 0; i < path.Length; i++)
                {
                    child = child.Find(path[i]);
                    if (null == child)
                    {
                        return null;
                    }
                }
                return child.gameObject;
            }
        }

        public static T FindComponentInChildren<T>(this GameObject gameObject, string name)
        {
            if (name.IndexOf('/') == -1)
            {
                Transform child = gameObject.transform.Find(name);
                if (child == null)
                    return default(T);
                return child.GetComponent<T>();
            }
            else
            {
                string[] path = name.Split('/');
                Transform child = gameObject.transform;
                for (int i = 0; i < path.Length; i++)
                {
                    child = child.Find(path[i]);
                    if (child == null)
                    {
                        return default(T);
                    }
                }
                return child.GetComponent<T>();
            }
        }
    }
}