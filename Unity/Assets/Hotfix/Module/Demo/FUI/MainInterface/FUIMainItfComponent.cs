using ETModel;
using FairyGUI;
using UnityEngine;

namespace ETHotfix
{
    public class FUIMainItfComponent : Component
    {
        public FUI m_point;

        public FUI m_BoomPoint;

        public FUI m_HP;

        public void UpdateBoomPoint( float x,float y)
        {
            m_BoomPoint.GObject.SetXY(x, y);
        }

        public void HpChange(float max,float current, float changeVal)
        {
            GProgressBar pb = this.m_HP.GObject.asProgress;

            pb.max = max;

            pb.value = current;
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

            self.m_HP = FGUICompunt.Get("hp");

            TurretComponent.UpdatePos += self.UpdateBoomPoint;

            Tank.m_hpChange += self.HpChange;
        }
        
    }

}
