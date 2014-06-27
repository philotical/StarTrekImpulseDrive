using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{
    class TransporterSystem
    {

        private string TransportMode = null;
        float zoomFactor = 1;
        private ProtoCrewMember kerbal;
        GimbalDebug gimbalDebug2 = null;
        PlanetScanner planetScanner = null;
        private Vessel CurrentMotherShip;
        GUIStyle scrollview_style;
        
        internal void init()
        {
                planetScanner = new PlanetScanner();
                planetScanner.SetMotherShip(CurrentMotherShip);
                Utilities.SetLoadDistance(100000, 100000);



        }
        internal void SetMotherShip(Vessel MS)
        {
            this.CurrentMotherShip = MS;
            //FlightGlobals.ActiveVessel
        }
        internal void GUI(Rect screen_rect)
        {
            scrollview_style = new GUIStyle();
            scrollview_style.fixedHeight = 90;

            if (planetScanner == null)
            {
                init();
            }


            if(TransportMode!=null)
            {
                switch (TransportMode)
                {

                    case "ShipToEVA":
                        GUI_ShipToEVA(screen_rect);
                        break;

                    case "EVAToShip":
                        GUI_EVAToShip();
                        break;

                    case "ShipToShip":
                        GUI_ShipToShip();
                        break;
                }
            }
            else
            {
                if (gimbalDebug2 != null) { gimbalDebug2.removeGimbal(); }
                GUILayout.Label("Selet Transporter Mode: ");
                if (GUILayout.Button("Ship To EVA"))
                {
                    TransportMode = "ShipToEVA";
                }
                if (GUILayout.Button("EVA To Ship"))
                {
                    TransportMode = "EVAToShip";
                }
                if (GUILayout.Button("Ship To Ship"))
                {
                    TransportMode = "ShipToShip";
                }
            }
        }


        private GameObject transporter_target = new GameObject("GameObject");
        private float fixed_height = 0f;
        private Vector2 ShipToEVAscrollPosition;
        private string ShipToEVASubMode=null;
        private int fixed_beam_distance = 120;
        private void GUI_ShipToEVA(Rect screen_rect)
        {
            if(ShipToEVASubMode==null)
            {
                if (GUILayout.Button("Back"))
                {
                    TransportMode = null;
                    ShipToEVASubMode = null;
                }

                GUILayout.Label("Selet Target Location: ");
                if (GUILayout.Button("Ship To Ground"))
                {
                    ShipToEVASubMode = "ShipToGround";
                }
                if (GUILayout.Button("Ship To Space"))
                {
                    ShipToEVASubMode = "ShipToSpace";
                }

            }
            else
            {
                if (GUILayout.Button("Back"))
                {
                    ShipToEVASubMode = null;
                }

                switch (ShipToEVASubMode)
                {
                    case"ShipToGround":
                        // Planet Scanner
                        planetScanner.activateScanner(zoomFactor, screen_rect);
                        GUILayout.Label("zoomFactor: " + Math.Round(zoomFactor, 2));
                        zoomFactor = GUILayout.HorizontalSlider(zoomFactor, 1.01F, 100.0F);
                        // Planet Scanner
                        transporter_target.transform.parent = this.CurrentMotherShip.transform; // ...child to our part...
                        transporter_target.transform.localPosition = Vector3.zero;
                        transporter_target.transform.localEulerAngles = Vector3.zero;

                        if (gimbalDebug2 == null) { gimbalDebug2 = new GimbalDebug(); } else { gimbalDebug2.removeGimbal(); }
                        gimbalDebug2.drawGimbal(transporter_target, 3, 0.2f);

                        Vector3 gee = FlightGlobals.getGeeForceAtPosition(this.CurrentMotherShip.transform.position);

                        transporter_target.transform.rotation = Quaternion.LookRotation(gee.normalized);
                        float heightFromSurface = ((float)this.CurrentMotherShip.altitude - this.CurrentMotherShip.heightFromTerrain < 0F) ? (float)this.CurrentMotherShip.altitude : this.CurrentMotherShip.heightFromTerrain;
                        heightFromSurface = (heightFromSurface != -1) ? heightFromSurface : (float)this.CurrentMotherShip.altitude;
                        fixed_height = heightFromSurface - 0.5f;
                        transporter_target.transform.localPosition = Vector3.forward * fixed_height;

                        GUILayout.Label("Select a Kerbal");
                        // Kerbal List
                        List<ProtoCrewMember> crewList = this.CurrentMotherShip.GetVesselCrew();

                        GUILayout.BeginVertical(scrollview_style);
                        ShipToEVAscrollPosition = GUILayout.BeginScrollView(ShipToEVAscrollPosition);
                        foreach (ProtoCrewMember cm in crewList)
                        {
                            if (GUILayout.Button("EVA " + cm.name))
                            {
                                //TransportMode = null;
                                transportKerbal_ShipToGround(cm);
                            }
                        }
                        GUILayout.EndScrollView();
                        GUILayout.EndVertical();
                        break;

                    case"ShipToSpace":

                        GUILayout.Label("Set Required Distance from Ship in Meter");
                        fixed_beam_distance = Int32.Parse(GUILayout.TextField(fixed_beam_distance.ToString(), 25));

                        transporter_target.transform.parent = this.CurrentMotherShip.transform; // ...child to our part...
                        transporter_target.transform.localPosition = Vector3.zero;
                        transporter_target.transform.localEulerAngles = Vector3.zero;
                        transporter_target.transform.localPosition = Vector3.up * fixed_beam_distance;


                        GUILayout.Label("Select a Kerbal");
                        // Kerbal List
                        List<ProtoCrewMember> crewList2 = this.CurrentMotherShip.GetVesselCrew();
                        GUILayout.BeginVertical(scrollview_style);
                        ShipToEVAscrollPosition = GUILayout.BeginScrollView(ShipToEVAscrollPosition);
                        foreach (ProtoCrewMember cm in crewList2)
                        {
                            if (GUILayout.Button("EVA " + cm.name))
                            {
                                //TransportMode = null;
                                transportKerbal_ShipToSpace(cm);
                            }
                        }
                        GUILayout.EndScrollView();
                        GUILayout.EndVertical();
                        break;

                }
            }

        }






        public Vector2 EVAToShipscrollPosition;
        private void GUI_EVAToShip()
        {
            if (GUILayout.Button("Back"))
            {
                TransportMode = null;
            }
            GUILayout.Label("Select a Kerbal");

            GUILayout.BeginVertical(scrollview_style);
            EVAToShipscrollPosition = GUILayout.BeginScrollView(EVAToShipscrollPosition);
            foreach (Vessel v in FlightGlobals.Vessels)
            {
                if (v.checkVisibility() && v.vesselType == VesselType.EVA)
                {
                    if (GUILayout.Button("Board " + v.vesselName))
                    {
                        transportKerbal_EVAToShip(v);
                    }

                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
           
            
            /*
            KerbalSeat EmptySeat;

            kerbal = null;
            for (int i = FlightGlobals.Vessels.Count - 1; i >= 0; i--)
            {
                if (MotherShipGuid == FlightGlobals.Vessels[i].id)
                {
                    kerbal = 
                    UnityEngine.Debug.Log(KerbalEVA.BoardSeat(kerbal, p, p.airlock));
                        KerbalEVA.BoardSeat(EmptySeat)
                    return;
                }
            }


            // Kerbal List
            if (kerbal != null)
            {
                transportKerbal_GroundToShip(kerbal);
            }
            */
        }

        Vessel SourceShipSelected = null;
        Vessel TargetShipSelected = null;
        ProtoCrewMember KerbalToMoveSelected = null;
        public Vector2 ShipToShipscrollPosition1;
        public Vector2 ShipToShipscrollPosition2;
        private void GUI_ShipToShip()
        {
            if (GUILayout.Button("Back"))
            {
                TransportMode = null;
                SourceShipSelected = null;
                TargetShipSelected = null;
                KerbalToMoveSelected = null;
            }
            GUILayout.Label(" ");

            GUI_ShipSource_List();

            GUILayout.Label(" ");

                GUI_ShipTarget_List();

            return;
        }

        internal int Get_EmptySeatCount(Vessel v)
        {
            return v.GetCrewCapacity() - v.GetCrewCount();
        }

        internal void GUI_ShipSource_List()
        {
            if (SourceShipSelected == null)
            {
                GUILayout.Label("Select Source Ship");
                GUILayout.BeginVertical(scrollview_style);
                ShipToShipscrollPosition1 = GUILayout.BeginScrollView(ShipToShipscrollPosition1);
                foreach (Vessel v in FlightGlobals.Vessels)
                {
                    if (v.checkVisibility())
                    {
                        bool shipHasCrew = false;
                        foreach (Part part in v.Parts)
                        {
                            if (part.protoModuleCrew.Count > 0)
                            {
                                shipHasCrew = true;
                            }
                        }
                        if(shipHasCrew)
                        {
                            if (GUILayout.Button("" + v.vesselName + " (" + v.GetCrewCount() + ")"))
                            {
                                SourceShipSelected = v;
                            }
                        }
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label("Source Ship Selected: " + SourceShipSelected.vesselName);
                GUILayout.BeginVertical(scrollview_style);
                ShipToShipscrollPosition1 = GUILayout.BeginScrollView(ShipToShipscrollPosition1);
                if (KerbalToMoveSelected == null)
                {
                    GUILayout.Label("Select a Kerbal");
                    List<ProtoCrewMember> crewList = SourceShipSelected.GetVesselCrew();
                    // Kerbal List
                    foreach (ProtoCrewMember cm in crewList)
                    {
                        if (GUILayout.Button("EVA " + cm.name))
                        {
                            KerbalToMoveSelected = cm;
                        }
                    }
                }
                else
                {
                    GUILayout.Label("Kerbal Selected: " + KerbalToMoveSelected.name);
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
        }

        internal void GUI_ShipTarget_List()
        {
            if (TargetShipSelected == null)
            {
                GUILayout.Label("Select Target Ship");
                GUILayout.BeginVertical(scrollview_style);
                ShipToShipscrollPosition2 = GUILayout.BeginScrollView(ShipToShipscrollPosition2);
                foreach (Vessel v in FlightGlobals.Vessels)
                {
                    if (v.checkVisibility())
                    {
                        bool shipHasSeat = false;
                        foreach (Part part in v.Parts)
                        {
                            if (part.CrewCapacity > 0)
                            {
                                shipHasSeat = true;
                            }
                        }
                        if (shipHasSeat)
                        {
                            if (GUILayout.Button("" + v.vesselName + " (" + Get_EmptySeatCount(v) + ")"))
                            {

                                TargetShipSelected = v;
                            }

                        }
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            else 
            {
                GUILayout.Label("Target Ship Selected: " + TargetShipSelected.vesselName);
                if (SourceShipSelected != null)
                {
                    if (KerbalToMoveSelected == null)
                    {
                        GUILayout.Label("No Kerbal Selected");
                    }
                    else 
                    {
                        GUILayout.Label("Select Target Part");
                        GUILayout.BeginVertical(scrollview_style);
                        ShipToShipscrollPosition2 = GUILayout.BeginScrollView(ShipToShipscrollPosition2);
                        foreach (Part part in TargetShipSelected.Parts)
                        {
                            if (!ShipToShip_PartIsFull(part))
                            {
                                if (GUILayout.Button("=>" + part.name))
                                {
                                    // transport kerbal
                                    //UnityEngine.Debug.Log("ImpulseDrive Transporter GUI_ShipTarget_List Button clicked " + part.name);
                                   // UnityEngine.Debug.Log("ImpulseDrive Transporter GUI_ShipTarget_List SourceShipSelected" + SourceShipSelected.vesselName);
                                    //UnityEngine.Debug.Log("ImpulseDrive Transporter GUI_ShipTarget_List TargetShipSelected" + TargetShipSelected.vesselName);
                                    //UnityEngine.Debug.Log("ImpulseDrive Transporter GUI_ShipTarget_List KerbalToMoveSelected" + KerbalToMoveSelected.name);
                                    transportKerbal_ShipToShip(SourceShipSelected, TargetShipSelected,part, KerbalToMoveSelected);
                                    //UnityEngine.Debug.Log("ImpulseDrive Transporter GUI_ShipTarget_List Button done " + part.name);
                                }
                            }
                        }
                        GUILayout.EndScrollView();
                        GUILayout.EndVertical();
                    }
                }
                else
                {
                        GUILayout.Label("No Source Ship Selected");
                }
            }
        }


        // //////////////////////////////////////////////////////////////////////////////////////////////////
        private KerbalEVA kerbalEVA;
        internal void transportKerbal_EVAToShip(Vessel evaKerbal)
        {
            UnityEngine.Debug.Log("ImpulseDrive Transporter EVAToShip Button begin " + evaKerbal.name);
            kerbalEVA = (KerbalEVA)evaKerbal.rootPart.Modules["KerbalEVA"];

            UnityEngine.Debug.Log("ImpulseDrive Transporter EVAToShip Button 1 " );
            this.CurrentMotherShip.GoOffRails();
            foreach (Part part in this.CurrentMotherShip.Parts)
            {
                if (part.CrewCapacity > 0)
                {
                    //UnityEngine.Debug.Log("ImpulseDrive Transporter EVAToShip Button 1 ");

                    kerbalEVA.BoardPart(part);
                    //UnityEngine.Debug.Log("ImpulseDrive Transporter EVAToShip Button 1 ");

                    this.CurrentMotherShip.SpawnCrew();
                    //UnityEngine.Debug.Log("ImpulseDrive Transporter EVAToShip Button 1 ");

                    this.CurrentMotherShip.ResumeStaging();
                    //UnityEngine.Debug.Log("ImpulseDrive Transporter EVAToShip Button 1 ");

                    this.CurrentMotherShip.MakeActive();
                    //UnityEngine.Debug.Log("ImpulseDrive Transporter EVAToShip Button end " );
                    kerbalEVA = null;
                    return;
                }
            }
        }
        // //////////////////////////////////////////////////////////////////////////////////////////////////


        // //////////////////////////////////////////////////////////////////////////////////////////////////
        internal void transportKerbal_ShipToShip(Vessel sourceShip, Vessel targetShip, Part targetPart, ProtoCrewMember kerbalToMove)
        {
            //UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip Button begin " + kerbalToMove.name);
            ShipToShip_RemoveCrew(sourceShip, kerbalToMove);
            //UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip Button 1 " );

            ShipToShip_AddCrew(targetShip, targetPart, kerbalToMove);
            //UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip Button end ");
        }

        private void ShipToShip_RemoveCrew(Vessel sourceShip, ProtoCrewMember kerbalToMove)
        {
            //UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_RemoveCrew begin " + kerbalToMove.name);
            sourceShip.GoOffRails();

            foreach (Part part in sourceShip.Parts)
            {
                kerbal = null;
                foreach (ProtoCrewMember availableKerbal in part.protoModuleCrew)
                {
                    if (kerbalToMove.name == availableKerbal.name)
                    {
                        //UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_RemoveCrew 1 ");
                        kerbal = availableKerbal;
                        //UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_RemoveCrew 2 ");

                        part.RemoveCrewmember(kerbalToMove);
                        //UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_RemoveCrew 3 ");

                        kerbalToMove.seat = null;
                        //UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_RemoveCrew end ");
                        return;
                    }
                }
            }
        }

        private void ShipToShip_AddCrew(Vessel targetShip, Part targetPart, ProtoCrewMember kerbalToMove)
        {
            //UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_AddCrew begin " + kerbalToMove.name);
            targetShip.GoOffRails();
            if (!ShipToShip_PartIsFull(targetPart))
                {
                    targetPart.AddCrewmember(kerbalToMove);
                    targetShip.SpawnCrew();
                    targetShip.ResumeStaging();
                    targetShip.MakeActive();
            
                }
            /*
            foreach (Part part in targetShip.Parts)
            {
                if (!ShipToShip_PartIsFull(part))
                {
                    UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_AddCrew 1 " );

                    part.AddCrewmember(kerbalToMove);
                    UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_AddCrew 2 " );
                    
                    targetShip.SpawnCrew();
                    UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_AddCrew 3 ");
                    
                    targetShip.ResumeStaging();
                    UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_AddCrew 4 ");
                    
                    targetShip.MakeActive();
                    UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_AddCrew end ");
                    return;
                }
            }
            */
        }

        private bool ShipToShip_PartIsFull(Part part)
        {
            //UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_PartIsFull part=" + part);
            bool pF = !(part.protoModuleCrew.Count < part.CrewCapacity);
            //UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToShip_PartIsFull pF=" + pF);
            return pF;
        }
        /*
        internal static void FireVesselUpdated()
        {
            // Notify everything that we've made a change to the vessel, TextureReplacer uses this, per shaw:
            // http://forum.kerbalspaceprogram.com/threads/60936-0-23-0-Kerbal-Crew-Manifest-v0-5-6-2?p=1051394&viewfull=1#post1051394

            GameEvents.onVesselChange.Fire(FlightGlobals.ActiveVessel);
        }
        */
        // //////////////////////////////////////////////////////////////////////////////////////////////////






        // //////////////////////////////////////////////////////////////////////////////////////////////////
        private void transportKerbal_ShipToSpace(ProtoCrewMember kerbalToMove)
        {
            foreach (Part p in this.CurrentMotherShip.parts)
            {
                if (p.protoModuleCrew.Count == 0)
                {
                    UnityEngine.Debug.Log("nobody inside");
                    continue;
                }


                kerbal = null;
                foreach (ProtoCrewMember availableKerbal in p.protoModuleCrew)
                {
                    if (kerbalToMove.name == availableKerbal.name)
                    {
                        kerbal = availableKerbal;
                    }
                }
                if (kerbal == null) //Probably not necessary
                {
                    continue;
                }

                TransportMode = null;
                UnityEngine.Debug.Log(FlightEVA.fetch.spawnEVA(kerbal, p, p.airlock));

                for (int i = FlightGlobals.Vessels.Count - 1; i >= 0; i--)
                {
                    if (kerbal.name == FlightGlobals.Vessels[i].vesselName)
                    {
                        Vessel v = FlightGlobals.Vessels[i];
                        FlightGlobals.Vessels[i].rootPart.AddModule("StarTrekTricorder");
                        FlightGlobals.Vessels[i].transform.position = transporter_target.transform.position;
                        continue;
                    }
                }
            }

        }
        // //////////////////////////////////////////////////////////////////////////////////////////////////






        // //////////////////////////////////////////////////////////////////////////////////////////////////
        internal void transportKerbal_ShipToGround(ProtoCrewMember kerbalToMove)
        {

            if (kerbalToMove != null)
            {
                UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToGround Button begin " + kerbalToMove.name);
                //Kerbal kerbal = new Kerbal();
                Guid MotherShipGuid = this.CurrentMotherShip.id;

                foreach (Part p in this.CurrentMotherShip.parts)
                {
                    if (p.protoModuleCrew.Count == 0)
                    {
                        UnityEngine.Debug.Log("nobody inside");
                        continue;
                    }


                    kerbal = null;
                    foreach (ProtoCrewMember availableKerbal in p.protoModuleCrew)
                    {
                        if (kerbalToMove.name == availableKerbal.name)
                        {
                            kerbal = availableKerbal;
                        }
                    }
                    if (kerbal == null) //Probably not necessary
                    {
                        continue;
                    }
                    //this.CurrentMotherShip.save();
                    //this.CurrentMotherShip.GoOffRails();
                    Utilities.SetLoadDistance(fixed_height * 2, fixed_height * 2);

                    TransportMode = null;
                    UnityEngine.Debug.Log(FlightEVA.fetch.spawnEVA(kerbal, p, p.airlock));
                    
                    for (int i = FlightGlobals.Vessels.Count - 1; i >= 0; i--)
                    {
                        if (kerbal.name == FlightGlobals.Vessels[i].vesselName)
                        {
                            //UnityEngine.Debug.Log("adding force");
                            
                            Vessel v = FlightGlobals.Vessels[i];
                            //FlightGlobals.Vessels[i].distancePackThreshold = fixed_height * 2;

                            //StarTrekTricorder f = new StarTrekTricorder();

                            FlightGlobals.Vessels[i].rootPart.AddModule("StarTrekTricorder");
                            
                            //FlightGlobals.Vessels[i].rootPart.Rigidbody.AddForce(FlightGlobals.Vessels[i].rootPart.transform.up * ejectionForce);
                            FlightGlobals.Vessels[i].transform.position = transporter_target.transform.position;
                            continue; 
                        }
                    }
                    for (int i = FlightGlobals.Vessels.Count - 1; i >= 0; i--)
                    {
                        if (MotherShipGuid == FlightGlobals.Vessels[i].id)
                        {
                            FlightGlobals.Vessels[i].MakeActive();
                            return;
                        }
                    }
                }


                //Part.RemoveCrewmember(ProtoCrewMember)

                /*
                ProtoCrewMember protoCrewMember = new ProtoCrewMember();
                UnityEngine.Debug.Log("ImpulseDrive Transporter EVA Button 1 ");

                Kerbal kerbal = new Kerbal();
                UnityEngine.Debug.Log("ImpulseDrive Transporter EVA Button 2 ");
                                
                kerbal = protoCrewMember.Spawn();
                UnityEngine.Debug.Log("ImpulseDrive Transporter EVA Button 3 ");

                FlightEVA.SpawnEVA(kerbal);
                UnityEngine.Debug.Log("ImpulseDrive Transporter EVA Button 4 ");
                */

                /*
                KerbalEVA.GetEjectPoint(UnityEngine.Vector3, float, float, float)
                KerbalEVA.BoardPart(Part)
                KerbalEVA.BoardSeat(KerbalSeat)
                */
                UnityEngine.Debug.Log("ImpulseDrive Transporter ShipToGround Button end:");
            }
        }
        // //////////////////////////////////////////////////////////////////////////////////////////////////

    
    
    }
}
