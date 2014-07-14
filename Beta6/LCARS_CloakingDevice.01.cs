using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Philotical
{
    class LCARS_CloakingDevice : PartModule
    {
        public LCARS_CloakingDevice()
        {
        }

        Vessel ShipSelected = null;
        LCARS_PowerSystem PowSys;
        PowerTaker PT1 = null;

        internal void SetShip(Vessel v, LCARS_PowerSystem thisPowSys)
        {
            this.ShipSelected = v;
            this.PowSys = thisPowSys;
            PT1 = this.PowSys.setPowerTaker("CloakingDevice", "SubSystem", 2500, 50000f, 0);
        }

        public void set_opacity(float force)
        {

                foreach (Part p in this.ShipSelected.Parts)
                {
                    p.setOpacity(force);
                }

            
            
            /*
            if (GUILayout.Button("test"))
            {
                foreach (Part p in this.ShipSelected.Parts)
                {
                    //UnityEngine.Debug.Log("LCARS_CloakingDevice: " + p.renderer.materials.Count() + " material(s)");
                    foreach (Material m in p.renderer.materials)
                    {
                        UnityEngine.Debug.Log("LCARS_CloakingDevice m.name=" + m.name);
                        m.color = Color.black;
                        m.mainTexture = new Texture2D(1, 1);//  someTexture;
                    }

                    //this.ShipSelected.renderer.materials[0].color = Color.black;
                    //this.ShipSelected.renderer.materials[1].mainTexture = someTexture;
                }
            }
            */
        
            /*
        UnityEngine.Debug.Log("LCARS_CloakingDevice: out of order ");
            UnityEngine.Debug.Log("LCARS_CloakingDevice: set_opacity begin ");
            GameObject go = this.ShipSelected.gameObject;
            UnityEngine.Debug.Log("LCARS_CloakingDevice: set_opacity 1 ");
            UnityEngine.Debug.Log("LCARS_CloakingDevice: " + go.renderer.materials.Length + " material(s)");
            foreach (Material m in go.renderer.materials)
            {
                UnityEngine.Debug.Log("LCARS_CloakingDevice m.name=" + m.name);
            }

           
            Color c = go.renderer.material.color;
            UnityEngine.Debug.Log("LCARS_CloakingDevice: set_opacity 2 ");

            c.a = force;
            UnityEngine.Debug.Log("LCARS_CloakingDevice: set_opacity 3 ");
            
            go.renderer.material.color = c;
            UnityEngine.Debug.Log("LCARS_CloakingDevice: set_opacity 4 ");

                force = (force>0) ? force : 0.01f;
                float power = PT1.L1_usage +(PT1.L2_usage / force);
                this.PowSys.draw(PT1.takerName, power);


            */
            /*
            foreach (Part p in this.ShipSelected.Parts)
            {
                GameObject go = p.gameObject;

                UnityEngine.Debug.Log("LCARS_CloakingDevice: " + go.renderer.materials.Length + " material(s)");
                foreach(Material m in go.renderer.materials)
                {
                    UnityEngine.Debug.Log("LCARS_CloakingDevice m.name=" + m.name);
                }

                Color c = go.renderer.material.color;
                UnityEngine.Debug.Log("LCARS_CloakingDevice: set_opacity 1 ");

                c.a = force;
                UnityEngine.Debug.Log("LCARS_CloakingDevice: set_opacity 2 ");
                
                go.renderer.material.color = c;
                UnityEngine.Debug.Log("LCARS_CloakingDevice: set_opacity 3 ");

                
                

            }
            UnityEngine.Debug.Log("LCARS_CloakingDevice: set_opacity end ");

            float power = force * 15000f;
            this.PowSys.draw(power);
            this.PowSys.Powerstats["consumption_total"] += power;
            this.PowSys.Powerstats["consumption_main_systems"] += 0f;
            this.PowSys.Powerstats["consumption_sub_systems"] += power;

             
             */
        }
    }
}

/*
 * http://wiki.unity3d.com/index.php?title=AlphaVertexLitZ
 Shader "Transparent/VertexLit with Z" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _SpecColor ("Spec Color", Color) = (1,1,1,0)
    _Emission ("Emissive Color", Color) = (0,0,0,0)
    _Shininess ("Shininess", Range (0.1, 1)) = 0.7
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
 
SubShader {
    Tags {"RenderType"="Transparent" "Queue"="Transparent"}
    // Render into depth buffer only
        Pass {
        ColorMask 0
    }
    // Render normally
    Pass {
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        Material {
            Diffuse [_Color]
            Ambient [_Color]
            Shininess [_Shininess]
            Specular [_SpecColor]
            Emission [_Emission]
        }
        Lighting On
        SetTexture [_MainTex] {
            Combine texture * primary DOUBLE, texture * primary
        }
    }
}
}
 */