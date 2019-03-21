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
            tank.GameObject = UnityEngine.Object.Instantiate(prefab);
            GameObject parent = GameObject.Find($"/Global/Unit");
            tank.GameObject.transform.SetParent(parent.transform, false);

            tank.AddComponent<TankMoveComponent>();
            tank.AddComponent<CameraComponent>();


            tankComponent.Add(tank);
            return tank;
        }
    }
}