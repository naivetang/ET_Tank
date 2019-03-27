using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    public static class ExplosionEffectFactory
    {
        public static GameObject Create(Vector3 pos)
        {
            // 创建爆炸效果

            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();

            resourcesComponent.LoadBundle("Unit.unity3d");

            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset("Unit.unity3d", "Unit");

            resourcesComponent.UnloadBundle("Unit.unity3d");

            GameObject explosion = bundleGameObject.Get<GameObject>("Explosion");

            GameObject explosionGameObject = resourcesComponent.NewObj(PrefabType.BulletBoom, explosion);

            explosionGameObject.SetActive(false);

            explosionGameObject.transform.position = pos;

            explosionGameObject.SetActive(true);

            FairyGUI.Timers.inst.Add(7f, 1, (_) =>
            {
                resourcesComponent.RecycleObj(PrefabType.BulletBoom, explosionGameObject);
            });

            return explosionGameObject;
        }

    }
}
