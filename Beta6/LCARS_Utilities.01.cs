using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{
    static class LCARS_Utilities
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
        public static Dictionary<string, float> CalculatePowerConsumption(Dictionary<string, float> Powerstats, Vessel thisVessel, bool gravityEnabled, bool UseFullImpulse, bool UseReservePower, float UseFullImpulse_multiplier, float UseReservePower_multiplier, float vSliderValue, float hSliderValue, float zSliderValue)
        {
            /*
            Powerstats.Add("charge", 0);
            Powerstats.Add("total_force", 0);
            Powerstats.Add("force_x", 0);
            Powerstats.Add("force_y", 0);
            Powerstats.Add("force_z", 0);
            */
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
                LCARS_Utilities.usePower(charge, thisVessel);
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
                LCARS_Utilities.usePower(charge, thisVessel);
            }
        }

        public static Rect ClampToScreen(Rect r)
        {
            r.x = Mathf.Clamp(r.x, 0, Screen.width - r.width);
            r.y = Mathf.Clamp(r.y, 0, Screen.height - r.height);
            return r;
        }

        static public void SetLoadDistance(float loadDistance = 2500, float unloadDistance = 2250)
        {
            Vessel.loadDistance = loadDistance;
            Vessel.unloadDistance = unloadDistance;
        }

        static public string AlphaCharlyTango(string prefix,string input,int length=0)
        {
            length = (length > 0) ? length : input.Count();
			//pid = 92b38429ef9548439ec82d1f6ceccaf9
            //pid = 67cab79924cb484d94f99aed0914a036
            Dictionary<string,string> ACT = new Dictionary<string,string>(){};
            ACT.Add(" ", "Space");
            ACT.Add("-", "Dash");
            ACT.Add("A", "Alpha");
            ACT.Add("B","Bravo");
            ACT.Add("C","Charlie");
            ACT.Add("D","Delta");
            ACT.Add("E","Echo");
            ACT.Add("F","Foxtrot");
            ACT.Add("G","Golf");
            ACT.Add("H","Hotel");
            ACT.Add("I","India");
            ACT.Add("J","Juliet");
            ACT.Add("K","Kilo");
            ACT.Add("L","Lima");
            ACT.Add("M","Mike");
            ACT.Add("N","November");
            ACT.Add("O","Oscar");
            ACT.Add("P","Papa");
            ACT.Add("Q","Quebec");
            ACT.Add("R","Romeo");
            ACT.Add("S","Sierra");
            ACT.Add("T","Tango");
            ACT.Add("U","Uniform");
            ACT.Add("V","Victor");
            ACT.Add("W","Whiskey");
            ACT.Add("X","X-ray");
            ACT.Add("Y","Yankee");
            ACT.Add("Z","Zulu");
            ACT.Add("0","Zero");
            ACT.Add("1","One");
            ACT.Add("2","Two");
            ACT.Add("3","Three");
            ACT.Add("4","Four");
            ACT.Add("5","Five");
            ACT.Add("6","Six");
            ACT.Add("7","Seven");
            ACT.Add("8","Eight");
            ACT.Add("9","Nine");

            input = input.ToUpper();
            string output = "";
            string last_output=null;
            for (int i = 0; i < length; i++)
            {
                string c = input[i].ToString();
                //Console.WriteLine(input[i]);
                if (last_output==null)
                {
                    output = prefix + "-" + ACT[c];
                }
                else
                {
                    output = last_output + "-" + ACT[c];
                }

                last_output = output;
                output = "";
            }
            output = null;
            output = last_output;
            last_output = null;

            return output;

        }





    }
}
