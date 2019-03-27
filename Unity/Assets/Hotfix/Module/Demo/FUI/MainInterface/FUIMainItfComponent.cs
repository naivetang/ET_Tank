using ETModel;
using FairyGUI;
using UnityEngine;

namespace ETHotfix
{
    public class FUIMainItfComponent : Component
    {
        public FUI m_point;

        public FUI m_BoomPoint;

        public void UpdateBoomPoint( float x,float y)
        {
            m_BoomPoint.GObject.SetXY(x, y);
        }
    }

    

    [ObjectSystem]
    public class MainIntAwakeSystem: AwakeSystem<FUIMainItfComponent>
    {
        public override void Awake(FUIMainItfComponent self)
        {
            FUI FGUICompunt = self.GetParent<FUI>();

            self.m_point = FGUICompunt.Get("point");

            self.m_BoomPoint = FGUICompunt.Get("BoomPoint");

            TurretComponent.UpdatePos += self.UpdateBoomPoint;
        }
        
    }

}
