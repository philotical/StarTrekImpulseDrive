using System;
using System.Collections.Generic;
using UnityEngine;

namespace Philotical
{
    class LCARS_SensorArray : PartModule
    {

        LCARS_VesselPartsInventory VPI = null;
        LCARS_VesselPartsInventory sub_VPI = null;
        LCARS_CargoBay STCB = null;
        LCARS_PowerSystem PowSys = null;
        PowerTaker PT1 = null;
        Vessel MotherShip = null;
        Vessel TargetShip_1 = null;
        Vessel TargetShip_2 = null;
        string ScanMode = null;
        int ShipScanMode = 0;

        public Vector2 scrollPosition1;
        public Vector2 scrollPosition2;
        public Vector2 scrollPosition3;
        GUIStyle scrollview_style;

        internal void setVPI(Vessel thisVessel, LCARS_VesselPartsInventory thisVPI, LCARS_PowerSystem thisPowSys)
        {

            this.VPI = thisVPI;
            this.MotherShip = thisVessel;
            this.PowSys = thisPowSys;
            this.PT1 = this.PowSys.setPowerTaker("SensorArray", "SubSystem", 1250, 2500, 5000);
        }

        internal void GUI()
        {
            scrollview_style = new GUIStyle();
            scrollview_style.fixedHeight = 150;

            GUILayout.BeginVertical();
            if (ScanMode != null)
            {
                if (GUILayout.Button("Back"))
                {
                    ScanMode = null;
                }

            }
            switch (ScanMode)
            {

                case "Planet":
                    GUILayout.Label("ToDo");
                    break;

                case "Distance":
                    GUILayout.Label("ToDo");
                    break;

                case "Ship":
                    ShipScanner_GUI();
                    break;

                default:
                    if (GUILayout.Button("Scan a planet"))
                    {
                        ScanMode = "Planet";
                    }

                    if (GUILayout.Button("Messure a Distance"))
                    {
                        ScanMode = "Distance";
                    }

                    if (GUILayout.Button("Scan a ship"))
                    {
                        ScanMode = "Ship";
                    }
                    break;
            }
            GUILayout.EndVertical();
        }

           
        private void ShipScanner_GUI()
        {
            if (TargetShip_1 == null)
            {
                GUI_Ship_List();
            }
            else
            {
                float power = 0f;
                if (GUILayout.Button("Unset object"))
                {
                    TargetShip_1 = null;
                    ShipScanMode = 0;
                }
                switch (ShipScanMode)
                {

                    default: //case 1
                        sub_VPI = new LCARS_VesselPartsInventory();
                        sub_VPI.init(TargetShip_1);
                        sub_VPI.scanVessel();

                        GUILayout.Label("Standard Scan Results");
                        GUI_Ship_Scan_Level1(TargetShip_1);
                        power = PT1.L1_usage;
                        this.PowSys.draw(PT1.takerName, power);

                            if (GUILayout.Button("Perform a Tactical Scan"))
                            {
                                ShipScanMode = 2;
                            }
                        break;

                    case 2:
                        GUILayout.Label("Tactical Scan Results");
                        GUI_Ship_Scan_Level1(TargetShip_1);
                        GUI_Ship_Scan_Level2(TargetShip_1);
                        power = PT1.L1_usage + PT1.L2_usage;
                        this.PowSys.draw(PT1.takerName, power);

                            if (GUILayout.Button("Scan the Interior"))
                            {
                                ShipScanMode = 3;
                            }
                        break;

                    case 3:
                        GUILayout.Label("Interior Scan Results");
                        GUI_Ship_Scan_Level1(TargetShip_1);
                        GUI_Ship_Scan_Level2(TargetShip_1);
                        GUI_Ship_Scan_Level3(TargetShip_1);
                        power = PT1.L1_usage + PT1.L2_usage + PT1.L3_usage;
                        this.PowSys.draw(PT1.takerName, power);
                        break;
                }
            }
        }

        private void GUI_Ship_Scan_Level1(Vessel TargetShip_1)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(); GUILayout.Label("Name: "); GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace(); GUILayout.Label("" + TargetShip_1.vesselName); GUILayout.EndHorizontal();
            GUILayout.Label("Mass: " + Math.Round(TargetShip_1.GetTotalMass(), 2) + " t");
            GUILayout.Label("DryMass: " + Math.Round(this.sub_VPI.getVesselDryMass(), 2) + " t");
            GUILayout.Label("Parts: " + TargetShip_1.parts.Count);
            GUILayout.Label("Ship Configuration: " + determine_ship_configuration());
            GUILayout.EndVertical();
        }

