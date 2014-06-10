/*
 * 
 * StarTrekImpulseDrive by Philotical
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

    public class ImpulseVesselType
    {
        public string name { get; set; }
        public string pid { get; set; }
        public Rect windowPosition { get; set; }
        public Vector3 geeVector { get; set; }
        public bool is_active_vessel { get; set; }
        public bool is_gravity_enabled { get; set; }
        public bool is_fullHalt_enabled { get; set; }
        public bool is_MakeSlowToSave_enabled { get; set; }
        public bool is_fullImpulse_enabled { get; set; }
        public bool is_UseReserves_enabled { get; set; }
        public bool is_accelerationLock_enabled { get; set; }
        public bool is_pilotMode_enabled { get; set; }
        public bool is_formationMode_enabled { get; set; }
        public bool is_ignoreformationcommandos_enabled { get; set; }
        public bool is_holdspeed_enabled { get; set; }
        public double HoldSpeed_value { get; set; }
        public bool is_holdheight_enabled { get; set; }
        public double HoldHeight_value { get; set; }
        public Vessel v { get; set; }
    }

    public class StarTrekImpulseDrive : PartModule
	{
        private PluginConfiguration pluginConfig = PluginConfiguration.CreateForType<StarTrekImpulseDrive>();
        private int windowID = new System.Random().Next();
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
        int int_FullImpulse_multiplier = 5;
        int int_UseReserves_multiplier = 2;
        bool bool_AccelerationLock = false;
        bool bool_PilotMode = false;
        bool bool_FormationMode = false;
        bool bool_IgnoreFormationCommandos = false;
        bool bool_HoldSpeed_enabled = false;
        double HoldSpeed_value;
        bool bool_HoldHeight_enabled = false;
        double HoldHeight_value;


        public float makeStationarySpeedMax = 1f, makeStationarySpeedClamp = 0.0f;

        bool bool_SlowDown;
        bool resetSlider;

        public Dictionary<string, float> Powerstats = new Dictionary<string, float>();

        [KSPField(isPersistant = true)]
        bool bool_FullHalt = false;

        [KSPField(isPersistant = true)]
        bool bool_MakeSlowToSave = false;

        [KSPField(isPersistant = true)]
        public Rect windowPosition = new Rect(20, 120, 375, 230);

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
        public ImpulseVesselType activeImpulseVessel;

        private float lastUpdate = 0.0f;
        private float lastFixedUpdate = 0.0f;
        //private float lastflyUpdate = 0.0f;
        private float logInterval = 5.0f;

        public ImpulseVessel_manager man;


        [KSPEvent(guiName = "Activate ImpulseDrive",/* category = "ImpulseDrive",isDefault=true,*/ guiActive = true)]
        public void ActivateImpulseDrive()
		{
            //man.populateImpulseVesselList(this.vessel);
            UnityEngine.Debug.Log("ImpulseDrive: ActivateImpulseDrive  pre start check ");
            this.part.force_activate();
            UnityEngine.Debug.Log("ImpulseDrive: ActivateImpulseDrive  1 - locating Laforge");
            activeVessel = FlightGlobals.ActiveVessel;
            UnityEngine.Debug.Log("ImpulseDrive: ActivateImpulseDrive  2 - heating Earl Gray");
            ActiveVessel_pid = activeVessel.id.ToString();
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
            activeVessel = FlightGlobals.ActiveVessel;
            ActiveVessel_pid = activeVessel.id.ToString();
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



        
        public override void OnAwake()
        {
            UnityEngine.Debug.Log("ImpulseDrive: OnAwake  ");
            grav = new GravityTools();

            man = ImpulseVessel_manager.Instance;
            Debug.Log("ImpulseDrive OnAwake man=" + man);
            //Dictionary<string, ImpulseVesselType> ImpulseVesselList = ImpulseVessel_manager.ImpulseVesselList;
            //ImpulseVesselList = man.getImpulseVesselList();

            //ImpulseVessel_manager.IsFormationLeaderDefined();
        }
        public override void OnStart(StartState state)
        {
            activeVessel = FlightGlobals.ActiveVessel;
            ActiveVessel_pid = activeVessel.id.ToString();
            man.populateImpulseVesselList(this.vessel);
            UnityEngine.Debug.Log("ImpulseDrive: OnStart  this.vessel.id=" + this.vessel.id.ToString() + "  ActiveVessel_pid=" + ActiveVessel_pid);

            Powerstats.Add("charge", 0);
            Powerstats.Add("total_force", 0);
            Powerstats.Add("force_x", 0);
            Powerstats.Add("force_y", 0);
            Powerstats.Add("force_z", 0);


            //GameEvents.onVesselChange.Add(onVesselChange);

            if (state == StartState.Editor)
            {
                gravityEnabled = false;
            }
            if (FlightGlobals.ActiveVessel.id != this.vessel.id)
            {
                //gravityEnabled = false;
                return;
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
            UnityEngine.Debug.Log("ImpulseDrive: onVesselChange 1 v.vesselName=" + v.vesselName + " v.id=" + v.id + "  this.vessel.id=" + this.vessel.id.ToString() + "  ActiveVessel_pid=" + ActiveVessel_pid);
            //GameEvents.onVesselChange.Remove(onVesselChange);
            
            activeVessel = FlightGlobals.ActiveVessel;
            ActiveVessel_pid = activeVessel.id.ToString();
            //man.populateImpulseVesselList(this.vessel);
            UnityEngine.Debug.Log("ImpulseDrive: onVesselChange 2 activeVessel.vesselName=" + activeVessel.vesselName + "  ActiveVessel_pid=" + ActiveVessel_pid);
        }

        public override void OnInactive()
        {
            //man.populateImpulseVesselList(this.vessel);
            Debug.Log("ImpulseDrive OnInactive");
        }

        public override void OnActive()
        {
            activeVessel = FlightGlobals.ActiveVessel;
            ActiveVessel_pid = activeVessel.id.ToString();
            //man.populateImpulseVesselList(this.vessel);
            UnityEngine.Debug.Log("ImpulseDrive: OnActive  this.vessel.id=" + this.vessel.id.ToString() + "  ActiveVessel_pid=" + ActiveVessel_pid);

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

            if (FlightGlobals.ActiveVessel.rigidbody == null)
            {
                //return;
            }

            if (FlightGlobals.ActiveVessel.id != this.vessel.id)
            {
                //return;
            }


        }
        /// <summary>
        /// called on every FixedUpdate frame - performs most of the physics
        /// </summary>
        public void FixedUpdate()
		{
            if (HighLogic.LoadedSceneIsEditor)
                return;

            //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 2 ");
            if (FlightGlobals.ActiveVessel.rigidbody == null)
            {
                //return;
            }
            if (FlightGlobals.ActiveVessel.id != this.vessel.id)
            {
                return;
            }


            activeVessel = FlightGlobals.ActiveVessel;
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
                    UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate currentVessel 10 " + currentVessel_pid);
                    //gravityEnabled = true;
                }
                /*
                if (!man.ContainsShip(ActiveVessel_pid))
                {
                    //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 8 ");
                    man.AddImpulseVessel(ActiveVessel_pid);
                    //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 9 ");
                    man.ImpulseVesselList[ActiveVessel_pid].v = activeVessel;
                    man.ImpulseVesselList[ActiveVessel_pid].pid = ActiveVessel_pid;
                    man.ImpulseVesselList[ActiveVessel_pid].name = activeVessel.vesselName;
                    man.ImpulseVesselList[ActiveVessel_pid].geeVector = this.geeVector;
                    man.ImpulseVesselList[ActiveVessel_pid].windowPosition = this.windowPosition;
                    man.ImpulseVesselList[ActiveVessel_pid].is_active_vessel = (currentVessel_pid == ActiveVessel_pid) ? true : false;
                    man.ImpulseVesselList[ActiveVessel_pid].is_gravity_enabled = true;
                    man.ImpulseVesselList[ActiveVessel_pid].is_fullHalt_enabled = this.bool_FullHalt;
                    man.ImpulseVesselList[ActiveVessel_pid].is_MakeSlowToSave_enabled = this.bool_MakeSlowToSave;
                    man.ImpulseVesselList[ActiveVessel_pid].is_fullImpulse_enabled = this.bool_FullImpulse;
                    man.ImpulseVesselList[ActiveVessel_pid].is_UseReserves_enabled = this.bool_UseReserves;
                    man.ImpulseVesselList[ActiveVessel_pid].is_accelerationLock_enabled = this.bool_AccelerationLock;
                    man.ImpulseVesselList[ActiveVessel_pid].is_pilotMode_enabled = this.bool_PilotMode;
                    man.ImpulseVesselList[ActiveVessel_pid].is_formationMode_enabled = this.bool_FormationMode;
                    man.ImpulseVesselList[ActiveVessel_pid].is_ignoreformationcommandos_enabled = this.bool_IgnoreFormationCommandos;
                    man.ImpulseVesselList[ActiveVessel_pid].is_holdspeed_enabled = this.bool_HoldSpeed_enabled;
                    man.ImpulseVesselList[ActiveVessel_pid].is_holdheight_enabled = this.bool_HoldHeight_enabled;
                    UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate ActiveVessel 10 " + ActiveVessel_pid);
                    //gravityEnabled = true;

                }
                */

            }
            //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 14 ");




            /*
            */

            //if (this.bool_PilotMode) { bool_AccelerationLock = true; }
            //UnityEngine.Debug.Log("ImpulseDrive: bool_FullImpulse Pre vSliderValue=" + vSliderValue);
            /*
            */
            if (this.bool_FullImpulse)
            {
                    vSliderValue = vSliderValue * int_FullImpulse_multiplier;
                    hSliderValue = hSliderValue * int_FullImpulse_multiplier;
                    zSliderValue = zSliderValue * int_FullImpulse_multiplier;
                //UnityEngine.Debug.Log("ImpulseDrive: bool_FullImpulse applied");
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
            
            /*
            */
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
            }




            //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 20 ");
                /*
                if (pair.Value.pid == ActiveVessel_pid && this.vessel.id.ToString() == pair.Value.pid)
                {
                    man.ImpulseVesselList[pair.Value.pid].is_active_vessel = true;
                    man.ImpulseVesselList[ActiveVessel_pid].geeVector = this.geeVector;
                    man.ImpulseVesselList[ActiveVessel_pid].is_fullHalt_enabled = this.bool_FullHalt;
                    man.ImpulseVesselList[ActiveVessel_pid].is_MakeSlowToSave_enabled = this.bool_MakeSlowToSave;
                    man.ImpulseVesselList[ActiveVessel_pid].is_fullImpulse_enabled = this.bool_FullImpulse;
                    man.ImpulseVesselList[ActiveVessel_pid].is_UseReserves_enabled = this.bool_UseReserves;
                    man.ImpulseVesselList[ActiveVessel_pid].is_accelerationLock_enabled = this.bool_AccelerationLock;
                    man.ImpulseVesselList[ActiveVessel_pid].is_pilotMode_enabled = this.bool_PilotMode;
                    man.ImpulseVesselList[ActiveVessel_pid].is_formationMode_enabled = this.bool_FormationMode;
                    man.ImpulseVesselList[ActiveVessel_pid].is_ignoreformationcommandos_enabled = this.bool_IgnoreFormationCommandos;
                    man.ImpulseVesselList[ActiveVessel_pid].is_holdspeed_enabled = this.bool_HoldSpeed_enabled;
                    man.ImpulseVesselList[ActiveVessel_pid].HoldSpeed_value = this.HoldSpeed_value;
                    man.ImpulseVesselList[ActiveVessel_pid].is_holdheight_enabled = this.bool_HoldHeight_enabled;
                    man.ImpulseVesselList[ActiveVessel_pid].HoldHeight_value = this.HoldHeight_value;
                    float HoldSpeed_value;
                    float HoldHeight_value;
                }
                else
                {
                    man.ImpulseVesselList[pair.Value.pid].is_active_vessel = false;
                }
                */
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

                Powerstats = Utilities.CalculatePowerConsumption(currentVessel, true, man.ImpulseVesselList[currentVessel_pid].is_fullImpulse_enabled, man.ImpulseVesselList[currentVessel_pid].is_UseReserves_enabled, int_FullImpulse_multiplier, int_UseReserves_multiplier, vSliderValue, hSliderValue, zSliderValue);
                total_force = Powerstats["total_force"];
                charge = Powerstats["charge"];


                //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 1a ActiveVessel_pid=" + ActiveVessel_pid + "  currentVessel_pid=" + currentVessel_pid + "  currentVessel_pid=" + currentVessel_pid + "  [" + this.GetInstanceID().ToString("X") + "][" + Time.time.ToString("0.0000") + "]");
                ///////////////////
                // the active vessel
                /////////////////////
                //man.print_stats(this.vessel.id.ToString());
                    if ((Time.time - lastFixedUpdate) > logInterval)
                    {
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
                    }
            }
            foreach (KeyValuePair<string, ImpulseVesselType> pair in man.getImpulseVesselList())
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

                            Powerstats = Utilities.CalculatePowerConsumption(pair.Value.v, true, this.bool_FullImpulse, this.bool_UseReserves, int_FullImpulse_multiplier, int_UseReserves_multiplier, vSliderValue, hSliderValue, zSliderValue);
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
                        Powerstats = Utilities.CalculatePowerConsumption(pair.Value.v, true, false, false, int_FullImpulse_multiplier, int_UseReserves_multiplier, 0, 0, 0);
                    }
                }
            }







            //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 26 ");

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
                /*
                */
            }
            else
            {
                clickedx = false;
                clickedy = false;
                clickedz = false;
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


        private void OnGUI()
        {
            if (HighLogic.LoadedSceneIsEditor)
                return;

            if (!gravityEnabled)
                return;

            if (this.vessel == FlightGlobals.ActiveVessel)
            {
                GUIStyle myStyle = new GUIStyle();
                myStyle.margin = new RectOffset(0, 0, 0, 0);
                myStyle.padding = new RectOffset(0, 0, -11, 0);
                //GUI.skin = HighLogic.Skin;
                //GUI.skin = new GUISkin();
                GUI.skin = (GUISkin)ScriptableObject.CreateInstance(typeof(GUISkin));
                Color colGUI = GUI.color;
                colGUI.a = 0f;
                GUI.color = colGUI;


                windowPosition = GUI.Window(windowID, windowPosition, Window, "", myStyle);
            }
        }


        private void Window(int windowID)
        {
            if (HighLogic.LoadedSceneIsEditor)
                return;

            if (!gravityEnabled)
                return;
            OnWindow();
            GUI.DragWindow();
        }

        /// <summary>
        /// Put all of the code needed for the drawing of your window components in here.
        /// </summary>
        public virtual void OnWindow()
        {
            activeVessel = FlightGlobals.ActiveVessel;

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
        private GUIStyle iconStyle;
        private GUIStyle button_style_fullHalt;
        private GUIStyle label_style_dark;
        private GUIStyle label_style_light;
        private GUIStyle toggle_style_dark;
        private GUIStyle paddingTopStyle;
        private GUIStyle BackGroundLayoutStyle;


        public void window_gui_content_LCARS_v3()
        {
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

            StyleLib_LCARS_v3 styleLib = new StyleLib_LCARS_v3();
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
            }
            if (button_style_fullHalt == null)
            {
                button_style_fullHalt = styleLib.button_style_fullHalt;
            }
            if (label_style_light == null)
            {
                label_style_light = styleLib.label_style_light;

            }


            activeVessel = FlightGlobals.ActiveVessel;
            ActiveVessel_pid = activeVessel.id.ToString();
            clickedx = false;
            clickedy = false;
            clickedz = false;
            // main
            Texture2D texture = GameDatabase.Instance.GetTexture(styleLib.BackGroundLayout_image, false);
            GUILayout.BeginVertical(texture, BackGroundLayoutStyle, GUILayout.Width(375), GUILayout.Height(230));


            //bool_info_window = GUI.Toggle(Rect(8, 15, 11, 11), bool_info_window, "", toggle_style_pilotmode);


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
