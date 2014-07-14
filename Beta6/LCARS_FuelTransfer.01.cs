using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{
    class LCARS_FuelTransfer : PartModule
    {

        public string FuelTransferSoundFile = "SciFi/StarTrekImpulseDrive/sounds/transporterbeam";
        public float FuelTransferSoundVolume = 2.2f;
        public bool loopFuelTransferSound = false;
        public FXGroup FuelTransferSound = null;
        private ParticleEmitter emitter_FuelTransfer;

        System.Random rnd = new System.Random();


        LCARS_VesselPartsInventory VPI = null;
        LCARS_CargoBay STCB = null;
        LCARS_PowerSystem PowSys = null;
        PowerTaker PT1 = null;
        PowerTaker PT2 = null;

        public void setVPI(Vessel thisVessel, LCARS_VesselPartsInventory thisVPI, LCARS_CargoBay thisSTCB, LCARS_PowerSystem thisPowSys)
        {
            this.VPI = thisVPI;
            this.STCB = thisSTCB;
            TargetShipSelected = thisVessel;
            this.PowSys = thisPowSys;
            PT1 = this.PowSys.setPowerTaker("FuelTransfer", "SubSystem", 1250, 1250, 0);
            PT2 = this.PowSys.setPowerTaker("FuelTankManager", "SubSystem", 1250, 1250, 0);
        }

        /*protected override void onPartDestroy()
        {
            this.VPI.scanVessel();
        }*/

        Vessel SourceShipSelected = null;
        Vessel TargetShipSelected = null;
        string ResourceToMoveSelected = null;
        public Vector2 FTscrollPosition1;
        public Vector2 FTscrollPosition2;
        GUIStyle scrollview_style;
        GUIStyle scrollview_style2;
        string GUISelection = null;
        float tanksize = 0f;

        public void GUI()
        {
            if(GUISelection!=null)
            {
                if (GUILayout.Button("Back"))
                {
                    GUISelection = null;
                    pr_list = null;
                }

            }
            switch (GUISelection)
            {
                case "FuelTransferMain":
                    GUI_FuelTransferMain();
                        break;
                case "FuelTanksMain":
                    GUI_CargoBayFuelTanksMain();
                        break;
                default:
                    GUI_MainMenu();
                       break;

            }
            float power = PT1.L1_usage;
            this.PowSys.draw(PT1.takerName, power);
        }

        private void GUI_MainMenu()
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("FuelTransfer Main"))
            {
                GUISelection = "FuelTransferMain";
            }

            if (GUILayout.Button("FuelTanks Main"))
            {
                GUISelection = "FuelTanksMain";
            }
            GUILayout.EndVertical();
        }

        private void GUI_FuelTransferMain()
        {
            if (this.VPI.getModules().ContainsKey("LCARS_CargoBay"))
            {
                TargetShipSelected = FlightGlobals.ActiveVessel;

                scrollview_style = new GUIStyle();
                scrollview_style.fixedHeight = 90;
                scrollview_style2 = new GUIStyle();
                scrollview_style2.fixedHeight = 150;

                GUILayout.Label(" ");

                if (SourceShipSelected == null)
                {
                    GUI_Ship_List();
                }
                else
                {
                    if (ResourceToMoveSelected == null)
                    {
                        if (GUILayout.Button("Unset Ship"))
                        {
                            SourceShipSelected = null;
                        }
                        GUI_Source_Ship_Resource_List();
                    }
                    else
                    {
                        if (GUILayout.Button("Unset Resource"))
                        {
                            ResourceToMoveSelected = null;
                        }
                        GUI_transfer_options();
                    }
                }
            }
            else
            {
                GUILayout.Label("FuelTransfer? They're not going to be functioning without CargoBay");
            }
        }

        private void setTankSize(PartResource pr,float tanksize)
        {
            pr.maxAmount =Math.Round(tanksize, 2);
        }

        Dictionary<PartResource, float> pr_list = new Dictionary<PartResource, float>() { };
        private void GUI_CargoBayFuelTanksMain()
        {
            if (this.VPI.getModules().ContainsKey("LCARS_CargoBay"))
            {
                if (pr_list==null)
                {
                    pr_list = new Dictionary<PartResource, float>() { };
                }
                Part CB_Part = findCargoBay();
                GUILayout.Label("The CargoBay holds the following FuelTanks:");
                GUILayout.BeginVertical();
                PartResourceList pRL = CB_Part.Resources;
                    foreach (PartResource pr in pRL)
                    {

                        if (!pr_list.ContainsKey(pr))
                        {
                            pr_list.Add(pr,(float)pr.maxAmount);
                        }

                        GUILayout.BeginHorizontal();
                            GUILayout.Label(pr.resourceName);
                            GUILayout.Label(Math.Round(pr.amount, 2).ToString());
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                            GUILayout.Label("min");
                            GUILayout.Label("current");
                            GUILayout.Label("max");
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                            GUILayout.Label(Math.Round(pr.amount, 2).ToString());
                            pr_list[pr] = Single.Parse(GUILayout.TextField(Math.Round(pr_list[pr], 2).ToString()
                                    , 25));
                            GUILayout.Label(Math.Round((this.STCB.getFreeCargoSpace() / pr.info.density), 2).ToString());
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("save "+pr.resourceName))
                            {
                                float clamped = Mathf.Clamp(pr_list[pr], (float)pr.amount, (this.STCB.getFreeCargoSpace() / pr.info.density));
                                setTankSize(pr,clamped);
                            }
                        GUILayout.EndHorizontal();
                    }
                GUILayout.EndVertical();
                GUILayout.Label(" ");

                    GUILayout.Label("The rest of the ship holds the following FuelTanks:");
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name");
                GUILayout.Label("amount");
                GUILayout.Label("max");
                GUILayout.EndHorizontal();
                foreach (Part p in TargetShipSelected.Parts)
                {
                    if (p != CB_Part)
                    {
                        pRL = p.Resources;
                        foreach (PartResource pr in pRL)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(pr.resourceName);
                            GUILayout.Label(Math.Round(pr.amount, 2).ToString());
                            GUILayout.Label(Math.Round(pr.maxAmount, 2).ToString());
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                GUILayout.EndVertical();


                float power = PT2.L1_usage;
                this.PowSys.draw(PT2.takerName, power);
            }
            else
            {
                GUILayout.Label("CargoBays? They're not going to be installed until next Tuesday");
            }
        }

        private void GUI_Ship_List()
        {
            GUILayout.Label("Select Source Ship");
                GUILayout.BeginVertical(scrollview_style);
                FTscrollPosition1 = GUILayout.BeginScrollView(FTscrollPosition1);
                foreach (Vessel v in FlightGlobals.Vessels)
                {
                    if (v.checkVisibility() && v.loaded && v.id != TargetShipSelected.id)
                    {
                        if (GUILayout.Button("" + v.vesselName))
                        {
                            SourceShipSelected = v;
                        }
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
        }

        private void GUI_Source_Ship_Resource_List()
        {
            GUILayout.Label("Select Resource");
            GUILayout.BeginVertical(scrollview_style);
            FTscrollPosition2 = GUILayout.BeginScrollView(FTscrollPosition2);
            foreach (KeyValuePair<string, PartResource> pair in getVesselResources(this.SourceShipSelected))
            {
                if (GUILayout.Button("" + pair.Key))
                {
                    ResourceToMoveSelected = pair.Key;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }


        private void GUI_transfer_options()
        {

            if (tanksize == 0f)
            {
                tanksize = 10f;
            }


            resData resDat = getResourcesInfo(this.SourceShipSelected, ResourceToMoveSelected);
            resData resDat2 = getResourcesInfo(this.TargetShipSelected, ResourceToMoveSelected);


            bool tank_present = check_resourceTank(resDat);
            float freeCB = this.STCB.getFreeCargoSpace();


            if (tank_present && resDat2.totalFreeSpace > 0)
            {

                GUILayout.Label("Own Ship Data: ");

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Resource Tank available: " + tank_present);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Amount present: " + resDat2.totalAmount);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Free Capacity: " + Math.Round(resDat2.totalFreeSpace, 2));
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Free Cargo Space: " + freeCB+"t");
                GUILayout.EndHorizontal();

                
                GUI_transfer_gui();
            }
            else
            {
                if (tank_present && resDat2.totalFreeSpace <= 0)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Capt'n, this Tank is overfilled!");
                    GUILayout.Label("The ventile is completely sealed.");
                    GUILayout.Label("We can force the ventile open!");
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Aye");
                    GUILayout.Label("But this will destroy some of the resource?");
                    GUILayout.Label("How Much you ask.");
                    GUILayout.Label("Well - how should I know in advance?");
                    GUILayout.Label("I can only do my best now - can I?");
                    if (GUILayout.Button("Unlock the ventile for " + resDat.resourceName + " in the CargoBay"))
                    {
                        resDat2.PartResource.amount = resDat2.PartResource.amount - rnd.Next(1, 15);
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Yes - if we have enough cargobay, we can");
                    GUILayout.Label("make it bigger without loss.");
                    GUILayout.Label("You should talk to the fuel tank manager.");
                    if (GUILayout.Button("Ask the fuel tank manager"))
                    {
                        GUISelection = "FuelTanksMain";
                    }
                    GUILayout.EndVertical();
                }
                else 
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Capt'n, she doesn't have");
                    GUILayout.Label("this type of storage capacity!");
                    GUILayout.Label("Shall we rig someth'n up in ye CargoBay?");
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Aye - just select the tanksize and hit the button to confirm the order.");
                    GUILayout.Label("Ye CargoBay has currently " + freeCB + "t free");
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Label("Make it big enough, if you overfill the tank,");
                    GUILayout.Label("it might lock down the ventile!");
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("min");
                    GUILayout.Label("current");
                    GUILayout.Label("max");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(Math.Round(resDat.PartResource.amount / resDat.PartResource.info.density, 2).ToString());
                    tanksize = float.Parse(GUILayout.TextField(tanksize.ToString(), 25));
                    GUILayout.Label(Math.Round(freeCB, 2).ToString());
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                

                    if (GUILayout.Button("Create a "+tanksize+"t Tank for " + resDat.resourceName + " in the CargoBay"))
                    {
                        float clamped = Mathf.Clamp(tanksize, 0f, freeCB);
                        createTank(resDat, clamped);
                        tanksize = 0f;
                    }
                    GUILayout.EndHorizontal();
                }
                

            }

        }

        private PartResource findCargoBay_partResource(Vessel vesselToSearch, string ResourceToSearch)
        {
            PartResource CargoBayPartResourceRef = null;
            foreach (Part p in vesselToSearch.Parts)
            {
                PartResourceList pRL = p.Resources;
                foreach (PartResource pr in pRL)
                {
                    if (pr.resourceName == ResourceToSearch)
                    {
                        CargoBayPartResourceRef = pr;
                    }
                }
            }

            return CargoBayPartResourceRef;
        }


        private void GUI_transfer_gui()
        {
            resData resDat = getResourcesInfo(this.SourceShipSelected, ResourceToMoveSelected);
            resData resDat2 = getResourcesInfo(this.TargetShipSelected, ResourceToMoveSelected);
            PartResource CargoBayPartResourceRef = findCargoBay_partResource(this.TargetShipSelected, ResourceToMoveSelected);

            GUILayout.BeginVertical();
            
                GUILayout.Label("Source Ship Data:");

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Amount pesent: " + Math.Round(resDat.totalAmount, 2));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Free Capacity: " + Math.Round(resDat.totalFreeSpace, 2));
                GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical(scrollview_style2,GUILayout.Width(280));
            FTscrollPosition1 = GUILayout.BeginScrollView(FTscrollPosition1);
            foreach (Part p in this.SourceShipSelected.Parts)
            {
                PartResourceList pRL = p.Resources;
                foreach (PartResource pr in pRL)
                {
                    if (pr.resourceName == resDat.resourceName)
                    {
                        GUILayout.Label(pr.part.partInfo.title + " (" + Math.Round(pr.amount, 2) + ")");

                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Drain");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Fill");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.RepeatButton("-100"))
            {
                //tranfer_resource(pr, CargoBayPartResourceRef,100);
                tranfer_resource_shipwide(this.SourceShipSelected, this.TargetShipSelected, ResourceToMoveSelected, 100);
            }
            if (GUILayout.RepeatButton("-10"))
            {
                //tranfer_resource(pr, CargoBayPartResourceRef, 10);
                tranfer_resource_shipwide(this.SourceShipSelected, this.TargetShipSelected, ResourceToMoveSelected, 10);
            }
            if (GUILayout.RepeatButton("-1"))
            {
                //tranfer_resource(pr, CargoBayPartResourceRef, 1);
                tranfer_resource_shipwide(this.SourceShipSelected, this.TargetShipSelected, ResourceToMoveSelected, 1);
            }
            if (GUILayout.RepeatButton("+1"))
            {
                //tranfer_resource(CargoBayPartResourceRef, pr, 1);
                tranfer_resource_shipwide(this.TargetShipSelected, this.SourceShipSelected, ResourceToMoveSelected, 1);
            }
            if (GUILayout.RepeatButton("+10"))
            {
                //tranfer_resource(CargoBayPartResourceRef, pr, 10);
                tranfer_resource_shipwide(this.TargetShipSelected, this.SourceShipSelected, ResourceToMoveSelected, 10);
            }
            if (GUILayout.RepeatButton("+100"))
            {
                //tranfer_resource(CargoBayPartResourceRef, pr, 100);
                tranfer_resource_shipwide(this.TargetShipSelected, this.SourceShipSelected, ResourceToMoveSelected, 100);
            }
            GUILayout.EndHorizontal();



        }

        private void tranfer_resource_shipwide(Vessel SourceShip, Vessel TargetShip, string resourceName, float demand)
        {
            PartResource SourcepartResource = null;
            PartResource TargetpartResource = null;
            foreach (Part p in SourceShip.Parts)
            {
                if (SourcepartResource!=null)
                {
                    continue;
                }
                PartResourceList pRL = p.Resources;
                foreach (PartResource pr in pRL)
                {
                    if (pr.resourceName == resourceName && pr.amount>0)
                    {
                        SourcepartResource = pr;
                    }
                }
            }
            foreach (Part p2 in TargetShip.Parts)
            {
                if (TargetpartResource != null)
                {
                    continue;
                }
                PartResourceList pRL2 = p2.Resources;
                foreach (PartResource pr2 in pRL2)
                {
                    if (pr2.resourceName == resourceName && (pr2.maxAmount - pr2.amount) > 0)
                    {
                        TargetpartResource = pr2;
                    }
                }
            }
            tranfer_resource(SourcepartResource, TargetpartResource, demand);

            float power = demand * PT1.L2_usage;
            this.PowSys.draw(PT1.takerName, power);
        }

        private void tranfer_resource(PartResource SourcepartResource, PartResource TargetpartResource, float demand)
        {

            demand = (demand > (float)SourcepartResource.amount) ? (float)SourcepartResource.amount : demand;

            SourcepartResource.part.RequestResource(SourcepartResource.resourceName, demand);
            TargetpartResource.part.RequestResource(TargetpartResource.resourceName, demand * -1);


        }


        private Part findCargoBay()
        {
            //if (this.VPI.getModules().ContainsKey("LCARS_ShuttleBay") && this.VPI.getModules().ContainsKey("LCARS_CargoBay"))
            if (this.VPI.getModules().ContainsKey("LCARS_CargoBay"))
            {
                foreach (Part p in this.TargetShipSelected.Parts)
                {
                    PartModuleList pML = p.Modules;
                    foreach (PartModule pm in pML)
                    {
                        if (pm.moduleName == "LCARS_CargoBay")
                        {
                            return p;
                        }
                    }
                }
            }
            return null;
        }

        private bool check_resourceTank(resData resDat)
        {
            foreach (KeyValuePair<string, PartResource> pair in getVesselResources(this.TargetShipSelected))
            {
                if (pair.Key == resDat.resourceName)
                {
                    return true;
                }
            }
            return false;
        }


        private void createTank(resData resDat,float tanksize)
        {

            Part CargoBayPart = findCargoBay();
            if (CargoBayPart!=null)
            {
                ConfigNode newRes = new ConfigNode("RESOURCE");
                newRes.AddValue("name",resDat.resourceName);
                newRes.AddValue("amount",0F);
                newRes.AddValue("maxAmount", tanksize / resDat.PartResource.info.density);

                CargoBayPart.Resources.Add(newRes);
                //this.STCB.useCargoSpace(tanksize);

                float power = tanksize * PT2.L2_usage;
                this.PowSys.draw(PT2.takerName, power);
            }
        }



        private Dictionary<string, PartResource> getVesselResources(Vessel vesselToSearch)
        {
            Dictionary<string, PartResource> return_prl = new Dictionary<string, PartResource>() { };
            foreach (Part p in vesselToSearch.Parts)
            {
                PartResourceList pRL = p.Resources;
                foreach (PartResource pr in pRL)
                {
                    if (!return_prl.ContainsKey(pr.resourceName))
                    {
                        return_prl.Add(pr.resourceName,pr);
                    }
                }
            }
            return return_prl;
        }

        private resData getResourcesInfo(Vessel vesselToSearch, string ResourceToSearch)
        {
            resData tempDat = new resData();
            tempDat.resourceName = ResourceToSearch;
            foreach (Part p in vesselToSearch.Parts)
            {
                PartResourceList pRL = p.Resources;
                foreach (PartResource pr in pRL)
                {
                    if (pr.resourceName == tempDat.resourceName)
                    {
                        tempDat.PartResource = pr;
                        tempDat.totalAmount += pr.amount;
                        tempDat.totalWeight += pr.amount * pr.info.density;
                        tempDat.totalFreeSpace += pr.maxAmount - pr.amount;
                    }
                }
            }
            return tempDat;
        }


    }

    class resData
    {
        public PartResource PartResource { get; set; }
        public double totalAmount { get; set; }
        public double totalFreeSpace { get; set; }

        public double totalWeight { get; set; }
        public double totalFreeWeight { get; set; }

        public string resourceName { get; set; }
    }
 
}
