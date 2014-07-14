using System.Collections.Generic;
using UnityEngine;

namespace Philotical
{
    class LCARS_StructuralIntegrityField : PartModule
    {

        public LCARS_StructuralIntegrityField()
        {
        }

        Dictionary<string, float> backup_values = null;
        Dictionary<Part, Dictionary<string, float>> backup_Parts = null;
        Vessel ShipSelected = null;
        LCARS_PowerSystem PowSys;
        PowerTaker PT = null;

        internal void SetShip(Vessel v, LCARS_PowerSystem thisPowSys)
        {
            this.ShipSelected = v;
            this.PowSys = thisPowSys;
            PT = this.PowSys.setPowerTaker("StructuralIntagrityField", "SubSystem", 1250, 10000f, 0);
        }

        public void reset_StructuralIntegrityField()
        {
            if (backup_Parts != null)
            {
                foreach (Part p in this.ShipSelected.Parts)
                {
                    p.crashTolerance = backup_Parts[p]["crashTolerance"];
                    p.breakingForce = backup_Parts[p]["breakingForce"];
                    p.breakingTorque = backup_Parts[p]["breakingTorque"];
                    p.maxTemp = backup_Parts[p]["maxTemp"];
                }
            }
            backup_Parts = null;
        }

        public void set_StructuralIntegrityField(float force)
        {
            UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: set_StructuralIntegrityField  beginn");
            if (backup_Parts == null)
            {
                backup_Parts = new Dictionary<Part, Dictionary<string, float>>() { };
            }

            foreach (Part p in this.ShipSelected.Parts)
            {
                if(!backup_Parts.ContainsKey(p))
                {
                    backup_values = new Dictionary<string,float>(){};
                    backup_values.Add("crashTolerance", p.crashTolerance);
                    backup_values.Add("breakingForce",p.breakingForce);
                    backup_values.Add("breakingTorque",p.breakingTorque);
                    backup_values.Add("maxTemp", p.maxTemp);
                    backup_values.Add("temperature", p.temperature);
                    backup_Parts.Add(p, backup_values);
                    backup_values = null;
                }

                p.crashTolerance = (force >= 1) ? backup_Parts[p]["crashTolerance"] * force : backup_Parts[p]["crashTolerance"];

                p.breakingForce = (force >= 1) ? backup_Parts[p]["breakingForce"] * force : backup_Parts[p]["breakingForce"];

                p.breakingTorque = (force >= 1) ? backup_Parts[p]["breakingTorque"] * force : backup_Parts[p]["breakingTorque"];

                p.maxTemp = (force>=1) ? backup_Parts[p]["maxTemp"] * force : backup_Parts[p]["maxTemp"];

                p.temperature = (p.temperature>20f) ? p.temperature - (0.005f * force) : p.temperature;

                //FromGO(UnityEngine.GameObject obj)

                foreach (AttachNode an in p.attachNodes)
                {
                    //an.attachedPart
                        //an.attachMethod
                    //an.breakingForce
                        //an.breakingTorque
                    //an.nodeType
                    //an.position


                }
                //p.attachNodes
                //AttachNode.attachedPart

                Part parentPart1 = p.localRoot;
                if (parentPart1 != null && p != parentPart1 && p.attachJoint == null)
                {
                    UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: set_StructuralIntegrityField 1 parentPart1=" + parentPart1.name);
                    AttachNode nodeToParent = p.findAttachNodeByPart(parentPart1);
                    AttachNode nodeFromParent = parentPart1.findAttachNodeByPart(p);
                    AttachModes m = (AttachModes)AttachNodeMethod.FIXED_JOINT;
                    p.attachJoint = PartJoint.Create(p, parentPart1, nodeToParent, nodeFromParent, m);
                    p.attachJoint.SetBreakingForces(p.breakingForce * force * 20000, p.breakingTorque * force * 20000);

                    UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: set_StructuralIntegrityField 3 parentPart1=" + parentPart1.name);
                    addForceToAttachNodes(parentPart1, p, nodeToParent, nodeFromParent, force);
                    UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: set_StructuralIntegrityField 3 parentPart1=" + parentPart1.name);
                }
                Part parentPart2 = p.parent;
                if (parentPart2 != null && p != parentPart2 && p.attachJoint == null)
                {
                    UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: set_StructuralIntegrityField 1 parentPart2=" + parentPart2.name);
                    AttachNode nodeToParent = p.findAttachNodeByPart(parentPart2);
                    AttachNode nodeFromParent = parentPart2.findAttachNodeByPart(p);
                    AttachModes m = (AttachModes)AttachNodeMethod.LOCKED_JOINT;
                    p.attachJoint = PartJoint.Create(p, parentPart2, nodeToParent, nodeFromParent, m);
                    p.attachJoint.SetBreakingForces(p.breakingForce * force * 20000, p.breakingTorque * force * 20000);
                    UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: set_StructuralIntegrityField 2 parentPart2=" + parentPart2.name);
                    
                    //addForceToAttachNodes(parentPart2, p, nodeToParent, nodeFromParent, force);
                    UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: set_StructuralIntegrityField 3 parentPart2=" + parentPart2.name);
                }
                /*
                */
                

                if(p.children.Count>0)
                {
                    //addForceToPartChildConnections(p,force);
                }


                
                /*
                if (p.parent != null)
                {
                }
                */


                float power = PT.L1_usage + (force / 10 * PT.L2_usage / 10);
                this.PowSys.draw(PT.takerName, power);
            
            }


        }

        private void addForceToAttachNodes(Part parent ,Part part,AttachNode node,AttachNode nodeParent, float force)
        {

            UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  beginn node.id="+node.id);
            UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  node.size="+node.size);
            UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  nodeParent.size="+nodeParent.size);
            UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  node.attachMethod="+node.attachMethod);
            UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  node.breakingTorque="+node.breakingTorque);
            UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  node.breakingForce="+node.breakingForce);
            UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  node.nodeType="+node.nodeType);
            node.size = 5;
            nodeParent.size = 5;
            Vector3 ForceVector = Vector3.zero;
                ForceVector = node.position - nodeParent.position;
                if (parent.rigidbody != null)
                {
                    parent.rigidbody.AddForce(ForceVector * force, ForceMode.Force);
                }

                UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  node.size=" + node.size);
                UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  nodeParent.size=" + nodeParent.size);
                UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  node.attachMethod=" + node.attachMethod);
                UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  node.breakingTorque=" + node.breakingTorque);
                UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  node.breakingForce=" + node.breakingForce);
                UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  node.nodeType=" + node.nodeType);
                UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToAttachNodes  end node.id=" + node.id);
        }

        private void addForceToPartChildConnections(Part part, float force)
        {
            UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToPartChildConnections begin ");
            Vector3 ForceVector = Vector3.zero;

            Vector3 parent_position = part.transform.position;
            UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToPartChildConnections 1 ");

            foreach (Part p in part.children)
            {
                UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToPartChildConnections 2 ");

                //p.transform.parent = part.transform;
                UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToPartChildConnections 3 ");

                //vesselPart.force_activate();
                ForceVector = parent_position - p.transform.position;
                UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToPartChildConnections 4 ");
                if (p.rigidbody!=null)
                {
                    p.rigidbody.AddForce(ForceVector * force, ForceMode.Force);
                }
                UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToPartChildConnections 5 ");
            }
            UnityEngine.Debug.Log("LCARS_StructuralIntegrityField: addForceToPartChildConnections end ");
        }
        /*
        */
    }
}
