/*
 * 
 * LCARS_ImpulseDrive by Philotical
 * Licence: CC BY
 * 
 * Acknoledgements:
 * The git-repo of exsurgent helped me a lot with the antigrav code!
 * Thank's to who ever wrote it.
 * 
 * StarVision is allowerd to distribute this with his models.
 * 
 * http://wiki.unity3d.com/index.php?title=3d_Math_functions
 * 
 */


using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.IO;

namespace Philotical
{


    public class LCARS_ImpulseDrive : PartModule
	{
        private PluginConfiguration pluginConfig = PluginConfiguration.CreateForType<LCARS_ImpulseDrive>();
        private int windowID = new System.Random().Next();
        //private int info_windowID = new System.Random().Next();
        private string windowTitle = "";
        private float vSliderValue = (float)0.0;
        public float hSliderValue;
        public float zSliderValue;
        private float deadzone_pixel = 10;
        private float display_zSliderValue;
        private float display_hSliderValue;
        private float display_vSliderValue;

        private float x = 0;
        private float x2 = 0;
        private float y = 0;
        private float y2 = 0;
        private float z = 0;
        private bool clickedx = false;
        private bool clickedy = false;
        private bool clickedz = false;
        
        private float total_force = 0.00F;
        private float charge = 0.00F;

        bool bool_FullImpulse = false;
        bool bool_UseReserves = false;
        float MainComputerDefaultPowerUsage = 1250f;
        float MainImpulseDefaultPowerUsage = 850f;
        float LCARS_ImpulseDrive_Part_add_heat = 0;
        float CoreOverHeating_PowerRate = 580000f;
        float MaxPowerGeneratorRate = 1800000f;
        int int_FullImpulse_multiplier = 7;
        int int_UseReserves_multiplier = 4;
        bool bool_AccelerationLock = false;
        bool bool_PilotMode = false;
        bool bool_FormationMode = false;
        bool bool_IgnoreFormationCommandos = false;
        bool bool_HoldSpeed_enabled = false;
        double HoldSpeed_value;
        bool bool_HoldHeight_enabled = false;
        bool bool_info_window = false;

        bool bool_info_window_ship_info = false;
        bool bool_info_window_transporter = false;
        bool bool_info_window_Tractorbeam = false;
        bool bool_info_window_Weappons = false;
        bool bool_info_window_fueltransfer = false;
        bool bool_info_window_StructuralIntegrityField = false;
        bool reset_StructuralIntegrityField_run_once = true;
        bool bool_info_window_CloakingDevice = false;
        bool bool_info_window_SensorArray = false;
        bool bool_info_window_PowerSystem = false;

        bool bool_info_window_system_selector_expand = true;
        bool bool_info_window_ship_info_expand = true;
        bool bool_info_window_transporter_expand = true;
        bool bool_info_window_Tractorbeam_expand = true;
        bool bool_info_window_Weappons_expand = true;
        bool bool_info_window_fueltransfer_expand = true;
        bool bool_info_window_StructuralIntegrityField_expand = true;
        bool bool_HoldHeight_enabled_expand = true;
        bool bool_HoldSpeed_enabled_expand = true;
        bool bool_FormationMode_expand = true;
        bool bool_info_window_CloakingDevice_expand = true;
        bool bool_info_window_SensorArray_expand = true;
        bool bool_info_window_PowerSystem_expand = true;
        bool bool_info_window_PowerSystem_expand_main = false;
        bool bool_info_window_PowerSystem_expand_sub = false;

        double HoldHeight_value;
        float SIF_force = 1f;
        float CD_force = 1f;

        public float makeStationarySpeedMax = 1f, makeStationarySpeedClamp = 0.0f;

        bool bool_SlowDown;
        bool resetSlider;

        public Dictionary<string, float> Powerstats = new Dictionary<string, float>();

        [KSPField(isPersistant = true)]
        bool bool_FullHalt = false;

        [KSPField(isPersistant = true)]
        bool bool_MakeSlowToSave = false;

        [KSPField(isPersistant = true)]
        public Rect windowPosition = new Rect(120, 120, 375, 230);

        public Rect info_windowPosition = new Rect(0, 120, 220, 20);

        [KSPField(guiActive=false, isPersistant=true)]
        private bool gravityEnabled = true;

        [KSPField(guiActive = false, guiName = "Gravity power", guiFormat = "F2", guiUnits = "m/s", isPersistant = true)]
        public float
			gee = 0f;

        [KSPField(isPersistant = true)]
        public Vector3 geeVector = Vector3.up;


        public List<Vessel> vesselList;
        public List<Vessel> visibleVessels;
        public List<Vessel> ImpulseVessels;
        public string ActiveVessel_pid;
        public Vessel activeVessel;
        public string currentVessel_pid;
        public Vessel currentVessel;
        public LCARS_ImpulseVesselType activeImpulseVessel;

        private float lastUpdate = 0.0f;
        private float lastFixedUpdate = 0.0f;
        //private float lastflyUpdate = 0.0f;
        private float logInterval = 10.0f;

        public ImpulseVessel_manager man;
        private Bounds myBounds;

        [KSPEvent(guiName = "Activate ImpulseDrive",/* category = "ImpulseDrive",isDefault=true,*/ guiActive = true)]
        public void ActivateImpulseDrive()
		{
            //man.populateImpulseVesselList(this.vessel);
            UnityEngine.Debug.Log("ImpulseDrive: ActivateImpulseDrive  pre start check ");
            this.part.force_activate();
            UnityEngine.Debug.Log("ImpulseDrive: ActivateImpulseDrive  1 - locating Laforge");
            UnityEngine.Debug.Log("ImpulseDrive: ActivateImpulseDrive  2 - heating Earl Gray");
            UnityEngine.Debug.Log("ImpulseDrive: ActivateImpulseDrive  3 - starting reactor");
            gravityEnabled = true;
            UnityEngine.Debug.Log("ImpulseDrive: ActivateImpulseDrive  4 - untangling space tape");
            this.Events["ActivateImpulseDrive"].active = !gravityEnabled;
            UnityEngine.Debug.Log("ImpulseDrive: ActivateImpulseDrive  5 - activation sequence finished");
            this.Events["DeactivateImpulseDrive"].active = gravityEnabled;
            UnityEngine.Debug.Log("ImpulseDrive: ActivateImpulseDrive  6 - Impulse Drive activation: nominal");
            try
            {
                man.ImpulseVesselList[ActiveVessel_pid].is_gravity_enabled = gravityEnabled;
            }
            catch
            {
                UnityEngine.Debug.Log("ImpulseDrive: ImpulseVesselList not ready yet ");

            }
            UnityEngine.Debug.Log("ImpulseDrive: ActivateImpulseDrive  done ");

		}

        [KSPEvent(guiName = "Deactivate ImpulseDrive",/* category = "ImpulseDrive", isDefault = false*/ guiActive = true)]
        public void DeactivateImpulseDrive()
		{
			gravityEnabled = false;
            this.Events["ActivateImpulseDrive"].active = !gravityEnabled;
            this.Events["DeactivateImpulseDrive"].active = gravityEnabled;
            man.ImpulseVesselList[ActiveVessel_pid].is_gravity_enabled = gravityEnabled;
            man.ImpulseVesselList.Remove(ActiveVessel_pid);
        }

        [KSPAction("ActivateImpulseDrive")]
        public void ActivateImpulseDriveAction(KSPActionParam param)
		{
            ActivateImpulseDrive();
		}

        [KSPAction("DeactivateImpulseDrive")]
        public void DeactivateImpulseDriveAction(KSPActionParam param)
		{
            DeactivateImpulseDrive();
		}

        [KSPAction("ToggleImpulseDriveImpulseDrive")]
        public void ToggleImpulseDriveAction(KSPActionParam param)
		{
			if (gravityEnabled)
                DeactivateImpulseDrive();
			else
                ActivateImpulseDrive();
		}


        GravityTools grav;
        Rect info_windowPosition_live = new Rect();
        LCARS_TractorBeam TB;
        LCARS_WeaponSystems WS;
        //PlanetScanner planetScanner;
        LCARS_TransporterSystem TS;
        LCARS_VesselPartsInventory VPI;
        LCARS_PowerSystem PowSys;
        PowerTaker PT1 = null;
        PowerTaker PT2 = null;
        LCARS_StructuralIntegrityField SIF;
        LCARS_CloakingDevice CD;


        LCARS_CrewQuartier STCQ;
        LCARS_CargoBay STCB;
        LCARS_FuelTransfer FT;
        LCARS_SensorArray SA; 

        public override void OnAwake()
        {
            //UnityEngine.Debug.Log("ImpulseDrive: OnAwake  ");

            man = ImpulseVessel_manager.Instance;
            //Debug.Log("ImpulseDrive OnAwake man=" + man);
            //Dictionary<string, ImpulseVesselType> ImpulseVesselList = ImpulseVessel_manager.ImpulseVesselList;
            //ImpulseVesselList = man.getImpulseVesselList();

            //ImpulseVessel_manager.IsFormationLeaderDefined();

            info_windowPosition_live = info_windowPosition;
        }

