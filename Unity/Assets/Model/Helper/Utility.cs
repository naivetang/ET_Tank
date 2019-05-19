
using System;
using UnityEngine;

namespace ETModel
{
    public static class Utility
    {
        public static void ChangeLayer(GameObject go, string targetLayer)
        {
            if(LayerMask.NameToLayer(targetLayer) == -1)
            {
                Log.Error("Layer中不存在,请手动添加LayerName");

                return;
            }
            //遍历更改所有子物体layer
            go.layer = LayerMask.NameToLayer(targetLayer);
            foreach (Transform child in go.transform)
            {
                ChangeLayer(child.gameObject, targetLayer);
            }
        }

        public static string Int2TwoChar(this int num)
        {
            if(num < 0)
                return String.Empty;

            if (num > 9)
                return num.ToString();

            else
            {
                return $"0{num}";
            }
        }

    }
}
