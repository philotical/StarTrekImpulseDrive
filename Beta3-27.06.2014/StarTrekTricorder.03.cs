using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{
    class StarTrekTricorder : PartModule
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






        public override void OnAwake()
        {
            man = ImpulseVessel_manager.Instance;
            grav = new GravityTools();
        }


        public void FixedUpdate()
        {
            DontFallOnMyHead();
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

                TricorderwindowPosition = Utilities.ClampToScreen(GUILayout.Window(windowID, TricorderwindowPosition, TriCorderWindow, "", myStyle));
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


            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Identification: ");
            GUILayout.Label("Clearance Code: ");
            GUILayout.Label("Clearance Level: ");

            GUILayout.Label("Location: ");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //GUILayout.Label("Label Ship Info");
            GUILayout.Label("" + vessel.vesselName);

            string[] nameszz = vessel.vesselName.Split(' ');

            GUILayout.Label("" + nameszz[0] + "-" + Utilities.AlphaCharlyTango(vessel.id.ToString(), 4));
            GUILayout.Label("5");

            GUILayout.Label("" + vessel.RevealSituationString() + "");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

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

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Longtitude: ");
            GUILayout.Label("Latitude: ");

            GUILayout.Label("Situation: ");

            GUILayout.Label("Weather: ");

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //GUILayout.Label("Label Ship Info");
            GUILayout.Label("" + vessel.longitude);

            GUILayout.Label("" + vessel.latitude);

            GUILayout.Label("" + vessel.RevealSituationString() + "");
            GUILayout.Label("The weather is fine ");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();



        }

        private void Atmo_Subsystem()
        {
            Subsystem_header("Atmo", "Subsystem Atmospheric Data");

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Local Temperature: ");
            GUILayout.Label("Atm. Pressure: ");
            GUILayout.Label("GC MassSpectrometer: ");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("To Do");
            GUILayout.Label("To Do");
            GUILayout.Label("To Do");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void Hydro_Subsystem()
        {
            Subsystem_header("Hydro", "Subsystem Hydrospheric Data");

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Ocean Temperature: ");
            GUILayout.Label("LC MassSpectrometer: ");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("To Do");
            GUILayout.Label("To Do");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void Grav_Subsystem()
        {
            Subsystem_header("Grav", "Subsystem Gravitation Data");

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Local Gravity: ");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("" + FlightGlobals.getGeeForceAtPosition(vessel.findWorldCenterOfMass()));
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// prevents the mother ship from loosing antigrav after beam down
        /// </summary>
        TransporterSystem TS = null;
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
                if (GUILayout.Button("Disconnect Ship"))
                {
                    ShipConnected = null;
                }
                GUILayout.Label("Connected with Ship: " + ShipConnected.vesselName);

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
            }
            if (ActivateSubsystem)
            {
                    switch (SubsystemSelected)
                    {
                        case "transporter":
                            if(TS==null)
                            {
                                TS = new TransporterSystem();
                            }
                            LinkUp_Transporter();
                            break;

                        case "auxiliary_helm":
                            GUILayout.Label("auxiliary_helm ToDo ");
                           break;

                        default:
                           GUILayout.Label("unknown subsystem exception ");
                           break;
                    }
            }
        }

        private void LinkUp_Transporter()
        {
            if (ShipConnected != null)
            {
                TS.SetMotherShip(ShipConnected);
                TS.GUI(new Rect(78, 192, 292, 292));
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
            foreach (KeyValuePair<string, ImpulseVesselType> pair in man.getImpulseVesselList())
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
                        
                    Utilities.CalculatePowerConsumption(Powerstats, pair.Value.v, true, false, false, 1, 1, 0, 0, 0);
                }
            }
            //////////////////////////////////
            //// END keep impulse vessels floating
            //////////////////////////////////
        }

    }
}
