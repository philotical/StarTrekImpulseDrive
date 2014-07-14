using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{

    class _LCARS_PhotonTorpedo : PartModule
    {
        GameObject TorpedoClone = null;
        GimbalDebug foo = null;
        System.Random rnd = new System.Random();
        public void PhotonTorpedo()
        {

        }

        
        
        public GameObject get_PhotonTorpedo(Bounds ShipBounds, Vessel parent,Vector3 position)
        {
            TorpedoClone = null;
            TorpedoClone = (GameObject)GameDatabase.Instance.GetModel("SciFi/StarTrekImpulseDrive/Parts/PhotonTorpedo/PhotonTorpedo");
            if (foo == null)
            {
                //foo = new GimbalDebug();
            }
            //foo.drawGimbal(TorpedoClone, 5, 0.2f);

            TorpedoClone.name = "Torpedo_" + TorpedoClone.GetInstanceID(); //  (TID1 + "_" + TID2 + "_" + TID3).ToString();
            TorpedoClone.AddComponent("Rigidbody");
            TorpedoClone.AddComponent("BoxCollider");
            TorpedoClone.AddComponent("physicalObject");

            TorpedoClone.transform.localScale -= new Vector3(0.3F, 0.3F, 0.3F);

            TorpedoClone.rigidbody.isKinematic = false;
            TorpedoClone.rigidbody.detectCollisions = true;
            TorpedoClone.rigidbody.collider.isTrigger = false;
            TorpedoClone.rigidbody.mass = 0.1F;
            TorpedoClone.rigidbody.drag = 0.2F;
            TorpedoClone.transform.position = FlightGlobals.ActiveVessel.GetWorldPos3D();
            TorpedoClone.SetActive(true);
            
            

            return TorpedoClone;
        }
    }


    public class LCARS_Explosion : MonoBehaviour
    {

        private ParticleEmitter emitter_debris;
        private ParticleEmitter emitter_explosion;
        private Material glowingPhotons;
        private Transform parent;
        private float radius; 

        public string ExplosionSoundFile = "SciFi/StarTrekImpulseDrive/sounds/DepthChargeShort";
        public float ExplosionSoundVolume = 2.2f;
        public bool loopExplosionSound = false;
        public FXGroup ExplosionSound = null;


        public void Initialize(Transform parent, float radius)
        {
            this.parent = parent;
            transform.parent = parent;
            this.radius = radius;

            //Debris
            GameObject go_debris = new GameObject("PhotonTorpedo Debris");
            emitter_debris = go_debris.AddComponent("EllipsoidParticleEmitter") as ParticleEmitter;
            ParticleAnimator animator_debris = go_debris.AddComponent<ParticleAnimator>();
            go_debris.AddComponent<ParticleRenderer>();
            (go_debris.renderer as ParticleRenderer).uvAnimationXTile = 2;
            (go_debris.renderer as ParticleRenderer).uvAnimationYTile = 2;
            Material mat = new Material(Shader.Find("Particles/Additive"));
            mat.mainTexture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/particles/particle_debris3_photontorpedo", false);
            go_debris.renderer.material = mat;
            emitter_debris.emit = false;
            emitter_debris.minSize = (radius / 100) * 0.5f; //radius * 0.05f;  //4    3 + 
            emitter_debris.maxSize = (radius / 100) * 0.9f; //radius * 0.1f;  //8
            emitter_debris.minEnergy = 1;
            emitter_debris.maxEnergy = 5;
            emitter_debris.rndVelocity = Vector3.one * (20 +(radius / 100)); //1.6f * radius; //150
            emitter_debris.useWorldSpace = false;
            emitter_debris.rndAngularVelocity = 50;
            animator_debris.rndForce = new Vector3(40, 10, 40);
            animator_debris.sizeGrow = -0.1f;

            GameObject go_explosion = new GameObject("PhotonTorpedo Explosion");
            emitter_explosion = go_explosion.AddComponent("EllipsoidParticleEmitter") as ParticleEmitter;
            ParticleAnimator animator_explosion = go_explosion.AddComponent<ParticleAnimator>();
            go_explosion.AddComponent<ParticleRenderer>();
            glowingPhotons = new Material(Shader.Find("Particles/Additive"));
            glowingPhotons.mainTexture = GameDatabase.Instance.GetTexture("SciFi/StarTrekImpulseDrive/particles/particle_photontorpedo", false);
            go_explosion.renderer.material = glowingPhotons;
            (go_explosion.renderer as ParticleRenderer).uvAnimationXTile = 2;
            (go_explosion.renderer as ParticleRenderer).uvAnimationYTile = 2;
            emitter_explosion.emit = false;
            emitter_explosion.minSize = radius / 20; //120
            emitter_explosion.maxSize = radius / 20 * 3.2f; //180
            emitter_explosion.minEnergy = 0.5f;
            emitter_explosion.maxEnergy = 2.5f;
            emitter_explosion.rndVelocity = Vector3.one * (50 + (radius/100));
            emitter_explosion.useWorldSpace = false;
            animator_explosion.rndForce = new Vector3(10, 10, 10);
            animator_explosion.sizeGrow = -0.1f;
            Color[] colors = animator_explosion.colorAnimation;
            colors[0] = new Color(1f, 1f, 1f, 1f);
            colors[1] = new Color(1f, 1f, 1f, 0.9f);
            colors[2] = new Color(1f, 1f, 1f, 0.8f);
            colors[3] = new Color(1f, 1f, 1f, 0.7f);
            colors[4] = new Color(1f, 1f, 1f, 0.5f);
            animator_explosion.colorAnimation = colors;


            ExplosionSound = new FXGroup("ExplosionSound");
            GameObject audioObj = new GameObject();
            audioObj.transform.position = FlightGlobals.ActiveVessel.transform.position;
            audioObj.transform.parent = FlightGlobals.ActiveVessel.transform;	// add to parent
            ExplosionSound.audio = audioObj.AddComponent<AudioSource>();
            ExplosionSound.audio.dopplerLevel = 0f;
            ExplosionSound.audio.Stop();
            ExplosionSound.audio.clip = GameDatabase.Instance.GetAudioClip(ExplosionSoundFile);
            ExplosionSound.audio.loop = false;
            ExplosionSound.audio.Play();
            ExplosionSound.audio.enabled = false;

        
        
        }
        public void Explode(Vector3 position)
        {
            emitter_debris.transform.position = position;
            emitter_explosion.transform.position = position;
            emitter_debris.Emit(20);
            emitter_explosion.Emit(10);
            try
            {
                if (ExplosionSound != null && ExplosionSound.audio != null)
                {
                    float soundVolume = GameSettings.SHIP_VOLUME * ExplosionSoundVolume;
                    ExplosionSound.audio.enabled = true;
                    ExplosionSound.audio.volume = soundVolume;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("ExplosionSound Error Explode: " + ex.Message);
            }
        }

        public void stopSound()
        {
            ExplosionSound.audio.Stop();
            ExplosionSound.audio.enabled = false;
            ExplosionSound = null;
        }
    }	
    class LCARS_PhotonTorpedoType
    {
        public int ID { get; set; }
        public Target Target { get; set; }
        public GameObject TorpedoClone { get; set; }
        public Vector3 currVel { get; set; }
        public Vector3 savedVel { get; set; }
        public Vector3 prevPos { get; set; }



        public float TorpedoYield { get; set; }
    }


    class LCARS_PhotonTorpedo : PartModule
    {
        public int TorpedoCounter=0;
        public GameObject Torpedo;
        public float speed = 10f;
        private Target t;
        GameObject TorpedoClone = null;
        private Vector3 prevPos;
        private Vector3 thiscurrVel;
        private Dictionary<string, LCARS_PhotonTorpedoType> TorpedoList;
        LCARS_Explosion expl=null; 

        GimbalDebug foo = null;

        public string TorpedoSoundFile = "SciFi/StarTrekImpulseDrive/sounds/photorp3";
        public float TorpedoSoundVolume = 2.2f;
        public bool loopTorpedoSound = false;
        public FXGroup TorpedoSound = null;




        LCARS_VesselPartsInventory VPI = null;
        LCARS_PowerSystem PowSys = null;
        PowerTaker PT1 = null;

        public void setVPI(LCARS_VesselPartsInventory thisVPI, LCARS_PowerSystem thisPowSys)
        {
            this.VPI = thisVPI;
            this.PowSys = thisPowSys;
            PT1 = this.PowSys.setPowerTaker("PhotonTorpedo", "SubSystem", 0, 10000f, 0);
        }

        public void onPartDestroy()
        {
            this.VPI.scanVessel();
        }

        public LCARS_PhotonTorpedo()
        {
            this.TorpedoList = new Dictionary<string, LCARS_PhotonTorpedoType>();

        }


        private List<Vessel> vesselList;
        private List<string> visibleVessels;
        public void DestroyPhotonTorpedo(string ID)
        {

            if(expl!=null)
            {
                expl.stopSound();
            }
            
            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo DestroyPhotonTorpedo begin ID=" + ID);
           
            //Transform expl = Instantiate(explosion, transform.position, Quaternion.identity) as Transform;
            //Instantiate(explosionPrefab, pos, rot);

            LCARS_PhotonTorpedoType PTT = this.TorpedoList[ID];
            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo DestroyPhotonTorpedo 1 ");

            expl = PTT.TorpedoClone.AddComponent<LCARS_Explosion>() as LCARS_Explosion;
            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo DestroyPhotonTorpedo 2 ");

            expl.Initialize(PTT.TorpedoClone.transform, 10 * PTT.TorpedoYield);
            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo DestroyPhotonTorpedo 3 PTT.TorpedoYield=" + PTT.TorpedoYield);

            expl.Explode(PTT.TorpedoClone.transform.position);
            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo DestroyPhotonTorpedo 4 ");

            vesselList = FlightGlobals.Vessels;
            visibleVessels = new List<string>() { };
            foreach (Vessel v in vesselList)
            {
                float check_distance = Vector3.Distance(PTT.TorpedoClone.transform.position, v.findWorldCenterOfMass());
                if (check_distance < (PTT.TorpedoYield * 2 ))
                {
                    v.GoOffRails();
                    float mass = v.GetTotalMass();
                    float temperatur_damage = PTT.TorpedoYield * 2000 / mass / check_distance;
                    //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo DestroyPhotonTorpedo temperatur_damage=" + temperatur_damage + " PTT.TorpedoYield=" + PTT.TorpedoYield + " mass=" + mass);
                    foreach (var vesselPart in v.parts.Where(p => p.rigidbody != null))
                    {
                        //vesselPart.force_activate();
                        vesselPart.rigidbody.WakeUp();

                        vesselPart.rigidbody.AddExplosionForce(PTT.TorpedoYield * 40000f, PTT.TorpedoClone.transform.position, PTT.TorpedoYield, 0.01F);
                        if (check_distance < PTT.TorpedoYield)
                        {
                            vesselPart.temperature += temperatur_damage;
                        }
                    }
                }
            }



            Destroy(PTT.TorpedoClone, 0.1f);
            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo DestroyPhotonTorpedo 5 ");

            Destroy(expl, 1f);
            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo DestroyPhotonTorpedo 6 ");
            
            
            TorpedoList.Remove(ID);
            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo DestroyPhotonTorpedo end ");



        }
        public void CalcVelocity()
        {
            foreach (KeyValuePair<string, LCARS_PhotonTorpedoType> pair in TorpedoList)
            {
                LCARS_PhotonTorpedoType PTT = pair.Value;
                try // in case the target ship explodes..
                {

                    string ID = pair.Key;
                    GameObject TorpedoClone = PTT.TorpedoClone;
                    PTT.savedVel = (prevPos - TorpedoClone.transform.position) / Time.deltaTime;
                    PTT.prevPos = TorpedoClone.transform.position;
                }
                catch // in case the target ship explodes..
                {
                    //DestroyPhotonTorpedo(PTT.TorpedoClone.name);
                    continue;
                }

            }
        }


        public void resetTorpedoList(Target t)
        {
            this.TorpedoList = null;
            this.TorpedoList = new Dictionary<string, LCARS_PhotonTorpedoType>();
        }


        public void SpawnPhotonTorpedo(Vector3 location, Target t)
        {
            try
            {
                TorpedoSound = new FXGroup("TorpedoSound");
                GameObject audioObj = new GameObject();
                audioObj.transform.position = FlightGlobals.ActiveVessel.transform.position;
                audioObj.transform.parent = FlightGlobals.ActiveVessel.transform;	// add to parent
                TorpedoSound.audio = audioObj.AddComponent<AudioSource>();
                TorpedoSound.audio.dopplerLevel = 0f;
                TorpedoSound.audio.Stop();
                TorpedoSound.audio.clip = GameDatabase.Instance.GetAudioClip(TorpedoSoundFile);
                TorpedoSound.audio.loop = false;
                TorpedoSound.audio.Play();
                TorpedoSound.audio.enabled = false;
                if (TorpedoSound != null && TorpedoSound.audio != null)
                {
                    float soundVolume = GameSettings.SHIP_VOLUME * TorpedoSoundVolume;
                    TorpedoSound.audio.enabled = true;
                    TorpedoSound.audio.volume = soundVolume;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("ExplosionSound Error Explode: " + ex.Message);
            }




            float TorpedoPower = t.TorpedoYield;
            TorpedoClone = null;

            Collider[] myColliders = FlightGlobals.ActiveVessel.GetComponentsInChildren<Collider>();
            Bounds myBounds = new Bounds(FlightGlobals.ActiveVessel.transform.position, Vector3.zero);
            foreach (Collider nextCollider in myColliders)
            {
                myBounds.Encapsulate(nextCollider.bounds);
            }

            _LCARS_PhotonTorpedo bar = new _LCARS_PhotonTorpedo();
            TorpedoClone = bar.get_PhotonTorpedo(myBounds, FlightGlobals.ActiveVessel, FlightGlobals.ActiveVessel.GetWorldPos3D());
            Vector3 TargetVector = TorpedoClone.transform.position - t.ship.transform.position;
            TorpedoClone.transform.rotation = Quaternion.LookRotation(TargetVector.normalized);
            LCARS_PhotonTorpedoType PTT = new LCARS_PhotonTorpedoType();
            
            PTT.ID = TorpedoCounter++;
            PTT.Target = t;
            PTT.TorpedoClone = TorpedoClone;
            PTT.TorpedoYield = PTT.Target.TorpedoYield;
            PTT.prevPos = TorpedoClone.transform.position;
            PTT.savedVel = new Vector3(0, 0, 0);

            TorpedoList.Add(TorpedoClone.name, PTT);


            float power = PTT.Target.TorpedoYield * PT1.L2_usage;
            this.PowSys.draw(PT1.takerName, power);
        }

        public int TorpedoCount_active()
        {
            return TorpedoList.Count;
        }
        public void TorpedoCount_armory()
        {
        }

        //values for internal use
        private Quaternion _lookRotation;
        private Vector3 _direction;
        BeamDrawer BD1 = new BeamDrawer();
        LineRenderer LR1 = new LineRenderer();
        public void TorpedoUpdate()
        {

            CalcVelocity();

            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate begin ");
            int i = 0;
            foreach (KeyValuePair<string, LCARS_PhotonTorpedoType> pair in TorpedoList)
            {


                LCARS_PhotonTorpedoType PTT = pair.Value;
                string ID = pair.Key;
                Target t = PTT.Target;

                GameObject TorpedoClone = PTT.TorpedoClone;
                float homing_distance = Vector3.Distance(FlightGlobals.ActiveVessel.GetWorldPos3D(), TorpedoClone.transform.position);
                float target_distance = Vector3.Distance(t.ship.transform.localPosition, TorpedoClone.transform.position);
                UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate  ID=" + ID + " homing_distance=" + homing_distance + " target_distance=" + target_distance);

                if (homing_distance>2000)
                {
                    Destroy(TorpedoClone, 0.1f);
                    TorpedoList.Remove(ID);
                    continue;
                }

                try // in case the target ship explodes..
                {

                    TorpedoClone.SetActive(true);
                    TorpedoClone.rigidbody.collider.enabled = true;
                    TorpedoClone.rigidbody.collider.isTrigger = true;
                    TorpedoClone.rigidbody.isKinematic = false;
                    //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate 5 ");



                        t.ship.GoOffRails();
                        //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate 7 ");
                        t.ship.rigidbody.isKinematic = false;
                        //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate 8 ");
                        t.ship.rigidbody.detectCollisions = true;
                        //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate 9 ");
                        //t.ship.rigidbody.collider.isTrigger = true;
                        //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate 10 ");

                        //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate 6 ");

                        if (TorpedoClone.rigidbody != null && t.ship.transform!=null)
                        {
                            if (target_distance > 5F)
                            {
                                //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate 7 ");


                                Vector3 geeVector = FlightGlobals.getGeeForceAtPosition(TorpedoClone.transform.position);
                                //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate 8 geeVector=" + geeVector);
                                CancelG2(TorpedoClone, geeVector);

                                //TorpedoClone.transform.LookAt(t.ship.transform, TorpedoClone.transform.forward);
                                //TorpedoClone.transform.LookAt(t.ship.transform.localPosition, Vector3.up);
                                //TorpedoClone.transform.LookAt(t.ship.transform, Vector3.up);
                                //TorpedoClone.transform.LookAt(t.ship.transform, t.ship.transform.forward);

                                TorpedoClone.transform.LookAt(t.ship.transform, t.ship.upAxis);
                        




                                //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate 10 ");

                                //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate PTT.savedVel.sqrMagnitude=" + PTT.savedVel.sqrMagnitude);
                                    this.AddNewForce_ffwd_back(1200f, TorpedoClone);
                                if (PTT.savedVel.sqrMagnitude < (2f * 2f))
                                {
                                }
                                //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate 11 ");

                                //PilotMode(TorpedoClone, PTT.savedVel, true);

                                //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate 12 ");

                            }
                            else
                            {
                                //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate proximity alert!!!! check_distance=" + check_distance);
                                DestroyPhotonTorpedo(PTT.TorpedoClone.name);
                            }


                        }
                        else
                        {
                            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate 13 ");
                            DestroyPhotonTorpedo(PTT.TorpedoClone.name);
                        }


                        i++;




                }
                catch // in case the target ship explodes..
                {
                    DestroyPhotonTorpedo(PTT.TorpedoClone.name);
                    //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList 7 ");

                }

            }
            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo TorpedoUpdate end ");
  
        }


        public void AddNewForce_ffwd_back(float vSliderValue2, GameObject thisVessel)
        {
            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo AddNewForce_ffwd_back begin ");
            if (vSliderValue2 != 0 && thisVessel.rigidbody != null)
            {
                //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo AddNewForce_ffwd_back 2 ");
                thisVessel.rigidbody.AddForce((Vector3d)thisVessel.transform.forward * vSliderValue2, ForceMode.Acceleration);
                //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo AddNewForce_ffwd_back 2 ");
            }
        }


        public void AddNewForce_left_right(float hSliderValue2, GameObject thisVessel)
        {
            if (hSliderValue2 != 0 && thisVessel.rigidbody != null)
            {
                thisVessel.rigidbody.AddForce((Vector3d)thisVessel.transform.right * hSliderValue2, ForceMode.Acceleration);
            }
        }


        public void AddNewForce_up_down(float zSliderValue2, GameObject thisVessel)
        {
            if (zSliderValue2 != 0 && thisVessel.rigidbody != null)
            {
                thisVessel.rigidbody.AddForce((Vector3d)thisVessel.transform.up * zSliderValue2, ForceMode.Acceleration);
            }
        }
        /// <summary>
        /// Takes a Vessel and a GeeVector as argument and tries to cancel out the gee with counter forces
        /// problem - seems to be called several times per vessel somehow..
        /// </summary>
        public void CancelG2(GameObject thisVessel, Vector3d thisGeeVector)
        {
            var antiGeeVector = thisGeeVector*-1;
            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo CancelG2 beginn antiGeeVector=" + antiGeeVector + " thisVessel=" + thisVessel);
            thisVessel.rigidbody.AddForce(antiGeeVector, ForceMode.Acceleration);
            //UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo CancelG2 end ");
        }


        public void PilotMode(GameObject thisVessel, Vector3d currVel, bool bool_PilotMode)
        {
            if (bool_PilotMode)
            {
                //position of the ship's center of mass:
                Vector3d position = thisVessel.transform.position;

                //velocity as seen in planet's rotating frame
                Vector3d rotatingFrameVelocity = currVel;
                //direction in which the ship currently points:
                Vector3d heading = (Vector3d)thisVessel.transform.forward;
                Vector3 fooVector = Vector3.Reflect(Vector3.Reflect(rotatingFrameVelocity, thisVessel.transform.right), thisVessel.transform.up);
                float angle_up = PosNegAngle(fooVector, thisVessel.transform.forward, thisVessel.transform.right) * 5F;
                //float angle_forward = Vector3.Angle(foo, thisVessel.transform.forward);
                float angle_right = PosNegAngle(fooVector, thisVessel.transform.forward, thisVessel.transform.up) * 5f;

                UnityEngine.Debug.Log("ImpulseDrive: PhotonTorpedo PilotMode angle_up=" + angle_up + " angle_right=" + angle_right);
                AddNewForce_up_down(angle_up, thisVessel);
                AddNewForce_left_right(angle_right, thisVessel);
            }
        }
        /*
        */
        public float PosNegAngle(Vector3 a1, Vector3 a2, Vector3 normal)
        {
            float angle = Vector3.Angle(a1, a2);
            float sign = Mathf.Sign(Vector3.Dot(normal, Vector3.Cross(a1, a2)));
            return angle * sign;
        }
    }
}
