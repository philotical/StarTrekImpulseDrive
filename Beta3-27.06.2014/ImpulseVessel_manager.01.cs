using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{
    public sealed class ImpulseVessel_manager
    {
        private static ImpulseVessel_manager _instance;

        public static ImpulseVessel_manager Instance
		{
			get
			{
				if (_instance == null)
				{
                    _instance = new ImpulseVessel_manager();
                }

				return _instance;
			}
		}



        internal Dictionary<string, ImpulseVesselType> ImpulseVesselList;
        public static ImpulseVesselType IVship;

        private ImpulseVessel_manager()
        {
            this.ImpulseVesselList = new Dictionary<string, ImpulseVesselType>();
        }

        public void populateImpulseVesselList(Vessel currentVessel)
        {
            UnityEngine.Debug.Log("ImpulseDrive: rebuild_vessel_list checking..");
            List<Vessel> vesselList;
            List<string> visibleVessels;
            List<string> removeVessels;
            vesselList = FlightGlobals.Vessels;
            visibleVessels = new List<string>() { };
            removeVessels = new List<string>() { };
            foreach (Vessel v in vesselList)
            {
                //UnityEngine.Debug.Log("ImpulseDrive: rebuild_vessel_list v in vesselList : " + v.vesselName);
                if (v.checkVisibility() && this.IsImpulseVessel(v) && !visibleVessels.Contains(v.id.ToString()) && v.loaded)
                {
                    visibleVessels.Add(v.id.ToString());
                    UnityEngine.Debug.Log("ImpulseDrive: rebuild_vessel_list confirmed Impulse Vessel: " + v.id.ToString() + " " + v.vesselName);
                    if (!this.ContainsShip(v.id.ToString()))
                    {
                    /*
                        //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 8 ");
                        this.AddImpulseVessel(v.id.ToString());
                        //UnityEngine.Debug.Log("ImpulseDrive: FixedUpdate 9 ");
                        this.ImpulseVesselList[v.id.ToString()].v = v;
                        this.ImpulseVesselList[v.id.ToString()].pid = v.id.ToString();
                        this.ImpulseVesselList[v.id.ToString()].name = v.vesselName;

                        List<StarTrekImpulseDrive> pM = v.FindPartModulesImplementing<StarTrekImpulseDrive>();
                        var foo = pM[0].Fields;
                        this.ImpulseVesselList[v.id.ToString()].is_active_vessel = (v.id.ToString() == currentVessel.id.ToString()) ? true : false;
                        this.ImpulseVesselList[v.id.ToString()].is_ignoreformationcommandos_enabled = false;
                        this.ImpulseVesselList[v.id.ToString()].is_formationMode_enabled = false;
                        this.ImpulseVesselList[v.id.ToString()].is_gravity_enabled = (foo["gravityEnabled"].ToString() == "True") ? true : false;
                        this.ImpulseVesselList[v.id.ToString()].geeVector = ConfigNode.ParseVector3(foo["geeVector"].ToString());
                        /*
                        this.ImpulseVesselList[v.id.ToString()].windowPosition = ConfigNode.ParseRect(foo["windowPosition"].ToString());
                        man.ImpulseVesselList[v.id.ToString()].is_fullHalt_enabled = this.bool_FullHalt;
                        man.ImpulseVesselList[v.id.ToString()].is_MakeSlowToSave_enabled = this.bool_MakeSlowToSave;
                        man.ImpulseVesselList[v.id.ToString()].is_fullImpulse_enabled = this.bool_FullImpulse;
                        man.ImpulseVesselList[v.id.ToString()].is_UseReserves_enabled = this.bool_UseReserves;
                        man.ImpulseVesselList[v.id.ToString()].is_accelerationLock_enabled = this.bool_AccelerationLock;
                        man.ImpulseVesselList[v.id.ToString()].is_pilotMode_enabled = this.bool_PilotMode;
                        man.ImpulseVesselList[v.id.ToString()].is_formationMode_enabled = this.bool_FormationMode;
                        man.ImpulseVesselList[v.id.ToString()].is_ignoreformationcommandos_enabled = this.bool_IgnoreFormationCommandos;
                        man.ImpulseVesselList[v.id.ToString()].is_holdspeed_enabled = this.bool_HoldSpeed_enabled;
                        man.ImpulseVesselList[v.id.ToString()].is_holdheight_enabled = this.bool_HoldHeight_enabled;
                        */
                        UnityEngine.Debug.Log("ImpulseDrive: rebuild_vessel_list this Impulse Vessel is not in the inventory: " + v.id.ToString() + " " + v.vesselName);
                        UnityEngine.Debug.Log("ImpulseDrive: this vessel will not respond to formation commands, you need to change vessel at least once to activate it");
                    }
                    else 
                    {
                        UnityEngine.Debug.Log("ImpulseDrive: rebuild_vessel_list inventory contains confirmed Impulse Vessel allready: " + v.id.ToString() + " " + v.vesselName);
                    }
                }
            }
            UnityEngine.Debug.Log("ImpulseDrive: rebuild_vessel_list checking for vanished Impulse Vessels now.. ");
            foreach (KeyValuePair<string, ImpulseVesselType> pair in this.getImpulseVesselList())
            {

                if (!visibleVessels.Contains(pair.Value.pid) && pair.Value.pid != currentVessel.id.ToString())
                {
                    removeVessels.Add(pair.Value.pid);
                    UnityEngine.Debug.Log("ImpulseDrive: rebuild_vessel_list obsolete Impulse Vessel found in inventory: " + pair.Value.pid + " " + pair.Value.name);
                }
            }
            foreach (string v in removeVessels)
            {
                UnityEngine.Debug.Log("ImpulseDrive: rebuild_vessel_list Removing Impulse Vessel from inventory: " + this.ImpulseVesselList[v].pid + " " + this.ImpulseVesselList[v].name);
                this.RemoveImpulseVessel(v);
            }
            UnityEngine.Debug.Log("ImpulseDrive: rebuild_vessel_list done, inventory up to date ");
            vesselList = null;
            visibleVessels = null;
            removeVessels = null;
        }

        public Dictionary<string, ImpulseVesselType> getImpulseVesselList()
        {
            return this.ImpulseVesselList;
        }
        
        public bool ContainsShip(string key)
        {
            if (this.ImpulseVesselList.ContainsKey(key))
            {
                return this.ImpulseVesselList.ContainsKey(key);
            }
            return false;
        }
        public int Count()
        {
            return this.ImpulseVesselList.Count;
        }
        public bool IsImpulseVessel(Vessel v)
        {
            List<Part> partsList = v.parts;
            bool is_ImpulseVessel = false;
            //UnityEngine.Debug.Log("ImpulseDrive: IsImpulseVessel ");
            foreach (Part p in partsList)
            {
                foreach (StarTrekImpulseDrive vesselPart in p.FindModulesImplementing<StarTrekImpulseDrive>())
                {
                    is_ImpulseVessel = true;
                }
            }
            partsList = null;
            return is_ImpulseVessel;
        }
        public ImpulseVesselType AddImpulseVessel(string key)
        {
            if (!this.ImpulseVesselList.ContainsKey(key))
            {
                ImpulseVesselType activeImpulseVessel = new ImpulseVesselType();
                this.ImpulseVesselList.Add(key, activeImpulseVessel);
                return activeImpulseVessel;
            }
            else 
            {
                return this.ImpulseVesselList[key];
            }
        }
        public void RemoveImpulseVessel(string key)
        {
            if (this.ImpulseVesselList.ContainsKey(key))
            {
                this.ImpulseVesselList.Remove(key);
            }
        }
        public bool IsFormationLeaderDefined()
        {
            bool leader = false;

            foreach (KeyValuePair<string, ImpulseVesselType> pair in this.ImpulseVesselList)
            {
                if (pair.Value.is_formationMode_enabled)
                {
                    leader = true;
                }
            }
            return leader;
        }
        public bool GetFormationLeaderSetting_bool(string key=null)
        {
            bool is_ = false;
            if(key!=null)
            {
                ImpulseVesselType leader = null;

                foreach (KeyValuePair<string, ImpulseVesselType> pair in this.ImpulseVesselList)
                {
                    if (pair.Value.is_formationMode_enabled)
                    {
                        leader = pair.Value;
                    }
                }
                switch (key)
                {
                    case"is_fullHalt_enabled":
                        is_ = leader.is_fullHalt_enabled;
                    break;

                    case"is_MakeSlowToSave_enabled":
                        is_ = leader.is_MakeSlowToSave_enabled;
                    break;

                    case"is_fullImpulse_enabled":
                        is_ = leader.is_fullImpulse_enabled;
                    break;

                    case"is_UseReserves_enabled":
                        is_ = leader.is_UseReserves_enabled;
                    break;

                    case"is_accelerationLock_enabled":
                        is_ = leader.is_accelerationLock_enabled;
                    break;

                    case"is_pilotMode_enabled":
                        is_ = leader.is_pilotMode_enabled;
                    break;

                    case"is_holdspeed_enabled":
                        is_ = leader.is_holdspeed_enabled;
                    break;

                    case"is_holdheight_enabled":
                        is_ = leader.is_holdheight_enabled;
                    break;

                }
            }
            return is_;
        }
        public Vessel GetFormationLeaderVessel()
        {
            //UnityEngine.Debug.Log("ImpulseDrive: GetFormationLeaderVessel begin");
            Vessel leader = null;

            foreach (KeyValuePair<string, ImpulseVesselType> pair in this.ImpulseVesselList)
            {
                if (pair.Value.is_formationMode_enabled)
                {
                    leader = pair.Value.v;
                }
            }
            return leader;
        }
        public void ChangeFormationLeader(string new_Leader_key)
        {
            //UnityEngine.Debug.Log("ImpulseDrive: ChangeFormationLeader begin new_Leader_key=" + new_Leader_key);
            foreach (KeyValuePair<string, ImpulseVesselType> pair in this.ImpulseVesselList)
            {
                //UnityEngine.Debug.Log("ImpulseDrive: ChangeFormationLeader  pair.Value.is_formationMode_enabled=" + pair.Value.is_formationMode_enabled);
                if (pair.Value.is_formationMode_enabled)
                {
                    //UnityEngine.Debug.Log("ImpulseDrive: ChangeFormationLeader  pair.Key=" + pair.Key);
                    this.ImpulseVesselList[pair.Key].is_active_vessel = false;
                    this.ImpulseVesselList[pair.Key].is_formationMode_enabled = false;
                }
            }
            this.ImpulseVesselList[new_Leader_key].is_active_vessel = true;
            this.ImpulseVesselList[new_Leader_key].is_formationMode_enabled = true;
            //UnityEngine.Debug.Log("ImpulseDrive: ChangeFormationLeader done new_Leader_key=" + new_Leader_key);
        }
        public void print_control_values(string called_by,bool is_active_vessel,bool is_gravity_enabled,bool is_fullHalt_enabled,bool is_MakeSlowToSave_enabled,bool is_fullImpulse_enabled,bool is_UseReserves_enabled, bool is_accelerationLock_enabled,bool is_pilotMode_enabled,bool is_formationMode_enabled,bool is_ignoreformationcommandos_enabled,bool is_holdspeed_enabled,bool is_holdheight_enabled)
        {
            UnityEngine.Debug.Log("================================================================");
            UnityEngine.Debug.Log("ImpulseDrive: ImpulseVessel_manager print_control_values:");
            UnityEngine.Debug.Log("called_by: " + called_by);

                UnityEngine.Debug.Log("-------------------------");
                UnityEngine.Debug.Log("is_active_vessel: " + is_active_vessel);
                UnityEngine.Debug.Log("is_gravity_enabled: " + is_gravity_enabled);
                UnityEngine.Debug.Log("is_fullHalt_enabled: " + is_fullHalt_enabled);
                UnityEngine.Debug.Log("is_MakeSlowToSave_enabled: " + is_MakeSlowToSave_enabled);
                UnityEngine.Debug.Log("is_fullImpulse_enabled: " + is_fullImpulse_enabled);
                UnityEngine.Debug.Log("is_UseReserves_enabled: " + is_UseReserves_enabled);
                UnityEngine.Debug.Log("is_accelerationLock_enabled: " + is_accelerationLock_enabled);
                UnityEngine.Debug.Log("is_pilotMode_enabled: " + is_pilotMode_enabled);
                UnityEngine.Debug.Log("is_formationMode_enabled: " + is_formationMode_enabled);
                UnityEngine.Debug.Log("is_ignoreformationcommandos_enabled: " + is_ignoreformationcommandos_enabled);
                UnityEngine.Debug.Log("is_holdspeed_enabled: " + is_holdspeed_enabled);
                UnityEngine.Debug.Log("is_holdheight_enabled: " + is_holdheight_enabled);

            UnityEngine.Debug.Log("================================================================");
        }
        public void print_stats(string called_by)
        {
            UnityEngine.Debug.Log("================================================================");
            UnityEngine.Debug.Log("ImpulseDrive: ImpulseVessel_manager print_stats:");
            UnityEngine.Debug.Log("number of items in List: " + this.ImpulseVesselList.Count);
            UnityEngine.Debug.Log("called_by: " + called_by);
            foreach (KeyValuePair<string, ImpulseVesselType> pair in this.ImpulseVesselList)
            {
                IVship = pair.Value;

                UnityEngine.Debug.Log("-------------------------");
                UnityEngine.Debug.Log("ship: " + IVship.pid);
                UnityEngine.Debug.Log("name: " + IVship.name);
                UnityEngine.Debug.Log("windowPosition: " + IVship.windowPosition.ToString());
                UnityEngine.Debug.Log("geeVector: " + IVship.geeVector.ToString());
                UnityEngine.Debug.Log("is_active_vessel: " + IVship.is_active_vessel.ToString());
                UnityEngine.Debug.Log("is_gravity_enabled: " + IVship.is_gravity_enabled.ToString());
                UnityEngine.Debug.Log("is_fullHalt_enabled: " + IVship.is_fullHalt_enabled.ToString());
                UnityEngine.Debug.Log("is_MakeSlowToSave_enabled: " + IVship.is_MakeSlowToSave_enabled.ToString());
                UnityEngine.Debug.Log("is_fullImpulse_enabled: " + IVship.is_fullImpulse_enabled.ToString());
                UnityEngine.Debug.Log("is_UseReserves_enabled: " + IVship.is_UseReserves_enabled.ToString());
                UnityEngine.Debug.Log("is_accelerationLock_enabled: " + IVship.is_accelerationLock_enabled.ToString());
                UnityEngine.Debug.Log("is_pilotMode_enabled: " + IVship.is_pilotMode_enabled.ToString());
                UnityEngine.Debug.Log("is_formationMode_enabled: " + IVship.is_formationMode_enabled.ToString());
                UnityEngine.Debug.Log("is_ignoreformationcommandos_enabled: " + IVship.is_ignoreformationcommandos_enabled.ToString());
                UnityEngine.Debug.Log("is_holdspeed_enabled: " + IVship.is_holdspeed_enabled.ToString());
                UnityEngine.Debug.Log("HoldSpeed_value: " + IVship.HoldSpeed_value);
                UnityEngine.Debug.Log("is_holdheight_enabled: " + IVship.is_holdheight_enabled.ToString());
                UnityEngine.Debug.Log("HoldHeight_value: " + IVship.HoldHeight_value);
                


            }
            Vessel activeVessel = FlightGlobals.ActiveVessel;
            string ActiveVessel_pid = activeVessel.id.ToString();
            UnityEngine.Debug.Log("activeVessel.Landed: (" + ActiveVessel_pid+") " + activeVessel.Landed);
            UnityEngine.Debug.Log("Vessel.GetSituationString(activeVessel): (" + ActiveVessel_pid + ") " + Vessel.GetSituationString(activeVessel));

            UnityEngine.Debug.Log("================================================================");
        }
    }
}
