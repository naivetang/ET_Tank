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

            resourcesComponent.LoadBundle(AssetBundleName.Unit);

            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset(AssetBundleName.Unit, PrefabName.Unit);

            resourcesComponent.UnloadBundle(AssetBundleName.Unit);

            GameObject explosion = bundleGameObject.Get<GameObject>(PrefabName.BulletBoomEffect);

            GameObject explosionGameObject = resourcesComponent.NewObj(PrefabType.BulletBoom, explosion);

            explosionGameObject.SetActive(false);

            explosionGameObject.transform.position = pos;
            

            // 创建爆炸音效

            AudioClip audio = bundleGameObject.Get<AudioClip>(PrefabName.BulletBoomAudio);

            AudioSource audioSource = explosionGameObject.GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = explosionGameObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1;
                audioSource.loop = false;
                audioSource.clip = audio;
                audioSource.playOnAwake = true;
            }


            explosionGameObject.SetActive(true);



            FairyGUI.Timers.inst.Add(7f, 1, (_) => { resourcesComponent.RecycleObj(PrefabType.BulletBoom, explosionGameObject); });

            return explosionGameObject;
        }

    }
}