        bool loded_once = false;
        Part LCARS_ImpulseDrive_Part = null;
        ModuleGenerator.GeneratorResource LCARS_EC_Generator = null;
        public void loadOnce()
        {
            if (!loded_once && FlightGlobals.ActiveVessel.id == this.vessel.id)
            {
                activeVessel = FlightGlobals.ActiveVessel; // FlightGlobals.ActiveVessel;
                ActiveVessel_pid = activeVessel.id.ToString();

                man.populateImpulseVesselList(activeVessel);

                grav = new GravityTools();
                VPI = new LCARS_VesselPartsInventory();
                PowSys = new LCARS_PowerSystem();
            
                VPI.init(activeVessel);
                VPI.scanVessel();
                LCARS_ImpulseDrive_Part = this.VPI.getPartWithModule("LCARS_ImpulseDrive");
                LCARS_EC_Generator = this.VPI.find_EC_Generator();

                PowSys.SetShip(activeVessel);
                PT1 = this.PowSys.setPowerTaker("LCARS-ODN", "MainSystem", MainComputerDefaultPowerUsage, 0, 0);
                float Level1 = 1250f;
                PT2 = this.PowSys.setPowerTaker("ImpulseDrive-EPS", "MainSystem", MainImpulseDefaultPowerUsage, MainImpulseDefaultPowerUsage * int_FullImpulse_multiplier * 2, MainImpulseDefaultPowerUsage * int_FullImpulse_multiplier * int_UseReserves_multiplier * 3);


                if (VPI.getModules().ContainsKey("LCARS_TractorBeam"))
                {
                    TB = new LCARS_TractorBeam();
                    TB.setVPI(VPI, PowSys);
                }
                if (VPI.getModules().ContainsKey("LCARS_WeaponSystems"))
                {
                    WS = new LCARS_WeaponSystems();
                    WS.setVPI(VPI, PowSys);
                }
                if (VPI.getModules().ContainsKey("LCARS_TransporterSystem"))
                {
                    TS = new LCARS_TransporterSystem();
                    TS.setVPI(VPI, PowSys);
                }
                if (VPI.getModules().ContainsKey("LCARS_FuelTransfer"))
                {
                    FT = new LCARS_FuelTransfer();
                    FT.setVPI(activeVessel, VPI, STCB, PowSys);
                }
                if (VPI.getModules().ContainsKey("LCARS_StructuralIntegrityField"))
                {
                    SIF = new LCARS_StructuralIntegrityField();
                    SIF.SetShip(activeVessel, PowSys);
                }
                if (VPI.getModules().ContainsKey("LCARS_CloakingDevice"))
                {
                    CD = new LCARS_CloakingDevice();
                    CD.SetShip(activeVessel, PowSys);
                }
                if (VPI.getModules().ContainsKey("LCARS_SensorArray"))
                {
                    SA = new LCARS_SensorArray();
                    SA.setVPI(activeVessel, VPI, PowSys);
                }
                if (VPI.getModules().ContainsKey("LCARS_CrewQuartier"))
                {
                    STCQ = new LCARS_CrewQuartier();
                    STCQ.setVessel(activeVessel);
                    STCQ.setVPI(VPI);
                    STCQ.addCrewSpace();
                    STCQ.addHatch();
                }
                //if (VPI.getModules().ContainsKey("LCARS_ShuttleBay") && VPI.getModules().ContainsKey("LCARS_CargoBay"))
                if (VPI.getModules().ContainsKey("LCARS_CargoBay"))
                {
                    STCB = new LCARS_CargoBay();
                    STCB.setVessel(activeVessel, VPI);
                    STCB.setupCargoSpace();
                }
                loded_once = true;
            }
        }

        public override void OnStart(StartState state)
        {
            if (FlightGlobals.ActiveVessel.id != this.vessel.id)
            {
                //gravityEnabled = false;
                return;
            }

            //UnityEngine.Debug.Log("ImpulseDrive: OnStart  this.vessel.id=" + this.vessel.id.ToString() + "  ActiveVessel_pid=" + ActiveVessel_pid);

            Powerstats.Add("charge", 0);
            Powerstats.Add("total_force", 0);
            Powerstats.Add("force_x", 0);
            Powerstats.Add("force_y", 0);
            Powerstats.Add("force_z", 0);

            Powerstats.Add("consumption_total", 0);
            Powerstats.Add("consumption_main_systems", 0);
            Powerstats.Add("consumption_sub_systems", 0);


            //GameEvents.onVesselChange.Add(onVesselChange);

            if (state == StartState.Editor)
            {
                gravityEnabled = false;
            }
            if (gravityEnabled)
            {
                UnityEngine.Debug.Log("ImpulseDrive: OnStart ActivateImpulseDrive  pre start check ");
                UnityEngine.Debug.Log("ImpulseDrive: OnStart ActivateImpulseDrive  1 - locating Laforge");
                UnityEngine.Debug.Log("ImpulseDrive: OnStart ActivateImpulseDrive  2 - heating Earl Gray");
                UnityEngine.Debug.Log("ImpulseDrive: OnStart ActivateImpulseDrive  3 - starting reactor");
                UnityEngine.Debug.Log("ImpulseDrive: OnStart ActivateImpulseDrive  4 - untangling space tape");
                UnityEngine.Debug.Log("ImpulseDrive: OnStart ActivateImpulseDrive  5 - activation sequence finished");
                UnityEngine.Debug.Log("ImpulseDrive: OnStart ActivateImpulseDrive  6 - Impulse Drive activation: nominal");
                UnityEngine.Debug.Log("ImpulseDrive: OnStart ActivateImpulseDrive  done ");
            }


        }
        private void onVesselChange(Vessel v)
        {
        }

        public override void OnInactive()
        {
        }

        public override void OnActive()
        {
        }

