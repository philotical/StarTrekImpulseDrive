using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Philotical
{
    public class PowerTaker
    {

        public string takerName { get; set; }

        public string takerType { get; set; }

        public float L1_usage { get; set; } // Level1: allways power draw
        public float L2_usage { get; set; } // Level2: on action power draw
        public float L3_usage { get; set; } // Level3: something else additionally

        public float consumption_current { get; set; }
        public float consumption_total { get; set; }
    }

    class LCARS_PowerSystem
    {

        Vessel ShipSelected = null;
        public Dictionary<string, PowerTaker> PowerTakers = null;


        internal PowerTaker getPowerTaker(string takerName)
        {
            return this.PowerTakers[takerName];
        }

        internal Dictionary<string, PowerTaker> getPowerTakers()
        {
            return this.PowerTakers;
        }

        internal PowerTaker setPowerTaker(string takerName, string takerType, float L1_usage = 0f, float L2_usage = 0f, float L3_usage = 0f)
        {
            PowerTaker foo = new PowerTaker();
            foo.takerName = takerName;
            foo.takerType = takerType;
            foo.L1_usage = L1_usage;
            foo.L2_usage = L2_usage;
            foo.L3_usage = L3_usage;
            if (this.PowerTakers.ContainsKey(takerName))
            {
                this.PowerTakers[takerName] = foo;
            }
            else
            {
                foo.consumption_current = 0f;
                foo.consumption_total = 0f;
                this.PowerTakers.Add(takerName, foo);
            }
            return this.PowerTakers[takerName];
        }

        internal void SetShip(Vessel v)
        {
            this.ShipSelected = v;
            this.PowerTakers = new Dictionary<string, PowerTaker>() { };

        }

        internal void reset_Powerstats()
        {
            foreach (KeyValuePair<string, PowerTaker> pair in this.PowerTakers)
            {
                    this.PowerTakers[pair.Value.takerName].consumption_current = 0;
            }
        }

        internal void draw(string takerName, float amount)
        {
            //UnityEngine.Debug.Log("LCARS_PowerSystem: draw takerName=" + takerName + " amount=" + amount);
            ShipSelected.rootPart.RequestResource("ElectricCharge", amount);
            this.PowerTakers[takerName].consumption_current = amount;
            this.PowerTakers[takerName].consumption_total += amount;
            
        }

        internal float get_consumption_total(bool alltime = false)
        {
            float returntotal = 0;
            foreach (KeyValuePair<string, PowerTaker> pair in this.PowerTakers)
            {
                float foo = (alltime) ? this.PowerTakers[pair.Value.takerName].consumption_total : this.PowerTakers[pair.Value.takerName].consumption_current;
                returntotal += foo;
            }
            return returntotal;
        }

        internal float get_consumption_main_systems(bool alltime = false)
        {
            float returntotal = 0;
            foreach (KeyValuePair<string, PowerTaker> pair in this.PowerTakers)
            {
                if (pair.Value.takerType == "MainSystem")
                {
                    float foo = (alltime) ? this.PowerTakers[pair.Value.takerName].consumption_total : this.PowerTakers[pair.Value.takerName].consumption_current;
                    returntotal += foo;
                }
            }
            return returntotal;
        }

        internal float get_consumption_sub_systems(bool alltime = false)
        {
            float returntotal = 0;
            foreach (KeyValuePair<string, PowerTaker> pair in this.PowerTakers)
            {
                if (pair.Value.takerType == "SubSystem")
                {
                    float foo = (alltime) ? this.PowerTakers[pair.Value.takerName].consumption_total : this.PowerTakers[pair.Value.takerName].consumption_current;
                    returntotal += foo;
                }
            }
            return returntotal;
        }




    }
}
