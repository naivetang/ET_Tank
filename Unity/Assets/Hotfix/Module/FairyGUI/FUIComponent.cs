using System;
using System.Collections.Generic;
using ETModel;
using FairyGUI;

namespace ETHotfix
{
    public enum ViewGroup : int
    {
        Background, //登录选角色界面的背景，单独一个canvas
        Home,       //主界面
        View,       //正常界面
        Dialog,     //弹出式对话框
        Tip,        //Tip
        Loading,
        DebugInfo,
        //UpperFGUI,   //临时添加为FGUI的出现用，当没有UGUI的UI时，它也可以不再存在。
        //当有需求如FGUI上的UGUI 物品tips，此时把tips动态移到UpperFGUI层里，以实现UI高于FGUI，当然也可以调回去。
        None,
    }
    [ObjectSystem]
	public class FUIComponentAwakeSystem : AwakeSystem<FUIComponent>
	{
		public override void Awake(FUIComponent self)
		{
			self.Awake();
		}
	}

	/// <summary>
	/// 管理所有顶层UI, 顶层UI都是GRoot的孩子
	/// </summary>
	public class FUIComponent: Component
	{
		public FUI Root;

        private static FUI[] m_FGUIGrps = new FUI[(int)ViewGroup.None];
        private static Dictionary<string, int> s_ViewDepthMap = new Dictionary<string, int>();
        private static int s_cDefaultViewDepth = 50;
        private static List<string> s_UIChangeSceneNotClose = new List<string>();

        public void Awake()
        {
            
            this.InitFairyGUI();

        }

        private void InitFairyGUI()
        {
            this.Root = ComponentFactory.Create<FUI, GObject>(GRoot.inst);

            for (int i = 0; i < (int)ViewGroup.None; i++)
            {
                m_FGUIGrps[i] = CreateLayer(i);
            }

            this.InitViewDepth();

            this.InitUIChangeSceneNotClose();
        }

        private FUI CreateLayer(int layer)
        {
            GComponent com = UIPackage.CreateObject("RootLayer", "RootLayer").asCom;

            com.gameObjectName = ((ViewGroup)layer).ToString();

            com.SetSize(GRoot.inst.width, GRoot.inst.height);

            com.AddRelation(GRoot.inst, RelationType.Size);

            FUI fui = ComponentFactory.Create<FUI, GObject>(com);



            fui.Name = ((ViewGroup)layer).ToString();

            this.Root.Add(fui,layer);

            return fui;
        }

        private void InitViewDepth()
        {
            RegViewDepth(FUIType.Loading, 100);
            RegViewDepth(FUIType.MainInterface, 99);
        }
        private static void RegViewDepth(string viewName, int depth)
        {
            if (s_ViewDepthMap.ContainsKey(viewName))
            {
                return;
            }
            s_ViewDepthMap[viewName] = depth;
        }

        private int GetViewDepth(string strUI)
        {
            int nDepth = s_cDefaultViewDepth;

            if (s_ViewDepthMap.TryGetValue(strUI, out nDepth))
            {
                return nDepth;
            }
            return s_cDefaultViewDepth;
        }

       

        public void AddToGroup(FUI fui, ViewGroup grp)
        {
            if (!(fui.GObject is GComponent com))
            {
                throw new Exception($"this ui is not GComponent,  {fui.Name}");
            }
            int newComDepth;
            if (com.packageItem != null)
                newComDepth = GetViewDepth(com.packageItem.name);
            else
                newComDepth = s_cDefaultViewDepth;
            if (grp != ViewGroup.None && m_FGUIGrps[(int)grp] != null)
            {
                //GComponent viewGrp = m_FGUIGrps[(int)grp];
                FUI viewGrp = m_FGUIGrps[(int)ViewGroup.View];
                int iIndex = viewGrp.numChildren;
                for (int i = 0; i < viewGrp.numChildren; i++)
                {

                    GObject child = viewGrp.GObject.asCom.GetChildAt(i);
                    if (child == null)
                    {
                        continue;
                    }
                    int childDepth;
                    if (child.packageItem == null)
                        childDepth = s_cDefaultViewDepth;
                    else
                        childDepth = GetViewDepth(child.packageItem.name);
                    if (childDepth > newComDepth)
                    {
                        iIndex = i;
                        break;
                    }
                }
                viewGrp.Add(fui,iIndex);
            }

        }

        private void InitUIChangeSceneNotClose()
        {
            RegUIChangeSceneNotClose(FUIType.Loading);
        }

        private static void RegUIChangeSceneNotClose(string comName)
        {
            if (s_UIChangeSceneNotClose.Contains(comName))
            {
                return;
            }

            s_UIChangeSceneNotClose.Add(comName);
        }

        private bool IsUIChangeSceneNotClose(string comName)
        {
            return s_UIChangeSceneNotClose.Contains(comName);
        }

        public void CloseUIWhenChangeScene()
        {
            List<string> removeList = new List<string>();
            foreach (FUI layer in this.Root.children.Values)
            {
                removeList.Clear();
                foreach (FUI fui in layer.children.Values)
                {
                    if (!IsUIChangeSceneNotClose(fui.Name))
                    {
                        removeList.Add(fui.Name);
                    }
                }

                foreach (string s in removeList)
                {
                    layer.Remove(s);
                }
            }
        }

        public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();
			
			this.Root.RemoveChildren();
		}

        public void Add(FUI ui, ViewGroup grp = ViewGroup.View)
		{
            grp = ViewGroup.View;

            AddToGroup(ui, grp);

        }
		
		public void Remove(string name)
		{
            foreach (FUI layer in this.Root.children.Values)
            {
                if(layer.Get(name) != null)
                    layer.Remove(name);
            }

		}
		
		public FUI Get(string name)
		{
            foreach (FUI layer in this.Root.children.Values)
            {
                FUI fui = layer.Get(name);
                if (fui != null)
                    return fui;
            }

            return null;
        }

	}
}