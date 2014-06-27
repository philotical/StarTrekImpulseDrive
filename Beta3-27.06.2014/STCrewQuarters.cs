
using System;
namespace Philotical
{
    class STCrewQuartiers : PartModule
    {

        Part part = null;

        internal void thisPart(Part thisPart)
        {
            this.part = thisPart;
            this.part.AddModule("STCrewQuarters");
            //this.part.CreateInternalModel(PartLoader.GetInternalPart("mk1PodCockpit"));
                
        }

        internal void addCrewSpace()
        {
            this.part.CrewCapacity += calculateCrewSpace();
        }

        internal void addHatch()
        {
            if (this.part.airlock == null)
            {
                this.part.airlock = FlightGlobals.ActiveVessel.transform;
            }
        }

        internal int calculateCrewSpace()
        {
            return (int)Math.Round((FlightGlobals.ActiveVessel.GetTotalMass() / 10), 0);
        }
    }
}
