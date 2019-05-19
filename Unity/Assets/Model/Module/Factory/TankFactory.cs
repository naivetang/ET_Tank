using UnityEngine;

namespace ETModel
{
    public static class TankFactory
    {
        public static Tank Create(TankInfoFirstEnter firstInfo,Vector3 Pos, Vector3 Rot)
        {
            long id = firstInfo.TankFrameInfo.TankId;

            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle($"Unit.unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset("Unit.unity3d", "Unit");
            Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"Unit.unity3d");
            GameObject prefab = bundleGameObject.Get<GameObject>("Tank");

            TankComponent tankComponent = Game.Scene.GetComponent<TankComponent>();

            Tank tank = ComponentFactory.CreateWithId<Tank>(id);
            tank.GameObject = resourcesComponent.NewObj(PrefabType.Tank, prefab);

            tank.GameObject.transform.position = Pos;

            tank.GameObject.transform.eulerAngles = Rot;


            GameObject parent = GameObject.Find($"/Global/Unit");
            tank.GameObject.transform.SetParent(parent.transform, false);

            NumericComponent numericComponent = tank.AddComponent<NumericComponent>();

            tank.Name = firstInfo.Name;

            tank.TankCamp = firstInfo.TankCamp;  

            if (id == 10000 || PlayerComponent.Instance.MyPlayer.TankId == id)
            {
                tank.TankType = TankType.Local;

                tank.AddComponent<TankMoveComponent>();

                tank.AddComponent<CameraComponent>();

                tank.AddComponent<TurretComponent>();

                // 子弹管理组件
                tank.AddComponent<BulletComponent>();

                // 发射子弹的组件
                tank.AddComponent<TankShootComponent>();

                tank.AddComponent<LocalTankComponent>();

                tankComponent.MyTank = tank;

                // 如果是自己设置层级为9，为了让坦克不打中自己

                Utility.ChangeLayer(tank.GameObject, LayerNames.OwnTank);

                tank.GameObject.layer = 9;
            }
            else
            {
                tank.TankType = TankType.Remote;

                tank.AddComponent<RemoteTankComponent>();

                tank.AddComponent<TurretComponent>();

                tank.AddComponent<TankShootComponent>();

                tank.AddComponent<BulletComponent>();

                tank.AddComponent<OverHeadComponent>();
            }

            

            tankComponent.Add(tank);

            // 先将坦克加入TankComponent再赋值，因为赋值的时候会触发事件，事件中可能要从TankComponent中取坦克
            numericComponent[NumericType.MaxHpBase] = firstInfo.MaxHpBase;

            numericComponent[NumericType.HpBase] = firstInfo.HpBase;

            numericComponent[NumericType.AtkBase] = firstInfo.AtkBase;

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