        /// <summary>
        /// called on every Update frame
        /// </summary>
        public void Update()
		{
            //UnityEngine.Debug.Log("ImpulseDrive: Update 1 ");
            if (HighLogic.LoadedSceneIsEditor)
            {
                return;
            }

            if (this.vessel.rigidbody == null)
            {
                //return;
            }

            if (FlightGlobals.ActiveVessel.id != this.vessel.id)
            {
                return;
            }

            // run garbage collector
            if (Time.frameCount % 30 == 0)
            {
               System.GC.Collect();
            }

        }
        /// <summary>
        /// called on every FixedUpdate frame - performs most of the physics
        /// </summary>
        public void FixedUpdate()
		{
            //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 1 ");

            if (HighLogic.LoadedSceneIsEditor)
                return;

            //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 2 ");
            if (this.vessel.rigidbody == null)
            {
                //return;
            }
            if (FlightGlobals.ActiveVessel.id != this.vessel.id)
            {
                return;
            }

            loadOnce();
            PowSys.reset_Powerstats();

            activeVessel = this.vessel;
            ActiveVessel_pid = activeVessel.id.ToString();
            //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 1 ActiveVessel_pid=" + ActiveVessel_pid);
            currentVessel = this.vessel;
            currentVessel_pid = currentVessel.id.ToString();

            bool is_ImpulseVessel = man.IsImpulseVessel(currentVessel);


            geeVector = FlightGlobals.getGeeForceAtPosition(currentVessel.findWorldCenterOfMass());
            //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 2 ");

            if (is_ImpulseVessel)
            {
                //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 7 ");
                if (!man.ContainsShip(currentVessel_pid))
                {
                    //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 8 ");
                    man.AddImpulseVessel(currentVessel_pid);
                    //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 9 ");
                    man.ImpulseVesselList[currentVessel_pid].v = currentVessel;
                    man.ImpulseVesselList[currentVessel_pid].pid = currentVessel_pid;
                    man.ImpulseVesselList[currentVessel_pid].name = currentVessel.vesselName;
                    man.ImpulseVesselList[currentVessel_pid].geeVector = this.geeVector;
                    man.ImpulseVesselList[currentVessel_pid].windowPosition = this.windowPosition;
                    man.ImpulseVesselList[currentVessel_pid].is_active_vessel = (currentVessel_pid == ActiveVessel_pid) ? true : false;
                    man.ImpulseVesselList[currentVessel_pid].is_gravity_enabled = true;
                    man.ImpulseVesselList[currentVessel_pid].is_fullHalt_enabled = this.bool_FullHalt;
                    man.ImpulseVesselList[currentVessel_pid].is_MakeSlowToSave_enabled = this.bool_MakeSlowToSave;
                    man.ImpulseVesselList[currentVessel_pid].is_fullImpulse_enabled = this.bool_FullImpulse;
                    man.ImpulseVesselList[currentVessel_pid].is_UseReserves_enabled = this.bool_UseReserves;
                    man.ImpulseVesselList[currentVessel_pid].is_accelerationLock_enabled = this.bool_AccelerationLock;
                    man.ImpulseVesselList[currentVessel_pid].is_pilotMode_enabled = this.bool_PilotMode;
                    man.ImpulseVesselList[currentVessel_pid].is_formationMode_enabled = this.bool_FormationMode;
                    man.ImpulseVesselList[currentVessel_pid].is_ignoreformationcommandos_enabled = this.bool_IgnoreFormationCommandos;
                    man.ImpulseVesselList[currentVessel_pid].is_holdspeed_enabled = this.bool_HoldSpeed_enabled;
                    man.ImpulseVesselList[currentVessel_pid].is_holdheight_enabled = this.bool_HoldHeight_enabled;
                    //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate currentVessel 10 " + currentVessel_pid);
                    //gravityEnabled = true;
                }

            }
            //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 14 ");





            //if (this.bool_PilotMode) { bool_AccelerationLock = true; }
            //UnityEngine.Debug.Log("ImpulseDrive: bool_FullImpulse Pre vSliderValue=" + vSliderValue);
            float _total_force = vSliderValue + hSliderValue + zSliderValue;
            float _charge = PT2.L1_usage;
            if (this.bool_FullImpulse)
            {
                    vSliderValue = vSliderValue * int_FullImpulse_multiplier;
                    hSliderValue = hSliderValue * int_FullImpulse_multiplier;
                    zSliderValue = zSliderValue * int_FullImpulse_multiplier;
                //UnityEngine.Debug.Log("ImpulseDrive: bool_FullImpulse applied");

                    _total_force *= int_FullImpulse_multiplier;
                    _charge = PT2.L2_usage;
            }
            if (!this.bool_FullImpulse)
            {
                this.bool_UseReserves = false;
                //UnityEngine.Debug.Log("ImpulseDrive: bool_FullImpulse reset");
                zSliderValue = (zSliderValue > 100) ? 100 : zSliderValue;
                hSliderValue = (hSliderValue > 100) ? 100 : hSliderValue;
                vSliderValue = (vSliderValue > 100) ? 100 : vSliderValue;
                zSliderValue = (zSliderValue < -100) ? -100 : zSliderValue;
                hSliderValue = (hSliderValue < -100) ? -100 : hSliderValue;
                vSliderValue = (vSliderValue < -100) ? -100 : vSliderValue;
            }
            if (!this.bool_UseReserves && this.bool_FullImpulse)
            {
                zSliderValue = (zSliderValue > 500) ? 500 : zSliderValue;
                hSliderValue = (hSliderValue > 500) ? 500 : hSliderValue;
                vSliderValue = (vSliderValue > 500) ? 500 : vSliderValue;
                zSliderValue = (zSliderValue < -500) ? -500 : zSliderValue;
                hSliderValue = (hSliderValue < -500) ? -500 : hSliderValue;
                vSliderValue = (vSliderValue < -500) ? -500 : vSliderValue;
            }
            
            if (this.bool_FullImpulse && this.bool_UseReserves)
            {
                    vSliderValue = vSliderValue * int_UseReserves_multiplier;
                    hSliderValue = hSliderValue * int_UseReserves_multiplier;
                    zSliderValue = zSliderValue * int_UseReserves_multiplier;
                //UnityEngine.Debug.Log("ImpulseDrive: bool_UseReserves applied");
            }
            if (!this.bool_UseReserves)
            {
                //UnityEngine.Debug.Log("ImpulseDrive: bool_UseReserves reset");
            }
            else
            { 
                zSliderValue = (zSliderValue > 1000) ? 1000 : zSliderValue;
                hSliderValue = (hSliderValue > 1000) ? 1000 : hSliderValue;
                vSliderValue = (vSliderValue > 1000) ? 1000 : vSliderValue;
                zSliderValue = (zSliderValue < -1000) ? -1000 : zSliderValue;
                hSliderValue = (hSliderValue < -1000) ? -1000 : hSliderValue;
                vSliderValue = (vSliderValue < -1000) ? -1000 : vSliderValue;

                _total_force *= int_UseReserves_multiplier;
                _charge = PT2.L3_usage;
            }




            //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 20 ");
                        ///////////////////
                        // the active vessel
                        /////////////////////

            if (gravityEnabled)
            {
                man.ImpulseVesselList[currentVessel_pid].is_active_vessel = true;
                grav.CancelG2(this.vessel, geeVector, man);
                if (this.bool_SlowDown) { grav.FullHalt(currentVessel, this.bool_SlowDown); }

                if (this.bool_FullHalt) 
                {
                    grav.FullHalt(currentVessel, this.bool_FullHalt);
                }
                man.ImpulseVesselList[currentVessel_pid].is_fullHalt_enabled = this.bool_FullHalt;

                // Freeze in place if requested
                if (this.bool_MakeSlowToSave && this.bool_FullHalt) { grav.makeSlowToSave(currentVessel, this.bool_MakeSlowToSave, makeStationarySpeedClamp); }
                man.ImpulseVesselList[currentVessel_pid].is_MakeSlowToSave_enabled = this.bool_MakeSlowToSave;

                if (this.bool_FullImpulse) { }
                man.ImpulseVesselList[currentVessel_pid].is_fullImpulse_enabled = this.bool_FullImpulse;

                if (this.bool_UseReserves) { }
                man.ImpulseVesselList[currentVessel_pid].is_UseReserves_enabled = this.bool_UseReserves;

                if (this.bool_AccelerationLock)
                {
                    man.ImpulseVesselList[currentVessel_pid].is_holdspeed_enabled = false;
                }
                man.ImpulseVesselList[currentVessel_pid].is_accelerationLock_enabled = this.bool_AccelerationLock;

                if (this.bool_PilotMode)
                {
                    grav.PilotMode(currentVessel, this.bool_PilotMode);
                    man.ImpulseVesselList[currentVessel_pid].is_holdheight_enabled = false;
                }
                man.ImpulseVesselList[currentVessel_pid].is_pilotMode_enabled = this.bool_PilotMode;

                man.ImpulseVesselList[currentVessel_pid].is_formationMode_enabled = false;
                if (this.bool_FormationMode)
                {
                    man.ImpulseVesselList[currentVessel_pid].is_formationMode_enabled = true;
                    man.ChangeFormationLeader(currentVessel_pid);
                }
                man.ImpulseVesselList[currentVessel_pid].is_formationMode_enabled = this.bool_FormationMode;


                if (this.bool_HoldSpeed_enabled) 
                {
                    grav.HoldSpeed(currentVessel, this.bool_HoldSpeed_enabled, this.HoldSpeed_value);
                }
                man.ImpulseVesselList[currentVessel_pid].is_holdspeed_enabled = this.bool_HoldSpeed_enabled;
                man.ImpulseVesselList[currentVessel_pid].HoldSpeed_value = this.HoldSpeed_value;

                if (this.bool_HoldHeight_enabled) 
                {
                    grav.HoldHeight(currentVessel, this.bool_HoldHeight_enabled, HoldHeight_value);
                }
                man.ImpulseVesselList[currentVessel_pid].is_holdheight_enabled = this.bool_HoldHeight_enabled;
                man.ImpulseVesselList[currentVessel_pid].HoldHeight_value = this.HoldHeight_value;

                if (this.bool_IgnoreFormationCommandos) { this.bool_FormationMode = false; man.ImpulseVesselList[currentVessel_pid].is_formationMode_enabled = false; }
                man.ImpulseVesselList[currentVessel_pid].is_ignoreformationcommandos_enabled = this.bool_IgnoreFormationCommandos;

                //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate AddNewForce_ffwd_back 1 current loop item =" + currentVessel_pid);
                grav.AddNewForce_ffwd_back(vSliderValue, currentVessel);
                grav.AddNewForce_left_right(hSliderValue, currentVessel);
                grav.AddNewForce_up_down(zSliderValue, currentVessel);
                //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate AddNewForce_ffwd_back 2 current loop item =" + currentVessel_pid);

                //LCARS_Utilities.CalculatePowerConsumption(Powerstats,currentVessel, true, man.ImpulseVesselList[currentVessel_pid].is_fullImpulse_enabled, man.ImpulseVesselList[currentVessel_pid].is_UseReserves_enabled, int_FullImpulse_multiplier, int_UseReserves_multiplier, vSliderValue, hSliderValue, zSliderValue);

                //LCARS_ImpulseDrive_Part
                //LCARS_EC_Generator

                if (LCARS_EC_Generator.rate > CoreOverHeating_PowerRate)
                {
                    float heat_percentage = LCARS_EC_Generator.rate / (MaxPowerGeneratorRate / 100);
                    float heat_percentage2 = CoreOverHeating_PowerRate / (MaxPowerGeneratorRate / 100);
                    LCARS_ImpulseDrive_Part_add_heat = (heat_percentage - heat_percentage2)/100;
                    LCARS_ImpulseDrive_Part.temperature += LCARS_ImpulseDrive_Part_add_heat;
                }


                total_force = _total_force;
                charge = _charge;
                float tot = charge + (total_force * 12) * Time.deltaTime;

                // main computer LCARS
                this.PowSys.draw(PT1.takerName, MainComputerDefaultPowerUsage * Time.deltaTime);

                // impulse drive
                float power2 = tot; // PT2.L1_usage* total_force;
                power2 = (power2==0f) ? PT2.L1_usage : power2;
                this.PowSys.draw(PT2.takerName, power2);


                //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 1a ActiveVessel_pid=" + ActiveVessel_pid + "  currentVessel_pid=" + currentVessel_pid + "  currentVessel_pid=" + currentVessel_pid + "  [" + this.GetInstanceID().ToString("X") + "][" + Time.time.ToString("0.0000") + "]");
                ///////////////////
                // the active vessel
                /////////////////////
                //man.print_stats(this.vessel.id.ToString());
                    if ((Time.time - lastFixedUpdate) > logInterval)
                    {
                        Resources.UnloadUnusedAssets();

                        UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate [" + this.GetInstanceID().ToString("X") + "][" + Time.time.ToString("0.0000") + "]");
                        lastFixedUpdate = Time.time;
                        man.populateImpulseVesselList(this.vessel);
                        man.print_stats(currentVessel_pid);
                        //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate after man.print_stats()  [" + this.GetInstanceID().ToString("X") + "][" + Time.time.ToString("0.0000") + "]");
                        /*
                        man.print_control_values("2b.) " + ActiveVessel_pid, true, this.gravityEnabled, this.bool_FullHalt, this.bool_MakeSlowToSave, this.bool_FullImpulse, this.bool_UseReserves, this.bool_AccelerationLock, this.bool_PilotMode, this.bool_FormationMode, this.bool_IgnoreFormationCommandos, this.bool_HoldSpeed_enabled, this.bool_HoldHeight_enabled);
                        man.print_control_values("3b.) " + pair.Value.pid,
                            man.ImpulseVesselList[pair.Value.pid].is_active_vessel,
                            man.ImpulseVesselList[pair.Value.pid].is_gravity_enabled,
                            man.ImpulseVesselList[pair.Value.pid].is_fullHalt_enabled,
                            man.ImpulseVesselList[pair.Value.pid].is_MakeSlowToSave_enabled,
                            man.ImpulseVesselList[pair.Value.pid].is_fullImpulse_enabled,
                            man.ImpulseVesselList[pair.Value.pid].is_UseReserves_enabled,
                            man.ImpulseVesselList[pair.Value.pid].is_accelerationLock_enabled,
                            man.ImpulseVesselList[pair.Value.pid].is_pilotMode_enabled,
                            man.ImpulseVesselList[pair.Value.pid].is_formationMode_enabled,
                            man.ImpulseVesselList[pair.Value.pid].is_ignoreformationcommandos_enabled,
                            man.ImpulseVesselList[pair.Value.pid].is_holdspeed_enabled,
                            man.ImpulseVesselList[pair.Value.pid].is_holdheight_enabled
                            );
                        */

                        // reset the size of info window
                        info_windowPosition_live.height = 20f;
                        info_windowPosition_live.width = 320f;
                    }
            }
            foreach (KeyValuePair<string, LCARS_ImpulseVesselType> pair in man.getImpulseVesselList())
            {
                man.ImpulseVesselList[pair.Value.pid].is_active_vessel = (pair.Value.pid == currentVessel_pid) ? true : false; 

                if (pair.Value.is_gravity_enabled && !man.ImpulseVesselList[pair.Value.pid].is_active_vessel)
                {
                    grav.CancelG2(pair.Value.v, geeVector, man);

                    if (man.ImpulseVesselList[currentVessel_pid].is_formationMode_enabled && !pair.Value.is_ignoreformationcommandos_enabled)
                    {
                        man.ImpulseVesselList[pair.Value.pid].is_formationMode_enabled = false;
                             pair.Value.v.GoOffRails();
                                //pair.Value.v.rigidbody.WakeUp();
                                //pair.Value.v.ResumeStaging();
                                //pair.Value.v.Initialize(false);

                            
                            if (this.bool_SlowDown) { grav.FullHalt(pair.Value.v, this.bool_SlowDown); }
                        
                            if (this.bool_FullHalt)
                            {
                                grav.FullHalt(pair.Value.v, true);
                                man.ImpulseVesselList[pair.Value.pid].is_holdspeed_enabled = false;
                                man.ImpulseVesselList[pair.Value.pid].is_pilotMode_enabled = false;
                            }
                            man.ImpulseVesselList[pair.Value.pid].is_fullHalt_enabled = this.bool_FullHalt;

                            // Freeze in place if requested
                            if (this.bool_MakeSlowToSave && this.bool_FullHalt) { grav.makeSlowToSave(pair.Value.v, true, makeStationarySpeedClamp); }
                            man.ImpulseVesselList[pair.Value.pid].is_MakeSlowToSave_enabled = this.bool_MakeSlowToSave;
                        
                            man.ImpulseVesselList[pair.Value.pid].is_fullImpulse_enabled = this.bool_FullImpulse;
                            man.ImpulseVesselList[pair.Value.pid].is_UseReserves_enabled = this.bool_UseReserves;

                            if (this.bool_AccelerationLock)
                            {
                                man.ImpulseVesselList[pair.Value.pid].is_holdspeed_enabled = false;
                            }
                            man.ImpulseVesselList[pair.Value.pid].is_accelerationLock_enabled = this.bool_AccelerationLock;
                            
                            if (this.bool_PilotMode)
                            {
                                grav.PilotMode(pair.Value.v, true);
                                man.ImpulseVesselList[pair.Value.pid].is_holdheight_enabled = false;
                                //this.bool_HoldHeight_enabled = man.ImpulseVesselList[pair.Value.pid].is_holdheight_enabled;
                            }
                            man.ImpulseVesselList[pair.Value.pid].is_pilotMode_enabled = this.bool_PilotMode;

                            if (this.bool_HoldSpeed_enabled) { grav.HoldSpeed(pair.Value.v, this.bool_HoldSpeed_enabled, this.HoldSpeed_value); }
                            man.ImpulseVesselList[pair.Value.pid].is_holdspeed_enabled = this.bool_HoldSpeed_enabled;
                            man.ImpulseVesselList[pair.Value.pid].HoldSpeed_value = this.HoldSpeed_value;

                            if (this.bool_HoldHeight_enabled) { grav.HoldHeight(pair.Value.v, this.bool_HoldHeight_enabled, this.HoldHeight_value); }
                            man.ImpulseVesselList[pair.Value.pid].is_holdheight_enabled = this.bool_HoldHeight_enabled;
                            man.ImpulseVesselList[pair.Value.pid].HoldHeight_value = this.HoldHeight_value;

                            grav.FormationFlight(pair.Value.v, true, man);
                            grav.AddNewForce_ffwd_back(vSliderValue, pair.Value.v);
                            grav.AddNewForce_left_right(hSliderValue, pair.Value.v);
                            grav.AddNewForce_up_down(zSliderValue, pair.Value.v);

                            LCARS_Utilities.CalculatePowerConsumption(Powerstats,pair.Value.v, true, this.bool_FullImpulse, this.bool_UseReserves, int_FullImpulse_multiplier, int_UseReserves_multiplier, vSliderValue, hSliderValue, zSliderValue);
                    }
                    if (!man.ImpulseVesselList[currentVessel_pid].is_formationMode_enabled || pair.Value.is_ignoreformationcommandos_enabled)
                    {
                        if (man.ImpulseVesselList[pair.Value.pid].is_fullHalt_enabled)
                        {
                            grav.FullHalt(pair.Value.v, true);
                            man.ImpulseVesselList[pair.Value.pid].is_holdspeed_enabled = false;
                            man.ImpulseVesselList[pair.Value.pid].is_pilotMode_enabled = false;
                        }

                        // Freeze in place if requested
                        if (man.ImpulseVesselList[pair.Value.pid].is_MakeSlowToSave_enabled && man.ImpulseVesselList[pair.Value.pid].is_fullHalt_enabled) { grav.makeSlowToSave(pair.Value.v, true, makeStationarySpeedClamp); }
                        LCARS_Utilities.CalculatePowerConsumption(Powerstats,pair.Value.v, true, false, false, int_FullImpulse_multiplier, int_UseReserves_multiplier, 0, 0, 0);
                    }
                }
            }







            //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 26 ");
            TB.Update_RenderBeams();
            WS.TorpedoUpdate();
            display_zSliderValue = zSliderValue;
            display_hSliderValue = hSliderValue;
            display_vSliderValue = vSliderValue;
            if (!this.bool_AccelerationLock)
            {
                ///UnityEngine.Debug.Log("ImpulseDrive: RESET SLIDERVALUES ");
                zSliderValue = 0;
                hSliderValue = 0;
                vSliderValue = 0;
                x = 0;
                x2 = 0;
                y = 0;
                y2 = 0;
                z = 0;
            }
            else
            {
                clickedx = false;
                clickedy = false;
                clickedz = false;
            }
            if ((Time.time - lastFixedUpdate) > logInterval)
            {
            }

        }



