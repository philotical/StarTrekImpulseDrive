using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace Philotical
{
    class TractorBeam
    {
        private List<Vessel> vesselList;
        private List<string> visibleVessels;
        private List<Vessel> ShipList = new List<Vessel>();
        private List<TBeam> BeamList = new List<TBeam>();
        private List<string> BeamIDList = new List<string>();
        System.Random rnd = new System.Random();

        GravityTools grav;


        [KSPField]
        public string TractorSoundFile = "SciFi/StarTrekImpulseDrive/sounds/tng_tractor_clean";
        [KSPField]
        public float TractorSoundVolume = 1.2f;
        [KSPField]
        public bool loopTractorSound = true;
        public FXGroup TractorSound = null;


        internal void Draw_ShipSelector()
        {
            grav = new GravityTools();
            vesselList = FlightGlobals.Vessels;
            visibleVessels = new List<string>() { };

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Ship");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Options");
            GUILayout.EndHorizontal();

            foreach (Vessel v in vesselList)
            {
                float check_distance = Vector3.Distance(FlightGlobals.ActiveVessel.findWorldCenterOfMass(), v.findWorldCenterOfMass());
                if (check_distance < 2000F && v.id != FlightGlobals.ActiveVessel.id && !this.BeamIDList.Contains(v.id.ToString()))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(""+v.vesselName);
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button("+"))
                    {
                        UnityEngine.Debug.Log("ImpulseDrive TractorBeam: Draw_ShipSelector CreateBeam was clicked : " + v.vesselName);
                        this.CreateBeam(v);
                    }
                    GUILayout.EndHorizontal();

                }
            }
            GUILayout.EndVertical();

        }
        //GimbalDebug foo1 = new GimbalDebug();
        GimbalDebug foo2 = new GimbalDebug();
        //GimbalDebug foo4 = new GimbalDebug();
        internal void CreateBeam(Vessel v)
        {


            UnityEngine.Debug.Log("ImpulseDrive TractorBeam: CreateBeam 1 ");
            Vessel Parent = FlightGlobals.ActiveVessel;
            
            //Vector3d parentPosition_current = Parent.findWorldCenterOfMass();
            //Vector3d childPosition_current = v.findWorldCenterOfMass();
            Vector3d parentPosition_current = Parent.GetWorldPos3D();
            Vector3d childPosition_current = v.GetWorldPos3D();

           
            
            float distance = Vector3.Distance(parentPosition_current, childPosition_current);
            Vector3 newVector = childPosition_current - parentPosition_current;
            //UnityEngine.Debug.Log("ImpulseDrive TractorBeam: CreateBeam 2 ");

            TBeam nb = new TBeam();
            UnityEngine.Debug.Log("ImpulseDrive TractorBeam: CreateBeam 3-7 ");

            BeamDrawer BD1 = new BeamDrawer();
            LineRenderer LR1 = new LineRenderer();
            BeamDrawer BD2 = new BeamDrawer();
            LineRenderer LR2 = new LineRenderer();
            BeamDrawer debug_BD1 = new BeamDrawer();
            LineRenderer debug_LR1 = new LineRenderer();
            BeamDrawer debug_BD2 = new BeamDrawer();
            LineRenderer debug_LR2 = new LineRenderer();

            nb.beamID = v.id.ToString();
            nb.ship = v;
            nb.ShipPosition = childPosition_current;
            nb.ShipRotation = nb.ship.transform.rotation;
            nb.ParentPosition = parentPosition_current;
            nb.ParentRotation = Parent.transform.rotation;

            
            nb.force = 0.5F;
            nb.distance = distance;

            nb.Empty1 = new GameObject("Empty1");
            nb.Empty2 = new GameObject("Empty2");
            /*
            */
            nb.ship.GoOffRails();

            //foo1.drawGimbal(nb.Empty1, 10, 1f);
            foo2.drawGimbal(nb.Empty2, 12, 0.5f);
            //foo4.drawGimbal(childPosition_current, 20, 0.5f);

            //nb.Empty2.transform.parent = nb.Empty1.transform;
            //nb.Empty2.transform.rotation = FlightGlobals.ActiveVessel.transform.rotation;
            nb.Vector = newVector;

            nb.y_slider = nb.yAngle;
            nb.z_slider = nb.zAngle;
            nb.x_slider = nb.xAngle;

            nb.BeamDrawer1 = BD1;
            nb.LineRenderer1 = LR1;
            nb.BeamDrawer2 = BD2;
            nb.LineRenderer2 = LR2;
            nb.debug1_BeamDrawer = debug_BD1;
            nb.debug1_LineRenderer = debug_LR1;
            nb.debug2_BeamDrawer = debug_BD2;
            nb.debug2_LineRenderer = debug_LR2;

            nb.hold_distance = true;
            nb.hold_Vector = true;

            UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: CreateTarget TractorSound ");
            nb.TractorSound = new FXGroup("TractorSound");
            GameObject audioObj = new GameObject();
            nb.audioObj = audioObj;
            nb.audioObj.transform.position = FlightGlobals.ActiveVessel.transform.position;
            nb.audioObj.transform.parent = FlightGlobals.ActiveVessel.transform;	// add to parent
            nb.TractorSound.audio = audioObj.AddComponent<AudioSource>();
            nb.TractorSound.audio.dopplerLevel = 0f;
            nb.TractorSound.audio.Stop();
            nb.TractorSound.audio.clip = GameDatabase.Instance.GetAudioClip(TractorSoundFile);
            nb.TractorSound.audio.loop = true;
            // Seek to a random position in the sound file so we don't have
            // harmonic effects when burning at multiple RCS nozzles.
            nb.TractorSound.audio.time = UnityEngine.Random.Range(0, nb.TractorSound.audio.clip.length);
            nb.TractorSound.audio.Play();
            nb.TractorSound.audio.enabled = false;

            try
            {
                if (nb.TractorSound != null && nb.TractorSound.audio != null)
                {
                    nb.soundVolume = GameSettings.SHIP_VOLUME * TractorSoundVolume;
                    nb.TractorSound.audio.enabled = true;
                    nb.TractorSound.audio.volume = nb.soundVolume;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("TractorSound Error OnUpdate: " + ex.Message);
            }
            
            
            this.BeamList.Add(nb);
            this.BeamIDList.Add(nb.beamID);

            nb = null;
            BD1 = null;
            LR1 = null;
            BD2 = null;
            LR2 = null;
            debug_BD1 = null;
            debug_LR1 = null;
            debug_BD2 = null;
            debug_LR2 = null;
            childPosition_current = Vector3.zero;

        }
        public float PosNegAngle(Vector3 a1, Vector3 a2, Vector3 normal)
        {
            float angle = Vector3.Angle(a1, a2);
            float sign = Mathf.Sign(Vector3.Dot(normal, Vector3.Cross(a1, a2)));
            return angle * sign;
        }


        private float Empty1Angle = Mathf.Infinity, Empty2Angle = Mathf.Infinity;
        public void Update_RenderBeams()
        {

            foreach (TBeam b in this.BeamList)
            {
            Vessel Parent = FlightGlobals.ActiveVessel;
                Vector3 parentPosition_current = Parent.GetWorldPos3D();
                Vector3 childPosition_current = b.ship.GetWorldPos3D();
                //Vector3 parentPosition_current = Parent.findWorldCenterOfMass();
                //Vector3 childPosition_current = b.ship.findWorldCenterOfMass();
                //Vector3 parentPosition_current = Parent.findLocalCenterOfMass();
                //Vector3 childPosition_current = b.ship.findLocalCenterOfMass();
                //Vector3 parentPosition_current = Parent.transform.position;
                //Vector3 childPosition_current = b.ship.transform.position;
                //Vector3 parentPosition_current = Parent.transform.localPosition;
                //Vector3 childPosition_current = b.ship.transform.localPosition;

                BeamDrawer BD1 = b.BeamDrawer1;
                LineRenderer LR1 = b.LineRenderer1;
                BeamDrawer BD2 = b.BeamDrawer2;
                LineRenderer LR2 = b.LineRenderer2;
                BeamDrawer debug_BD1 = b.debug1_BeamDrawer;
                LineRenderer debug_LR1 = b.debug1_LineRenderer;
                BeamDrawer debug_BD2 = b.debug2_BeamDrawer;
                LineRenderer debug_LR2 = b.debug2_LineRenderer;

                GameObject Empty1 = b.Empty1;
                GameObject Empty2 = b.Empty2;



                int width = 0;
                float check_distance = Vector3.Distance(parentPosition_current, childPosition_current);
                if (check_distance < 2000F)
                {

                    b.ship.GoOffRails();

                    //* works!!!!
                    // MOVE THE HELPER LINE - NO FORCE APPLIED HERE
                    Empty1.transform.parent = Parent.transform;
                    Empty1.transform.position = Parent.transform.position;
                    Empty2.transform.parent = Empty1.transform;
                    Empty2.transform.position = Empty1.transform.position;
                    Empty2.transform.rotation = Empty1.transform.rotation;
                    Vector3 temp2 = Empty2.transform.TransformDirection(b.Vector);
                    Vector3 GimbalPosition = SetVectorLength(temp2, b.distance);
                    //Empty2.transform.position = GimbalPosition;
                    Empty2.transform.localPosition = GimbalPosition;
                    Empty1.transform.Rotate(b.x_slider, 0, b.z_slider);
                    b.xAngle = Empty1.transform.localEulerAngles.x;
                    b.zAngle = Empty1.transform.localEulerAngles.z;
                    b.x_slider = 0;
                    b.y_slider = 0;
                    b.z_slider = 0;
                    // MOVE THE HELPER LINE - NO FORCE APPLIED HERE
                    // OBSOLETE - RENDER THE HELPER LINE - NO FORCE APPLIED HERE
                    b.distance = Mathf.Clamp(b.distance,5, 2000);
                    b.debug1_LineRenderer = debug_BD1.render2(Empty1.transform, Empty1.transform.up, Empty1.transform.localPosition, b.distance, 0.1f, Color.green, Color.red);
                    b.debug1_LineRenderer.enabled = false;
                    // OBSOLETE - RENDER THE HELPER LINE - NO FORCE APPLIED HERE
                    // works !!!! */
                    
                    
                    // FORCE APPLIED HERE
                    //Vector3 newVector = SetVectorLength(Empty1.transform.TransformPoint(Empty1.transform.up), b.distance);
                    //float current_Distance = Vector3.Distance(parentPosition_current, childPosition_current);
                    //float step = current_Distance * Time.deltaTime;

                    //b.ship.transform.position = Vector3.Lerp(childPosition_current, Empty2.transform.position, 0.5F * Time.deltaTime);
                    
                    //b.ship.transform.position = Vector3.Lerp(b.ship.transform.position, Empty2.transform.position, 0.5F * Time.deltaTime);
                    
                    float targetError = Vector3.Distance(b.ship.transform.position, Empty2.transform.position);
                    b.targetError = targetError;
                    float step = targetError * Time.deltaTime;
                    b.ship.transform.position = Vector3.MoveTowards(b.ship.transform.position, Empty2.transform.position, step);
                    
                    
                    

                    Vector3 targetErrorVector = b.ship.transform.position - Empty2.transform.position;

                    foreach (var vesselPart in b.ship.parts.Where(p => p.rigidbody != null))
                    {
                        vesselPart.rigidbody.AddForce(targetErrorVector * -1 * (b.force / targetError), ForceMode.Acceleration);
                    }
                    // FORCE APPLIED HERE
                    /*
                    */













                    // RENDER THE TRACTOR BEAM - NO FORCE APPLIED HERE
                    Vector3 TractorBeamVector = b.Vector;
                    System.Random rnd = new System.Random();
                    width = rnd.Next(5, 12);
                    LR1 = BD1.render(TractorBeamVector, Color.blue, 1);
                    LR1.SetWidth(1, width);
                    LR1.SetPosition(0, FlightGlobals.ActiveVessel.findWorldCenterOfMass());
                    LR1.SetPosition(1, b.ship.GetWorldPos3D());

                    Color fc = new Color(0.469F, 0.484F, 0.371F, 0.8F);
                    width = rnd.Next(1, 5);
                    LR2 = BD2.render(TractorBeamVector, fc, 1);
                    LR2.SetWidth(1, width);
                    LR2.SetPosition(0, FlightGlobals.ActiveVessel.findWorldCenterOfMass());
                    LR2.SetPosition(1, b.ship.GetWorldPos3D());
                    // RENDER THE TRACTOR BEAM - NO FORCE APPLIED HERE







                }
                else
                {
                    this.RemoveBeam(b);
                }
            }
        }
        private LineRenderer DebugLine(GameObject o ,LineRenderer line, Transform parentTransform, Vector3 transformDirection, Vector3 origin, float length, float width, Color color)
        {
            //GameObject o = new GameObject("Test");
            o.transform.parent = parentTransform;
            o.transform.localEulerAngles = Vector3.zero;
            //LineRenderer line = o.AddComponent<LineRenderer>();
            line.transform.parent = parentTransform;
            line.useWorldSpace = false;
            line.transform.localPosition = origin;
            line.transform.localEulerAngles = Vector3.zero;
            line.material = new Material(Shader.Find("Particles/Additive"));
            line.SetWidth(width, width);
            line.SetVertexCount(2);
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, parentTransform.InverseTransformDirection(transformDirection.normalized) * length);
            line.SetColors(color, color);
            return line;
        }


        //Returns the forward vector of a quaternion
        public static Vector3 GetForwardVector(Quaternion q)
        {

            return q * Vector3.forward;
        }

        //Returns the up vector of a quaternion
        public static Vector3 GetUpVector(Quaternion q)
        {

            return q * Vector3.up;
        }

        //Returns the right vector of a quaternion
        public static Vector3 GetRightVector(Quaternion q)
        {

            return q * Vector3.right;
        }
       
        //create a vector of direction "vector" with length "size"
        public static Vector3 SetVectorLength(Vector3 vector, float size)
        {

            //normalize the vector
            Vector3 vectorNormalized = Vector3.Normalize(vector);

            //scale the vector
            return vectorNormalized *= size;
        }
        private static Transform tempChild = null;
        private static Transform tempParent = null;

        public static void Init()
        {

            tempChild = (new GameObject("Math3d_TempChild")).transform;
            tempParent = (new GameObject("Math3d_TempParent")).transform;

            tempChild.gameObject.hideFlags = HideFlags.HideAndDontSave;
            //DontDestroyOnLoad(tempChild.gameObject);

            tempParent.gameObject.hideFlags = HideFlags.HideAndDontSave;
            //DontDestroyOnLoad(tempParent.gameObject);

            //set the parent
            tempChild.parent = tempParent;
        }
        //This function transforms one object as if it was parented to the other.
        //Before using this function, the Init() function must be called
        //Input: parentRotation and parentPosition: the current parent transform.
        //Input: startParentRotation and startParentPosition: the transform of the parent object at the time the objects are parented.
        //Input: startChildRotation and startChildPosition: the transform of the child object at the time the objects are parented.
        //Output: childRotation and childPosition.
        //All transforms are in world space.
        public static void TransformWithParent(out Quaternion childRotation, out Vector3 childPosition, Quaternion parentRotation, Vector3 parentPosition, Quaternion startParentRotation, Vector3 startParentPosition, Quaternion startChildRotation, Vector3 startChildPosition)
        {
            if (tempChild==null)
            {
                Init();
            }
            childRotation = Quaternion.identity;
            childPosition = Vector3.zero;

            //set the parent start transform
            tempParent.rotation = startParentRotation;
            tempParent.position = startParentPosition;
            tempParent.localScale = Vector3.one; //to prevent scale wandering

            //set the child start transform
            tempChild.rotation = startChildRotation;
            tempChild.position = startChildPosition;
            tempChild.localScale = Vector3.one; //to prevent scale wandering

            //translate and rotate the child by moving the parent
            tempParent.rotation = parentRotation;
            tempParent.position = parentPosition;

            //get the child transform
            childRotation = tempChild.rotation;
            childPosition = tempChild.position;
        }
        //This function transforms one object as if it was parented to the other.
        //Before using this function, the Init() function must be called
        //Input: parentRotation and parentPosition: the current parent transform.
        //Input: startParentRotation and startParentPosition: the transform of the parent object at the time the objects are parented.
        //Input: startChildRotation and startChildPosition: the transform of the child object at the time the objects are parented.
        //Output: childRotation and childPosition.
        //All transforms are in world space.
        public static Vector3 PositionWithParent(Vector3 parentPosition, Vector3 startParentPosition, Vector3 startChildPosition)
        {
            tempChild = (new GameObject("Math3d_TempChild")).transform;
            tempParent = (new GameObject("Math3d_TempParent")).transform;

            //set the parent
            tempChild.parent = tempParent;

            Vector3 childPosition = Vector3.zero;

            //set the parent start transform
            tempParent.position = startParentPosition;
            tempParent.localScale = Vector3.one; //to prevent scale wandering

            //set the child start transform
            tempChild.position = startChildPosition;
            tempChild.localScale = Vector3.one; //to prevent scale wandering

            //translate and rotate the child by moving the parent
            tempParent.position = parentPosition;

            //get the child transform
            childPosition = tempChild.position;
            return childPosition;
        }

        private void RemoveBeam(TBeam b)
        {
            b.TractorSound.audio.Stop();
            b.TractorSound.audio.enabled = false;

            //foo1.removeGimbal();
            foo2.removeGimbal();
            //foo4.removeGimbal();

            UnityEngine.Debug.Log("ImpulseDrive TractorBeam: RemoveBeam 1 : " + b.ship.vesselName);
            BeamDrawer BD1 = b.BeamDrawer1;
            LineRenderer LR1 = b.LineRenderer1;
            BeamDrawer BD2 = b.BeamDrawer2;
            LineRenderer LR2 = b.LineRenderer2;
            BeamDrawer debug_BD1 = b.debug1_BeamDrawer;
            LineRenderer debug_LR1 = b.debug1_LineRenderer;
            BeamDrawer debug_BD2 = b.debug2_BeamDrawer;
            LineRenderer debug_LR2 = b.debug2_LineRenderer;
            UnityEngine.Debug.Log("ImpulseDrive TractorBeam: RemoveBeam 2 : " + b.ship.vesselName);

            UnityEngine.Debug.Log("ImpulseDrive TractorBeam: RemoveBeam 3 : " + b.ship.vesselName);
            //b.debug1_LineRenderer.enabled = false;
            UnityEngine.Debug.Log("ImpulseDrive TractorBeam: RemoveBeam 4 : " + b.ship.vesselName);
            


            LR1 = BD1.render(Vector3.zero, Color.blue, 1);
            LR1.SetWidth(0, 0);
            LR1.SetPosition(0, Vector3.zero);
            LR1.SetPosition(1, Vector3.zero);
            //BD1.Destroy();
            UnityEngine.Debug.Log("ImpulseDrive TractorBeam: RemoveBeam 5 : " + b.ship.vesselName);

            Color fc = new Color(0.469F, 0.484F, 0.371F, 0.8F);
            LR2 = BD2.render(Vector3.zero, fc,1);
            LR2.SetWidth(0, 0);
            LR2.SetPosition(0, Vector3.zero);
            LR2.SetPosition(1, Vector3.zero);
            //BD2.Destroy();
            UnityEngine.Debug.Log("ImpulseDrive TractorBeam: RemoveBeam 6 : " + b.ship.vesselName);

            /*
            debug_LR1 = debug_BD1.render(Vector3.zero, Color.green);
            debug_LR1.SetWidth(0, 0);
            debug_LR1.SetPosition(0, Vector3.zero);
            debug_LR1.SetPosition(1, Vector3.zero);
            //debug_BD1.Destroy();
            UnityEngine.Debug.Log("ImpulseDrive TractorBeam: RemoveBeam 7 : " + b.ship.vesselName);

            debug_LR2 = debug_BD2.render(Vector3.zero, Color.yellow);
            debug_LR2.SetWidth(0, 0);
            debug_LR2.SetPosition(0, Vector3.zero);
            debug_LR2.SetPosition(1, Vector3.zero);
            //debug_BD2.Destroy();
            UnityEngine.Debug.Log("ImpulseDrive TractorBeam: RemoveBeam 8 : " + b.ship.vesselName);
            */
            this.BeamIDList.Remove(b.beamID);
            this.BeamList.Remove(b);
            //BD = null;
            //LR = null;
            //b = null;
            UnityEngine.Debug.Log("ImpulseDrive TractorBeam: RemoveBeam 9 : " + b.ship.vesselName);
        }

        internal void Draw_BeamList()
        {

            GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Beam");
                    GUILayout.Label("Ship");
                GUILayout.FlexibleSpace();
                    GUILayout.Label("Options");
                GUILayout.EndHorizontal();
                int b_count = 1;
                foreach (TBeam b in this.BeamList)
                {
                    try // in case the target ship explodes..
                    {
                    //UnityEngine.Debug.Log("ImpulseDrive: rebuild_vessel_list v in vesselList : " + v.vesselName);
                        GUILayout.BeginHorizontal();
                            GUILayout.BeginVertical();
                                GUILayout.BeginHorizontal();
                                    GUILayout.Label("" + b_count);
                                    GUILayout.Label("" + b.ship.vesselName);
                                    GUILayout.FlexibleSpace();
                                    if (GUILayout.Button("-"))
                                    {
                                        this.RemoveBeam(b);
                                        UnityEngine.Debug.Log("ImpulseDrive TractorBeam: Draw_ShipSelector RemoveBeam was clicked : " + b.ship.vesselName);
                                    }
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                    GUILayout.Label("Force: " + Math.Round(b.force, 2) + " Err:" + Math.Round(b.targetError, 2));
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                    b.force = GUILayout.HorizontalSlider(b.force, 0.5F, 300.0F);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Distance: " + Math.Round(b.distance, 2) + "/" + Math.Round(Vector3.Distance(FlightGlobals.ActiveVessel.findWorldCenterOfMass(), b.ship.findWorldCenterOfMass()), 2));
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                //b.distance = GUILayout.HorizontalSlider(b.distance, 10.0F, 2000.0F);
                                if (GUILayout.RepeatButton("-100"))
                                {
                                    b.distance -= 100F;
                                }
                                if (GUILayout.RepeatButton("-10"))
                                {
                                    b.distance -= 10F;
                                }
                                if (GUILayout.RepeatButton("-1"))
                                {
                                    b.distance -= 1F;
                                }
                                if (GUILayout.RepeatButton("+1"))
                                {
                                    b.distance += 1F;
                                }
                                if (GUILayout.RepeatButton("+10"))
                                {
                                    b.distance += 10F;
                                }
                                if (GUILayout.RepeatButton("+100"))
                                {
                                    b.distance += 100F;
                                }

                                GUILayout.EndHorizontal();
                                
                        /*
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Auto Lock Positions:");
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                if (GUILayout.Button("A")) //ahead
                                {
                                    b.x_slider = 360F;
                                    b.z_slider = 360F;
                                }
                                if (GUILayout.Button("S")) // stern
                                {
                                    b.x_slider = 360F;
                                    b.z_slider = 180F; // 338.81F;
                                }
                                if (GUILayout.Button("LSB")) // Lateral Starboard
                                {
                                    b.x_slider = 360F;
                                    b.z_slider = 90F;
                                }
                                if (GUILayout.Button("LP")) // Lateral Port
                                {
                                    b.x_slider = 360F;
                                    b.z_slider = 270F;
                                }
                                if (GUILayout.Button("AB")) // above
                                {
                                    b.x_slider = 90F;
                                    b.z_slider = 360F;
                                }
                                if (GUILayout.Button("BL")) // Below
                                {
                                    b.x_slider = 360F;
                                    b.z_slider = 90F;
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("x: " + Math.Round(b.xAngle, 2));
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                b.x_slider = GUILayout.HorizontalSlider(b.x_slider, -1.0F, 1.0F);
                                GUILayout.EndHorizontal();
                        */

                                /*
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("y: " + Math.Round(b.yAngle, 2));
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                b.y_slider = GUILayout.HorizontalSlider(b.y_slider, -1.0F, 1.0F);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("z: " + Math.Round(b.zAngle, 2));
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                b.z_slider = GUILayout.HorizontalSlider(b.z_slider, -180.0F, 180.0F);
                                GUILayout.EndHorizontal();
                                */

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("x (blue): " + Math.Round(b.xAngle, 2));
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                if (GUILayout.RepeatButton("-10"))
                                {
                                    b.x_slider -= 10F;
                                }
                                if (GUILayout.RepeatButton("-1"))
                                {
                                    b.x_slider -= 1F;
                                }
                                if (GUILayout.RepeatButton("-0.1"))
                                {
                                    b.x_slider -= 0.1F;
                                }
                                if (GUILayout.RepeatButton("+0.1"))
                                {
                                    b.x_slider += 0.1F;
                                }
                                if (GUILayout.RepeatButton("+1"))
                                {
                                    b.x_slider += 1F;
                                }
                                if (GUILayout.RepeatButton("+10"))
                                {
                                    b.x_slider += 10F;
                                }
                                GUILayout.EndHorizontal();

                                /*
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("y: " + Math.Round(b.yAngle, 2));
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                if (GUILayout.RepeatButton("-10"))
                                {
                                    b.y_slider -= 10F;
                                }
                                if (GUILayout.RepeatButton("-1"))
                                {
                                    b.y_slider -= 1F;
                                }
                                if (GUILayout.RepeatButton("-0.1"))
                                {
                                    b.y_slider -= 0.1F;
                                }
                                if (GUILayout.RepeatButton("+0.1"))
                                {
                                    b.y_slider += 0.1F;
                                }
                                if (GUILayout.RepeatButton("+1"))
                                {
                                    b.y_slider += 1F;
                                }
                                if (GUILayout.RepeatButton("+10"))
                                {
                                    b.y_slider += 10F;
                                }
                                GUILayout.EndHorizontal();
                                */


                                GUILayout.BeginHorizontal();
                                GUILayout.Label("z (red): " + Math.Round(b.zAngle, 2));
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                if (GUILayout.RepeatButton("-10"))
                                {
                                    b.z_slider -= 10F;
                                }
                                if (GUILayout.RepeatButton("-1"))
                                {
                                    b.z_slider -= 1F;
                                }
                                if (GUILayout.RepeatButton("-0.1"))
                                {
                                    b.z_slider -= 0.1F;
                                }
                                if (GUILayout.RepeatButton("+0.1"))
                                {
                                    b.z_slider += 0.1F;
                                }
                                if (GUILayout.RepeatButton("+1"))
                                {
                                    b.z_slider += 1F;
                                }
                                if (GUILayout.RepeatButton("+10"))
                                {
                                    b.z_slider += 10F;
                                }
                                GUILayout.EndHorizontal();



                            GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                        b_count++;
                    }
                    catch // in case the target ship explodes..
                    {
                        this.RemoveBeam(b);

                    }

                    GUILayout.Label("---------------");

                }
            GUILayout.EndVertical();
        }
    }

    public class BeamDrawer
    {
        public GameObject lineObj = null;
        public LineRenderer lr;
        public LineRenderer line;
        public BeamDrawer()
        {
        }
        public LineRenderer render(Vector3 transform_up, Color c, int Width)
        {
            if (this.lineObj == null)
            {
                this.lineObj = new GameObject();
            }
            if (this.lr == null)
            {
                this.lineObj.transform.up = transform_up;
                this.lr = lineObj.AddComponent<LineRenderer>();
                this.lr.material = new Material(Shader.Find("Particles/Additive"));

                this.lr.SetWidth(1, Width);
                this.lr.SetColors(c, c);

                this.lr.SetVertexCount(2);
                this.lr.useWorldSpace = true;
            }
            return lr;
        }
        public void SetPosition(int index, Vector3 pos)
        {
            this.lr.SetPosition(index, pos);
        }
        public void Destroy()
        {
            this.lr = null;
            this.lineObj = null;
        }


        public LineRenderer render2(Transform parentTransform, Vector3 transformDirection, Vector3 origin, float length, float width, Color color1, Color color2)
        {
            GameObject o = new GameObject("Test");
            o.transform.parent = parentTransform;
            o.transform.localEulerAngles = Vector3.zero;
            if (this.line == null)
            {
                line = o.AddComponent<LineRenderer>();
                line.transform.parent = parentTransform;
                line.useWorldSpace = false;
                line.transform.localPosition = origin;
                line.transform.localEulerAngles = Vector3.zero;
                line.material = new Material(Shader.Find("Particles/Additive"));
                line.SetWidth(width, width);
                line.SetVertexCount(2);
            }

            line.SetPosition(0, Vector3.zero);
            UnityEngine.Debug.Log("ImpulseDrive TractorBeam: render2 : 12 length=" + length);

            //Vector3 temp1 = parentTransform.TransformPoint(transformDirection * length);
            Vector3 temp1 = parentTransform.InverseTransformDirection(transformDirection);
            Vector3 temp2 = SetVectorLength(temp1, length);
            //line.SetPosition(1, Vector3.zero);
            //line.SetPosition(1, parentTransform.InverseTransformDirection(transformDirection).normalized * length);
            line.SetPosition(1, temp2);
            line.SetColors(color1, color2);
            return line;
        }
        public static Vector3 SetVectorLength(Vector3 vector, float size)
        {

            //normalize the vector
            Vector3 vectorNormalized = Vector3.Normalize(vector);

            //scale the vector
            return vectorNormalized *= size;
        }

    }

    class TBeam
    {

        public string beamID { get; set; }
        public Vessel ship { get; set; }

        public float force { get; set; }
        public float distance { get; set; }
        public Vector3 Vector { get; set; }

        public bool hold_distance { get; set; }
        public bool hold_Vector { get; set; }


        public Vector3d ShipPosition { get; set; }

        public Vector3d ParentPosition { get; set; }

        public Quaternion ShipRotation { get; set; }

        public Quaternion ParentRotation { get; set; }


        public float xAngle { get; set; }

        public float zAngle { get; set; }

        public float yAngle { get; set; }

        public BeamDrawer BeamDrawer1 { get; set; }
        public LineRenderer LineRenderer1 { get; set; }

        public BeamDrawer BeamDrawer2 { get; set; }
        public LineRenderer LineRenderer2 { get; set; }

        public BeamDrawer debug1_BeamDrawer { get; set; }

        public LineRenderer debug1_LineRenderer { get; set; }

        public BeamDrawer debug2_BeamDrawer { get; set; }

        public LineRenderer debug2_LineRenderer { get; set; }

        public GameObject Empty1 { get; set; }

        public GameObject Empty2 { get; set; }

        public float y_slider { get; set; }

        public float z_slider { get; set; }

        public float x_slider { get; set; }


        public GameObject audioObj { get; set; }

        public FXGroup TractorSound { get; set; }

        public float soundVolume { get; set; }

        public float soundPitch { get; set; }

        public float targetError { get; set; }
    }

}
