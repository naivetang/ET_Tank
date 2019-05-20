using System.Collections.Generic;
using ETModel;
using FairyGUI;
using Google.Protobuf.Collections;

namespace ETHotfix
{
    [ObjectSystem]
    public class LoadingAwakeComponent : AwakeSystem<LoadingViewComponent>
    {
        public override void Awake(LoadingViewComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class LoadingUpdateComponent : UpdateSystem<LoadingViewComponent>
    {
        public override void Update(LoadingViewComponent self)
        {
            self.Update();
        }
    }

    public class LoadingViewComponent : FUIBase
    {

        private GProgressBar m_bar;

        private SceneChangeComponent SceneChangeComponent;

        private float m_limitTime = 2f;

        private long m_time;

        private float m_process = 0f;

        public bool CanClose { get; set; }

        public void Awake()
        {
            this.FUIComponent = this.GetParent<FUI>();

            SceneChangeComponent = Game.Scene.GetComponent<SceneChangeComponent>();

            m_time = TimeHelper.NowSecond();

            CanClose = false;

            this.StartFUI();
        }

        

        protected override void StartFUI()
        {
            this.m_bar = this.FUIComponent.Get("n3").GObject.asProgress;

            this.m_bar.max = 100f;

            this.m_bar.value = 0f;

           this.Lanaguage();
        }

        private void Lanaguage()
        {
            this.FUIComponent.Get("n5").GObject.asTextField.text = Message.Get(1025);
        }

        public void Update()
        {
            if(this.SceneChangeComponent != null)
                m_process = SceneChangeComponent.Process;
            else
            {
                m_process = 100f;
            }

            int p = (int)((TimeHelper.NowSecond() - this.m_time) / this.m_limitTime * 100);

            m_process = m_process < p ? this.m_process : p;

            this.UI();
        }

        private void UI()
        {
            if(this.m_process - this.m_bar.value > 1)
            {
                this.m_bar.value += 1;
            }
            else
            {
                this.m_bar.value = this.m_process;
            }

            

            if (CanClose && (this.m_bar.max - this.m_bar.value) <= 0.1f)
            {
                this.OnClose();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
