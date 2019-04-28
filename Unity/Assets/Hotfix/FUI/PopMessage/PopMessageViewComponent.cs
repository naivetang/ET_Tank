using System;
using System.Collections.Generic;
using FairyGUI;
using ETModel;

namespace ETHotfix
{

    [ObjectSystem]
    internal class PopMessageAwakeSystem:AwakeSystem<PopMessageViewComponent>
    {
        public override void Awake(PopMessageViewComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    internal class PopMessageUpdateSystem : UpdateSystem<PopMessageViewComponent>
    {
        public override void Update(PopMessageViewComponent self)
        {
            self.Update();
        }
    }


    internal class PopMessageViewComponent : FUIBase
    {

        private class EmitTip
        {
            public GComponent m_Comp;
            public EmitTip()
            {
                m_Comp = FUIFactory.Create<PopMessageViewComponent>(FUIType.PopMessage).Result.GObject.asCom;
            }
            public void Init()
            {
                m_Comp.touchable = false;
            }
            public Transition GetAnim()
            {
                if (m_Comp == null)
                {
                    return null;
                }
                Transition anim = m_Comp.GetTransition("t0");
                return anim;
            }
            public bool IsFinish()
            {
                Transition anim = GetAnim();
                if (anim == null)
                {
                    return false;
                }
                return !anim.playing;
            }
            public EmitTip Downcast()
            {
                return this;
            }
        }

        private GGroup m_TipGrp;


        private Queue<EmitTip> m_emitTipItems;


        public void Awake()
        {
            this.FUIComponent = this.GetParent<FUI>();
            this.StartFUI();
        }

        protected override void StartFUI()
        {
            m_TipGrp = this.FUIComponent.Get("n3").GObject.asGroup;

            m_TipGrp.visible = false;
        }

        public void Update()
        {

        }
    }
}
