using UnityEngine;

namespace ETModel
{
    public static class TankFactory
    {
        public static Tank Create(long id)
        {

            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle($"Unit.unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset("Unit.unity3d", "Unit");
            Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"Unit.unity3d");
            GameObject prefab = bundleGameObject.Get<GameObject>("Tank");

            TankComponent tankComponent = Game.Scene.GetComponent<TankComponent>();

            Tank tank = ComponentFactory.CreateWithId<Tank>(id);
            tank.GameObject = resourcesComponent.NewObj(PrefabType.Tank, prefab);
            GameObject parent = GameObject.Find($"/Global/Unit");
            tank.GameObject.transform.SetParent(parent.transform, false);

            

            if (id == 10000 || PlayerComponent.Instance.MyPlayer.TankId == id)
            {
                tank.m_tankType = TankType.Local;

                tank.AddComponent<TankMoveComponent>();

                tank.AddComponent<CameraComponent>();

                tank.AddComponent<TurretComponent>();

                // 子弹管理组件
                tank.AddComponent<BulletComponent>();

                // 发射子弹的组件
                tank.AddComponent<TankShootComponent>();

                tankComponent.MyTank = tank;

                tank.GameObject.layer = 9;
            }
            else
            {
                tank.m_tankType = TankType.Remote;

                tank.AddComponent<RemoteTankComponent>();
            }



            tankComponent.Add(tank);
            return tank;
        }

        public static GameObject CreateTankBoomEffect(Tank tank)
        {
            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();

            resourcesComponent.LoadBundle(AssetBundleName.Unit);

            GameObject unit = (GameObject)resourcesComponent.GetAsset(AssetBundleName.Unit, PrefabName.Unit);

            resourcesComponent.UnloadBundle(AssetBundleName.Unit);

            GameObject boomPrefab = unit.Get<GameObject>(PrefabName.TankBoomEffect);

            UnityEngine.GameObject boomEffect = resourcesComponent.NewObj(PrefabType.TankBoom, boomPrefab);

            boomEffect.transform.SetParent(tank.GameObject.FindComponentInChildren<Transform>("BoomEffect"), false);

            boomEffect.transform.localPosition = Vector3.zero;

            boomEffect.transform.localScale = Vector3.one * 10;

            return boomEffect;
        }
    }

    
}