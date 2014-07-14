using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{
    class LCARS_Tricorder : PartModule
    {
        private  float makeStationarySpeedMax = 1f, makeStationarySpeedClamp = 0.0f;
        private Dictionary<string, float> Powerstats = new Dictionary<string, float>();
        private ImpulseVessel_manager man;
        private GravityTools grav;
        private Texture2D texture;
        private string SubSytem = null;

        [KSPField(guiActive=false, isPersistant=true)]
        private bool TriCorderEnabled = true;

        [KSPField(isPersistant = true)]
        private Rect TricorderwindowPosition = new Rect(120, 120, 380, 230);
        private int windowID = new System.Random().Next();



        [KSPEvent(guiName = "Activate TriCorder",/* category = "TriCorder",isDefault=true,*/ guiActive = true)]
        public void ActivateTriCorder()
        {
            UnityEngine.Debug.Log("TriCorder: ActivateTriCorder");
            TriCorderEnabled = true;
            this.Events["ActivateTriCorder"].active = !TriCorderEnabled;
            this.Events["DeactivateTriCorder"].active = TriCorderEnabled;

        }

        [KSPEvent(guiName = "Deactivate TriCorder",/* category = "TriCorder", isDefault = false*/ guiActive = true)]
        public void DeactivateTriCorder()
        {
            UnityEngine.Debug.Log("TriCorder: ActivateTriCorder");
            TriCorderEnabled = false;
            this.Events["ActivateTriCorder"].active = !TriCorderEnabled;
            this.Events["DeactivateTriCorder"].active = TriCorderEnabled;
        }

        [KSPAction("ActivateTriCorder")]
        public void ActivateImpulseDriveAction(KSPActionParam param)
        {
            ActivateTriCorder();
        }

        [KSPAction("DeactivateTriCorder")]
        public void DeactivateImpulseDriveAction(KSPActionParam param)
        {
            DeactivateTriCorder();
        }





        public string TransporterSoundFile = "SciFi/StarTrekImpulseDrive/sounds/transporterbeam";
        public float TransporterSoundVolume = 4.2f;
        public bool loopTransporterSound = false;
        public FXGroup TransporterSound = null;

        LCARS_TransporterSystem TS = null;

        private ParticleEmitter emitter_Transporter;
        private float radius;
        private Dictionary<string, ModuleScienceExperiment> ModuleScienceExperimentList = null;


        public override void OnStart(StartState state)
        {
            man = ImpulseVessel_manager.Instance;
            grav = new GravityTools();
            try
            {
                //Debris
                GameObject go_Transporter = new GameObject("emitter_Transporter");
                emitter_Transporter = go_Transporter.AddComponent("EllipsoidParticleEmitter") as ParticleEmitter;
                ParticleAnimator animator_debris = go_Transporter.AddComponent<ParticleAnimator>();
                go_Transporter.AddComponent<ParticleRenderer>();
                (go_Transporter.renderer as ParticleRenderer).uvAnimationXTile = 7;
                (go_Transporter.renderer as ParticleRenderer).uvAnimationYTile = 7;
                Material mat = new Material(Shader.Find("Particles/Additive"));
                mat.mainTexture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/particles/transporter_7x7_optimized2", false);
                go_Transporter.renderer.material = mat;
                emitter_Transporter.emit = false;
                emitter_Transporter.minSize = 10f; //radius * 0.05f;  //4    3 + 
                emitter_Transporter.maxSize = 10f; //radius * 0.1f;  //8
                emitter_Transporter.minEnergy = 1;
                emitter_Transporter.maxEnergy = 2;
                emitter_Transporter.rndVelocity = Vector3.zero; //1.6f * radius; //150
                emitter_Transporter.useWorldSpace = false;
                emitter_Transporter.rndAngularVelocity = 0;
                animator_debris.rndForce = new Vector3(0, 0, 0);
                animator_debris.sizeGrow = 0f;

                emitter_Transporter.transform.position = FlightGlobals.ActiveVessel.transform.position;
                emitter_Transporter.Emit(1);

                
                
                TransporterSound = new FXGroup("TransporterSound");
                GameObject audioObj = new GameObject();
                audioObj.transform.position = FlightGlobals.ActiveVessel.transform.position;
                audioObj.transform.parent = FlightGlobals.ActiveVessel.transform;	// add to parent
                TransporterSound.audio = audioObj.AddComponent<AudioSource>();
                TransporterSound.audio.dopplerLevel = 0f;
                TransporterSound.audio.Stop();
                TransporterSound.audio.clip = GameDatabase.Instance.GetAudioClip(TransporterSoundFile);
                TransporterSound.audio.loop = false;
                TransporterSound.audio.Play();
                TransporterSound.audio.enabled = false;
                if (TransporterSound != null && TransporterSound.audio != null)
                {
                    TransporterSound.audio.time = 0;
                    float soundVolume = GameSettings.SHIP_VOLUME * TransporterSoundVolume;
                    TransporterSound.audio.enabled = true;
                    TransporterSound.audio.volume = soundVolume;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("TransporterSound Error : " + ex.Message);
            }

            if (TS == null)
            {
                TS = new LCARS_TransporterSystem();
            }

            try
            {
                ConfigNode newMSE = null;
                newMSE = new ConfigNode("MODULE");
                newMSE.AddValue("name", "ModuleScienceExperiment");
                newMSE.AddValue("experimentID", "TricorderAwayTeamStatus");
                newMSE.AddValue("experimentActionName", "AwayTeam's Log");
                newMSE.AddValue("resetActionName", "Delete Log");
                newMSE.AddValue("useStaging", "False");
                newMSE.AddValue("useActionGroups", "True");
                newMSE.AddValue("hideUIwhenUnavailable", "True");
                newMSE.AddValue("resettable", "True");
                newMSE.AddValue("xmitDataScalar", "10.0");
                newMSE.AddValue("FxModules", "0");
                this.vessel.rootPart.AddModule(newMSE);

                newMSE = new ConfigNode("MODULE");
                newMSE.AddValue("name", "ModuleScienceExperiment");
                newMSE.AddValue("experimentID", "TricorderAwayTeamSurfacescan");
                newMSE.AddValue("experimentActionName", "Surface Scan");
                newMSE.AddValue("resetActionName", "Delete Surface Scan");
                newMSE.AddValue("useStaging", "False");
                newMSE.AddValue("useActionGroups", "True");
                newMSE.AddValue("hideUIwhenUnavailable", "True");
                newMSE.AddValue("resettable", "True");
                newMSE.AddValue("xmitDataScalar", "10.0");
                newMSE.AddValue("FxModules", "0");
                this.vessel.rootPart.AddModule(newMSE);

                newMSE = new ConfigNode("MODULE");
                newMSE.AddValue("name", "ModuleScienceExperiment");
                newMSE.AddValue("experimentID", "TricorderAwayTeamAtmosphereScan");
                newMSE.AddValue("experimentActionName", "Atmosphere Scan");
                newMSE.AddValue("resetActionName", "Delete Atmosphere Scan");
                newMSE.AddValue("useStaging", "False");
                newMSE.AddValue("useActionGroups", "True");
                newMSE.AddValue("hideUIwhenUnavailable", "True");
                newMSE.AddValue("resettable", "True");
                newMSE.AddValue("xmitDataScalar", "10.0");
                newMSE.AddValue("FxModules", "0");
                this.vessel.rootPart.AddModule(newMSE);

                newMSE = new ConfigNode("MODULE");
                newMSE.AddValue("name", "ModuleScienceExperiment");
                newMSE.AddValue("experimentID", "TricorderAwayTeamLiquidScan");
                newMSE.AddValue("experimentActionName", "Liquid Scan");
                newMSE.AddValue("resetActionName", "Delete Liquid Scan");
                newMSE.AddValue("useStaging", "False");
                newMSE.AddValue("useActionGroups", "True");
                newMSE.AddValue("hideUIwhenUnavailable", "True");
                newMSE.AddValue("resettable", "True");
                newMSE.AddValue("xmitDataScalar", "10.0");
                newMSE.AddValue("FxModules", "0");
                this.vessel.rootPart.AddModule(newMSE);
                
                
                ModuleScienceExperimentList = new Dictionary<string, ModuleScienceExperiment>() { };
                foreach (PartModule PM in this.vessel.rootPart.Modules)
                {
                    if(PM.moduleName=="ModuleScienceExperiment")
                    {
                        ModuleScienceExperiment foo = PM as ModuleScienceExperiment;
                        ModuleScienceExperimentList.Add(foo.experimentID,foo);

                    }
                    //Debug.LogError("ModuleScienceExperiment rootPart.Module=" + PM.name);
                }
                /*
                MODULE
                    {
	                    name = ModuleScienceExperiment
	                    experimentID = enterprisestatus

	                    experimentActionName = Captain's Log
	                    resetActionName = Delete Log

	                    useStaging = False
	                    useActionGroups = True
	                    hideUIwhenUnavailable = True 
	                    resettable = True
		
	                    xmitDataScalar = 10.0
	
	                    FxModules = 0
                    }

                    MODULE
                    {
	                    name = ModuleScienceContainer
	
	                    reviewActionName = Review Mission Data
	                    storeActionName = Log Data
	                    evaOnlyStorage = False
	                    storageRange = 1000.0
                    }

                    MODULE
                    {
	                    name = ModuleDataTransmitter
	
	                    packetInterval = 0.01
	                    packetSize = 0.5
	
	                    packetResourceCost = 1000.0
	                    requiredResource = ElectricCharge
		
	                    DeployFxModules = 0
                    }
                 */


            }
            catch (Exception ex)
            {
                Debug.LogError("ModuleScienceExperiment Error : " + ex.Message);
            }
        }


        public void FixedUpdate()
        {
            DontFallOnMyHead();
            if (this.vessel.rootPart.checkSplashed())
            {
                ModuleScienceExperimentList["TricorderAwayTeamLiquidScan"].Inoperable = false;
            }
            else
            {
                ModuleScienceExperimentList["TricorderAwayTeamLiquidScan"].Inoperable = true;
            }
        }





        GUIStyle myStyle = null;
        private void OnGUI()
        {
            if (HighLogic.LoadedSceneIsEditor)
                return;

            if (!TriCorderEnabled)
                return;

            if (this.vessel == FlightGlobals.ActiveVessel)
            {
                myStyle = new GUIStyle();
                myStyle.margin = new RectOffset(0, 0, 0, 0);
                myStyle.padding = new RectOffset(0, 0, -11, 0);
                myStyle.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/blind", false);
                myStyle.onNormal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/blind", false);
                myStyle.onHover.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/blind", false);
                myStyle.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/v3/blind", false);

                TricorderwindowPosition = LCARS_Utilities.ClampToScreen(GUILayout.Window(windowID, TricorderwindowPosition, TriCorderWindow, "", myStyle));
            }
        }
        GUIStyle SubSystem_BackGroundLayoutStyle;
        GUIStyle SubSystem_BackGroundLayoutStyle2;
        private void TriCorderWindow(int windowID)
        {
            if (HighLogic.LoadedSceneIsEditor)
                return;

            if (!TriCorderEnabled)
                return;


            emitter_Transporter.transform.position = FlightGlobals.ActiveVessel.transform.position;


            SubSystem_BackGroundLayoutStyle = new GUIStyle(GUI.skin.box);
            SubSystem_BackGroundLayoutStyle.alignment = TextAnchor.UpperLeft;
            SubSystem_BackGroundLayoutStyle.padding = new RectOffset(0, 0, 0, 0);
            SubSystem_BackGroundLayoutStyle.margin = new RectOffset(0, 0, 0, 0);
            SubSystem_BackGroundLayoutStyle.fixedWidth = 380;
            SubSystem_BackGroundLayoutStyle.fixedHeight = 17;
            SubSystem_BackGroundLayoutStyle.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_background_header2", false);
            SubSystem_BackGroundLayoutStyle.onNormal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_background_header2", false);
            SubSystem_BackGroundLayoutStyle.onHover.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_background_header2", false);
            SubSystem_BackGroundLayoutStyle.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_background_header2", false);
            
            SubSystem_BackGroundLayoutStyle2 = new GUIStyle(GUI.skin.box);
            SubSystem_BackGroundLayoutStyle2.alignment = TextAnchor.UpperLeft;
            SubSystem_BackGroundLayoutStyle2.padding = new RectOffset(0, 0, 0, 0);
            SubSystem_BackGroundLayoutStyle2.margin = new RectOffset(0, 0, 0, 0);
            SubSystem_BackGroundLayoutStyle2.fixedWidth = 380;
            SubSystem_BackGroundLayoutStyle2.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_background_bottom2", false);
            SubSystem_BackGroundLayoutStyle2.onNormal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_background_bottom2", false);
            SubSystem_BackGroundLayoutStyle2.onHover.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_background_bottom2", false);
            SubSystem_BackGroundLayoutStyle2.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_background_bottom2", false);



                GUILayout.BeginHorizontal(SubSystem_BackGroundLayoutStyle, GUILayout.Width(380), GUILayout.Height(17));
                GUILayout.BeginVertical();
                    GUILayout.Label("  ", GUILayout.Height(17), GUILayout.Width(380));
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                if (GUI.Button(new Rect(370,1,10,10),"X"))
                {
                    TriCorderEnabled = false;
                }

                GUILayout.BeginHorizontal(SubSystem_BackGroundLayoutStyle2, GUILayout.Width(380));

                    GUILayout.BeginVertical(GUILayout.Width(70));
                    OnTriCorderMenu();
                    GUILayout.Label("  ", GUILayout.Width(70));
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(GUILayout.Width(280));
                    OnTriCorderWindow();
                    GUILayout.Label(" ", GUILayout.Width(280));
                    GUILayout.Label(">EOL..  ", GUILayout.Width(280));
                    GUILayout.EndVertical();

                GUILayout.EndHorizontal();



            
            GUI.DragWindow();
            SubSystem_BackGroundLayoutStyle = null;
            SubSystem_BackGroundLayoutStyle2 = null;

        }


        // Subsystems:
        private Vessel ShipConnected = null;
        private List<Vessel> vesselList;
        private List<string> visibleVessels;
        private bool ActivateSubsystem = false;
        private string SubsystemSelected = null;

        private GUIStyle SubSys_Button_Info_style;
        private GUIStyle SubSys_Button_Geo_style;
        private GUIStyle SubSys_Button_Atmo_style;
        private GUIStyle SubSys_Button_Hydro_style;
        private GUIStyle SubSys_Button_Temp_style;
        private GUIStyle SubSys_Button_Grav_style;
        private GUIStyle SubSys_Button_LCARS_style;

        public virtual void OnTriCorderMenu()
        {
            SubSys_Button_Info_style = new GUIStyle(GUI.skin.box);
            SubSys_Button_Info_style.alignment = TextAnchor.UpperLeft;
            SubSys_Button_Info_style.padding = new RectOffset(0, 0, 0, 0);
            SubSys_Button_Info_style.margin = new RectOffset(0, 0, 0, 0);
            SubSys_Button_Info_style.fixedWidth = 70;
            SubSys_Button_Info_style.fixedHeight = 54;
            SubSys_Button_Info_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Info", false);
            SubSys_Button_Info_style.onNormal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Info", false);
            SubSys_Button_Info_style.onHover.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Info", false);
            SubSys_Button_Info_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Info", false);

            SubSys_Button_Geo_style = new GUIStyle(GUI.skin.box);
            SubSys_Button_Geo_style.alignment = TextAnchor.UpperLeft;
            SubSys_Button_Geo_style.padding = new RectOffset(0, 0, 0, 0);
            SubSys_Button_Geo_style.margin = new RectOffset(0, 0, 0, 0);
            SubSys_Button_Geo_style.fixedWidth = 70;
            SubSys_Button_Geo_style.fixedHeight = 54;
            SubSys_Button_Geo_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Geo", false);
            SubSys_Button_Geo_style.onNormal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Geo", false);
            SubSys_Button_Geo_style.onHover.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Geo", false);
            SubSys_Button_Geo_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Geo", false);

            SubSys_Button_Atmo_style = new GUIStyle(GUI.skin.box);
            SubSys_Button_Atmo_style.alignment = TextAnchor.UpperLeft;
            SubSys_Button_Atmo_style.padding = new RectOffset(0, 0, 0, 0);
            SubSys_Button_Atmo_style.margin = new RectOffset(0, 0, 0, 0);
            SubSys_Button_Atmo_style.fixedWidth = 70;
            SubSys_Button_Atmo_style.fixedHeight = 54;
            SubSys_Button_Atmo_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Atmo", false);
            SubSys_Button_Atmo_style.onNormal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Atmo", false);
            SubSys_Button_Atmo_style.onHover.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Atmo", false);
            SubSys_Button_Atmo_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Atmo", false);

            SubSys_Button_Hydro_style = new GUIStyle(GUI.skin.box);
            SubSys_Button_Hydro_style.alignment = TextAnchor.UpperLeft;
            SubSys_Button_Hydro_style.padding = new RectOffset(0, 0, 0, 0);
            SubSys_Button_Hydro_style.margin = new RectOffset(0, 0, 0, 0);
            SubSys_Button_Hydro_style.fixedWidth = 70;
            SubSys_Button_Hydro_style.fixedHeight = 54;
            SubSys_Button_Hydro_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Hydro", false);
            SubSys_Button_Hydro_style.onNormal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Hydro", false);
            SubSys_Button_Hydro_style.onHover.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Hydro", false);
            SubSys_Button_Hydro_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Hydro", false);

            SubSys_Button_Temp_style = new GUIStyle(GUI.skin.box);
            SubSys_Button_Temp_style.alignment = TextAnchor.UpperLeft;
            SubSys_Button_Temp_style.padding = new RectOffset(0, 0, 0, 0);
            SubSys_Button_Temp_style.margin = new RectOffset(0, 0, 0, 0);
            SubSys_Button_Temp_style.fixedWidth = 70;
            SubSys_Button_Temp_style.fixedHeight = 54;
            SubSys_Button_Temp_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Temp", false);
            SubSys_Button_Temp_style.onNormal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Temp", false);
            SubSys_Button_Temp_style.onHover.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Temp", false);
            SubSys_Button_Temp_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Temp", false);

            SubSys_Button_Grav_style = new GUIStyle(GUI.skin.box);
            SubSys_Button_Grav_style.alignment = TextAnchor.UpperLeft;
            SubSys_Button_Grav_style.padding = new RectOffset(0, 0, 0, 0);
            SubSys_Button_Grav_style.margin = new RectOffset(0, 0, 0, 0);
            SubSys_Button_Grav_style.fixedWidth = 70;
            SubSys_Button_Grav_style.fixedHeight = 54;
            SubSys_Button_Grav_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Grav", false);
            SubSys_Button_Grav_style.onNormal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Grav", false);
            SubSys_Button_Grav_style.onHover.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Grav", false);
            SubSys_Button_Grav_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_Grav", false);

            SubSys_Button_LCARS_style = new GUIStyle(GUI.skin.box);
            SubSys_Button_LCARS_style.alignment = TextAnchor.UpperLeft;
            SubSys_Button_LCARS_style.padding = new RectOffset(0, 0, 0, 0);
            SubSys_Button_LCARS_style.margin = new RectOffset(0, 0, 0, 0);
            SubSys_Button_LCARS_style.fixedWidth = 70;
            SubSys_Button_LCARS_style.fixedHeight = 54;
            SubSys_Button_LCARS_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_LCARS", false);
            SubSys_Button_LCARS_style.onNormal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_LCARS", false);
            SubSys_Button_LCARS_style.onHover.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_LCARS", false);
            SubSys_Button_LCARS_style.normal.background = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/Icons/tricorder/tricorder_button_LCARS", false);


            GUILayout.Label("");
            
            if (GUILayout.Button("", SubSys_Button_Info_style))
            {
                SubSytem = null;
            }
            if (GUILayout.Button("", SubSys_Button_Geo_style))
            {
                SubSytem = "Geo";
            }
            if (GUILayout.Button("", SubSys_Button_Atmo_style))
            {
                SubSytem = "Atmo";
            }
            if (GUILayout.Button("", SubSys_Button_Hydro_style))
            {
                SubSytem = "Hydro";
            }
            /*
            if (GUILayout.Button("", SubSys_Button_Temp_style))
            {
                SubSytem = "Temp";
            }
            */
            if (GUILayout.Button("", SubSys_Button_Grav_style))
            {
                SubSytem = "Grav";
            }
            if (GUILayout.Button("", SubSys_Button_LCARS_style))
            {
                SubSytem = "ConnectToShip";
            }
        }

        public virtual void OnTriCorderWindow()
        {

            

            if (SubSytem == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                    Info_Subsystem();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();


                //GUILayout.Label("Label Ship Info");

            }
            else 
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                
                switch (SubSytem)
                {
                    case "ConnectToShip":
                        LCARSLinkUP();
                        break;

                    case "Geo":
                        Geo_Subsystem();
                        break;
                        
                    case "Atmo":
                        Atmo_Subsystem();
                        break;

                    case "Hydro":
                        Hydro_Subsystem();
                        break;

                    case "Grav":
                        Grav_Subsystem();
                        break;

                    default:
                            GUILayout.Label("");
                            GUILayout.Label("Subsystem " + SubSytem);

                            GUILayout.BeginHorizontal();
                            GUILayout.BeginVertical();
                                if (GUILayout.Button("Disable " + SubSytem + " Subsystem"))
                                {
                                    SubSytem = null;
                                }
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();
                        GUILayout.Label("Subsystem " + SubSytem + " is currently unavailable");
                        break;

                }
                
                

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

            }



        }



        private void Info_Subsystem()
        {
            GUILayout.Label("");
            GUILayout.Label("Tricorder Mainscreen");



            GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Identification: ");
                    GUILayout.Label("" + vessel.vesselName);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Clearance Code: ");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    string[] nameszz = vessel.vesselName.Split(' ');
                    GUILayout.FlexibleSpace(); 
                    GUILayout.Label(LCARS_Utilities.AlphaCharlyTango(nameszz[0], vessel.id.ToString(), 4));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Clearance Level: ");
                    GUILayout.Label("5");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Location: ");
                    GUILayout.Label("" + vessel.RevealSituationString() + "");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Away Team Report: ");
                    if (GUILayout.Button("Run TricorderAwayTeamStatus"))
                    {
                        ModuleScienceExperimentList["TricorderAwayTeamStatus"].DeployExperiment();
                    }
                GUILayout.EndHorizontal();

            GUILayout.EndVertical();


        }

        private void Subsystem_header(string subsys,string desc)
        {
            GUILayout.Label("");
            GUILayout.Label(desc);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            if (GUILayout.Button("Disable " + subsys + " Subsystem"))
            {
                SubSytem = null;
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void Geo_Subsystem()
        {

            Subsystem_header("Geo", "Subsystem Geographical Data");


            GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Longtitude: ");
                    GUILayout.Label("" + Math.Round(vessel.longitude,6) );
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Latitude: ");
                    GUILayout.Label("" + Math.Round(vessel.latitude,6));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Situation: ");
                    GUILayout.Label("" + vessel.RevealSituationString() + "");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Weather: ");
                    GUILayout.Label("The weather is fine ");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Scan the Surface: ");
                    if (GUILayout.Button("Run Surface Scan"))
                    {
                        ModuleScienceExperimentList["TricorderAwayTeamSurfacescan"].DeployExperiment();
                    }
                GUILayout.EndHorizontal();

            GUILayout.EndVertical();




        }

        private void Atmo_Subsystem()
        {
            Subsystem_header("Atmo", "Subsystem Atmospheric Data");

            GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Local Temperature: ");
                    GUILayout.Label("To Do");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Atm. Pressure: ");
                    GUILayout.Label("To Do");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("GC MassSpectrometer: ");
                    GUILayout.Label("To Do");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Atmosphere Scan: ");
                    if (GUILayout.Button("Run Atmosphere Scan"))
                    {
                        ModuleScienceExperimentList["TricorderAwayTeamAtmosphereScan"].DeployExperiment();
                    }
                GUILayout.EndHorizontal();


            GUILayout.EndVertical();

        }

        private void Hydro_Subsystem()
        {
            Subsystem_header("Hydro", "Subsystem Hydrospheric Data");

            GUILayout.BeginVertical();
                
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Ocean Temperature: ");
                    GUILayout.Label("To Do");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("LC MassSpectrometer: ");
                    GUILayout.Label("To Do");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Liquid Scan: ");
                    if (this.vessel.rootPart.checkSplashed())
                    {
                        if (GUILayout.Button("Run Liquid Scan"))
                        {
                            ModuleScienceExperimentList["TricorderAwayTeamLiquidScan"].DeployExperiment();
                        }
                    }
                    else 
                    {
                        GUILayout.Label("The docs make no scence.., it claims you need to be in water to make a liquid Scan!! Strange thing..");
                    }
                GUILayout.EndHorizontal();

            GUILayout.EndVertical();

        }

        private void Grav_Subsystem()
        {
            Subsystem_header("Grav", "Subsystem Gravitation Data");


            GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Local Gravity: ");
                    GUILayout.Label("" + Math.Round(FlightGlobals.getGeeForceAtPosition(vessel.findWorldCenterOfMass()).x, 4));
                GUILayout.EndHorizontal();

            GUILayout.EndVertical();

        }

        /// <summary>
        /// prevents the mother ship from loosing antigrav after beam down
        /// </summary>
        private void LCARSLinkUP()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("");
            GUILayout.Label("LCARS LinkUp:");

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            if (GUILayout.Button("Disable LCARS LinkUp"))
            {
                SubSytem = null;
                ShipConnected = null;
                ActivateSubsystem = false;
                SubsystemSelected = null;
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (ShipConnected == null)
            {
                GUILayout.Label("Select Ship: ");
                vesselList = FlightGlobals.Vessels;
                visibleVessels = new List<string>() { };
                foreach (Vessel v in vesselList)
                {
                    if (v.checkVisibility() && man.IsImpulseVessel(v) && v.loaded)
                    {
                        if (GUILayout.Button(v.vesselName + " (" + Vector3.Distance(v.findWorldCenterOfMass(), vessel.findWorldCenterOfMass()) + "m)"))
                        {
                            ShipConnected = v;
                        }
                    }
                }
            }
            else
            {
                //Debug.Log("ImpulseDrive: Tricorder SubsystemSelected=" + SubsystemSelected);
                if (GUILayout.Button("Disconnect Ship"))
                {
                    ShipConnected = null;
                    ActivateSubsystem = false;
                    SubsystemSelected = null;
                }
                //GUILayout.Label("Connected with Ship: " + ShipConnected.vesselName);

                if (ActivateSubsystem)
                {
                    if (GUILayout.Button("Disable Subsystem"))
                    {
                        ActivateSubsystem = false;
                        SubsystemSelected = null;
                    }
                }
                else
                {
                    GUILayout.Label("Choose Subsystem: ");
                    if (GUILayout.Button("Transporter LinkUp"))
                    {
                        ActivateSubsystem = true;
                        SubsystemSelected = "transporter";
                    }
                    if (GUILayout.Button("Auxiliary Helm LinkUp"))
                    {
                        ActivateSubsystem = true;
                        SubsystemSelected = "auxiliary_helm";
                    }
                }
                if (ActivateSubsystem)
                {
                        switch (SubsystemSelected)
                        {
                            case "transporter":
                                LinkUp_Transporter();
                                break;

                            case "auxiliary_helm":
                                GUILayout.Label("auxiliary_helm ToDo ");
                                LinkUp_Helm();
                               break;

                            default:
                               GUILayout.Label("unknown subsystem exception ");
                               break;
                        }
                }
            }
        }

        private void LinkUp_Helm()
        {
            if (this.ShipConnected != null)
            {
                //Debug.Log("ImpulseDrive: Tricorder LinkUp_Helm begin ");

                GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical(GUILayout.Width(30),GUILayout.Height(30));
                        GUILayout.Label(" ");
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical(GUILayout.Width(30), GUILayout.Height(30));
                        if (GUILayout.RepeatButton("↑"))
                        {
                            grav.AddNewForce_ffwd_back(10, this.ShipConnected);
                        }
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical(GUILayout.Width(30), GUILayout.Height(30));
                        if (GUILayout.RepeatButton("U"))
                        {
                            grav.AddNewForce_up_down(10, this.ShipConnected);
                        }
                        GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical(GUILayout.Width(30), GUILayout.Height(30));
                        if (GUILayout.RepeatButton("←"))
                        {
                            grav.AddNewForce_left_right(-10, this.ShipConnected);
                        }
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical(GUILayout.Width(30), GUILayout.Height(30));
                        if (GUILayout.RepeatButton("S"))
                        {
                            grav.FullHalt(ShipConnected, true);
                        }
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical(GUILayout.Width(30), GUILayout.Height(30));
                        if (GUILayout.RepeatButton("→"))
                        {
                            grav.AddNewForce_left_right(10, this.ShipConnected);
                        }
                        GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical(GUILayout.Width(30), GUILayout.Height(30));
                        GUILayout.Label(" ");
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical(GUILayout.Width(30), GUILayout.Height(30));
                        if (GUILayout.RepeatButton("↓"))
                        {
                            grav.AddNewForce_ffwd_back(-10, this.ShipConnected);
                        }
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical(GUILayout.Width(30), GUILayout.Height(30));
                        if (GUILayout.RepeatButton("D"))
                        {
                            grav.AddNewForce_up_down(-10, this.ShipConnected);
                        }
                        GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                GUILayout.EndVertical();


                //Debug.Log("ImpulseDrive: Tricorder LinkUp_Helm done ");
            }
            else
            {
                GUILayout.Label("ShipConnected == null ");
            }

        }

        private void LinkUp_Transporter()
        {
            //Debug.Log("ImpulseDrive: Tricorder ShipConnected.vesselName=" + ShipConnected.vesselName);
            if (ShipConnected != null)
            {
                //Debug.Log("ImpulseDrive: Tricorder LinkUp_Transporter TS.SetMotherShip ");
                TS.SetMotherShip(ShipConnected);
                //Debug.Log("ImpulseDrive: Tricorder LinkUp_Transporter TS.GUI ");
                TS.GUI(new Rect(78, 192, 292, 292));
                //Debug.Log("ImpulseDrive: Tricorder LinkUp_Transporter done ");
            }
            else
            {
                GUILayout.Label("ShipConnected == null ");
            }
        }



        /// <summary>
        /// prevents the mother ship from loosing antigrav after beam down
        /// </summary>
        private void DontFallOnMyHead()
        {
            if (FlightGlobals.ActiveVessel.id != this.vessel.id)
            {
                // only one instance should run this code..
                return;
            }
            //////////////////////////////////
            //// START keep impulse vessels floating
            //////////////////////////////////
            foreach (KeyValuePair<string, LCARS_ImpulseVesselType> pair in man.getImpulseVesselList())
            {
                man.ImpulseVesselList[pair.Value.pid].is_active_vessel = false;

                if (pair.Value.is_gravity_enabled && !man.ImpulseVesselList[pair.Value.pid].is_active_vessel)
                {
                    Vector3 geeVector = FlightGlobals.getGeeForceAtPosition(pair.Value.v.findWorldCenterOfMass());
                    grav.CancelG2(pair.Value.v, geeVector, man);

                        if (man.ImpulseVesselList[pair.Value.pid].is_fullHalt_enabled)
                        {
                            grav.FullHalt(pair.Value.v, true);
                            man.ImpulseVesselList[pair.Value.pid].is_holdspeed_enabled = false;
                            man.ImpulseVesselList[pair.Value.pid].is_pilotMode_enabled = false;
                        }

                        // Freeze in place if requested
                        if (man.ImpulseVesselList[pair.Value.pid].is_MakeSlowToSave_enabled && man.ImpulseVesselList[pair.Value.pid].is_fullHalt_enabled) { grav.makeSlowToSave(pair.Value.v, true, makeStationarySpeedClamp); }
                        
                    LCARS_Utilities.CalculatePowerConsumption(Powerstats, pair.Value.v, true, false, false, 1, 1, 0, 0, 0);
                }
            }
            //////////////////////////////////
            //// END keep impulse vessels floating
            //////////////////////////////////
        }

    }
}