        /// <summary>
        /// Gui button to turn off the impulse drive will trigger this function
        /// </summary>
        public bool ToogleTheDrive()
        {
            gravityEnabled = !gravityEnabled;
            //VisibleShips_id[ActiveVessel_pid] = gravityEnabled;
            return gravityEnabled;
        }


        GUIStyle myStyle = null;
        StyleLib_LCARS_v3 styleLib = null;
        GUIStyle BackGroundLayoutStyle = null;
        GUIStyle BackGroundLayoutStyle2 = null;
        GUIStyle SubSystem_BackGroundLayoutStyle = null;
        GUIStyle SubSystem_BackGroundLayoutStyle2 = null;
        GUIStyle button_style_subsystemheader = null;
        GUIStyle scrollview_style;
        private Vector2 PowerSystem_SubSystem_ScrollPosition;
        public Texture2D GeneratorSliderBackgroundTexture = null;
        public Texture2D GeneratorSliderThumbTexture = null;
        private void OnGUI()
        {
            if (HighLogic.LoadedSceneIsEditor)
                return;

            if (!gravityEnabled)
                return;

            if (this.vessel == FlightGlobals.ActiveVessel)
            {

                if (styleLib==null)
                {
                    styleLib = new StyleLib_LCARS_v3();
                }
                if (SubSystem_BackGroundLayoutStyle == null)
                {
                    SubSystem_BackGroundLayoutStyle = new GUIStyle(GUI.skin.box);
                    SubSystem_BackGroundLayoutStyle.alignment = TextAnchor.UpperLeft;
                    SubSystem_BackGroundLayoutStyle.padding = new RectOffset(0, 0, 0, 0);
                    SubSystem_BackGroundLayoutStyle.margin = new RectOffset(0, 0, 0, 4);
                }
                if (SubSystem_BackGroundLayoutStyle2 == null)
                {
                    SubSystem_BackGroundLayoutStyle2 = new GUIStyle();
                    SubSystem_BackGroundLayoutStyle2.alignment = TextAnchor.UpperLeft;
                    SubSystem_BackGroundLayoutStyle2.padding = new RectOffset(0, 0, 0, 10);
                    SubSystem_BackGroundLayoutStyle2.margin = new RectOffset(2, 2, 0, 0);
                }
                if (button_style_subsystemheader == null)
                {
                    button_style_subsystemheader = new GUIStyle(GUI.skin.button);
                    button_style_subsystemheader.alignment = TextAnchor.UpperLeft;
                    
                }
                if (label_style_light == null)
                {
                    label_style_light = styleLib.label_style_light;

                }
                scrollview_style = new GUIStyle();

                bool_info_window = (bool_info_window_ship_info) ? bool_info_window_ship_info : bool_info_window;
                bool_info_window = (bool_info_window_transporter) ? bool_info_window_transporter : bool_info_window;
                bool_info_window = (bool_info_window_Tractorbeam) ? bool_info_window_Tractorbeam : bool_info_window;
                bool_info_window = (bool_info_window_Weappons) ? bool_info_window_Weappons : bool_info_window;
                bool_info_window = (bool_HoldSpeed_enabled) ? bool_HoldSpeed_enabled : bool_info_window;
                bool_info_window = (bool_HoldHeight_enabled) ? bool_HoldHeight_enabled : bool_info_window;
                bool_info_window = (bool_FormationMode) ? bool_FormationMode : bool_info_window;
                bool_info_window = (bool_info_window_fueltransfer) ? bool_info_window_fueltransfer : bool_info_window;
                bool_info_window = (bool_info_window_StructuralIntegrityField) ? bool_info_window_StructuralIntegrityField : bool_info_window;
                bool_info_window = (bool_info_window_CloakingDevice) ? bool_info_window_CloakingDevice : bool_info_window;
                bool_info_window = (bool_info_window_SensorArray) ? bool_info_window_SensorArray : bool_info_window;
                bool_info_window = (bool_info_window_PowerSystem) ? bool_info_window_PowerSystem : bool_info_window;

                if (
                        bool_info_window ||
                        bool_info_window_ship_info ||
                        bool_info_window_transporter ||
                        bool_info_window_Tractorbeam ||
                        bool_info_window_Weappons ||
                        bool_HoldSpeed_enabled ||
                        bool_HoldHeight_enabled ||
                        bool_FormationMode ||
                        bool_info_window_fueltransfer ||
                        bool_info_window_StructuralIntegrityField ||
                        bool_info_window_CloakingDevice ||
                        bool_info_window_SensorArray ||
                        bool_info_window_PowerSystem
                    )
                {
                    info_windowPosition_live = GUILayout.Window(123, info_windowPosition_live, info_Window, "Subsystems:");
                    //UnityEngine.Debug.Log("ImpulseDrive: OnGUI info_windowPosition=" + info_windowPosition);
                }

                myStyle = new GUIStyle();
                myStyle.margin = new RectOffset(0, 0, 0, 0);
                myStyle.padding = new RectOffset(0, 0, -11, 0);
                myStyle.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/blind", false);
                myStyle.onNormal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/blind", false);
                myStyle.onHover.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/blind", false);
                myStyle.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/blind", false);

                windowPosition = LCARS_Utilities.ClampToScreen(GUI.Window(windowID, windowPosition, Window, "", myStyle));

                    //windowPosition = GUI.Window(windowID, windowPosition, Window, "", myStyle);
                myStyle = null;
            /*
        */
            }
        }
        private void info_Window(int info_windowID)
        {
            
            /*
            Color colGUI = GUI.color;
            colGUI.a = 0.5f;
            GUI.color = colGUI;
            */
            
            //UnityEngine.Debug.Log("ImpulseDrive: info_Window begin info_windowID=" + info_windowID);
            if (HighLogic.LoadedSceneIsEditor)
                return;

            if (!gravityEnabled)
                return;

            OnInfoWindow();
            GUI.DragWindow();
            //UnityEngine.Debug.Log("ImpulseDrive: info_Window done info_windowID=" + info_windowID);
        }


        private void Window(int windowID)
        {

            /*
            Color colGUI = GUI.color;
            colGUI.a = 0f;
            GUI.color = colGUI;
            */
            if (HighLogic.LoadedSceneIsEditor)
                return;

            if (!gravityEnabled)
                return;
            OnWindow();
            GUI.DragWindow();
        }

        public virtual void OnInfoWindow()
        {
            if (styleLib == null)
            {
                styleLib = new StyleLib_LCARS_v3();
            }
            if (label_style_light == null)
            {
                label_style_light = styleLib.label_style_light;

            }

            //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow begin ");
            GUILayout.BeginVertical();


                texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_Main", false);
                GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                if (GUILayout.Button(texture, button_style_subsystemheader)) 
                {
                    this.bool_info_window_system_selector_expand = !this.bool_info_window_system_selector_expand; 
                }
                GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);

