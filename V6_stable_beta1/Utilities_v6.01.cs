using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{
    static class Utilities
    {
        public static float total_force = 0;
        public static float charge = 0;

        /// <summary>
        /// Takes a Charge and a Vessel as argument and pulls that amount of energy from the provided vessel
        /// </summary>
        public static bool usePower(float eCharge, Vessel thisVessel)
        {
            foreach (Part part in thisVessel.parts)
            {
                if (eCharge <= 0)
                {
                    continue;
                }
                eCharge = eCharge - part.RequestResource("ElectricCharge", eCharge);
            }
            return false;
        }

        /// <summary>
        /// Takes a Vessel as argument and tryes to calculate the power consumption at current accelleration
        /// will call usePower(float eCharge, Vessel thisVessel)
        /// </summary>
        public static Dictionary<string, float> CalculatePowerConsumption(Vessel thisVessel, bool gravityEnabled, bool UseFullImpulse, bool UseReservePower, float UseFullImpulse_multiplier, float UseReservePower_multiplier, float vSliderValue, float hSliderValue, float zSliderValue)
        {
            Dictionary<string, float> Powerstats = new Dictionary<string, float>();
            Powerstats.Add("charge", 0);
            Powerstats.Add("total_force", 0);
            Powerstats.Add("force_x", 0);
            Powerstats.Add("force_y", 0);
            Powerstats.Add("force_z", 0);
            if (!gravityEnabled)
            {
                return Powerstats;
            }
            charge = 200;
            float x = (vSliderValue < 0) ? vSliderValue * -1 : vSliderValue;
            float y = (hSliderValue < 0) ? hSliderValue * -1 : hSliderValue;
            float z = (zSliderValue < 0) ? zSliderValue * -1 : zSliderValue;
            Powerstats["force_x"] = x;
            Powerstats["force_y"] = y;
            Powerstats["force_z"] = z;

            total_force = x + y + z;
            total_force = (UseFullImpulse) ? total_force * UseFullImpulse_multiplier : total_force;
            total_force = (UseReservePower) ? total_force * UseReservePower_multiplier : total_force;

            charge += total_force * 12;
            charge = (UseFullImpulse) ? charge * UseFullImpulse_multiplier : charge;
            charge = (UseReservePower) ? charge * UseReservePower_multiplier : charge;

            if (charge > 0)
            {
                Utilities.usePower(charge, thisVessel);
            }
            Powerstats["charge"] = charge;
            Powerstats["total_force"] = total_force;

            return Powerstats;
        }
        public static void AdditionalPowerConsumption(Vessel thisVessel, float vSliderValue, float hSliderValue, float zSliderValue)
        {
            charge = 200;
            float x = (vSliderValue < 0) ? vSliderValue * -1 : vSliderValue;
            float y = (hSliderValue < 0) ? hSliderValue * -1 : hSliderValue;
            float z = (zSliderValue < 0) ? zSliderValue * -1 : zSliderValue;
            total_force = x + y + z;
            charge += total_force * 12;
            if (charge > 0)
            {
                Utilities.usePower(charge, thisVessel);
            }
        }
    }
}
