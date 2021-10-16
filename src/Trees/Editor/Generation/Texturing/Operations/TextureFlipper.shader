Shader "internal/core/trees/TextureFlipper"
{
    Properties
    {        
        _MainTex ("_MainTex", 2D) = "black" {}       
      
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
                o.uv.y =  1 - o.uv.y;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {            
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                return tex;
            }
            ENDCG
        }     
    }
}