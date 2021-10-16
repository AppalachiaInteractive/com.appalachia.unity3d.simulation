Shader "internal/core/trees/TextureCombiner"
{
    Properties
    {        
        _RTex ("_RTex", 2D) = "black" {}
        _GTex ("_GTex", 2D) = "black" {}
        _BTex ("_BTex", 2D) = "black" {}
        _ATex ("_ATex", 2D) = "black" {}
        
        _RTexChannel ("_RTexChannel", Range(0.0, 3.0)) = 0.0
        _GTexChannel ("_GTexChannel", Range(0.0, 3.0)) = 1.0
        _BTexChannel ("_BTexChannel", Range(0.0, 3.0)) = 2.0
        _ATexChannel ("_ATexChannel", Range(0.0, 3.0)) = 3.0
        
        _RTexInvert ("_RTexInvert", Range(0.0, 1.0)) = 0.0
        _GTexInvert ("_GTexInvert", Range(0.0, 1.0)) = 0.0
        _BTexInvert ("_BTexInvert", Range(0.0, 1.0)) = 0.0
        _ATexInvert ("_ATexInvert", Range(0.0, 1.0)) = 0.0
        
        _RTexPackStyle ("_RTexPackStyle", Range(0.0, 2.0)) = 0.0
        _GTexPackStyle ("_GTexPackStyle", Range(0.0, 2.0)) = 0.0
        _BTexPackStyle ("_BTexPackStyle", Range(0.0, 2.0)) = 0.0
        _ATexPackStyle ("_ATexPackStyle", Range(0.0, 2.0)) = 0.0
        
        _RTexFixedValue ("_RTexFixedValue", Range(-1.0, 1.0)) = -1.0
        _GTexFixedValue ("_GTexFixedValue", Range(-1.0, 1.0)) = -1.0
        _BTexFixedValue ("_BTexFixedValue", Range(-1.0, 1.0)) = -1.0
        _ATexFixedValue ("_ATexFixedValue", Range(-1.0, 1.0)) = -1.0
        
        _RTexMode ("_RTexMode", Range(0.0, 3.0)) = 0.0
        _GTexMode ("_GTexMode", Range(0.0, 3.0)) = 0.0
        _BTexMode ("_BTexMode", Range(0.0, 3.0)) = 0.0
        _ATexMode ("_ATexMode", Range(0.0, 3.0)) = 0.0

        _ColorStyle ("_ColorStyle", Range(0.0, 2.0)) = 0.0
        _ColorBoost ("_ColorBoost", Range(0.0, 4.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "TextureCombiner.cginc"
            ENDCG
        }

     
    }
}