        private void GUI_Ship_Scan_Level2(Vessel TargetShip_1)
        {
            float current_temp_total = 0;
            float max_temp_total =0;
            foreach (Part p in this.sub_VPI.getParts())
            {
                current_temp_total += p.temperature;
                max_temp_total += p.maxTemp;
            }
            float heat_percentage = current_temp_total / (max_temp_total / 100);
            float hullintegrity_percentage = 100 - heat_percentage;

            GUILayout.BeginVertical();
            GUILayout.Label("Hull Temperatur is at: " + Math.Round(heat_percentage,2) + "%");
            GUILayout.Label("Hull Integrity is at: " + Math.Round(hullintegrity_percentage,2) + "%");
            GUILayout.EndVertical();

            bool weaponsystems_present = false;
            bool torpedo_present = false;
            bool cloak_present = false;
            bool SIF_present = false;

            weaponsystems_present = this.sub_VPI.checkForPartWithModule("LCARS_WeaponSystems");
            torpedo_present = this.sub_VPI.checkForPartWithModule("LCARS_PhotonTorpedo");
            cloak_present = this.sub_VPI.checkForPartWithModule("LCARS_CloakingDevice");
            SIF_present = this.sub_VPI.checkForPartWithModule("LCARS_StructuralIntegrityField");
            GUILayout.BeginVertical();
            GUILayout.Label("Phasers: " + weaponsystems_present);
            GUILayout.Label("Torpedos: " + torpedo_present);
            GUILayout.Label("Cloak: " + cloak_present);
            GUILayout.Label("Struct.Integr.Field: " + SIF_present);
            GUILayout.EndVertical();
        }

        private void GUI_Ship_Scan_Level3(Vessel TargetShip_1)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Crew compliment current: " + TargetShip_1.GetCrewCount());
            GUILayout.Label("Crew compliment max: " + TargetShip_1.GetCrewCapacity());
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("Installed Systems:");
            GUILayout.BeginVertical(scrollview_style);
            scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1);
            foreach (KeyValuePair<string, PartModule> pair in this.sub_VPI.getModules())
            {
                GUILayout.Label("System Name: " + pair.Value.moduleName);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("Available Resources: " + this.sub_VPI.getVesselResourceMass() + "t total");
            GUILayout.BeginVertical(scrollview_style);
            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2);
            foreach (KeyValuePair<string, PartResource> pair in this.sub_VPI.getResources())
            {
                GUILayout.Label( pair.Value.resourceName);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndVertical();

        }

        private void GUI_Ship_List()
        {
            GUILayout.Label("Select Source Ship");
            GUILayout.BeginVertical(scrollview_style);
            scrollPosition3 = GUILayout.BeginScrollView(scrollPosition3);
            foreach (Vessel v in FlightGlobals.Vessels)
            {
                if (v.checkVisibility() && v.loaded && v.id != MotherShip.id)
                {
                    if (GUILayout.Button("" + v.vesselName))
                    {
                        TargetShip_1 = v;
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private string determine_ship_configuration()
        {
            foreach (Part p in this.sub_VPI.getParts())
            {
                Debug.LogError("determine_ship_configuration p.partName=" + p.partName);
                Debug.LogError("determine_ship_configuration p.name=" + p.name);
                if (p.name.Contains("voy.saucer"))
                {
                    return "It seems to be a Intrepid Class Starship";
                }
                
                if (p.name.Contains("shuttle1"))
                {
                    return "It seems to be a Type 3 Shuttle";
                }
                if (p.name.Contains("shuttle2"))
                {
                    return "It seems to be a Runabout Class Starship";
                }
                if (p.name.Contains( "base"))
                {
                    return "It seems to be a Space Dock";
                }
                if (p.name.Contains( "enterprisescale"))
                {
                    return "It seems to be a  Constitution Class Starship";
                }
                if (p.name.Contains( "CardassianStation"))
                {
                    return "It seems to be a Cardassian Station";
                }
                if (p.name.Contains( "borg_cube"))
                {
                    return "It is Borg!";
                }
                if (p.name.Contains( "tng_saucer2")) //stargazer
                {
                    return "It seems to be a Constellation Class Starship";
                }
                if (p.name.Contains( "tng_saucer"))// galaxy
                {
                    return "It seems to be a Galaxy Class Starship";
                }
                if (p.name.Contains( "tardis"))
                {
                    return "It seems to be a phone both";
                }
                if (p.name.Contains( "car"))
                {
                    return "It seems to be a flying car - maybe a Ford";
                }
                if (p.name.Contains( "phoenix_cockpit"))
                {
                    return "It seems to be an ancient warp ship";
                }
                if (p.name.Contains("phoenix_cockpitRPM"))
                {
                    return "It seems to be an ancient warp ship";
                }
            }
            return "It's a ship of unknown configuration";



        }
    }
}
