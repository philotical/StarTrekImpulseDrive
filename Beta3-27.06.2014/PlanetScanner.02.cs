using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{
    class PlanetScanner : MonoBehaviour
    {
        private LineRenderer line = null;
        GameObject CameraProbe = null;
        GimbalDebug gimbalDebug2 = null;
        GUIStyle LayoutStyle;
        float zoomFactor = 2.05F;
        private Vessel CurrentMotherShip;

        RenderTexture pScannScreen;
        Texture2D camTex = null;
        Rect screen_rectangle = new Rect(7, 60, 240, 240);

        internal void SetMotherShip(Vessel MS)
        {
            this.CurrentMotherShip = MS;
            //FlightGlobals.ActiveVessel
        }

        public void PlanetScanner_initialize(Rect screen_rect)
        {
            screen_rectangle = screen_rect;

            Debug.Log("ImpulseDrive: PlanetScanner PlanetScanner_initialize 1 ");


            Debug.Log("ImpulseDrive: PlanetScanner PlanetScanner_initialize 2 ");

            CameraProbe = new GameObject("CameraProbe");
            CameraProbe.transform.parent = CurrentMotherShip.transform; // ...child to our part...
            CameraProbe.transform.localPosition = Vector3.zero;
            CameraProbe.transform.localEulerAngles = Vector3.zero;

            Debug.Log("ImpulseDrive: PlanetScanner PlanetScanner_initialize 3 ");


            Debug.Log("ImpulseDrive: PlanetScanner PlanetScanner_initialize 4 ");
            
            pScannScreen = new RenderTexture(240, 240, 16, RenderTextureFormat.RGB565);
            pScannScreen.Create();
            CameraProbe.AddComponent<Camera>();
            CameraProbe.camera.targetTexture = pScannScreen;
            CameraProbe.camera.enabled = false;
            //CameraProbe.camera.orthographic = true;

            Debug.Log("ImpulseDrive: PlanetScanner PlanetScanner_initialize 5 ");


            
            if (gimbalDebug2 == null)
            {
                //gimbalDebug2 = new GimbalDebug();
            }
            else 
            {
                //gimbalDebug2.removeGimbal();
            }
            //gimbalDebug2.drawGimbal(CameraProbe, 300, 8f);
            /*
            */


        }
        private float lastUpdate = 0.0f;
        private float lastFixedUpdate = 0.0f;
        //private float lastflyUpdate = 0.0f;
        private float logInterval = 5.0f;
        public void activateScanner(float zoom, Rect screen_rect)
        {
            //Debug.Log("ImpulseDrive: PlanetScanner activateScanner 1 ");

            if (CameraProbe==null)
            {
                PlanetScanner_initialize(screen_rect);
            }

            zoomFactor = zoom;
            // As our part moves make the line point downwards to planet's center.
            Vector3 gee = FlightGlobals.getGeeForceAtPosition(this.CurrentMotherShip.transform.position);
            CameraProbe.transform.rotation = Quaternion.LookRotation(gee.normalized);

            //Debug.Log("ImpulseDrive: PlanetScanner activateScanner 2 ");

            float heightFromSurface = ((float)this.CurrentMotherShip.altitude - this.CurrentMotherShip.heightFromTerrain < 0F) ? (float)this.CurrentMotherShip.altitude : this.CurrentMotherShip.heightFromTerrain;
            heightFromSurface = (heightFromSurface != -1) ? heightFromSurface : (float)this.CurrentMotherShip.altitude;
            float fixed_height = heightFromSurface - (1000F / zoomFactor);
            CameraProbe.transform.localPosition = Vector3.forward * fixed_height;
            //CameraProbe.transform.localPosition = gee.normalized * fixed_height;

            float vFOVrad = CameraProbe.camera.fieldOfView * Mathf.Deg2Rad;
            float cameraHeightAt1  = Mathf.Tan(vFOVrad *.5f);
            float picWidth = (heightFromSurface - fixed_height) * 0.866025f;
            /*
            float hFOVrad = Mathf.Atan(cameraHeightAt1 * CameraProbe.camera.aspect) * 2;
            float hFOV  = hFOVrad * Mathf.Rad2Deg;
            //float fov = Mathf.Atan((0.5f * (1000F)) / (heightFromSurface  - fixed_height ) ) * 2.0f * Mathf.Rad2Deg;
            */


            //Debug.Log("ImpulseDrive: PlanetScanner activateScanner ---3-- ");
            //Debug.Log("ImpulseDrive: PlanetScanner activateScanner hFOV=" + hFOV);
            //Debug.Log("ImpulseDrive: PlanetScanner activateScanner this.CurrentMotherShip.heightFromTerrain=" + this.CurrentMotherShip.heightFromTerrain);
            //Debug.Log("ImpulseDrive: PlanetScanner activateScanner this.CurrentMotherShip.altitude=" + this.CurrentMotherShip.altitude);
            //Debug.Log("ImpulseDrive: PlanetScanner activateScanner heightFromSurface=" + heightFromSurface);
            //Debug.Log("ImpulseDrive: PlanetScanner activateScanner fixed_height=" + fixed_height);

            if (heightFromSurface > 100000F)
            {
                GUILayout.Label("PlanetScanner out of Range - max range 50Km");
            }
            else 
            {
                GUILayout.Label("foo4");
                GUILayout.BeginHorizontal(GUILayout.Width(240), GUILayout.Height(240));
                camTex = null;
                RTImage(CameraProbe.camera);
                camTex = null;
                GUILayout.Label("");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            
            //Debug.Log("ImpulseDrive: PlanetScanner activateScanner 4 ");


            GUILayout.Label("picWidth=" + picWidth);
            GUILayout.Label("position1=" + this.CurrentMotherShip.transform.position.ToString());
            //GUILayout.Label("position2=" + ConvertWordCor2TerrCor(this.CurrentMotherShip.transform.position));
            GUILayout.Label("position2=" + this.CurrentMotherShip.latitude + "/" + this.CurrentMotherShip.longitude);
            GUILayout.Label("GetWorldSurfacePosition=" + this.CurrentMotherShip.mainBody.GetWorldSurfacePosition(this.CurrentMotherShip.latitude, this.CurrentMotherShip.longitude, 0));

            //Debug.Log("ImpulseDrive: PlanetScanner activateScanner 4 ");

            //DetectLeaks foooo = new DetectLeaks();
            //foooo.buttons();
            //Debug.Log("ImpulseDrive: PlanetScanner activateScanner 6 ");

            if ((Time.time - lastFixedUpdate) > logInterval)
            {
                Resources.UnloadUnusedAssets();
            }

            if ((Time.time - lastFixedUpdate) > logInterval)
            {
                Resources.UnloadUnusedAssets();
            }

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

        private void RTImage(Camera cam)
        {
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = cam.targetTexture;
            cam.Render();
            camTex = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
            camTex.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
            camTex.Apply();
            RenderTexture.active = currentRT;
            GUI.DrawTexture(screen_rectangle, camTex);
            cam = null;
            currentRT = null;
            camTex = null;
        }
    }
}
