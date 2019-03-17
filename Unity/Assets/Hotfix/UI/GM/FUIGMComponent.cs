using System;
using ETModel;

namespace ETHotfix
{
    public class FUIGMComponent : Component
    {
        public FUI Input;

        public FUI Execute;
    }

    [ObjectSystem]
    public class FUIGMComponentSystem : AwakeSystem<FUIGMComponent>
    {
        public override void Awake(FUIGMComponent self)
        {
            FUI FGUICompunt = self.GetParent<FUI>();

            self.Input = FGUICompunt.Get("Input");

            self.Execute = FGUICompunt.Get("Execute");

            self.Execute.GObject.asButton.onClick.Set(() => { ExecuteOnClick(self); });
        }

        public static void ExecuteOnClick(FUIGMComponent self)
        {
            string cmd =  self.Input.GObject.asCom.GetChild("n1").asTextInput.text;

            long a = Convert.ToInt64(cmd);

            Log.Warning("cmd = " + cmd);

            string[] cmds = cmd.Split(' ');

            Log.Info("长度 = " + cmds.Length);

            if (cmds.Length == 1)
            {
                Game.EventSystem.Run(cmds[0]);
            }
            else if(cmds.Length == 2)
            {
                Log.Info("正确进入");
                Log.Warning("cmd = " + cmds[0]);
                Log.Warning("参数 = " + cmds[1]);

                Game.EventSystem.Run(cmds[0],(object)cmds[1]);
            }
            else if (cmds.Length == 3)
            {
                Game.EventSystem.Run(cmds[0], (object)cmds[1], (object)cmds[2]);
            }
            else if (cmds.Length == 4)
            {
                Game.EventSystem.Run(cmds[0], (object)cmds[1], (object)cmds[2], (object)cmds[3]);
            }

            Game.Scene.GetComponent<FUIComponent>().Remove(FUIType.GM);
        }

    }
}
