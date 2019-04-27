using System.Collections.Generic;
using ETModel;
using FairyGUI;
using Google.Protobuf.Collections;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class GameSettingAwakeSystem: AwakeSystem<GameSettingsViewComponent>
    {
        public override void Awake(GameSettingsViewComponent self)
        {
            self.Awake();
        }
    }

    public class GameSettingsViewComponent : FUIBase
    {
        public static G2C_SettingInfo Data { get; set; } = null;

        private GTextField m_name;

        private GTextField m_dbID;

        private GComboBox m_language;

        private GButton m_sound;

        private GSlider m_volume;

        private GButton m_showName;

        private GButton m_showHP;

        private GSlider m_rotSpeed;

        private GButton m_determine;

        private List<string> m_lanageList = new List<string>{"中文","English"};



        public void Awake()
        {
            this.FUIComponent = this.GetParent<FUI>();

            this.StartFUI();
        }

        protected override void StartFUI()
        {
            this.m_name = this.FUIComponent.Get("n5").GObject.asTextField;
            this.m_dbID = this.FUIComponent.Get("n6").GObject.asTextField;
            this.m_language = this.FUIComponent.Get("n13").GObject.asComboBox;
            this.m_sound = this.FUIComponent.Get("n9").GObject.asButton;
            this.m_volume = this.FUIComponent.Get("n10").GObject.asSlider;
            this.m_showName = this.FUIComponent.Get("n22").GObject.asButton;
            this.m_showHP = this.FUIComponent.Get("n23").GObject.asButton;
            this.m_rotSpeed = this.FUIComponent.Get("n21").GObject.asSlider;
            this.m_determine = this.FUIComponent.Get("n17").GObject.asButton;

            m_determine.onClick.Set(DetermineBtn_OnClick);

            this.m_language.items = this.m_lanageList.ToArray();

            this.m_volume.max = 100;

            this.m_rotSpeed.max = 100;

            this.UI();
        }

        private void UI()
        {
            

            this.m_language.selectedIndex = GetLanguage() == Language.Chinese ? 0 : 1;

            this.m_name.text = PlayerComponent.Instance.MyPlayer.Name;

            this.m_dbID.text = $"游戏ID：{PlayerComponent.Instance.MyPlayer.DbID}";

            this.m_sound.selected = Data.Volume > 0;

            this.m_volume.value = Data.Volume;

            this.m_showName.selected = (Data.BinarySwitch & 1) > 0;

            this.m_showHP.selected = (Data.BinarySwitch & 2) > 0;

            this.m_rotSpeed.value = Data.RotSpeed;
        }

        private void DetermineBtn_OnClick()
        {
            this.Send_C2G_SettingInfo();

            this.OnClose();
        }

        private void Send_C2G_SettingInfo()
        {
            C2G_SettingInfo msg = new C2G_SettingInfo();

            msg.Language = this.m_language.selectedIndex == 0? Language.Chinese : Language.English;

            if (Data.Language != msg.Language)
            {
                Data.Language = msg.Language;

                PlayerPrefs.SetInt(PlayerPrefsKey.Language, (int)Data.Language);

                Game.EventSystem.Run(EventIdType.LanguageChange);
            }


            Data.Volume = msg.Volume = (int)this.m_volume.value;

            Data.BinarySwitch = msg.BinarySwitch = this.GetBinarySwitchVal();

            Data.RotSpeed = msg.RotSpeed = (int)this.m_rotSpeed.value;

            ETModel.SessionComponent.Instance.Session.Send(msg);
        }

        private int GetBinarySwitchVal()
        {
            int val = 0;

            val += (this.m_showName.selected? 1 : 0) * (int)Mathf.Pow(2, 0);

            val += (this.m_showHP.selected? 1 : 0) * (int)Mathf.Pow(2,1);

            return val;
        }

        public static Language GetLanguage()
        {
            if (Data == null)
            {
                if (PlayerPrefs.HasKey(PlayerPrefsKey.Language))
                {
                    return (Language)PlayerPrefs.GetInt(PlayerPrefsKey.Language);
                }
                else
                {
                    return Language.Chinese;
                }
            }
            else
            {
                return Data.Language;
            }
        }
    }
}
