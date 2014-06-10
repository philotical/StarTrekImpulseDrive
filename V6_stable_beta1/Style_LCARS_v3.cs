using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{
    public class StyleLib_LCARS_v3
    {
        private GUIStyle _toggle_style_AccelerationLock;
        private GUIStyle _toggle_style_fullHalt;
        private GUIStyle _toggle_style_MakeSlowToSave;
        private GUIStyle _toggle_style_YouCanSaveNow;
        private GUIStyle _toggle_style_EndSaveMode;
        private GUIStyle _toggle_style_FullImpulse;
        private GUIStyle _toggle_style_formationMode;
        private GUIStyle _toggle_style_IgnoreFormation;
        private GUIStyle _toggle_style_pilotmode;
        private GUIStyle _toggle_style_UseReserves;
        private GUIStyle _toggle_style_HoldSpeed;
        private GUIStyle _toggle_style_HoldHeight;

        private GUIStyle _iconStyle;
        private GUIStyle _button_style_fullHalt;
        private GUIStyle _label_style_dark;
        private GUIStyle _label_style_light;
        private GUIStyle _toggle_style_dark;
        private GUIStyle _paddingTopStyle;
        private GUIStyle _BackGroundLayoutStyle;

        public GUIStyle toggle_style_AccelerationLock { get { return _toggle_style_AccelerationLock; } }
        public GUIStyle toggle_style_fullHalt { get { return _toggle_style_fullHalt; } }
        public GUIStyle toggle_style_MakeSlowToSave { get { return _toggle_style_MakeSlowToSave; } }
        public GUIStyle toggle_style_YouCanSaveNow { get { return _toggle_style_YouCanSaveNow; } }
        public GUIStyle toggle_style_EndSaveMode { get { return _toggle_style_EndSaveMode; } }
        public GUIStyle toggle_style_FullImpulse { get { return _toggle_style_FullImpulse; } }
        public GUIStyle toggle_style_formationMode { get { return _toggle_style_formationMode; } }
        public GUIStyle toggle_style_IgnoreFormation { get { return _toggle_style_IgnoreFormation; } }
        
        public GUIStyle toggle_style_pilotmode { get { return _toggle_style_pilotmode; } }
        public GUIStyle toggle_style_UseReserves { get { return _toggle_style_UseReserves; } }
        public GUIStyle toggle_style_HoldSpeed { get { return _toggle_style_HoldSpeed; } }
        public GUIStyle toggle_style_HoldHeight { get { return _toggle_style_HoldHeight; } }

        public GUIStyle iconStyle { get { return _iconStyle; } }
        public GUIStyle button_style_fullHalt { get { return _button_style_fullHalt; } }
        public GUIStyle label_style_dark { get { return _label_style_dark; } }
        public GUIStyle label_style_light { get { return _label_style_light; } }
        public GUIStyle toggle_style_dark { get { return _toggle_style_dark; } }
        public GUIStyle paddingTopStyle { get { return _paddingTopStyle; } }
        public GUIStyle BackGroundLayoutStyle { get { return _BackGroundLayoutStyle; } }


        private string _LCARSToggle_pilotmode_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_pilotmode";
        private string _LCARSToggle_pilotmode_icon_active = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_pilotmode_active";
        public string LCARSToggle_pilotmode_icon { get { return _LCARSToggle_pilotmode_icon; } }
        public string LCARSToggle_pilotmode_icon_active { get { return _LCARSToggle_pilotmode_icon_active; } }

        private string _LCARSToggle_MakeSlowToSave_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_MakeSlowToSave";
        private string _LCARSToggle_MakeSlowToSave_icon_active = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_MakeSlowToSave_active";
        public string LCARSToggle_MakeSlowToSave_icon { get { return _LCARSToggle_MakeSlowToSave_icon; } }
        public string LCARSToggle_MakeSlowToSave_icon_active { get { return _LCARSToggle_MakeSlowToSave_icon_active; } }

        private string _LCARSToggle_YouCanSaveNow_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_YouCanSaveNow";
        private string _LCARSToggle_YouCanSaveNow_icon_active = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_YouCanSaveNow_active";
        public string LCARSToggle_YouCanSaveNow_icon { get { return _LCARSToggle_YouCanSaveNow_icon; } }
        public string LCARSToggle_YouCanSaveNow_icon_active { get { return _LCARSToggle_YouCanSaveNow_icon_active; } }

        private string _LCARSToggle_EndSaveMode_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_EndSaveMode";
        public string LCARSToggle_EndSaveMode_icon { get { return _LCARSToggle_EndSaveMode_icon; } }
        
        private string _LCARSToggle_formation_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_formation";
        private string _LCARSToggle_formation_icon_active = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_formation_active";
        public string LCARSToggle_formation_icon { get { return _LCARSToggle_formation_icon; } }
        public string LCARSToggle_formation_icon_active { get { return _LCARSToggle_formation_icon_active; } }

        private string _LCARSToggle_IgnoreFormation_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_IgnoreFormation";
        private string _LCARSToggle_IgnoreFormation_icon_active = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_IgnoreFormation_active";
        public string LCARSToggle_IgnoreFormation_icon { get { return _LCARSToggle_IgnoreFormation_icon; } }
        public string LCARSToggle_IgnoreFormation_icon_active { get { return _LCARSToggle_IgnoreFormation_icon_active; } }

        private string _LCARSToggle_UseReserves_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_UseReserves";
        private string _LCARSToggle_UseReserves_icon_active = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_UseReserves_active";
        public string LCARSToggle_UseReserves_icon { get { return _LCARSToggle_UseReserves_icon; } }
        public string LCARSToggle_UseReserves_icon_active { get { return _LCARSToggle_UseReserves_icon_active; } }

        private string _LCARSToggle_HoldSpeed_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_HoldSpeed";
        private string _LCARSToggle_HoldSpeed_icon_active = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_HoldSpeed_active";
        public string LCARSToggle_HoldSpeed_icon { get { return _LCARSToggle_HoldSpeed_icon; } }
        public string LCARSToggle_HoldSpeed_icon_active { get { return _LCARSToggle_HoldSpeed_icon_active; } }

        private string _LCARSToggle_HoldHeight_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_HoldHeight";
        private string _LCARSToggle_HoldHeight_icon_active = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_HoldHeight_active";
        public string LCARSToggle_HoldHeight_icon { get { return _LCARSToggle_HoldHeight_icon; } }
        public string LCARSToggle_HoldHeight_icon_active { get { return _LCARSToggle_HoldHeight_icon_active; } }

        private string _LCARSButton_Disengage_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSButton_Disengage";
        private string _LCARSButton_SlowDown_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSButton_SlowDown";
        private string _LCARSButton_SlowDown_icon_active = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSButton_SlowDown_active";
        public string LCARSButton_Disengage_icon { get { return _LCARSButton_Disengage_icon; } }
        public string LCARSButton_SlowDown_icon { get { return _LCARSButton_SlowDown_icon; } }
        public string LCARSButton_SlowDown_icon_active { get { return _LCARSButton_SlowDown_icon_active; } }

        private string _LCARSToggle_ALock_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_ALock";
        private string _LCARSToggle_ALock_icon_active = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_ALock_active";
        public string LCARSToggle_ALock_icon { get { return _LCARSToggle_ALock_icon; } }
        public string LCARSToggle_ALock_icon_active { get { return _LCARSToggle_ALock_icon_active; } }
        
        private string _LCARSToggle_FullHalt_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_FullHalt";
        private string _LCARSToggle_FullHalt_icon_active = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_FullHalt_active";
        public string LCARSToggle_FullHalt_icon { get { return _LCARSToggle_FullHalt_icon; } }
        public string LCARSToggle_FullHalt_icon_active { get { return _LCARSToggle_FullHalt_icon_active; } }

        private string _LCARSToggle_FullImpulse_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_FullImpulse";
        private string _LCARSToggle_FullImpulse_icon_active = "SciFi/StarTrekImpulseDrive/Icons/v3/LCARSToggle_FullImpulse_active";
        public string LCARSToggle_FullImpulse_icon { get { return _LCARSToggle_FullImpulse_icon; } }
        public string LCARSToggle_FullImpulse_icon_active { get { return _LCARSToggle_FullImpulse_icon_active; } }


        private string _BackGroundLayout_image = "SciFi/StarTrekImpulseDrive/Icons/v3/LcarsLayout_final";
        private string _ImpulseDrive_UpDownSlider_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/Lcarsslider_up_final";
        private string _ImpulseDrive_LeftRightslider_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/Lcarsslider_lateral_final";
        private string _ImpulseDrive_mainslider_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/Lcars_mainButton_final";
        private string _ImpulseDrive_forwardslider_icon = "SciFi/StarTrekImpulseDrive/Icons/v3/Lcarsslider_forward_final";
        public string BackGroundLayout_image { get { return _BackGroundLayout_image; } }
        public string ImpulseDrive_UpDownSlider_icon { get { return _ImpulseDrive_UpDownSlider_icon; } }
        public string ImpulseDrive_LeftRightslider_icon { get { return _ImpulseDrive_LeftRightslider_icon; } }
        public string ImpulseDrive_mainslider_icon { get { return _ImpulseDrive_mainslider_icon; } }
        public string ImpulseDrive_forwardslider_icon { get { return _ImpulseDrive_forwardslider_icon; } }

        public void Style()
        {
            if (_toggle_style_formationMode == null)
            {
                _toggle_style_formationMode = new GUIStyle();
                _toggle_style_formationMode.alignment = TextAnchor.MiddleCenter;
                _toggle_style_formationMode.normal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_formation_icon, false);
                _toggle_style_formationMode.onNormal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_formation_icon_active, false);
                _toggle_style_formationMode.onHover.background = GameDatabase.Instance.GetTexture(_LCARSToggle_formation_icon_active, false);
                _toggle_style_formationMode.padding = new RectOffset(0, 0, 0, 0);
                _toggle_style_formationMode.margin = new RectOffset(0, 0, 0, 0);
                _toggle_style_formationMode.fixedWidth = 97;
                _toggle_style_formationMode.fixedHeight = 11;
                //_toggle_style_formationMode.imagePosition = ImagePosition.ImageOnly;
                _toggle_style_formationMode.normal.textColor = Color.black;
            }
            if (_toggle_style_IgnoreFormation == null)
            {
                _toggle_style_IgnoreFormation = new GUIStyle();
                _toggle_style_IgnoreFormation.alignment = TextAnchor.MiddleCenter;
                _toggle_style_IgnoreFormation.normal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_IgnoreFormation_icon, false);
                _toggle_style_IgnoreFormation.onNormal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_IgnoreFormation_icon_active, false);
                _toggle_style_IgnoreFormation.onHover.background = GameDatabase.Instance.GetTexture(_LCARSToggle_IgnoreFormation_icon_active, false);
                _toggle_style_IgnoreFormation.padding = new RectOffset(0, 0, 0, 0);
                _toggle_style_IgnoreFormation.margin = new RectOffset(0, 0, 0, 0);
                _toggle_style_IgnoreFormation.fixedWidth = 97;
                _toggle_style_IgnoreFormation.fixedHeight = 11;
                //_toggle_style_IgnoreFormation.imagePosition = ImagePosition.ImageOnly;
                _toggle_style_IgnoreFormation.normal.textColor = Color.black;
            }
            if (_toggle_style_pilotmode == null)
            {
                _toggle_style_pilotmode = new GUIStyle();
                _toggle_style_pilotmode.alignment = TextAnchor.MiddleCenter;
                _toggle_style_pilotmode.normal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_pilotmode_icon, false);
                _toggle_style_pilotmode.onNormal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_pilotmode_icon_active, false);
                _toggle_style_pilotmode.onHover.background = GameDatabase.Instance.GetTexture(_LCARSToggle_pilotmode_icon_active, false);
                _toggle_style_pilotmode.padding = new RectOffset(0, 0, 0, 0);
                _toggle_style_pilotmode.margin = new RectOffset(0, 0, 0, 0);
                _toggle_style_pilotmode.fixedWidth = 97;
                _toggle_style_pilotmode.fixedHeight = 11;
                //_toggle_style_pilotmode.imagePosition = ImagePosition.ImageOnly;
                _toggle_style_pilotmode.normal.textColor = Color.black;
            }
            if (_toggle_style_fullHalt == null)
            {
                _toggle_style_fullHalt = new GUIStyle();
                _toggle_style_fullHalt.alignment = TextAnchor.MiddleCenter;
                _toggle_style_fullHalt.normal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_FullHalt_icon, false);
                _toggle_style_fullHalt.onNormal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_FullHalt_icon_active, false);
                _toggle_style_fullHalt.onHover.background = GameDatabase.Instance.GetTexture(_LCARSToggle_FullHalt_icon_active, false);
                _toggle_style_fullHalt.padding = new RectOffset(0, 0, 0, 0);
                _toggle_style_fullHalt.margin = new RectOffset(0, 0, 0, 0);
                _toggle_style_fullHalt.fixedWidth = 97;
                _toggle_style_fullHalt.fixedHeight = 11;
                //_toggle_style_fullHalt.imagePosition = ImagePosition.ImageOnly;
                _toggle_style_fullHalt.normal.textColor = Color.black;
            }
            if (_toggle_style_MakeSlowToSave == null)
            {
                _toggle_style_MakeSlowToSave = new GUIStyle();
                _toggle_style_MakeSlowToSave.alignment = TextAnchor.MiddleCenter;
                _toggle_style_MakeSlowToSave.normal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_MakeSlowToSave_icon, false);
                _toggle_style_MakeSlowToSave.onNormal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_MakeSlowToSave_icon_active, false);
                _toggle_style_MakeSlowToSave.onHover.background = GameDatabase.Instance.GetTexture(_LCARSToggle_MakeSlowToSave_icon_active, false);
                _toggle_style_MakeSlowToSave.padding = new RectOffset(0, 0, 0, 0);
                _toggle_style_MakeSlowToSave.margin = new RectOffset(0, 0, 0, 0);
                _toggle_style_MakeSlowToSave.fixedWidth = 97;
                _toggle_style_MakeSlowToSave.fixedHeight = 11;
                //_toggle_style_MakeSlowToSave.imagePosition = ImagePosition.ImageOnly;
                _toggle_style_MakeSlowToSave.normal.textColor = Color.black;
            }
            if (_toggle_style_YouCanSaveNow == null)
            {
                _toggle_style_YouCanSaveNow = new GUIStyle();
                _toggle_style_YouCanSaveNow.alignment = TextAnchor.MiddleCenter;
                _toggle_style_YouCanSaveNow.normal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_YouCanSaveNow_icon, false);
                _toggle_style_YouCanSaveNow.onNormal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_YouCanSaveNow_icon_active, false);
                _toggle_style_YouCanSaveNow.onHover.background = GameDatabase.Instance.GetTexture(_LCARSToggle_YouCanSaveNow_icon_active, false);
                _toggle_style_YouCanSaveNow.padding = new RectOffset(0, 0, 0, 0);
                _toggle_style_YouCanSaveNow.margin = new RectOffset(0, 0, 0, 0);
                _toggle_style_YouCanSaveNow.fixedWidth = 97;
                _toggle_style_YouCanSaveNow.fixedHeight = 11;
                //_toggle_style_YouCanSaveNow.imagePosition = ImagePosition.ImageOnly;
                _toggle_style_YouCanSaveNow.normal.textColor = Color.black;
            }
            if (_toggle_style_EndSaveMode == null)
            {
                _toggle_style_EndSaveMode = new GUIStyle();
                _toggle_style_EndSaveMode.alignment = TextAnchor.MiddleCenter;
                _toggle_style_EndSaveMode.normal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_EndSaveMode_icon, false);
                _toggle_style_EndSaveMode.onNormal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_EndSaveMode_icon, false);
                _toggle_style_EndSaveMode.onHover.background = GameDatabase.Instance.GetTexture(_LCARSToggle_EndSaveMode_icon, false);
                _toggle_style_EndSaveMode.padding = new RectOffset(0, 0, 0, 0);
                _toggle_style_EndSaveMode.margin = new RectOffset(0, 0, 0, 0);
                _toggle_style_EndSaveMode.fixedWidth = 97;
                _toggle_style_EndSaveMode.fixedHeight = 11;
                //_toggle_style_EndSaveMode.imagePosition = ImagePosition.ImageOnly;
                _toggle_style_EndSaveMode.normal.textColor = Color.black;
            }
            if (_toggle_style_AccelerationLock == null)
            {
                _toggle_style_AccelerationLock = new GUIStyle();
                _toggle_style_AccelerationLock.alignment = TextAnchor.MiddleCenter;
                _toggle_style_AccelerationLock.normal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_ALock_icon, false);
                _toggle_style_AccelerationLock.onNormal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_ALock_icon_active, false);
                _toggle_style_AccelerationLock.onHover.background = GameDatabase.Instance.GetTexture(_LCARSToggle_ALock_icon_active, false);
                _toggle_style_AccelerationLock.padding = new RectOffset(0, 0, 0, 0);
                _toggle_style_AccelerationLock.margin = new RectOffset(0, 0, 0, 0);
                _toggle_style_AccelerationLock.fixedWidth = 97;
                _toggle_style_AccelerationLock.fixedHeight = 11;
                //_toggle_style_AccelerationLock.imagePosition = ImagePosition.ImageOnly;
                _toggle_style_AccelerationLock.normal.textColor = Color.black;
            }
            if (_toggle_style_FullImpulse == null)
            {
                _toggle_style_FullImpulse = new GUIStyle();
                _toggle_style_FullImpulse.alignment = TextAnchor.MiddleCenter;
                _toggle_style_FullImpulse.normal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_FullImpulse_icon, false);
                _toggle_style_FullImpulse.onNormal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_FullImpulse_icon_active, false);
                _toggle_style_FullImpulse.onHover.background = GameDatabase.Instance.GetTexture(_LCARSToggle_FullImpulse_icon_active, false);
                _toggle_style_FullImpulse.padding = new RectOffset(0, 0, 0, 0);
                _toggle_style_FullImpulse.margin = new RectOffset(0, 0, 0, 0);
                _toggle_style_FullImpulse.fixedWidth = 97;
                _toggle_style_FullImpulse.fixedHeight = 11;
                //_toggle_style_FullImpulse.imagePosition = ImagePosition.ImageOnly;
                _toggle_style_FullImpulse.normal.textColor = Color.black;
            }
            if (_toggle_style_UseReserves == null)
            {
                _toggle_style_UseReserves = new GUIStyle();
                _toggle_style_UseReserves.alignment = TextAnchor.MiddleCenter;
                _toggle_style_UseReserves.normal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_UseReserves_icon, false);
                _toggle_style_UseReserves.onNormal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_UseReserves_icon_active, false);
                _toggle_style_UseReserves.onHover.background = GameDatabase.Instance.GetTexture(_LCARSToggle_UseReserves_icon_active, false);
                _toggle_style_UseReserves.padding = new RectOffset(0, 0, 0, 0);
                _toggle_style_UseReserves.margin = new RectOffset(0, 0, 0, 0);
                _toggle_style_UseReserves.fixedWidth = 97;
                _toggle_style_UseReserves.fixedHeight = 11;
                //_toggle_style_UseReserves.imagePosition = ImagePosition.ImageOnly;
                _toggle_style_UseReserves.normal.textColor = Color.black;
            }
            if (_toggle_style_HoldSpeed == null)
            {
                _toggle_style_HoldSpeed = new GUIStyle();
                _toggle_style_HoldSpeed.alignment = TextAnchor.MiddleCenter;
                _toggle_style_HoldSpeed.normal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_HoldSpeed_icon, false);
                _toggle_style_HoldSpeed.onNormal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_HoldSpeed_icon_active, false);
                _toggle_style_HoldSpeed.onHover.background = GameDatabase.Instance.GetTexture(_LCARSToggle_HoldSpeed_icon_active, false);
                _toggle_style_HoldSpeed.padding = new RectOffset(0, 0, 0, 0);
                _toggle_style_HoldSpeed.margin = new RectOffset(0, 0, 0, 0);
                _toggle_style_HoldSpeed.fixedWidth = 97;
                _toggle_style_HoldSpeed.fixedHeight = 11;
                //_toggle_style_HoldSpeed.imagePosition = ImagePosition.ImageOnly;
                _toggle_style_HoldSpeed.normal.textColor = Color.black;
            }
            if (_toggle_style_HoldHeight == null)
            {
                _toggle_style_HoldHeight = new GUIStyle();
                _toggle_style_HoldHeight.alignment = TextAnchor.MiddleCenter;
                _toggle_style_HoldHeight.normal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_HoldHeight_icon, false);
                _toggle_style_HoldHeight.onNormal.background = GameDatabase.Instance.GetTexture(_LCARSToggle_HoldHeight_icon_active, false);
                _toggle_style_HoldHeight.onHover.background = GameDatabase.Instance.GetTexture(_LCARSToggle_HoldHeight_icon_active, false);
                _toggle_style_HoldHeight.padding = new RectOffset(0, 0, 0, 0);
                _toggle_style_HoldHeight.margin = new RectOffset(0, 0, 0, 0);
                _toggle_style_HoldHeight.fixedWidth = 97;
                _toggle_style_HoldHeight.fixedHeight = 11;
                //_toggle_style_HoldHeight.imagePosition = ImagePosition.ImageOnly;
                _toggle_style_HoldHeight.normal.textColor = Color.black;
            }
            if (_label_style_dark == null)
            {
                _label_style_dark = new GUIStyle(GUI.skin.label);
                _label_style_dark.alignment = TextAnchor.MiddleCenter;
                _label_style_dark.padding = new RectOffset(0, 0, 0, 0);
                _label_style_dark.normal.textColor = Color.black;
            }
            if (_label_style_light == null)
            {
                _label_style_light = new GUIStyle(GUI.skin.label);
                _label_style_light.alignment = TextAnchor.UpperLeft;
                _label_style_light.padding = new RectOffset(0, 0, 0, 0);
                _label_style_light.margin = new RectOffset(0, 0, 0, 0);
                _label_style_light.fontSize = 14;
                _label_style_light.normal.textColor = new Color(0.969F, 0.984F, 0.871F, 1.0F);
            }



            
            if (_toggle_style_dark == null)
            {
                _toggle_style_dark = new GUIStyle(GUI.skin.toggle);
                _toggle_style_dark.normal.textColor = Color.black;

            }

            if (_button_style_fullHalt == null)
            {
                _button_style_fullHalt = new GUIStyle(GUI.skin.button);
                _button_style_fullHalt.alignment = TextAnchor.MiddleCenter;
                _button_style_fullHalt.padding = new RectOffset(0, 0, 0, 0);
                _button_style_fullHalt.normal.background = GameDatabase.Instance.GetTexture(_LCARSButton_SlowDown_icon, false);
                _button_style_fullHalt.active.background = GameDatabase.Instance.GetTexture(_LCARSButton_SlowDown_icon_active, false);
                _button_style_fullHalt.onHover.background = GameDatabase.Instance.GetTexture(_LCARSButton_SlowDown_icon_active, false);
                _button_style_fullHalt.hover.background = GameDatabase.Instance.GetTexture(_LCARSButton_SlowDown_icon, false);
            }


            if (_iconStyle == null)
            {
                _iconStyle = new GUIStyle(GUI.skin.button);
                _iconStyle.alignment = TextAnchor.MiddleCenter;
                _iconStyle.padding = new RectOffset(0, 0, 0, 0);
            }
            if (_BackGroundLayoutStyle == null)
            {
                _BackGroundLayoutStyle = new GUIStyle(GUI.skin.box);
                _BackGroundLayoutStyle.alignment = TextAnchor.MiddleCenter;
                _BackGroundLayoutStyle.padding = new RectOffset(0, 0, 0, 0);
                _BackGroundLayoutStyle.margin = new RectOffset(0, 0, 0, 0);
                _BackGroundLayoutStyle.fixedWidth = 375;
                _BackGroundLayoutStyle.fixedHeight = 230;
            }
            if (_paddingTopStyle == null)
            {
                _paddingTopStyle = new GUIStyle(GUI.skin.label);
                _paddingTopStyle.alignment = TextAnchor.MiddleCenter;
                _paddingTopStyle.normal.textColor = Color.black;
                //paddingTopStyle.padding = new RectOffset(0, 0, 4, 0);
            }
        }
    }
}
