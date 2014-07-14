using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{
    class LCARS_VesselPartsInventoryType
    {

        public Vessel vessel { get; set; }

        public List<ProtoCrewMember> vesselCrew { get; set; }

        public List<Part> vesselParts { get; set; }
        public float vesselDryMass { get; set; }
        public float vesselWetMass { get; set; }
        public float vesselResourceMass { get; set; }

        public Dictionary<int, ModuleGenerator> vesselResourceGenerators { get; set; }
        public Dictionary<string, PartModule> vesselPartModules { get; set; }
        public Dictionary<string, PartResource> vesselPartResources { get; set; }

        public float AveragePartTemperatureMax { get; set; }

        public float AveragePartTemperature { get; set; }

        public float heat_percentage { get; set; }

        public float hullintegrity_percentage { get; set; }
    }
    class LCARS_VesselPartsInventory
    {
        //            VesselPartsInventory foo = new VesselPartsInventory();

        private LCARS_VesselPartsInventoryType VPIT;
        private Vessel vessel;
        internal void init(Vessel thisVessel)
        {
            this.VPIT = new LCARS_VesselPartsInventoryType();
            this.vessel = thisVessel;
            this.VPIT.vessel = this.vessel;
        }

        internal void print_Modules()
        {
            UnityEngine.Debug.Log("VesselPartsInventory: print_Modules  vesselName=" + this.vessel.vesselName);
            foreach (KeyValuePair<string, PartModule> pair in this.VPIT.vesselPartModules)
            {
                UnityEngine.Debug.Log("VesselPartsInventory: print_Modules  moduleName=" + pair.Value.moduleName);
            }
        }
        internal void scanVessel()
        {

            List<ProtoCrewMember> VesselCrew = this.vessel.GetVesselCrew();

            this.VPIT.vesselCrew = VesselCrew;

            List<Part> vesselParts = this.vessel.Parts;

            this.VPIT.vesselParts = vesselParts;


            this.VPIT.vesselResourceGenerators = new Dictionary<int, ModuleGenerator>() { };
            
            //ModuleGenerator[] MG_list = UnityEngine.Object.FindObjectsOfType<ModuleGenerator>();
            List<ModuleGenerator> MG_list = this.VPIT.vessel.FindPartModulesImplementing<ModuleGenerator>();
            int i = 0;
            foreach (ModuleGenerator MG in MG_list)
            {
                List<ModuleGenerator.GeneratorResource> MG_GR = MG.outputList;
                if (!this.VPIT.vesselResourceGenerators.ContainsKey(i))
                {
                    this.VPIT.vesselResourceGenerators.Add(i, MG);
                }
                i++;
            }

            this.VPIT.vesselPartModules = new Dictionary<string, PartModule>() { };
            this.VPIT.vesselPartResources = new Dictionary<string, PartResource>() { };
            this.VPIT.AveragePartTemperature = 0f;
            this.VPIT.AveragePartTemperatureMax = 0f;
            foreach (Part p in this.VPIT.vesselParts)
            {
                this.VPIT.AveragePartTemperature += p.temperature;
                this.VPIT.AveragePartTemperatureMax += p.maxTemp;

                this.VPIT.vesselResourceMass += p.GetResourceMass();

                PartModuleList pML = p.Modules;
                
                foreach (PartModule pm in pML)
                {
                    if (!this.VPIT.vesselPartModules.ContainsKey(pm.moduleName))
                    {
                        this.VPIT.vesselPartModules.Add(pm.moduleName, pm);
                    }
                }

                PartResourceList pRL = p.Resources;
                foreach (PartResource pr in pRL)
                {
                    if (!this.VPIT.vesselPartResources.ContainsKey(pr.resourceName))
                    {
                        this.VPIT.vesselPartResources.Add(pr.resourceName, pr);
                    }
                }
            }
            this.VPIT.heat_percentage = this.VPIT.AveragePartTemperature / (this.VPIT.AveragePartTemperatureMax / 100);
            this.VPIT.hullintegrity_percentage = 100 - this.VPIT.heat_percentage;
            this.VPIT.vesselWetMass = this.vessel.GetTotalMass();
            this.VPIT.vesselDryMass += this.VPIT.vesselWetMass - this.VPIT.vesselResourceMass;
            //UnityEngine.Debug.Log("VesselPartsInventory: print_Modules  vesselResourceMass=" + this.VPIT.vesselResourceMass + "  vesselWetMass=" + this.VPIT.vesselWetMass + "  vesselDryMass=" + this.VPIT.vesselDryMass);
        }

        internal Dictionary<int, ModuleGenerator> getResourceGenerators()
        {
            return this.VPIT.vesselResourceGenerators;
        }

        internal Dictionary<string, PartResource> getResources()
        {
            //no resource total - just a List of all available
            return this.VPIT.vesselPartResources;
        }

        internal List<Part> getParts()
        {
            return this.VPIT.vesselParts;
        }

        internal List<ProtoCrewMember> getCrew()
        {
            return this.VPIT.vesselCrew;
        }

        internal Dictionary<string, PartModule> getModules()
        {
            return this.VPIT.vesselPartModules;
        }

        internal bool checkForPartWithModule(string modName)
        {
            return (this.VPIT.vesselPartModules[modName].moduleName == modName) ? true : false; 
        }

        internal Part getPartWithModule(string modName)
        {
            return this.VPIT.vesselPartModules[modName].part;
        }

        internal ModuleGenerator.GeneratorResource find_EC_Generator()
        {
            /*
            MODULE
            {
                name = ModuleGenerator
                isAlwaysActive = false
                OUTPUT_RESOURCE
                {
                   name = WarpPlasma
                   rate = 5000000
                }	
            }
            */
            foreach (KeyValuePair<int, ModuleGenerator> pair in getResourceGenerators())
            {
                List<ModuleGenerator.GeneratorResource> MG_GR = pair.Value.outputList;
                foreach (ModuleGenerator.GeneratorResource GR in MG_GR)
                {
                    if (GR.name == "ElectricCharge")
                    {
                        return GR;
                    }
                }
            }
            //ModuleGenerator newMG = new ModuleGenerator();
            ConfigNode newMG = new ConfigNode("MODULE");
            newMG.AddValue("name", "ModuleGenerator");
            newMG.AddValue("isAlwaysActive", "true");
            ConfigNode OUTPUT_RESOURCE = new ConfigNode("OUTPUT_RESOURCE");
            OUTPUT_RESOURCE.AddValue("name", "true");
            OUTPUT_RESOURCE.AddValue("rate", "80000");
            newMG.AddNode(OUTPUT_RESOURCE);
            this.vessel.rootPart.AddModule(newMG);
            return find_EC_Generator();
        }

        internal float getVesselHullintegrity_percentage()
        {
             return this.VPIT.hullintegrity_percentage = 100 - this.VPIT.heat_percentage;
        }

        internal float getVesselHullHeat_percentage()
        {
            return this.VPIT.heat_percentage = getVesselAveragePartTemperature() / (getVesselAveragePartTemperatureMax() / 100);
        }

        internal float getVesselAveragePartTemperature()
        {
            int i = 0;
            float tmp = 0f;
            this.VPIT.AveragePartTemperature = 0f;
            foreach (Part p in this.VPIT.vesselParts)
            {
                tmp += p.temperature;
                i++;
            }
            this.VPIT.AveragePartTemperature = tmp / i;
            return this.VPIT.AveragePartTemperature;
        }

        internal float getVesselAveragePartTemperatureMax()
        {
            int i = 0;
            float tmp = 0f;
            this.VPIT.AveragePartTemperatureMax = 0f;
            foreach (Part p in this.VPIT.vesselParts)
            {
                tmp += p.maxTemp;
                i++;
            }
            this.VPIT.AveragePartTemperatureMax = tmp / i;
            return this.VPIT.AveragePartTemperatureMax;
        }

        internal float getVesselDryMass()
        {
            return this.VPIT.vesselDryMass;
        }

        internal float getVesselWetMass()
        {
            return this.VPIT.vesselWetMass;
        }

        internal float getVesselResourceMass()
        {
            return this.VPIT.vesselResourceMass;
        }
    }
}
