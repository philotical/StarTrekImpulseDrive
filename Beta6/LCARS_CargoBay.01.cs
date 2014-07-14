using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Philotical
{
    class LCARS_ShuttleBay : PartModule
    {
    }

    class LCARS_CargoBay : PartModule
    {
        Vessel vessel = null;
        float CargoSpaceTotal = 0f;
        float CargoSpaceUsed = 0f;
        float ResourceMass = 0f;

        LCARS_VesselPartsInventory VPI = null;

        public void onPartDestroy()
        {
            this.VPI.scanVessel();
        }

        internal void setVessel(Vessel thisVessel, LCARS_VesselPartsInventory thisVPI)
        {
            this.vessel = thisVessel;
            this.VPI = thisVPI;
        }
        private float getVessel_TotalResourceMass()
        {
            this.ResourceMass = 0f;
            foreach(Part p in this.vessel.Parts)
            {
                this.ResourceMass += p.GetResourceMass();

            }
            return this.ResourceMass;
        }
        internal float getTotalResourceMass()
        {
            return getVessel_TotalResourceMass();
        }

        internal void setupCargoSpace()
        {
            this.CargoSpaceTotal = calculateCargoSpace();
            this.ResourceMass = getVessel_TotalResourceMass();
        }

        internal void useCargoSpace(float weightAdded)
        {
            this.CargoSpaceUsed += weightAdded;
        }

        internal void releaseCargoSpace(float weightRemoved)
        {
            this.CargoSpaceUsed -= weightRemoved;
        }

        internal float getFreeCargoSpace()
        {
            return this.CargoSpaceTotal - this.CargoSpaceUsed - getVessel_TotalResourceMass();
        }


        internal float getUsedCargoSpace()
        {
            return this.CargoSpaceUsed + getVessel_TotalResourceMass();
        }

        internal float getTotalCargoSpace()
        {
            return this.CargoSpaceTotal;
        }

        private int calculateCargoSpace()
        {
            return (int)Math.Round(((this.VPI.getVesselDryMass() / 100 * 25) + getVessel_TotalResourceMass()), 0);
            //return (int)Math.Round((((FlightGlobals.ActiveVessel.GetTotalMass()) / 100 * 25) + getVessel_TotalResourceMass()), 0);
        }
    }
}
