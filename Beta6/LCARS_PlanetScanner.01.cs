using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{
    class LCARS_PlanetScanner
    {
        private LineRenderer line = null;
        internal GameObject CameraProbe = null;
        GimbalDebug gimbalDebug2 = null;
        GUIStyle LayoutStyle;
        float zoomFactor = 2.05F;
        private Vessel CurrentMotherShip;
        LCARS_PowerSystem PowSys;
        PowerTaker PT = null;

        RenderTexture pScannScreen = null;
        Texture2D camTex = null;
        Rect screen_rectangle = new Rect(7, 60, 240, 240);

        internal void SetMotherShip(Vessel MS, LCARS_PowerSystem thisPowSys)
        {
            this.CurrentMotherShip = MS;
            //FlightGlobals.ActiveVessel
            this.PowSys = thisPowSys;
            PT = this.PowSys.setPowerTaker("PlanetScanner", "SubSystem", 1250, 1000, 0);
        }

        private float lastFixedUpdate2 = 0.0f;
        private float logInterval2 = 10.0f;
        public void PlanetScanner_initialize(Rect screen_rect)
        {

            //screen_rect = new Rect(120, 120, 375, 375);

            if (this.CameraProbe==null)
            {

            screen_rectangle = screen_rect;

            Debug.Log("ImpulseDrive: PlanetScanner PlanetScanner_initialize 1 ");


            Debug.Log("ImpulseDrive: PlanetScanner PlanetScanner_initialize 2 ");

            this.CameraProbe = new GameObject("CameraProbe");
            
                //CameraProbe.transform.parent = this.CurrentMotherShip.transform; // ...child to our part...
                //CameraProbe.transform.localPosition = Vector3.zero;
                //CameraProbe.transform.localEulerAngles = Vector3.zero;
            }



            Debug.Log("ImpulseDrive: PlanetScanner PlanetScanner_initialize 3 ");

            if (this.pScannScreen == null)
            {

                //this.pScannScreen = new RenderTexture(240, 240, 16, RenderTextureFormat.RGB565);
                this.pScannScreen = new RenderTexture(240, 240, 16, RenderTextureFormat.RGB565, RenderTextureReadWrite.sRGB);
                this.pScannScreen.Create();
                CameraProbe.AddComponent<Camera>();
                CameraProbe.camera.targetTexture = this.pScannScreen;
                CameraProbe.camera.enabled = false;
                //CameraProbe.camera.orthographic = true;

                Debug.Log("ImpulseDrive: PlanetScanner PlanetScanner_initialize 4 ");
            }

            
            /*
            if (gimbalDebug2 == null)
            {
                //gimbalDebug2 = new GimbalDebug();
            }
            else 
            {
                //gimbalDebug2.removeGimbal();
            }
            //gimbalDebug2.drawGimbal(CameraProbe, 300, 8f);
            */

            if ((Time.time - lastFixedUpdate2) > logInterval2)
            {
                Resources.UnloadUnusedAssets();
                lastFixedUpdate2 = Time.time;
            }

            PS_initializeed = true;
        }
        private float lastUpdate = 0.0f;
        private float lastFixedUpdate = 0.0f;
        //private float lastflyUpdate = 0.0f;
        private float logInterval = 5.0f;
        private bool PS_initializeed = false;
        public Rect PlanetScannerWindowPosition = new Rect(120, 120, 375, 375);
        private int PlanetScannerWindowID = new System.Random().Next();
        private GameObject Camera_hinge = new GameObject("Camera_hinge");
        private float fixed_height = 0f;
        Texture2D CameraDisplayTexture = null;
        private GUIContent content;
        public void activateScanner(float zoom, Rect screen_rect)
        {

            Debug.Log("ImpulseDrive: PlanetScanner activateScanner 1 ");

            //this.CameraProbe = null;
            //this.pScannScreen = null;

               PlanetScanner_initialize(screen_rect) ;
                Debug.Log("ImpulseDrive: PlanetScanner activateScanner 2 ");
            

            zoomFactor = zoom;


            Camera_hinge.transform.parent = this.CurrentMotherShip.transform; // ...child to our part...
            Camera_hinge.transform.localPosition = Vector3.zero;
            Camera_hinge.transform.localEulerAngles = Vector3.zero;
            Vector3 gee = FlightGlobals.getGeeForceAtPosition(this.CurrentMotherShip.transform.position);
            Camera_hinge.transform.rotation = Quaternion.LookRotation(gee.normalized);

            Debug.Log("ImpulseDrive: PlanetScanner activateScanner 3 ");

            CameraProbe.transform.parent = Camera_hinge.transform; // ...child to our part...
            CameraProbe.transform.localPosition = Vector3.zero;
            CameraProbe.transform.localEulerAngles = Vector3.zero;

            Debug.Log("ImpulseDrive: PlanetScanner activateScanner 4 ");

            if (gimbalDebug2 != null) { gimbalDebug2.removeGimbal(); }
            if (gimbalDebug2 == null) { gimbalDebug2 = new GimbalDebug(); } else { gimbalDebug2.removeGimbal(); }
            gimbalDebug2.drawGimbal(CameraProbe, 3, 0.2f);


            Debug.Log("ImpulseDrive: PlanetScanner activateScanner 5 ");
            
            float heightFromSurface = ((float)this.CurrentMotherShip.altitude - this.CurrentMotherShip.heightFromTerrain < 0F) ? (float)this.CurrentMotherShip.altitude : this.CurrentMotherShip.heightFromTerrain;
            heightFromSurface = (heightFromSurface != -1) ? heightFromSurface : (float)this.CurrentMotherShip.altitude;
            fixed_height = heightFromSurface - (2000 / zoom);
            CameraProbe.transform.localPosition = Vector3.forward * fixed_height;

            Debug.Log("ImpulseDrive: PlanetScanner activateScanner 6 ");

            

            float vFOVrad = CameraProbe.camera.fieldOfView * Mathf.Deg2Rad;
            float cameraHeightAt1  = Mathf.Tan(vFOVrad *.5f);
            float picWidth = (heightFromSurface - fixed_height) * 0.866025f;



            if (heightFromSurface > 100000F)
            {
                GUILayout.Label("PlanetScanner out of Range - max range 50Km");
            }
            else 
            {
                GUILayout.Label("foo4");
                GUILayout.BeginHorizontal();

                CameraDisplayTexture = RTImage(CameraProbe.camera);
                content = new GUIContent(CameraDisplayTexture, "");
                GUILayout.Box(content,GUILayout.Height(260),GUILayout.Width(260));

                GUILayout.Label("");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                //PlanetScannerWindowPosition = LCARS_Utilities.ClampToScreen(GUI.Window(PlanetScannerWindowID, PlanetScannerWindowPosition, PlanetScannerWindow, ""));
            }
            
            //Debug.Log("ImpulseDrive: PlanetScanner activateScanner 4 ");


            GUILayout.Label("picWidth=" + picWidth);
            GUILayout.Label(this.CurrentMotherShip.RevealSituationString());
            GUILayout.Label(this.CurrentMotherShip.latitude + " lat /" + this.CurrentMotherShip.longitude + "lon");

            if ((Time.time - lastFixedUpdate) > logInterval)
            {
                Debug.Log("ImpulseDrive: PlanetScanner UnloadUnusedAssets");
                Resources.UnloadUnusedAssets();
                lastFixedUpdate = Time.time;
            }

            float power = PT.L1_usage + PT.L2_usage;
            this.PowSys.draw(PT.takerName ,power);

        }
        private void PlanetScannerWindow(int windowID)
        {

            /*
            Color colGUI = GUI.color;
            colGUI.a = 0.5f;
            GUI.color = colGUI;
            */

            if (HighLogic.LoadedSceneIsEditor)
                return;

            GUILayout.Label("foo4");
            GUILayout.BeginHorizontal(GUILayout.Width(370), GUILayout.Height(370));

            RTImage(CameraProbe.camera);
            GUILayout.Label("");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            
            
            GUI.DragWindow();
            //UnityEngine.Debug.Log("ImpulseDrive: info_Window done info_windowID=" + info_windowID);
        }

        private Vector3 ConvertWordCor2TerrCor(Vector3 wordCor) 
        { 
            Vector3 vecRet = new Vector3(); 
            Terrain ter = Terrain.activeTerrain; 
            Vector3 terPosition = ter.transform.position; 
            vecRet.x = ((wordCor.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth; 
            vecRet.z = ((wordCor.y - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight; 
            return vecRet; 
        } 
        //Is this right? or i should change it like this: 
        //vecRet.x = ((wordCor.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;

        private RenderTexture currentRT = null;
        private Texture2D RTImage(Camera cam)
        {
            currentRT = RenderTexture.active;
            RenderTexture.active = cam.targetTexture;
            cam.cullingMask = 557059;
            cam.Render();
            camTex = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
            camTex.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
            camTex.Apply();

            RenderTexture.active = currentRT;
            //Graphics.DrawTexture(screen_rectangle, camTex);
            return camTex;
            //Graphics.SetRenderTarget(CameraDisplayTexture);
            camTex = null;
            currentRT = null;
        }
    }
}
