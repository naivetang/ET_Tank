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

        private class EmitTip : IPoolAllocatedObject<EmitTip>
        {
            public GComponent m_comp;

            private PopMessageType m_type;

            public EmitTip()
            {
                this.m_comp = UIPackage.CreateObject(FUIType.PopMessage, FUIType.PopMessage).asCom;
            }
            public void Init(PopMessageType type)
            {
                this.m_comp.touchable = false;

                this.m_type = type;
            }
            public Transition GetAnim()
            {
                if (this.m_comp == null)
                {
                    return null;
                }

                Transition anim = null;

                anim = this.m_comp.GetTransition(this.m_type == PopMessageType.Float? "t0" : "t1");

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

            public void InitPool(ObjectPool<EmitTip> pool)
            {
                
            }

            public EmitTip Downcast()
            {
                return this;
            }
        }

        private GGroup m_TipGrp;

        private ObjectPool<EmitTip> m_EmitTipPool = new ObjectPool<EmitTip>();

        private List<EmitTip> m_EmitTipItems = new List<EmitTip>();


        public void Awake()
        {
            this.FUIComponent = this.GetParent<FUI>();
            this.StartFUI();
        }

        protected override void StartFUI()
        {
            m_TipGrp = this.FUIComponent.Get("n6").GObject.asGroup;

            m_TipGrp.visible = false;
        }

        public void AddEmitTip(string text,PopMessageType type)
        {
            EmitTip item = m_EmitTipPool.Alloc(NewEmitTip);

            item.Init(type);

            m_EmitTipItems.Add(item);

            this.FUIComponent.GObject.asCom.AddChild(item.m_comp);

            GRichTextField textContent = item.m_comp.GetChild("n7").asRichTextField;

            textContent.text = text;

            SetTextContent(item.m_comp);

            Transition anim = item.GetAnim();

            if (anim != null)
            {
                anim.Play();
            }
        }

        private void SetTextContent(GComponent comp)
        {
            GImage TextBkg1 = comp.GetChild("n3").asImage;

            GImage TextBkg2 = comp.GetChild("n4").asImage;

            GRichTextField TextContent = comp.GetChild("n7").asRichTextField;

            TextBkg1.width = TextContent.textWidth + 50;

            TextBkg2.width = TextBkg1.width;

            TextContent.x = GRoot.inst.width / 2 - TextContent.textWidth / 2;

            TextBkg1.x = GRoot.inst.width / 2 - TextBkg1.width / 2;

            TextBkg2.x = GRoot.inst.width / 2 - TextBkg2.width / 2;
        }

        EmitTip NewEmitTip()
        {
            return new EmitTip();
        }

        public void Update()
        {
            for (int i = 0; i < m_EmitTipItems.Count; i++)
            {
                if (m_EmitTipItems[i].IsFinish())
                {
                    EmitTip tipitem = m_EmitTipItems[i];

                    this.FUIComponent.GObject.asCom.RemoveChild(tipitem.m_comp);

                    m_EmitTipItems.RemoveAt(i);

                    m_EmitTipPool.Recycle(tipitem);

                    --i;
                }
            }
        }
    }
}
