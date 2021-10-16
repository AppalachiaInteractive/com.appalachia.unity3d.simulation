Shader "internal/core/trees/TexturePreview"
{
    Properties
    {        
        _MainTex ("_MainTex", 2D) = "black" {}       
      
    }
    SubShader
    {
        //Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Geometry" }
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			 
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200		

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
                        
            sampler2D _MainTex;
          
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {            
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                //tex.rgb = GammaToLinearSpace(tex);
                tex.rgb = LinearToGammaSpace(tex);
                
                return tex;
            }
            ENDCG
        }     
    }
}