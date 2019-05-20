using UnityEngine;

namespace ETModel
{
    public static class BulletFactory
    {
        public static Bullet Create(Tank tank)
        {
            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();

            //Game.Scene.GetComponent<ResourcesComponent>().LoadBundle($"Unit.unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset(AssetBundleName.Unit, PrefabName.Unit);
            //Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"Unit.unity3d");
            GameObject prefab = bundleGameObject.Get<GameObject>(PrefabName.Bullet);

            BulletComponent bulletComponent = tank.GetComponent<BulletComponent>();


            Bullet bullet = ComponentFactory.CreateWithId<Bullet, Tank>(IdGenerater.GenerateId(), tank);



            bullet.GameObject = resourcesComponent.NewObj(PrefabType.Bullet, prefab);

            GameObject parent = tank.GameObject.FindChildObjectByPath("bullets");

            bullet.GameObject.transform.SetParent(parent.transform,false);

            bulletComponent.Add(bullet);

            //BulletCollision bulletCollision = bullet.GameObject.AddComponent<BulletCollision>();

            // 子弹添加飞行
            //bulletCollision.BulletFly = bullet.AddComponent<BulletFlyComponent>();

            bullet.AddComponent<BulletFlyComponent>();


            return bullet;
            
        }
    }
}