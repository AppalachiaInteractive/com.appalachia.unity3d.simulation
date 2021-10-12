Shader "internal/touchbend/touchbend"
{
	Properties
	{

	}
	
	SubShader
	{
		Cull Off
		
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			//#pragma geometry geom

			#include "UnityCG.cginc"
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 data : TEXCOORD0;
			};
			
			float4 _TB_Pos;
			float _SIZE_XZ = 1;

			//[maxvertexcount(3)]
			//void geom(triangle v2f input[3], inout TriangleStream<v2f> OutputStream)
			//{

			//	float3 center = (input[0].pos.xyz + input[1].pos.xyz + input[2].pos.xyz) / 3;

			//	for (int i = 0; i < 3; i++)
			//	{
			//		input[i].pos.x = center.x + (input[i].pos.x - center.x)*2;
			//		//input[i].pos.y = center.y + (input[i].pos.y - center.y);
			//		input[i].pos.z = center.z + (input[i].pos.z - center.z)*2;
			//		OutputStream.Append(input[i]);
			//	}
			//}

			
			v2f vert(appdata_base v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				o.pos = UnityObjectToClipPos(v.vertex);
				
				float3 normal = (mul( unity_ObjectToWorld, v.normal ) * 0.5) + 0.5;
                
				//o.data.a = mul( unity_ObjectToWorld, v.vertex ).y/10000;
                
                o.data.r = normal.x;
                o.data.g = saturate(_SIZE_XZ - length(v.vertex.xz));
                o.data.b = normal.z;
				o.data.a = 1;

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				return i.data;
			}
			ENDCG

		}
	}
}
