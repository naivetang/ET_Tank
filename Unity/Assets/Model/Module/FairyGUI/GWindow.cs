using System;
using FairyGUI;

namespace ETModel
{
    public class GWindow: Window
    {
        private Action doShowAnimationEvent;
        
        public event Action DoShowAnimationEvent
        {
            add
            {
                this.doShowAnimationEvent += value;
            }
            remove
            {
                this.doShowAnimationEvent -= value;
            }
        }
        
        protected override void DoShowAnimation()
        {
            base.DoShowAnimation();
            this.doShowAnimationEvent?.Invoke();
        }
        
        private Action doHideAnimationEvent;
        
        public event Action DoHideAnimationEvent
        {
            add
            {
                this.doHideAnimationEvent += value;
            }
            remove
            {
                this.doHideAnimationEvent -= value;
            }
        }
        
        protected override void DoHideAnimation()
        {
            base.DoHideAnimation();
            this.doHideAnimationEvent?.Invoke();
        }
        
        private Action onShownEvent;
        
        public event Action OnShownEvent
        {
            add
            {
                this.onShownEvent += value;
            }
            remove
            {
                this.onShownEvent -= value;
            }
        }
        
        protected override void OnShown()
        {
            base.OnShown();
            this.onShownEvent?.Invoke();
        }
        
        private Action onHideEvent;
        
        public event Action OnHideEvent
        {
            add
            {
                this.onHideEvent += value;
            }
            remove
            {
                this.onHideEvent -= value;
            }
        }
        
        protected override void OnHide()
        {
            base.OnHide();
            this.onHideEvent?.Invoke();
        }
    }
}