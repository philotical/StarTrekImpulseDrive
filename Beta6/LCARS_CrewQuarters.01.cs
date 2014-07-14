
using System;

namespace Philotical
{

    /*
    MODULE
    {
        name = LCARS_ImpulseDrive
        RESOURCE
        {
            name = ElectricCharge
            rate = 0.1
        }
    }
    MODULE
    {
        name = LCARS_ShuttleBay
        volume = 450
    }
   MODULE
    {
        name = LCARS_CargoBay
        maxTonnage = 200
    }
     MODULE
    {
        name = LCARS_CloakingDevice
    }
     MODULE
    {
        name = LCARS_StructuralIntegrityField
    }
   MODULE
    {
        name = LCARS_FuelTransfer
    }
    MODULE
    {
        name = LCARS_CrewQuartier
    }
    MODULE
    {
        name = LCARS_TransporterSystem
    }
    MODULE
    {
        name = LCARS_WeaponSystems
    }
    MODULE
    {
        name = LCARS_PhotonTorpedo
    }
    MODULE
    {
        name = LCARS_TractorBeam
    }
    MODULE
    {
        name = LCARS_SensorArray
    }
    
     */


    class LCARS_CrewQuartier : PartModule
    {

        Vessel vessel = null;
        int CrewCapacityTotal = 0;
        int CrewQuartersTotal = 0;
        int CrewQuartersUsed = 0;
        LCARS_VesselPartsInventory VPI = null;

        public void setVPI(LCARS_VesselPartsInventory thisVPI)
        {
            this.VPI = thisVPI;
        }

        public void onPartDestroy()
        {
            this.VPI.scanVessel();
        }


        internal void setVessel(Vessel thisVessel)
        {
            this.vessel = thisVessel;
            this.CrewCapacityTotal = this.vessel.rootPart.CrewCapacity;
            //this.part.AddModule("STCrewQuarters");
            //this.part.CreateInternalModel(PartLoader.GetInternalPart("mk1PodCockpit"));
            UnityEngine.Debug.Log("StarTrekCrewQuartier: setVessel  end");

        }

        internal void KerbalCheckIn()
        {
            this.CrewQuartersUsed++;
        }

        internal void KerbalCheckOut()
        {
            this.CrewQuartersUsed--;
        }

        internal int getFreeCrewSpace()
        {
            while ((this.vessel.GetCrewCount() + (this.CrewQuartersTotal - this.CrewQuartersUsed)) > this.vessel.rootPart.CrewCapacity)
            {
                KerbalCheckIn();
            }
            while ((this.vessel.GetCrewCount() + (this.CrewQuartersTotal - this.CrewQuartersUsed)) < this.vessel.rootPart.CrewCapacity)
            {
                KerbalCheckOut();
            }
            return this.CrewQuartersTotal - this.CrewQuartersUsed;
        }

        internal int getCrewQuartersTotal()
        {
            return this.CrewQuartersTotal;
        }

        internal int getCrewQuartersUsed()
        {
            return this.CrewQuartersUsed;
        }

        internal void addCrewSpace()
        {
            UnityEngine.Debug.Log("StarTrekCrewQuartier: addCrewSpace  begin CrewCapacity=" + this.vessel.rootPart.CrewCapacity);
            this.CrewQuartersTotal = calculateCrewSpace();
            int CrewCountTotal = this.vessel.GetCrewCount();
            this.vessel.rootPart.CrewCapacity += CrewQuartersTotal;
            this.CrewCapacityTotal = this.vessel.rootPart.CrewCapacity;

            //Part.AddInternalPart(ConfigNode)
            //Part.CreateInternalModel()

            //InternalModel foo = PartLoader.GetInternalPart("GenericSpace1");
            //this.vessel.rootPart.
           // Part.internalModel
            //Part.InternalModelName
           // ProtoPart.newPart
            UnityEngine.Debug.Log("StarTrekCrewQuartier: addCrewSpace  done  CrewCapacity=" + this.vessel.rootPart.CrewCapacity);


        }

        internal void addHatch()
        {
            if (this.vessel.rootPart.airlock == null)
            {
                this.vessel.rootPart.airlock = FlightGlobals.ActiveVessel.transform;
            }
        }

        internal int calculateCrewSpace()
        {

            return (int)Math.Round((this.VPI.getVesselDryMass() / 10), 0);
            //return (int)Math.Round((FlightGlobals.ActiveVessel.GetTotalMass() / 10), 0);
        }
    }
}