                if (this.bool_info_window_system_selector_expand)
                {

                    GUILayout.BeginHorizontal();
                        bool_info_window_ship_info = GUILayout.Toggle(bool_info_window_ship_info, "Ship");
                        bool_info_window_PowerSystem = GUILayout.Toggle(bool_info_window_PowerSystem, "Power");
                        bool_info_window_SensorArray = GUILayout.Toggle(bool_info_window_SensorArray, "Sensor");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                        bool_info_window_transporter = GUILayout.Toggle(bool_info_window_transporter, "Transp.");
                        bool_info_window_Tractorbeam = GUILayout.Toggle(bool_info_window_Tractorbeam, "Tractor");
                        bool_info_window_fueltransfer = GUILayout.Toggle(bool_info_window_fueltransfer, "Fueltrans.");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                        bool_info_window_Weappons = GUILayout.Toggle(bool_info_window_Weappons, "Tactical");
                        bool_info_window_CloakingDevice = GUILayout.Toggle(bool_info_window_CloakingDevice, "Cloak");
                        bool_info_window_StructuralIntegrityField = GUILayout.Toggle(bool_info_window_StructuralIntegrityField, "SIF");
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndVertical();
                /*
                            bool_info_window_SensorArray ||
                            bool_info_window_PowerSystem
                 * bool_info_window_system_selector_expand
                */

            //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow 1 ");
            if (this.bool_info_window_transporter)
            {
                texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_Transporter", false);
                GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                if (GUILayout.Button(texture, button_style_subsystemheader)) 
                {
                    this.bool_info_window_transporter_expand = !this.bool_info_window_transporter_expand; 
                }
                GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);

                if (this.bool_info_window_transporter_expand)
                {

                    if (VPI.getModules().ContainsKey("LCARS_TransporterSystem"))
                    {
                        //Debug.Log("ImpulseDrive: InfoWindow TS 1 ");
                        TS.SetMotherShip(this.vessel);
                        TS.GUI(new Rect(23, 180, 260, 260));
                        //Debug.Log("ImpulseDrive: InfoWindow TS 2 ");
                    }
                    else
                    {
                        GUILayout.Label("Transporter? They're not going to be installed until next Tuesday");
                    }
                }


                //GUILayout.Label("Transporter Room");



                GUILayout.EndVertical();
                GUILayout.EndVertical();
            }
            //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow 2 ");
            if (this.bool_info_window_PowerSystem)
            {
                texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_Power", false);
                GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                if (GUILayout.Button(texture, button_style_subsystemheader))
                {
                    this.bool_info_window_PowerSystem_expand = !this.bool_info_window_PowerSystem_expand;
                }
                GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                if (this.bool_info_window_PowerSystem_expand)
                {
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    GUILayout.Label("Generators:");

                    //GUI.skin.GetStyle("HorizontalSlider").normal.background = GeneratorSliderBackgroundTexture;

                    foreach (KeyValuePair<int,ModuleGenerator> pair in this.VPI.getResourceGenerators())
                    {
                        GUILayout.BeginVertical();
                        pair.Value.generatorIsActive = GUILayout.Toggle(pair.Value.generatorIsActive, "Generator "+(pair.Key+1)+" is active");
                        List<ModuleGenerator.GeneratorResource> MG_GR = pair.Value.outputList;
                        foreach(ModuleGenerator.GeneratorResource GR in MG_GR)
                        {
                            GUILayout.Label(GR.name + ": " + GR.rate);
                            //GR.rate = GUILayout.HorizontalSlider(GR.rate, 0F, MaxPowerGeneratorRate, "myslider", "mysliderThumb");
                            GR.rate = GUILayout.HorizontalSlider(GR.rate, 0F, MaxPowerGeneratorRate);
                            if (GR.name == "ElectricCharge")
                            {
                                if (GR.rate > CoreOverHeating_PowerRate)
                                {
                                    GUILayout.Label("WARNING: You are overheating the core");
                                    GUILayout.Label("Core temp grow rate: " + LCARS_ImpulseDrive_Part_add_heat);
                                }
                                float heat_percentage = this.VPI.getVesselAveragePartTemperature() / (this.VPI.getVesselAveragePartTemperatureMax() / 100);
                                GUILayout.Label("Vessel Temp Max: " + Math.Round(this.VPI.getVesselAveragePartTemperatureMax(), 2) + "");
                                GUILayout.Label("Vessel Temp avg: " + Math.Round(this.VPI.getVesselAveragePartTemperature(), 2) + "");
                                GUILayout.Label("Core Temp perc.: " + Math.Round(heat_percentage, 2) + "%");
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndVertical();
                    /*
                    GUISkin mySkin = GUI.skin;
                    GeneratorSliderThumbTexture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/GeneratorSliderThumbTexture", false);
                    GUI.skin.GetStyle("mysliderThumb").normal.background = GeneratorSliderThumbTexture;
                    GeneratorSliderBackgroundTexture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/GeneratorSliderBackgroundTexture", false);
                    GUI.skin.GetStyle("myslider").normal.background = GeneratorSliderBackgroundTexture;
                    */

                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    GUILayout.Label("Power Drain Shipwide:");
                    GUILayout.Label("Power Current: " + PowSys.get_consumption_total() / 100000 + " MW");
                    GUILayout.Label("Power Total: " + PowSys.get_consumption_total(true) / 100000 + " MW");
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    GUILayout.Label("Main Systems Total:");
                    GUILayout.Label("    " + PowSys.get_consumption_main_systems(true) / 100000 + " MW");
                    

                    if (GUILayout.Button("Show Details"))
                    {
                        this.bool_info_window_PowerSystem_expand_main = !this.bool_info_window_PowerSystem_expand_main;
                    }
                    if (this.bool_info_window_PowerSystem_expand_main)
                    {
                        foreach (KeyValuePair<string, PowerTaker> pair in PowSys.getPowerTakers())
                        {
                            if (pair.Value.takerType == "MainSystem")
                            {
                                GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                                GUILayout.BeginHorizontal(); GUILayout.Label(pair.Value.takerName);GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal(); GUILayout.Label("Current:"); GUILayout.FlexibleSpace(); GUILayout.Label("Total: "); GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal(); GUILayout.Label(pair.Value.consumption_current / 100000 + " MW");GUILayout.FlexibleSpace(); GUILayout.Label(pair.Value.consumption_total / 100000 + " MW"); GUILayout.EndHorizontal();
                                GUILayout.EndVertical();
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    GUILayout.Label("Sub Systems Total:");
                    GUILayout.Label("    " + PowSys.get_consumption_sub_systems(true) / 100000 + " MW");
                    if (GUILayout.Button("Show Details"))
                    {
                        this.bool_info_window_PowerSystem_expand_sub = !this.bool_info_window_PowerSystem_expand_sub;
                    }
                    if (this.bool_info_window_PowerSystem_expand_sub)
                    {

                        scrollview_style.fixedHeight = 190;

                        GUILayout.BeginVertical(scrollview_style);
                        PowerSystem_SubSystem_ScrollPosition = GUILayout.BeginScrollView(PowerSystem_SubSystem_ScrollPosition);
                        foreach (KeyValuePair<string, PowerTaker> pair in PowSys.getPowerTakers())
                        {
                            if (pair.Value.takerType == "SubSystem")
                            {
                                GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                                GUILayout.BeginHorizontal(); GUILayout.Label(pair.Value.takerName); GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal(); GUILayout.Label("Current:"); GUILayout.FlexibleSpace(); GUILayout.Label("Total: "); GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal(); GUILayout.Label(pair.Value.consumption_current / 100000 + " MW"); GUILayout.FlexibleSpace(); GUILayout.Label(pair.Value.consumption_total / 100000 + " MW"); GUILayout.EndHorizontal();
                                GUILayout.EndVertical();
                            }
                        }
                        GUILayout.EndScrollView();
                        GUILayout.EndVertical();
                    }                   
                    GUILayout.EndVertical();


                }


                GUILayout.EndVertical();
                GUILayout.EndVertical();

            }
            //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow 2 ");
            if (this.bool_info_window_ship_info)
            {
                texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_ShipInfo", false);
                GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                if (GUILayout.Button(texture, button_style_subsystemheader))
                {
                    this.bool_info_window_ship_info_expand = !this.bool_info_window_ship_info_expand;
                }
                GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                if (this.bool_info_window_ship_info_expand)
                {
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                            GUILayout.BeginHorizontal(); GUILayout.Label("Name: "); GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace(); GUILayout.Label("" + activeVessel.vesselName); GUILayout.EndHorizontal();
                            GUILayout.Label("Mass: " + Math.Round(activeVessel.GetTotalMass(), 2) + " t");
                            GUILayout.Label("DryMass: " + Math.Round(this.VPI.getVesselDryMass(), 2) + " t");
                            GUILayout.Label("Parts: " + activeVessel.parts.Count);
                            GUILayout.Label("Crew compliment current: " + activeVessel.GetCrewCount());
                            GUILayout.Label("Crew compliment max: " + activeVessel.GetCrewCapacity());
                            GUILayout.EndVertical();


                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                        if (VPI.getModules().ContainsKey("LCARS_CrewQuartier"))
                        {
                            GUILayout.Label("Crew Cabins total: ");
                            GUILayout.Label("Crew Cabins free: ");
                            GUILayout.Label("Crew Cabins used:");
                        }
                        if (VPI.getModules().ContainsKey("LCARS_CargoBay"))
                        {
                            GUILayout.Label("CargoBay total: ");
                            GUILayout.Label("CargoBay free:");
                            GUILayout.Label("CargoBay used:");
                        }
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                        if (VPI.getModules().ContainsKey("LCARS_CrewQuartier"))
                        {
                            GUILayout.Label("" + STCQ.getCrewQuartersTotal());
                            GUILayout.Label("" + STCQ.getFreeCrewSpace());
                            GUILayout.Label("" + STCQ.getCrewQuartersUsed());
                        }
                        //VPI.getModules().ContainsKey("LCARS_ShuttleBay") && 
                        if (VPI.getModules().ContainsKey("LCARS_CargoBay"))
                        {
                            GUILayout.Label("" + Math.Round(STCB.getTotalCargoSpace(), 2) + " t");
                            GUILayout.Label("" + Math.Round(STCB.getFreeCargoSpace(), 2) + " t");
                            GUILayout.Label("" + Math.Round(STCB.getUsedCargoSpace(), 2) + " t");
                        }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    if (!VPI.getModules().ContainsKey("LCARS_CrewQuartier"))
                    {
                            GUILayout.Label("CrewQuartiers? They're not going to be installed until next Tuesday");
                    }
                    //!VPI.getModules().ContainsKey("LCARS_ShuttleBay") || 
                    if (!VPI.getModules().ContainsKey("LCARS_CargoBay"))
                    {
                            GUILayout.Label("CargoBays? They're not going to be installed until next Tuesday");
                    }

                    /*
                    bool_info_window_fueltransfer = GUILayout.Toggle(bool_info_window_fueltransfer, "LCARS_FuelTransfer");
                    if (GUILayout.Button("VPI.print_Modules"))
                    {
                        VPI.print_Modules();
                    }
                    */
                }


                GUILayout.EndVertical();
                GUILayout.EndVertical();
                




        /*
        Experiments
        */
                    /*
                    // BiomMap
                    CBAttributeMap map = FlightGlobals.ActiveVessel.mainBody.BiomeMap;
                    Texture2D map2 = map.Map;
                    GUILayout.BeginHorizontal(map2, SubSystem_BackGroundLayoutStyle, GUILayout.Width(420));
                    GUILayout.EndHorizontal();
                    // Biom Map

                    // Planet Scanner
                    planetScanner.activateScanner(zoomFactor);
                    GUILayout.Label("zoomFactor: " + Math.Round(zoomFactor, 2));
                    zoomFactor = GUILayout.HorizontalSlider(zoomFactor, 1.01F, 100.0F);
                    // Planet Scanner
                    //planetScanner = null;
                    */
        /*
        Experiments
        */





                }
            //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow 3 ");
                if (this.bool_FormationMode)
                {
                    texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_FormationData", false);
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                    if (GUILayout.Button(texture, button_style_subsystemheader))
                    {
                        this.bool_FormationMode_expand = !this.bool_FormationMode_expand;
                    }
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    if (this.bool_FormationMode_expand)
                    {
                        GUILayout.Label("Formation Info:");
                        int vCounter = 0;
                        foreach (KeyValuePair<string, LCARS_ImpulseVesselType> pair in man.getImpulseVesselList())
                        {
                            if (pair.Value.is_gravity_enabled && !man.ImpulseVesselList[pair.Value.pid].is_active_vessel)
                            {
                                if (!pair.Value.is_ignoreformationcommandos_enabled)
                                {
                                    vCounter++;
                                    string vN = pair.Value.v.vesselName;
                                    float dist = Vector3.Distance(this.vessel.findWorldCenterOfMass(), pair.Value.v.findWorldCenterOfMass());
                                    GUILayout.Label(vCounter + " " + vN + " : " + Math.Round(dist, 2)+"m");

                                }
                            }
                        }                    
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                }
                //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow 4 ");

                if (this.bool_HoldSpeed_enabled)
                {
                    texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_Speed", false);
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                    if (GUILayout.Button(texture, button_style_subsystemheader))
                    {
                        this.bool_HoldSpeed_enabled_expand = !this.bool_HoldSpeed_enabled_expand;
                    }
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    if (this.bool_HoldSpeed_enabled_expand)
                    {
                        GUILayout.Label("Set Required Speed");
                        HoldSpeed_value = Double.Parse(GUILayout.TextField(Math.Round(HoldSpeed_value, 2).ToString(), 25));
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                }
                //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow 5 ");
                if (this.bool_HoldHeight_enabled)
                {
                    texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_Height", false);
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                    if (GUILayout.Button(texture, button_style_subsystemheader))
                    {
                        this.bool_HoldHeight_enabled_expand = !this.bool_HoldHeight_enabled_expand;
                    }
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    if (this.bool_HoldHeight_enabled_expand)
                    {
                        GUILayout.Label("Set Required Height");
                            HoldHeight_value = Double.Parse(GUILayout.TextField(Math.Round(HoldHeight_value,2).ToString(), 25));
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                }
                //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow 6 ");
                if (this.bool_info_window_Tractorbeam)
                {
                    texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_Tractor", false);
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                    if (GUILayout.Button(texture, button_style_subsystemheader))
                    {
                        this.bool_info_window_Tractorbeam_expand = !this.bool_info_window_Tractorbeam_expand;
                    }
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    if (this.bool_info_window_Tractorbeam_expand)
                    {
                        //GUILayout.Label("Tractorbeam Console");

                        if (VPI.getModules().ContainsKey("LCARS_TractorBeam"))
                        {

                            GUILayout.Label("Ship Selector");
                            TB.Draw_ShipSelector();

                            GUILayout.Label("");

                            GUILayout.Label("Beam List");
                            TB.Draw_BeamList();

                        }
                        else
                        {
                            GUILayout.Label("Tractor Beam? They're not going to be installed until next Tuesday");
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                }
                //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow 6b ");
                if (this.bool_info_window_StructuralIntegrityField)
                {
                    reset_StructuralIntegrityField_run_once = false;

                    texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_SIF", false);
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                    if (GUILayout.Button(texture, button_style_subsystemheader))
                    {
                        this.bool_info_window_StructuralIntegrityField_expand = !this.bool_info_window_StructuralIntegrityField_expand;
                    }
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    if (this.bool_info_window_StructuralIntegrityField_expand)
                    {
                        GUILayout.Label("Partially ToDo");

                        if (VPI.getModules().ContainsKey("LCARS_StructuralIntegrityField"))
                        {
                            GUILayout.Label("Set Force: " + Math.Round(SIF_force,2));
                            SIF_force = GUILayout.HorizontalSlider(SIF_force, 0.0F, 100.0F);
                            this.SIF.set_StructuralIntegrityField(SIF_force);

                        }
                        else
                        {
                            GUILayout.Label("Structural Integrity Field? They're not going to be installed until next Tuesday");
                        }
                        /*
                        GUILayout.Label("StructuralIntegrityField? We have one, but it's fused..");
                        */

                    }
                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                }
                else
                {
                    if (VPI.getModules().ContainsKey("LCARS_StructuralIntegrityField"))
                    {
                        if (!reset_StructuralIntegrityField_run_once)
                        {
                            this.SIF.reset_StructuralIntegrityField();
                            reset_StructuralIntegrityField_run_once = true;
                        }
                    }
                }
                //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow 6s ");
                if (this.bool_info_window_CloakingDevice)
                {
                    texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_Cloak", false);
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                    if (GUILayout.Button(texture, button_style_subsystemheader))
                    {
                        this.bool_info_window_CloakingDevice_expand = !this.bool_info_window_CloakingDevice_expand;
                    }
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    if (this.bool_info_window_CloakingDevice_expand)
                    {
                        GUILayout.Label("ToDo");
                        //GUILayout.Label("Tractorbeam Console");
                        if (VPI.getModules().ContainsKey("LCARS_CloakingDevice"))
                        {
                            GUILayout.Label("Set Force: " + Math.Round(CD_force, 2));
                            CD_force = GUILayout.HorizontalSlider(CD_force, 1.0F, 0.0F);
                            this.CD.set_opacity(SIF_force);

                        }
                        else
                        {
                            GUILayout.Label("CloakingDevice? They're not going to be installed until next Tuesday");
                        }
                        /*
                        GUILayout.Label("CloakingDevice? We have one, but it's fused..");
                        */

                    }
                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                }
                //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow 7 ");
                if (this.bool_info_window_SensorArray)
                {
                    texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_SensorArray", false);
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                    if (GUILayout.Button(texture, button_style_subsystemheader))
                    {
                        this.bool_info_window_SensorArray_expand = !this.bool_info_window_SensorArray_expand;
                    }
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    if (this.bool_info_window_SensorArray_expand)
                    {
                        //GUILayout.Label("Tractorbeam Console");

                        if (VPI.getModules().ContainsKey("LCARS_SensorArray"))
                        {

                            GUILayout.Label("Partially ToDo");
                            SA.GUI();
                        }
                        else
                        {
                            GUILayout.Label("Sensor Array? They're not going to be installed until next Tuesday");
                        }
                    }


                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                }
                //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow 8 ");
                if (this.bool_info_window_Weappons)
                {
                    texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_Tactical", false);
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                    if (GUILayout.Button(texture, button_style_subsystemheader))
                    {
                        this.bool_info_window_Weappons_expand = !this.bool_info_window_Weappons_expand;
                    }
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    if (this.bool_info_window_Weappons_expand)
                    {
                    //GUILayout.Label("Tractorbeam Console");

                        if (VPI.getModules().ContainsKey("LCARS_WeaponSystems"))
                        {

                            GUILayout.Label("Ship Selector");
                            WS.Draw_ShipSelector();
                    
                            GUILayout.Label("");

                            GUILayout.Label("Target List");
                            WS.TorpedosInstalled(VPI.getModules().ContainsKey("LCARS_PhotonTorpedo"));
                            WS.Draw_TargetList();

                        }
                        else
                        {
                            GUILayout.Label("Weapon Systems? They're not going to be installed until next Tuesday");
                        }
                    }


                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                }
                //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow 9 ");
                if (this.bool_info_window_fueltransfer)
                {
                    texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/SubSystems_Fueltransfer", false);
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle);
                    if (GUILayout.Button(texture, button_style_subsystemheader))
                    {
                        this.bool_info_window_fueltransfer_expand = !this.bool_info_window_fueltransfer_expand;
                    }
                    GUILayout.BeginVertical(SubSystem_BackGroundLayoutStyle2);
                    if (this.bool_info_window_fueltransfer_expand)
                    {
                        //GUILayout.Label("Tractorbeam Console");

                        if (VPI.getModules().ContainsKey("LCARS_FuelTransfer"))
                        {

                            GUILayout.Label("Partially ToDo");
                            FT.GUI();

                        }
                        else
                        {
                            GUILayout.Label("Fuel Transfer Systems? They're not going to be installed until next Tuesday");
                        }
                    }


                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                }

                if (!this.bool_HoldHeight_enabled && !this.bool_HoldSpeed_enabled && !this.bool_FormationMode)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Close"))
                    {
                        bool_info_window = false;
                        bool_info_window_ship_info = false;
                        bool_info_window_transporter = false;
                        bool_info_window_Tractorbeam = false;
                        bool_info_window_Weappons  = false;
                        bool_HoldSpeed_enabled  = false;
                        bool_HoldHeight_enabled  = false;
                        bool_FormationMode  = false;
                        bool_info_window_fueltransfer  = false;
                        bool_info_window_StructuralIntegrityField  = false;
                        bool_info_window_CloakingDevice  = false;
                        bool_info_window_SensorArray = false;

                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                //UnityEngine.Debug.Log("ImpulseDrive: OnInfoWindow done ");
        }


        /// <summary>
        /// Put all of the code needed for the drawing of your window components in here.
        /// </summary>
        public virtual void OnWindow()
        {
            activeVessel = this.vessel;

            string pid = activeVessel.id.ToString();

            if (HighLogic.LoadedSceneIsEditor)
                return;

            //if (!VisibleShips_id[pid])
            //    return;

            if (vessel.rigidbody == null)
                return;

 
            resetSlider = true;
 
            var mousePosition = UnityEngine.Input.mousePosition;
            // mainslider
            x = (mousePosition.x - windowPosition.x - mainslider_buttonPosition.x) - 100;
            y = ((mousePosition.y - (Screen.height - windowPosition.y - mainslider_buttonPosition.y)) + 100);
            
            // left right slider
            x2 = (mousePosition.x - windowPosition.x - LeftRightslider_buttonPosition.x) - 100;
            // forward slider
            y2 = ((mousePosition.y - (Screen.height - windowPosition.y - forwardslider_buttonPosition.y)) + 100);
            
            // up down slider
            z = ((mousePosition.y - (Screen.height - windowPosition.y - UpDownslider_buttonPosition.y)) + 100);

            x = (x <= deadzone_pixel && x >= -deadzone_pixel) ? 0 : x;
            x = (x >= deadzone_pixel) ? x - deadzone_pixel : x;
            x = (x <= -deadzone_pixel) ? x + deadzone_pixel : x;
            x =  x * 1.1F;

            x2 = (x2 <= deadzone_pixel && x2 >= -deadzone_pixel) ? 0 : x2;
            x2 = (x2 >= deadzone_pixel) ? x2 - deadzone_pixel : x2;
            x2 = (x2 <= -deadzone_pixel) ? x2 + deadzone_pixel : x2;
            x2 = x2 * 1.1F;

            y = (y <= deadzone_pixel && y >= -deadzone_pixel) ? 0 : y;
            y = (y >= deadzone_pixel) ? y - deadzone_pixel : y;
            y = (y <= -deadzone_pixel) ? y + deadzone_pixel : y;
            y = y * 1.1F;

            y2 = (y2 <= deadzone_pixel && y2 >= -deadzone_pixel) ? 0 : y2;
            y2 = (y2 >= deadzone_pixel) ? y2 - deadzone_pixel : y2;
            y2 = (y2 <= -deadzone_pixel) ? y2 + deadzone_pixel : y2;
            y2 = y2 * 1.1F;

            z = (z <= deadzone_pixel && z >= -deadzone_pixel) ? 0 : z;
            z = (z >= deadzone_pixel) ? z - deadzone_pixel : z;
            z = (z <= -deadzone_pixel) ? z + deadzone_pixel : z;
            z = z * 1.1F;

            // The gui is hidden here
            window_gui_content_LCARS_v3();
            // The gui is hidden here

        }



        
        /// <summary>
        /// Window stuff GUI etc..
        /// </summary>
        private Rect Label_X_Position;
        private Rect Label_Y_Position;
        private Rect Label_Z_Position;
        private Rect Label_Charge_Position;
        private Rect Label_Force_Position;

        private Rect fullhalt_togglePosition;
        private Rect MakeSlowToSave_togglePosition;
        private Rect YouCanSaveNow_togglePosition;
        private Rect fullhalt_buttonPosition;
        private Rect alock_togglePosition;
        private Rect fullimpulse_togglePosition;
        private Rect PilotMode_togglePosition;
        private Rect formationMode_togglePosition;
        private Rect IgnoreFormationCommandos_togglePosition;
        private Rect UseReserves_togglePosition;
        private Rect HoldSpeed_togglePosition;
        private Rect HoldHeight_togglePosition;

        private Rect Disengage_buttonPosition;
        private Rect mainslider_buttonPosition;
        private Rect forwardslider_buttonPosition;
        private Rect UpDownslider_buttonPosition;
        private Rect LeftRightslider_buttonPosition;

        private GUIStyle toggle_style_AccelerationLock;
        private GUIStyle toggle_style_fullHalt;
        private GUIStyle toggle_style_MakeSlowToSave;
        private GUIStyle toggle_style_YouCanSaveNow;
        private GUIStyle toggle_style_EndSaveMode;
        private GUIStyle toggle_style_FullImpulse;
        private GUIStyle toggle_style_formationMode;
        private GUIStyle toggle_style_IgnoreFormation;
        private GUIStyle toggle_style_pilotmode;
        private GUIStyle toggle_style_UseReserves;
        private GUIStyle toggle_style_HoldSpeed;
        private GUIStyle toggle_style_HoldHeight;

        private GUIContent content;
        private Texture2D texture;
        private GUIStyle iconStyle;
        private GUIStyle button_style_fullHalt;
        private GUIStyle label_style_dark;
        private GUIStyle label_style_light;
        private GUIStyle toggle_style_dark;
        private GUIStyle paddingTopStyle;


        public void window_gui_content_LCARS_v3()
        {
            if (styleLib == null)
            {
                styleLib = new StyleLib_LCARS_v3();
            }
            if (label_style_light == null)
            {
                label_style_light = styleLib.label_style_light;

            }
            // 285/15
            Label_X_Position = new Rect(293, 10, 47, 15);
            Label_Y_Position = new Rect(293, 23, 47, 15);
            Label_Z_Position = new Rect(293, 36, 47, 15);
            Label_Force_Position = new Rect(54, 190, 97, 15);
            Label_Charge_Position = new Rect(54, 205, 97, 15);

            PilotMode_togglePosition = new Rect(14, 38, 97, 11);
            alock_togglePosition = new Rect(14, 49, 97, 11);
            HoldSpeed_togglePosition = new Rect(14, 60, 97, 11);
            HoldHeight_togglePosition = new Rect(14, 71, 97, 11);
            fullhalt_buttonPosition = new Rect(14, 82, 97, 11);
            fullhalt_togglePosition = new Rect(14, 93, 97, 11);
            MakeSlowToSave_togglePosition = new Rect(14, 104, 97, 11);
            YouCanSaveNow_togglePosition = new Rect(14, 115, 97, 11);



            Disengage_buttonPosition = new Rect(14, 133, 97, 11);
            fullimpulse_togglePosition = new Rect(14, 144, 97, 11);
            UseReserves_togglePosition = new Rect(14, 155, 97, 11);

            formationMode_togglePosition = new Rect(14, 168, 97, 11);
            IgnoreFormationCommandos_togglePosition = new Rect(14, 179, 97, 11);
            
            mainslider_buttonPosition = new Rect(132, 8, 200, 200);
            forwardslider_buttonPosition = new Rect(115, 8, 15, 200);
            UpDownslider_buttonPosition = new Rect(335, 8, 15, 200);
            LeftRightslider_buttonPosition = new Rect(132, 208, 200, 15);

            styleLib.Style();

            if (toggle_style_pilotmode == null)
            {
                toggle_style_pilotmode = styleLib.toggle_style_pilotmode;
            }
            toggle_style_pilotmode.active.background = (bool_PilotMode) ? GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_pilotmode_icon_active, false) : GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_pilotmode_icon, false);

            if (toggle_style_AccelerationLock == null)
            {
                toggle_style_AccelerationLock = styleLib.toggle_style_AccelerationLock;
            }
            toggle_style_AccelerationLock.active.background = (bool_AccelerationLock) ? GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_ALock_icon_active, false) : GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_ALock_icon, false);

            if (toggle_style_HoldSpeed == null)
            {
                toggle_style_HoldSpeed = styleLib.toggle_style_HoldSpeed;
            }
            toggle_style_HoldSpeed.active.background = (bool_UseReserves) ? GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_HoldSpeed_icon_active, false) : GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_HoldSpeed_icon, false);
            
            if (toggle_style_HoldHeight == null)
            {
                toggle_style_HoldHeight = styleLib.toggle_style_HoldHeight;
            }
            toggle_style_HoldHeight.active.background = (bool_UseReserves) ? GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_HoldHeight_icon_active, false) : GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_HoldHeight_icon, false);

            if (toggle_style_fullHalt == null)
            {
                toggle_style_fullHalt = styleLib.toggle_style_fullHalt;
            }
            toggle_style_fullHalt.active.background = (bool_FullHalt) ? GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_FullHalt_icon_active, false) : GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_FullHalt_icon, false);
            
            if (toggle_style_MakeSlowToSave == null)
            {
                toggle_style_MakeSlowToSave = styleLib.toggle_style_MakeSlowToSave;
            }
            toggle_style_MakeSlowToSave.active.background = (bool_MakeSlowToSave) ? GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_MakeSlowToSave_icon_active, false) : GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_MakeSlowToSave_icon, false);
            
