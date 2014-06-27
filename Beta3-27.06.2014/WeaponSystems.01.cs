using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace Philotical
{
    class WeaponSystems
    {
        private List<Vessel> vesselList;
        private List<string> visibleVessels;
        private List<Vessel> ShipList = new List<Vessel>();
        private List<Target> TargetList = new List<Target>();
        private List<string> TargetIDList = new List<string>();
        GravityTools grav;

        private string rootPath = null;
        AudioSource loopSource;
        AudioClip[] clips;


        [KSPField]
        public string PhaserSoundFile = "SciFi/StarTrekImpulseDrive/sounds/tng_phaser8_clean";
        [KSPField]
        public float PhaserSoundVolume = 0.8f;
        [KSPField]
        public bool loopPhaserSound = true;
        public FXGroup PhaserSound = null;

        internal void Draw_ShipSelector()
        {


                /*
            if (rootPath==null)
            {
                rootPath = "file://" + KSPUtil.ApplicationRootPath.Replace("KSP_Data/../", "") + "GameData/SciFi/StarTrekImpulseDrive/sounds/";
                clips = new AudioClip[1];
                WWW www = new WWW(rootPath + "tng_tractor_clean.wav");
                clips[0] = www.GetAudioClip(false);
                PhaserSound.audio.clip = clips[0];
            }
                */

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
                if (v.id != FlightGlobals.ActiveVessel.id && !this.TargetIDList.Contains(v.id.ToString()) && v.loaded)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("" + v.vesselName);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("+"))
                    {
                        UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_ShipSelector CreateTarget was clicked : " + v.vesselName);
                        this.CreateTarget(v);
                    }
                    GUILayout.EndHorizontal();

                }
            }
            GUILayout.EndVertical();

        }

        private void RemoveTarget(Target t)
        {
            t.PhaserSound.audio.Stop();

            LaserDrawer BD1 = t.LaserDrawer1;
            LineRenderer LR1 = t.LineRenderer1;
            LaserDrawer BD2 = t.LaserDrawer2;
            LineRenderer LR2 = t.LineRenderer2;
            LaserDrawer BD3 = t.LaserDrawer3;
            LineRenderer LR3 = t.LineRenderer3;


            LR1 = BD1.render(Vector3.zero, Color.blue);
            LR1.SetWidth(0, 0);
            LR1.SetPosition(0, Vector3.zero);
            LR1.SetPosition(1, Vector3.zero);
            LR2 = BD2.render(Vector3.zero, Color.magenta);
            LR2.SetWidth(0, 0);
            LR2.SetPosition(0, Vector3.zero);
            LR2.SetPosition(1, Vector3.zero);
            Color fc = new Color(0.469F, 0.484F, 0.371F, 0.8F);
            LR3 = BD3.render(Vector3.zero, fc);
            LR3.SetWidth(0, 0);
            LR3.SetPosition(0, Vector3.zero);
            LR3.SetPosition(1, Vector3.zero);

            this.TargetList.Remove(t);
            this.TargetIDList.Remove(t.targetID);
        }

        private void CreateTarget(Vessel v)
        {
            //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: CreateTarget 1 ");
            Vessel Parent = FlightGlobals.ActiveVessel;
            Vector3d parentPosition_current = Parent.transform.localPosition;
            Vector3d childPosition_current = v.transform.localPosition;
            float distance = Vector3.Distance(parentPosition_current, childPosition_current);
            Vector3 newVector = parentPosition_current - childPosition_current;
            //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: CreateTarget 2 ");

            Target nT = new Target();
            LaserDrawer LD1 = new LaserDrawer();
            LineRenderer LR1 = new LineRenderer();
            LaserDrawer LD2 = new LaserDrawer();
            LineRenderer LR2 = new LineRenderer();
            LaserDrawer LD3 = new LaserDrawer();
            LineRenderer LR3 = new LineRenderer();
            //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: CreateTarget 3 ");

            nT.targetID = v.id.ToString();
            nT.ship = v;
            nT.force = 0.5F;
            nT.distance = distance;
            nT.ParentPosition = parentPosition_current;
            nT.ShipPosition = childPosition_current;
            nT.Vector = newVector;
            nT.PhotonTorpedo = new PhotonTorpedo();


            nT.LaserDrawer1 = LD1;
            nT.LineRenderer1 = LR1;
            nT.LaserDrawer2 = LD2;
            nT.LineRenderer2 = LR2;
            nT.LaserDrawer3 = LD3;
            nT.LineRenderer3 = LR3;
            nT.PhaserYield = 0.5F;
            nT.TorpedoYield = 0.5F;
            UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: CreateTarget 4 ");
            this.TargetList.Add(nT);
            this.TargetIDList.Add(nT.targetID);
            UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: CreateTarget 5 ");

            UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: CreateTarget PhaserSound ");
                nT.PhaserSound = new FXGroup("PhaserSound");
                GameObject audioObj = new GameObject();
                nT.audioObj = audioObj;
                nT.audioObj.transform.position = FlightGlobals.ActiveVessel.transform.position;
                nT.audioObj.transform.parent = FlightGlobals.ActiveVessel.transform;	// add to parent
                nT.PhaserSound.audio = audioObj.AddComponent<AudioSource>();
                nT.PhaserSound.audio.dopplerLevel = 0f;
                nT.PhaserSound.audio.Stop();
                nT.PhaserSound.audio.clip = GameDatabase.Instance.GetAudioClip(PhaserSoundFile);
                nT.PhaserSound.audio.loop = true;
                // Seek to a random position in the sound file so we don't have
                // harmonic effects when burning at multiple RCS nozzles.
                nT.PhaserSound.audio.time = UnityEngine.Random.Range(0, nT.PhaserSound.audio.clip.length);
                nT.PhaserSound.audio.Play();
                nT.PhaserSound.audio.enabled = false;
                nT.soundVolume = GameSettings.SHIP_VOLUME * PhaserSoundVolume;
                nT.PhaserSound.audio.volume = nT.soundVolume;

        }

        System.Random rnd = new System.Random();
        internal void FirePhaser(Target t)
        {
            LaserDrawer BD1 = t.LaserDrawer1;
            LineRenderer LR1 = t.LineRenderer1;
            LaserDrawer BD2 = t.LaserDrawer2;
            LineRenderer LR2 = t.LineRenderer2;
            LaserDrawer BD3 = t.LaserDrawer3;
            LineRenderer LR3 = t.LineRenderer3;

            if (!t.PhaserSound.audio.enabled)
            {
                t.soundPitch = Mathf.Lerp(0.09f, 1f, t.PhaserYield / 500);
                t.PhaserSound.audio.pitch = t.soundPitch;
            }
            t.ship.GoOffRails();
            Vessel Parent = FlightGlobals.ActiveVessel;
            Vector3 parentPosition_current = Parent.transform.localPosition;
            Vector3 childPosition_current = t.ship.transform.localPosition;
            float width = 0;

            // RENDER THE Phaser BEAM
            Vector3 PhaserBeamVector = parentPosition_current - childPosition_current;
            LR1 = BD1.render(PhaserBeamVector, Color.red);
            width = 1F;
            LR1.SetWidth(0.2F, width);
            LR1.SetPosition(0, parentPosition_current);
            LR1.SetPosition(1, childPosition_current);

            LR2 = BD2.render(PhaserBeamVector, Color.magenta);
            width = 0.3F;
            LR2.SetWidth(0.1F, width);
            LR2.SetPosition(0, parentPosition_current);
            LR2.SetPosition(1, childPosition_current);

            /*
            Color fc = new Color(0.469F, 0.484F, 0.371F, 0.8F);
            LR3 = BD3.render(PhaserBeamVector, fc,7);
            LR3 = BD3.render(PhaserBeamVector, Color.magenta);
            LR3.SetWidth(0.1F, 0.4F);
            LR3.SetPosition(0, parentPosition_current);
            for (int i = 1; i <= 5; i++)
            {
                var pos = Vector3.Lerp(parentPosition_current, childPosition_current, i / 5.0f);

                pos.x += rnd.Next(-4, 4)/10;
                pos.y += rnd.Next(-4, 4)/10;
                LR3.SetPosition(i, pos);
            }            
            LR3.SetPosition(6, childPosition_current);
            // RENDER THE Phaser BEAM
            */
            
            float dist = Vector3.Distance(FlightGlobals.ActiveVessel.findWorldCenterOfMass(), t.ship.findWorldCenterOfMass());

            foreach (var vesselPart in t.ship.parts.Where(p => p.rigidbody != null))
            {
                vesselPart.temperature += t.PhaserYield * 2000 / dist * TimeWarp.deltaTime;
            }


        }

        internal void resetTorpedoList(Target t)
        {
            t.PhotonTorpedo.resetTorpedoList(t);
        }
        internal void FireTorpedo(Target t)
        {
            t.PhotonTorpedo.SpawnPhotonTorpedo(FlightGlobals.ActiveVessel.findWorldCenterOfMass(), t);
        }
        internal void TorpedoUpdate()
        {
            foreach (Target t in this.TargetList)
            {
                t.PhotonTorpedo.TorpedoUpdate();
            }
        }

        bool bool_two_frames = false;
        internal void Draw_TargetList()
        {


            //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList 1 ");
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Target");
            GUILayout.Label("Ship");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Options");
            GUILayout.EndHorizontal();
            //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList 2 ");
            int b_count = 1;
            //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList 3 ");
            foreach (Target t in this.TargetList)
            {
                //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList 4 ");
                try // in case the target ship explodes..
                {
                    //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList 5 ");
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("" + b_count);
                    GUILayout.Label("" + t.ship.vesselName);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("-"))
                    {
                        this.RemoveTarget(t);
                        UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList RemoveTarget was clicked : " + t.ship.vesselName);
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Distance: " + Vector3.Distance(FlightGlobals.ActiveVessel.findWorldCenterOfMass(), t.ship.findWorldCenterOfMass()));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Phaser Yield: " + t.PhaserYield);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    t.PhaserYield = GUILayout.HorizontalSlider(t.PhaserYield, 0.1F, 500.0F);
                    GUILayout.EndHorizontal();

                    
                    GUILayout.BeginHorizontal();
                    if (GUILayout.RepeatButton("Fire Phaser"))
                    {
                        this.FirePhaser(t);
                        UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList FirePhaser was clicked : " + t.ship.vesselName);

                        try
                        {
                            if (t.PhaserSound != null && t.PhaserSound.audio != null )
                            {
                                if (!t.PhaserSound.audio.enabled)
                                {
                                    t.PhaserSound.audio.enabled = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("PhaserSound Error OnUpdate: " + ex.Message);
                        }
                        /*
                        PhaserSound.enabled = true;
                        PhaserSound.loop = true;
                        PhaserSound.volume = GameSettings.SHIP_VOLUME;
                        */
                        bool_two_frames = false;
                    }
                    else 
                    {
                        if (bool_two_frames == true && t.PhaserSound.audio.enabled)
                        {
                            bool_two_frames = false;
                            t.PhaserSound.audio.enabled = false;
                        }
                        if (bool_two_frames == false)
                        {
                            bool_two_frames = true;
                        }
                            t.LineRenderer1 = t.LaserDrawer1.render(Vector3.zero, Color.red);
                            t.LineRenderer1.SetPosition(0, Vector3.zero);
                            t.LineRenderer1.SetPosition(1, Vector3.zero);
                            Color fc = new Color(0.469F, 0.484F, 0.371F, 0.8F);
                            t.LineRenderer2 = t.LaserDrawer2.render(Vector3.zero, fc);
                            t.LineRenderer2.SetPosition(0, Vector3.zero);
                            t.LineRenderer2.SetPosition(1, Vector3.zero);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Torpedo Yield: " + t.TorpedoYield);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Fire Torpedo (" + t.PhotonTorpedo.TorpedoCount_active() + ")"))
                    {
                        this.FireTorpedo(t);
                        UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList FireTorpedo was clicked : " + t.ship.vesselName);
                    }
                    if (GUILayout.Button("X"))
                    {
                        this.resetTorpedoList(t);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    t.TorpedoYield = GUILayout.HorizontalSlider(t.TorpedoYield, 0.1F, 500.0F);
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    b_count++;
                    //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList 6 ");
                }
                catch // in case the target ship explodes..
                {
                    this.RemoveTarget(t);
                    //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList 7 ");

                }
                //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList 8 ");
            }
            GUILayout.EndVertical();
            //UnityEngine.Debug.Log("ImpulseDrive WeaponSystems: Draw_TargetList 9 ");
        }

    }




    public class LaserDrawer
    {
        public GameObject lineObj = null;
        public LineRenderer lr;
        public LaserDrawer()
        {
        }
        public LineRenderer render(Vector3 transform_up, Color c,int VertexCount = 2)
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

                //this.lr.SetWidth(1, Width);
                this.lr.SetColors(c, c);

                this.lr.SetVertexCount(VertexCount);
                this.lr.useWorldSpace = false;
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
    }

    class Target
    {
        public string targetID { get; set; }

        public Vessel ship { get; set; }

        public float force { get; set; }

        public LineRenderer LineRenderer1 { get; set; }

        public LaserDrawer LaserDrawer1 { get; set; }

        public Vector3 Vector { get; set; }

        public Vector3d ShipPosition { get; set; }

        public Vector3d ParentPosition { get; set; }

        public float distance { get; set; }

        public float PhaserYield { get; set; }

        public float TorpedoYield { get; set; }
        public PhotonTorpedo PhotonTorpedo { get; set; }


    
        public GameObject audioObj { get; set; }

        public FXGroup PhaserSound { get; set; }

        public float soundVolume { get; set; }

        public float soundPitch { get; set; }

        public bool runOnce { get; set; }

        public LineRenderer LineRenderer2 { get; set; }

        public LaserDrawer LaserDrawer2 { get; set; }

        public LaserDrawer LaserDrawer3 { get; set; }

        public LineRenderer LineRenderer3 { get; set; }


    }


}
