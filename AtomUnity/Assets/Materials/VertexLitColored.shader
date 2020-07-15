// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified VertexLit shader. Differences from regular VertexLit one:
// - no per-material color
// - no specular
// - no emission

Shader "Mobile/VertexLitColor" {
    Properties{
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Color("Main Color, Alpha", COLOR) = (1,1,1,1)
        [HDR] _EmissionColor("Emit Color", COLOR) = (0,0,0)
    }
        
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 80

        Pass {
            Tags { "LightMode" = "Vertex" }
            Material {
                Diffuse[_Color]
                //Ambient[_Color]
                Emission[_EmissionColor]
            }
            Lighting On
            SetTexture[_MainTex] {
                constantColor[_Color]
                Combine texture * primary DOUBLE, constant // UNITY_OPAQUE_ALPHA_FFP
            }
        }        
    }
}