            if (toggle_style_YouCanSaveNow == null)
            {
                toggle_style_YouCanSaveNow = styleLib.toggle_style_YouCanSaveNow;
            }
            toggle_style_YouCanSaveNow.active.background = (bool_MakeSlowToSave) ? GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_YouCanSaveNow_icon_active, false) : GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_YouCanSaveNow_icon_active, false);
            if (toggle_style_EndSaveMode == null)
            {
                toggle_style_EndSaveMode = styleLib.toggle_style_EndSaveMode;
            }
            
            if (toggle_style_FullImpulse == null)
            {
                toggle_style_FullImpulse = styleLib.toggle_style_FullImpulse;
            }
            toggle_style_FullImpulse.active.background = (bool_FullImpulse) ? GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_FullImpulse_icon_active, false) : GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_FullImpulse_icon, false);
            
            if (toggle_style_UseReserves == null)
            {
                toggle_style_UseReserves = styleLib.toggle_style_UseReserves;
            }
            toggle_style_UseReserves.active.background = (bool_UseReserves) ? GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_UseReserves_icon_active, false) : GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_UseReserves_icon, false);
            
            if (toggle_style_formationMode == null)
            {
                toggle_style_formationMode = styleLib.toggle_style_formationMode;
            }
            toggle_style_formationMode.active.background = (bool_FormationMode) ? GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_formation_icon_active, false) : GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_formation_icon, false);

            if (toggle_style_IgnoreFormation == null)
            {
                toggle_style_IgnoreFormation = styleLib.toggle_style_IgnoreFormation;
            }
            toggle_style_IgnoreFormation.active.background = (bool_IgnoreFormationCommandos) ? GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_IgnoreFormation_icon_active, false) : GameDatabase.Instance.GetTexture(styleLib.LCARSToggle_IgnoreFormation_icon, false);


            if (BackGroundLayoutStyle == null)
            {
                BackGroundLayoutStyle = styleLib.BackGroundLayoutStyle;
                BackGroundLayoutStyle.normal.background = GameDatabase.Instance.GetTexture(styleLib.BackGroundLayout_image, false);
                BackGroundLayoutStyle.onNormal.background = GameDatabase.Instance.GetTexture(styleLib.BackGroundLayout_image, false);
                BackGroundLayoutStyle.onHover.background = GameDatabase.Instance.GetTexture(styleLib.BackGroundLayout_image, false);
                BackGroundLayoutStyle.normal.background = GameDatabase.Instance.GetTexture(styleLib.BackGroundLayout_image, false);
            }
            if (button_style_fullHalt == null)
            {
                button_style_fullHalt = styleLib.button_style_fullHalt;
            }
            if (label_style_light == null)
            {
                label_style_light = styleLib.label_style_light;

            }


            activeVessel = this.vessel;
            ActiveVessel_pid = activeVessel.id.ToString();
            clickedx = false;
            clickedy = false;
            clickedz = false;
            // main
            GUILayout.BeginVertical(BackGroundLayoutStyle, GUILayout.Width(375), GUILayout.Height(230));


            texture = null;
            content = null;
            texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/ShipInfoIcon", false);
            content = new GUIContent(texture, "Ship Info");
            bool_info_window_ship_info = GUI.Toggle(new Rect(21, 3, 9, 14), bool_info_window_ship_info, content, label_style_light);

            texture = null;
            content = null;
            texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/TractorBeamIcon", false);
            content = new GUIContent(texture, "Ship Info");
            bool_info_window_Tractorbeam = GUI.Toggle(new Rect(2, 23, 16, 14), bool_info_window_Tractorbeam, content, label_style_light);

            texture = null;
            content = null;
            texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/TacticalIcon", false);
            content = new GUIContent(texture, "Ship Info");
            bool_info_window_Weappons = GUI.Toggle(new Rect(20, 23, 14, 14), bool_info_window_Weappons, content, label_style_light);

            texture = null;
            content = null;
            texture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/TransporterRoomIcon", false);
            content = new GUIContent(texture, "Ship Info");
            bool_info_window_transporter = GUI.Toggle(new Rect(36, 23, 11, 14), bool_info_window_transporter, content, label_style_light);

            
                bool_PilotMode = GUI.Toggle(PilotMode_togglePosition, bool_PilotMode, "", toggle_style_pilotmode);
                if (bool_PilotMode)
                {
                    man.ImpulseVesselList[ActiveVessel_pid].is_pilotMode_enabled = bool_PilotMode;
                    this.bool_HoldHeight_enabled = false;
                }

                bool_AccelerationLock = GUI.Toggle(alock_togglePosition, bool_AccelerationLock, "", toggle_style_AccelerationLock);
                if (bool_AccelerationLock)
                {
                    man.ImpulseVesselList[ActiveVessel_pid].is_accelerationLock_enabled = bool_AccelerationLock;
                    this.bool_HoldSpeed_enabled = false;
                    this.bool_HoldHeight_enabled = false;
                }

                bool_HoldSpeed_enabled = GUI.Toggle(HoldSpeed_togglePosition, bool_HoldSpeed_enabled, "", toggle_style_HoldSpeed);
                if (this.bool_HoldSpeed_enabled)
                {
                    if (this.HoldSpeed_value == 0.00F)
                    {
                        this.HoldSpeed_value = this.vessel.horizontalSrfSpeed;
                    }
                    this.bool_AccelerationLock = false;
                }
                else
                {
                    this.HoldSpeed_value = 0.00F;
                }
                bool_HoldHeight_enabled = GUI.Toggle(HoldHeight_togglePosition, bool_HoldHeight_enabled, "", toggle_style_HoldHeight);
                if (this.bool_HoldHeight_enabled)
                {
                    if (this.HoldHeight_value == 0.00F)
                    {
                        Vector3 CoM = this.vessel.findWorldCenterOfMass();
                        double altitudeASL = this.vessel.mainBody.GetAltitude(CoM);

                        this.HoldHeight_value = altitudeASL;
                    }
                    this.bool_PilotMode = false;
                    this.bool_AccelerationLock = false;
                }
                else
                {
                    this.HoldHeight_value = 0.00F;
                }
                
                bool_FullHalt = GUI.Toggle(fullhalt_togglePosition, bool_FullHalt, "", toggle_style_fullHalt);
                if (bool_FullHalt)
                {
                    man.ImpulseVesselList[ActiveVessel_pid].is_fullHalt_enabled = bool_FullHalt;
                    this.bool_PilotMode = false;
                    this.bool_AccelerationLock = false;
                    this.bool_HoldSpeed_enabled = false;
                }
                
                bool_SlowDown = GUI.RepeatButton(fullhalt_buttonPosition, "", button_style_fullHalt);
                if (bool_FullHalt)
                {
                    if (grav.checkSlowToSave(activeVessel, makeStationarySpeedMax))
                    {
                        if (!bool_MakeSlowToSave)
                        {
                            bool_MakeSlowToSave = GUI.Toggle(MakeSlowToSave_togglePosition, bool_MakeSlowToSave, "", toggle_style_MakeSlowToSave);
                        }
                        else
                        {
                            if (GUI.Button(MakeSlowToSave_togglePosition, "", toggle_style_EndSaveMode))
                            {
                                grav.SatLandedFalse(activeVessel);
                                bool_MakeSlowToSave = false;
                                bool_FullHalt = false;
                            }
                        }
                    }
                    else
                    {
                        bool_MakeSlowToSave = false;
                        if (activeVessel.Landed == true)
                        {
                            //activeVessel.Landed = false;
                        }
                    }
                    if (bool_MakeSlowToSave)
                    {
                        if (grav.IsSlowToSave(activeVessel, makeStationarySpeedClamp))
                        {
                            //GUILayout.FlexibleSpace();
                            //GUILayout.Label("YOU CAN SAVE NOW");
                            GUI.Toggle(YouCanSaveNow_togglePosition, true, "", toggle_style_YouCanSaveNow);


                        }
                    }
                }
                else
                {
                    bool_MakeSlowToSave = false;
                }




                texture = null;
                content = null;
                texture = GameDatabase.Instance.GetTexture(styleLib.LCARSButton_Disengage_icon, false);
                content = new GUIContent(texture, "Disengage");
                if (GUI.Button(Disengage_buttonPosition, content, styleLib.iconStyle))
                {
                    man.ImpulseVesselList[ActiveVessel_pid].is_gravity_enabled = !man.ImpulseVesselList[ActiveVessel_pid].is_gravity_enabled;
                    ToogleTheDrive();
                }
                bool_FullImpulse = GUI.Toggle(fullimpulse_togglePosition, bool_FullImpulse, "", toggle_style_FullImpulse);
                if (bool_FullImpulse)
                {
                    man.ImpulseVesselList[ActiveVessel_pid].is_fullImpulse_enabled = bool_FullImpulse;
                }
                if (this.bool_FullImpulse)
                {
                    bool_UseReserves = GUI.Toggle(UseReserves_togglePosition, bool_UseReserves, "", toggle_style_UseReserves);
                }


                bool_FormationMode = GUI.Toggle(formationMode_togglePosition, bool_FormationMode, "", toggle_style_formationMode);

                bool_IgnoreFormationCommandos = GUI.Toggle(IgnoreFormationCommandos_togglePosition, bool_IgnoreFormationCommandos, "", toggle_style_IgnoreFormation);



                texture = null;
                content = null;
                texture = GameDatabase.Instance.GetTexture(styleLib.ImpulseDrive_mainslider_icon, false);
                content = new GUIContent(texture, "");
                if (GUI.RepeatButton(mainslider_buttonPosition, content, styleLib.iconStyle))
                {
                    // This code is executed every frame that the RepeatButton remains clicked
                    clickedx = true;
                    clickedy = true;
                    vSliderValue = y + deadzone_pixel;
                    hSliderValue = x + deadzone_pixel;
                    resetSlider = false;
                }
                texture = null;
                content = null;
                texture = GameDatabase.Instance.GetTexture(styleLib.ImpulseDrive_forwardslider_icon, false);
                content = new GUIContent(texture, "");
                if (GUI.RepeatButton(forwardslider_buttonPosition, content, styleLib.iconStyle))
                {
                    // This code is executed every frame that the RepeatButton remains clicked
                    clickedy = true;
                    vSliderValue = y2 + deadzone_pixel;
                    //vSliderValue = (bool_FullImpulse) ? vSliderValue * int_FullImpulse_multiplier : vSliderValue;
                    hSliderValue = 0;
                    resetSlider = false;
                }
                texture = null;
                content = null;
                texture = GameDatabase.Instance.GetTexture(styleLib.ImpulseDrive_LeftRightslider_icon, false);
                content = new GUIContent(texture, "");
                if (GUI.RepeatButton(LeftRightslider_buttonPosition, content, styleLib.iconStyle))
                {
                    // This code is executed every frame that the RepeatButton remains clicked
                    clickedx = true;
                    vSliderValue = 0;
                    hSliderValue = x2 + deadzone_pixel;
                    //hSliderValue = (bool_FullImpulse) ? hSliderValue * int_FullImpulse_multiplier : hSliderValue;
                    resetSlider = false;
                }
                texture = null;
                content = null;
                texture = GameDatabase.Instance.GetTexture(styleLib.ImpulseDrive_UpDownSlider_icon, false);
                content = new GUIContent(texture, "");
                if (GUI.RepeatButton(UpDownslider_buttonPosition, content, styleLib.iconStyle))
                {
                    // This code is executed every frame that the RepeatButton remains clicked
                    clickedz = true;
                    zSliderValue = z + deadzone_pixel * -1;
                    //zSliderValue = (bool_FullImpulse) ? zSliderValue * int_FullImpulse_multiplier : zSliderValue;
                    resetSlider = false;
                }
                float dx;
                float dy;
                float dz;
                if (!clickedx)
                {
                    dx = 0;
                }
                else
                {
                    dx = (x < -100) ? 0 : x;
                    dx = (x > 100) ? 0 : x;
                }
                if (!clickedy)
                {
                    dy = 0;
                }
                else
                {
                    dy = (y < -100) ? 0 : y;
                    dy = (y > 100) ? 0 : y;
                }
                if (!clickedz)
                {
                    dz = 0;
                }
                else
                {
                    dz = (z < -100) ? 0 : z;
                    dz = (z > 100) ? 0 : z;
                }
                GUI.Box(Label_X_Position, "X:" + Math.Round(dx, 1), label_style_light);
            GUI.Box(Label_Y_Position, "Y:" + Math.Round(dy, 1), label_style_light);
            GUI.Box(Label_Z_Position, "Z:" + Math.Round(dz, 1), label_style_light);
            GUI.Box(Label_Force_Position, "F:" + Math.Round(total_force, 2), label_style_light);
            GUI.Box(Label_Charge_Position, "C:" + Math.Round(charge, 2), label_style_light);
            
            GUILayout.EndVertical();

        //clean up
            texture = null;
            content = null;
            styleLib.style_cleanup();
            styleLib = null;
            toggle_style_pilotmode = null;
            toggle_style_AccelerationLock = null;
            toggle_style_HoldSpeed = null;
            toggle_style_HoldHeight = null;
            toggle_style_fullHalt = null;
            toggle_style_MakeSlowToSave = null;
            toggle_style_YouCanSaveNow = null;
            toggle_style_EndSaveMode = null;
            toggle_style_FullImpulse = null;
            toggle_style_UseReserves = null;
            toggle_style_formationMode = null;
            toggle_style_IgnoreFormation = null;
            BackGroundLayoutStyle = null;
            button_style_fullHalt = null;
            label_style_light = null;


        
        }






        /// <summary>
        /// Gets or sets the position of the window.
        /// </summary>
        public Rect WindowPosition
        {
            get { return windowPosition; }
            set { windowPosition = value; }
        }

        /// <summary>
        /// Gets the WindowID associated with the window.
        /// </summary>
        public int WindowID
        {
            get { return windowID; }
        }

        /// <summary>
        /// Gets or sets the title used for your window.
        /// </summary>
        public string WindowTitle
        {
            get { return windowTitle; }
            set { windowTitle = value; }
        }

    }

}
