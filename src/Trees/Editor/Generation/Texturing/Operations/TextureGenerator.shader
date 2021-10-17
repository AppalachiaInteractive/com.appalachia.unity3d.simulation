// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "appalachia/core/trees/TextureGenerator"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		[Toggle]_NormalMapPresent("Normal Map Present", Float) = 0
		[Toggle]_GenerateSurface("Generate Surface", Float) = 1
		_NormalScale("Normal Scale", Range( 0 , 20)) = 1
		_BumpMap("BumpMap", 2D) = "bump" {}
		_AOStrength("AO Strength", Range( 0 , 2)) = 0.85
		_TransmissionStrength("Transmission Strength", Range( 0 , 2)) = 0.85
		_TransmissionHueLow("Transmission Hue Low", Range( 0 , 1)) = 0.2058824
		_TransmissionHueHigh("Transmission Hue High", Range( 0 , 1)) = 0.3109002
		_SmoothnessStrength("Smoothness Strength", Range( 0 , 1)) = 0.075
		_Levels("Levels", Vector) = (1,1,1,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
			};

			uniform float _GenerateSurface;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			uniform float _NormalScale;
			uniform float4 _Levels;
			uniform float _NormalMapPresent;
			uniform sampler2D _BumpMap;
			uniform float _AOStrength;
			uniform float _TransmissionStrength;
			uniform float4 _MainTex_ST;
			uniform float _TransmissionHueLow;
			uniform float _TransmissionHueHigh;
			uniform float _SmoothnessStrength;
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				float2 appendResult14_g195 = (float2(_MainTex_TexelSize.x , 0.0));
				float2 temp_output_1_0_g195 = i.ase_texcoord.xy;
				float temp_output_4_0_g195 = 6.0;
				float temp_output_2_0_g195 = 0.1;
				float luminance36_g195 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult14_g195 + temp_output_1_0_g195 ), 0, temp_output_4_0_g195) ) + temp_output_2_0_g195 ).rgb);
				float2 appendResult16_g195 = (float2(-_MainTex_TexelSize.x , 0.0));
				float luminance51_g195 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult16_g195 + temp_output_1_0_g195 ), 0, temp_output_4_0_g195) ) + temp_output_2_0_g195 ).rgb);
				float2 appendResult15_g195 = (float2(0.0 , _MainTex_TexelSize.y));
				float luminance48_g195 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult15_g195 + temp_output_1_0_g195 ), 0, temp_output_4_0_g195) ) + temp_output_2_0_g195 ).rgb);
				float2 appendResult17_g195 = (float2(0.0 , -_MainTex_TexelSize.y));
				float luminance54_g195 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult17_g195 + temp_output_1_0_g195 ), 0, temp_output_4_0_g195) ) + temp_output_2_0_g195 ).rgb);
				float temp_output_68_0_g195 = length( sqrt( ( ( -( saturate( luminance36_g195 ) - saturate( luminance51_g195 ) ) * -( saturate( luminance36_g195 ) - saturate( luminance51_g195 ) ) * -( saturate( luminance48_g195 ) - saturate( luminance54_g195 ) ) * -( saturate( luminance48_g195 ) - saturate( luminance54_g195 ) ) ) + 1.0 ) ) );
				float temp_output_3_0_g195 = 0.8;
				float3 appendResult85_g195 = (float3(( ( ( ( 10.0 * temp_output_3_0_g195 * -( saturate( luminance36_g195 ) - saturate( luminance51_g195 ) ) ) / temp_output_68_0_g195 ) * 0.5 ) + 0.5 ) , ( ( ( ( 10.0 * temp_output_3_0_g195 * -( saturate( luminance48_g195 ) - saturate( luminance54_g195 ) ) ) / temp_output_68_0_g195 ) * 0.5 ) + 0.5 ) , ( temp_output_68_0_g195 * -1.0 )));
				float2 appendResult14_g187 = (float2(_MainTex_TexelSize.x , 0.0));
				float2 temp_output_1_0_g187 = i.ase_texcoord.xy;
				float temp_output_4_0_g187 = 5.0;
				float temp_output_2_0_g187 = 0.2;
				float luminance36_g187 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult14_g187 + temp_output_1_0_g187 ), 0, temp_output_4_0_g187) ) + temp_output_2_0_g187 ).rgb);
				float2 appendResult16_g187 = (float2(-_MainTex_TexelSize.x , 0.0));
				float luminance51_g187 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult16_g187 + temp_output_1_0_g187 ), 0, temp_output_4_0_g187) ) + temp_output_2_0_g187 ).rgb);
				float2 appendResult15_g187 = (float2(0.0 , _MainTex_TexelSize.y));
				float luminance48_g187 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult15_g187 + temp_output_1_0_g187 ), 0, temp_output_4_0_g187) ) + temp_output_2_0_g187 ).rgb);
				float2 appendResult17_g187 = (float2(0.0 , -_MainTex_TexelSize.y));
				float luminance54_g187 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult17_g187 + temp_output_1_0_g187 ), 0, temp_output_4_0_g187) ) + temp_output_2_0_g187 ).rgb);
				float temp_output_68_0_g187 = length( sqrt( ( ( -( saturate( luminance36_g187 ) - saturate( luminance51_g187 ) ) * -( saturate( luminance36_g187 ) - saturate( luminance51_g187 ) ) * -( saturate( luminance48_g187 ) - saturate( luminance54_g187 ) ) * -( saturate( luminance48_g187 ) - saturate( luminance54_g187 ) ) ) + 1.0 ) ) );
				float temp_output_3_0_g187 = 0.7;
				float3 appendResult85_g187 = (float3(( ( ( ( 10.0 * temp_output_3_0_g187 * -( saturate( luminance36_g187 ) - saturate( luminance51_g187 ) ) ) / temp_output_68_0_g187 ) * 0.5 ) + 0.5 ) , ( ( ( ( 10.0 * temp_output_3_0_g187 * -( saturate( luminance48_g187 ) - saturate( luminance54_g187 ) ) ) / temp_output_68_0_g187 ) * 0.5 ) + 0.5 ) , ( temp_output_68_0_g187 * -1.0 )));
				float2 appendResult14_g199 = (float2(_MainTex_TexelSize.x , 0.0));
				float2 temp_output_1_0_g199 = i.ase_texcoord.xy;
				float temp_output_4_0_g199 = 4.0;
				float temp_output_2_0_g199 = 0.3;
				float luminance36_g199 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult14_g199 + temp_output_1_0_g199 ), 0, temp_output_4_0_g199) ) + temp_output_2_0_g199 ).rgb);
				float2 appendResult16_g199 = (float2(-_MainTex_TexelSize.x , 0.0));
				float luminance51_g199 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult16_g199 + temp_output_1_0_g199 ), 0, temp_output_4_0_g199) ) + temp_output_2_0_g199 ).rgb);
				float2 appendResult15_g199 = (float2(0.0 , _MainTex_TexelSize.y));
				float luminance48_g199 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult15_g199 + temp_output_1_0_g199 ), 0, temp_output_4_0_g199) ) + temp_output_2_0_g199 ).rgb);
				float2 appendResult17_g199 = (float2(0.0 , -_MainTex_TexelSize.y));
				float luminance54_g199 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult17_g199 + temp_output_1_0_g199 ), 0, temp_output_4_0_g199) ) + temp_output_2_0_g199 ).rgb);
				float temp_output_68_0_g199 = length( sqrt( ( ( -( saturate( luminance36_g199 ) - saturate( luminance51_g199 ) ) * -( saturate( luminance36_g199 ) - saturate( luminance51_g199 ) ) * -( saturate( luminance48_g199 ) - saturate( luminance54_g199 ) ) * -( saturate( luminance48_g199 ) - saturate( luminance54_g199 ) ) ) + 1.0 ) ) );
				float temp_output_3_0_g199 = 0.6;
				float3 appendResult85_g199 = (float3(( ( ( ( 10.0 * temp_output_3_0_g199 * -( saturate( luminance36_g199 ) - saturate( luminance51_g199 ) ) ) / temp_output_68_0_g199 ) * 0.5 ) + 0.5 ) , ( ( ( ( 10.0 * temp_output_3_0_g199 * -( saturate( luminance48_g199 ) - saturate( luminance54_g199 ) ) ) / temp_output_68_0_g199 ) * 0.5 ) + 0.5 ) , ( temp_output_68_0_g199 * -1.0 )));
				float2 appendResult14_g183 = (float2(_MainTex_TexelSize.x , 0.0));
				float2 temp_output_1_0_g183 = i.ase_texcoord.xy;
				float temp_output_4_0_g183 = 3.0;
				float temp_output_2_0_g183 = 0.4;
				float luminance36_g183 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult14_g183 + temp_output_1_0_g183 ), 0, temp_output_4_0_g183) ) + temp_output_2_0_g183 ).rgb);
				float2 appendResult16_g183 = (float2(-_MainTex_TexelSize.x , 0.0));
				float luminance51_g183 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult16_g183 + temp_output_1_0_g183 ), 0, temp_output_4_0_g183) ) + temp_output_2_0_g183 ).rgb);
				float2 appendResult15_g183 = (float2(0.0 , _MainTex_TexelSize.y));
				float luminance48_g183 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult15_g183 + temp_output_1_0_g183 ), 0, temp_output_4_0_g183) ) + temp_output_2_0_g183 ).rgb);
				float2 appendResult17_g183 = (float2(0.0 , -_MainTex_TexelSize.y));
				float luminance54_g183 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult17_g183 + temp_output_1_0_g183 ), 0, temp_output_4_0_g183) ) + temp_output_2_0_g183 ).rgb);
				float temp_output_68_0_g183 = length( sqrt( ( ( -( saturate( luminance36_g183 ) - saturate( luminance51_g183 ) ) * -( saturate( luminance36_g183 ) - saturate( luminance51_g183 ) ) * -( saturate( luminance48_g183 ) - saturate( luminance54_g183 ) ) * -( saturate( luminance48_g183 ) - saturate( luminance54_g183 ) ) ) + 1.0 ) ) );
				float temp_output_3_0_g183 = 0.5;
				float3 appendResult85_g183 = (float3(( ( ( ( 10.0 * temp_output_3_0_g183 * -( saturate( luminance36_g183 ) - saturate( luminance51_g183 ) ) ) / temp_output_68_0_g183 ) * 0.5 ) + 0.5 ) , ( ( ( ( 10.0 * temp_output_3_0_g183 * -( saturate( luminance48_g183 ) - saturate( luminance54_g183 ) ) ) / temp_output_68_0_g183 ) * 0.5 ) + 0.5 ) , ( temp_output_68_0_g183 * -1.0 )));
				float2 appendResult14_g203 = (float2(_MainTex_TexelSize.x , 0.0));
				float2 temp_output_1_0_g203 = i.ase_texcoord.xy;
				float temp_output_4_0_g203 = 2.0;
				float temp_output_2_0_g203 = 0.5;
				float luminance36_g203 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult14_g203 + temp_output_1_0_g203 ), 0, temp_output_4_0_g203) ) + temp_output_2_0_g203 ).rgb);
				float2 appendResult16_g203 = (float2(-_MainTex_TexelSize.x , 0.0));
				float luminance51_g203 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult16_g203 + temp_output_1_0_g203 ), 0, temp_output_4_0_g203) ) + temp_output_2_0_g203 ).rgb);
				float2 appendResult15_g203 = (float2(0.0 , _MainTex_TexelSize.y));
				float luminance48_g203 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult15_g203 + temp_output_1_0_g203 ), 0, temp_output_4_0_g203) ) + temp_output_2_0_g203 ).rgb);
				float2 appendResult17_g203 = (float2(0.0 , -_MainTex_TexelSize.y));
				float luminance54_g203 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult17_g203 + temp_output_1_0_g203 ), 0, temp_output_4_0_g203) ) + temp_output_2_0_g203 ).rgb);
				float temp_output_68_0_g203 = length( sqrt( ( ( -( saturate( luminance36_g203 ) - saturate( luminance51_g203 ) ) * -( saturate( luminance36_g203 ) - saturate( luminance51_g203 ) ) * -( saturate( luminance48_g203 ) - saturate( luminance54_g203 ) ) * -( saturate( luminance48_g203 ) - saturate( luminance54_g203 ) ) ) + 1.0 ) ) );
				float temp_output_3_0_g203 = 0.4;
				float3 appendResult85_g203 = (float3(( ( ( ( 10.0 * temp_output_3_0_g203 * -( saturate( luminance36_g203 ) - saturate( luminance51_g203 ) ) ) / temp_output_68_0_g203 ) * 0.5 ) + 0.5 ) , ( ( ( ( 10.0 * temp_output_3_0_g203 * -( saturate( luminance48_g203 ) - saturate( luminance54_g203 ) ) ) / temp_output_68_0_g203 ) * 0.5 ) + 0.5 ) , ( temp_output_68_0_g203 * -1.0 )));
				float2 appendResult14_g191 = (float2(_MainTex_TexelSize.x , 0.0));
				float2 temp_output_1_0_g191 = i.ase_texcoord.xy;
				float temp_output_4_0_g191 = 1.0;
				float temp_output_2_0_g191 = 0.6;
				float luminance36_g191 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult14_g191 + temp_output_1_0_g191 ), 0, temp_output_4_0_g191) ) + temp_output_2_0_g191 ).rgb);
				float2 appendResult16_g191 = (float2(-_MainTex_TexelSize.x , 0.0));
				float luminance51_g191 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult16_g191 + temp_output_1_0_g191 ), 0, temp_output_4_0_g191) ) + temp_output_2_0_g191 ).rgb);
				float2 appendResult15_g191 = (float2(0.0 , _MainTex_TexelSize.y));
				float luminance48_g191 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult15_g191 + temp_output_1_0_g191 ), 0, temp_output_4_0_g191) ) + temp_output_2_0_g191 ).rgb);
				float2 appendResult17_g191 = (float2(0.0 , -_MainTex_TexelSize.y));
				float luminance54_g191 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult17_g191 + temp_output_1_0_g191 ), 0, temp_output_4_0_g191) ) + temp_output_2_0_g191 ).rgb);
				float temp_output_68_0_g191 = length( sqrt( ( ( -( saturate( luminance36_g191 ) - saturate( luminance51_g191 ) ) * -( saturate( luminance36_g191 ) - saturate( luminance51_g191 ) ) * -( saturate( luminance48_g191 ) - saturate( luminance54_g191 ) ) * -( saturate( luminance48_g191 ) - saturate( luminance54_g191 ) ) ) + 1.0 ) ) );
				float temp_output_3_0_g191 = 0.3;
				float3 appendResult85_g191 = (float3(( ( ( ( 10.0 * temp_output_3_0_g191 * -( saturate( luminance36_g191 ) - saturate( luminance51_g191 ) ) ) / temp_output_68_0_g191 ) * 0.5 ) + 0.5 ) , ( ( ( ( 10.0 * temp_output_3_0_g191 * -( saturate( luminance48_g191 ) - saturate( luminance54_g191 ) ) ) / temp_output_68_0_g191 ) * 0.5 ) + 0.5 ) , ( temp_output_68_0_g191 * -1.0 )));
				float2 appendResult14_g207 = (float2(_MainTex_TexelSize.x , 0.0));
				float2 temp_output_1_0_g207 = i.ase_texcoord.xy;
				float temp_output_4_0_g207 = 0.0;
				float temp_output_2_0_g207 = 0.7;
				float luminance36_g207 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult14_g207 + temp_output_1_0_g207 ), 0, temp_output_4_0_g207) ) + temp_output_2_0_g207 ).rgb);
				float2 appendResult16_g207 = (float2(-_MainTex_TexelSize.x , 0.0));
				float luminance51_g207 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult16_g207 + temp_output_1_0_g207 ), 0, temp_output_4_0_g207) ) + temp_output_2_0_g207 ).rgb);
				float2 appendResult15_g207 = (float2(0.0 , _MainTex_TexelSize.y));
				float luminance48_g207 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult15_g207 + temp_output_1_0_g207 ), 0, temp_output_4_0_g207) ) + temp_output_2_0_g207 ).rgb);
				float2 appendResult17_g207 = (float2(0.0 , -_MainTex_TexelSize.y));
				float luminance54_g207 = Luminance(( tex2Dbias( _MainTex, float4( ( appendResult17_g207 + temp_output_1_0_g207 ), 0, temp_output_4_0_g207) ) + temp_output_2_0_g207 ).rgb);
				float temp_output_68_0_g207 = length( sqrt( ( ( -( saturate( luminance36_g207 ) - saturate( luminance51_g207 ) ) * -( saturate( luminance36_g207 ) - saturate( luminance51_g207 ) ) * -( saturate( luminance48_g207 ) - saturate( luminance54_g207 ) ) * -( saturate( luminance48_g207 ) - saturate( luminance54_g207 ) ) ) + 1.0 ) ) );
				float temp_output_3_0_g207 = 0.2;
				float3 appendResult85_g207 = (float3(( ( ( ( 10.0 * temp_output_3_0_g207 * -( saturate( luminance36_g207 ) - saturate( luminance51_g207 ) ) ) / temp_output_68_0_g207 ) * 0.5 ) + 0.5 ) , ( ( ( ( 10.0 * temp_output_3_0_g207 * -( saturate( luminance48_g207 ) - saturate( luminance54_g207 ) ) ) / temp_output_68_0_g207 ) * 0.5 ) + 0.5 ) , ( temp_output_68_0_g207 * -1.0 )));
				float2 temp_cast_35 = (1.0).xx;
				float2 temp_output_2_0_g91 = ( ( ( (( ( ( (( temp_output_68_0_g195 > 0.0 ) ? float4( appendResult85_g195 , 0.0 ) :  float4(0.5,0.5,0,1) ) * 6.0 ) + ( (( temp_output_68_0_g187 > 0.0 ) ? float4( appendResult85_g187 , 0.0 ) :  float4(0.5,0.5,0,1) ) * 5.0 ) + ( (( temp_output_68_0_g199 > 0.0 ) ? float4( appendResult85_g199 , 0.0 ) :  float4(0.5,0.5,0,1) ) * 4.0 ) + ( (( temp_output_68_0_g183 > 0.0 ) ? float4( appendResult85_g183 , 0.0 ) :  float4(0.5,0.5,0,1) ) * 3.0 ) + ( (( temp_output_68_0_g203 > 0.0 ) ? float4( appendResult85_g203 , 0.0 ) :  float4(0.5,0.5,0,1) ) * 2.0 ) + (( temp_output_68_0_g191 > 0.0 ) ? float4( appendResult85_g191 , 0.0 ) :  float4(0.5,0.5,0,1) ) + ( (( temp_output_68_0_g207 > 0.0 ) ? float4( appendResult85_g207 , 0.0 ) :  float4(0.5,0.5,0,1) ) * 2.0 ) ) / 23.0 )).xy * 2.0 ) - temp_cast_35 ) * 12.0 * _NormalScale );
				float dotResult42_g91 = dot( temp_output_2_0_g91 , temp_output_2_0_g91 );
				float3 appendResult38_g91 = (float3(temp_output_2_0_g91 , sqrt( ( 1.0 - saturate( dotResult42_g91 ) ) )));
				float3 normalizeResult39_g91 = normalize( appendResult38_g91 );
				float3 normalizeResult125 = normalize( normalizeResult39_g91 );
				float3 temp_output_117_0 = ( ( normalizeResult125 * 0.5 ) + 0.5 );
				float3 break18 = (( _NormalMapPresent )?( ( ( UnpackNormal( tex2D( _BumpMap, i.ase_texcoord.xy ) ) * 0.5 ) + 0.5 ) ):( temp_output_117_0 ));
				float2 uv_MainTex = i.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode41 = tex2D( _MainTex, uv_MainTex );
				float3 hsvTorgb60 = RGBToHSV( tex2DNode41.rgb );
				float temp_output_3_0_g97 = _TransmissionHueLow;
				float temp_output_1_0_g97 = min( temp_output_3_0_g97 , temp_output_3_0_g97 );
				float temp_output_2_0_g97 = max( temp_output_3_0_g97 , _TransmissionHueHigh );
				float temp_output_2_0_g95 = break18.y;
				float temp_output_3_0_g95 = break18.x;
				float4 appendResult16 = (float4(0.0 , ( break18.z * break18.z * _AOStrength ) , ( _TransmissionStrength * (( hsvTorgb60.x >= temp_output_1_0_g97 && hsvTorgb60.x <= temp_output_2_0_g97 ) ? saturate( (( temp_output_2_0_g95 < 0.5 ) ? ( temp_output_2_0_g95 * temp_output_3_0_g95 * 2.0 ) :  ( 1.0 - ( 2.0 * ( 1.0 - temp_output_2_0_g95 ) * ( 1.0 - temp_output_3_0_g95 ) ) ) ) ) :  0.0 ) * tex2DNode41.a ) , ( break18.z * _SmoothnessStrength * tex2DNode41.a )));
				
				
				finalColor = (( _GenerateSurface )?( (( length( _Levels ) > 0.0 ) ? ( _Levels * appendResult16 ) :  float4( (( _NormalMapPresent )?( ( ( UnpackNormal( tex2D( _BumpMap, i.ase_texcoord.xy ) ) * 0.5 ) + 0.5 ) ):( temp_output_117_0 )) , 0.0 ) ) ):( float4( temp_output_117_0 , 0.0 ) ));
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=17500
0;-864;1536;843;1286.731;1169.668;1;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;93;-5030.273,-200.2928;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;5cfe1f95f8d8d4d4397d9520a5bb690a;3060c3af56e28844695839a5a8799a89;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-4688,672;Inherit;False;Constant;_Float7;Float 0;11;0;Create;True;0;0;False;0;0.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;150;-4560,1568;Inherit;False;Constant;_Float14;Float 12;11;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;100;-4384,96;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-4560,1408;Inherit;False;Constant;_Float12;Float 12;11;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;149;-4560,1488;Inherit;False;Constant;_Float13;Float 12;11;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;95;-4384,96;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;151;-4560,1648;Inherit;False;Constant;_Float15;Float 12;11;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-4560,1728;Inherit;False;Constant;_Float16;Float 12;11;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;96;-4384,96;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WireNode;94;-4384,96;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WireNode;97;-4384,96;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;141;-4688,512;Inherit;False;Constant;_Float2;Float 0;11;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;98;-4384,96;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;147;-4688,1008;Inherit;False;Constant;_Float11;Float 0;11;0;Create;True;0;0;False;0;0.8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-4688,592;Inherit;False;Constant;_Float6;Float 0;11;0;Create;True;0;0;False;0;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;154;-4560,1888;Inherit;False;Constant;_Float18;Float 12;11;0;Create;True;0;0;False;0;6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;145;-4688,832;Inherit;False;Constant;_Float9;Float 0;11;0;Create;True;0;0;False;0;0.6;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;146;-4688,928;Inherit;False;Constant;_Float10;Float 0;11;0;Create;True;0;0;False;0;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-4688,752;Inherit;False;Constant;_Float8;Float 0;11;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-4688,432;Inherit;False;Constant;_Float0;Float 0;11;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;82;-4709,137;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;153;-4560,1808;Inherit;False;Constant;_Float17;Float 12;11;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;99;-4384,96;Inherit;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WireNode;157;-3840,1264;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;158;-3840,1328;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;180;-4048,768;Inherit;False;GenerateNormal;1;;203;01ab47aab38620346b2bdb5516212fe4;0;5;6;SAMPLER2D;;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;160;-3840,1456;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;156;-3840,1584;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;161;-3840,1520;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;177;-4048,928;Inherit;False;GenerateNormal;1;;191;01ab47aab38620346b2bdb5516212fe4;0;5;6;SAMPLER2D;;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;181;-4048,1088;Inherit;False;GenerateNormal;1;;207;01ab47aab38620346b2bdb5516212fe4;0;5;6;SAMPLER2D;;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;176;-4048,288;Inherit;False;GenerateNormal;1;;187;01ab47aab38620346b2bdb5516212fe4;0;5;6;SAMPLER2D;;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;178;-4048,128;Inherit;False;GenerateNormal;1;;195;01ab47aab38620346b2bdb5516212fe4;0;5;6;SAMPLER2D;;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;179;-4048,448;Inherit;False;GenerateNormal;1;;199;01ab47aab38620346b2bdb5516212fe4;0;5;6;SAMPLER2D;;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;175;-4048,608;Inherit;False;GenerateNormal;1;;183;01ab47aab38620346b2bdb5516212fe4;0;5;6;SAMPLER2D;;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;159;-3840,1392;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-3664,288;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;5;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-3664,448;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;4;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;108;-3520,960;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-3664,128;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;6;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-3664,1088;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;2;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-3664,768;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;2;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-3664,608;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;107;-3440,128;Inherit;False;7;7;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-3428,401;Inherit;False;Constant;_Float19;Float 0;11;0;Create;True;0;0;False;0;23;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;109;-3264,128;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;23;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;112;-3152,128;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;164;-3152,352;Inherit;False;Constant;_Float21;Float 0;11;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-3152,272;Inherit;False;Constant;_Float22;Float 0;11;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;124;-2960,128;Inherit;False;Times X Minus Y;-1;;98;a6a44ab1efae0f9468db4c6cd6fa074b;0;3;5;FLOAT2;2,0;False;2;FLOAT;2;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-3072,464;Inherit;False;Property;_NormalScale;Normal Scale;5;0;Create;True;0;0;False;0;1;3.823529;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-2989,372;Inherit;False;Constant;_Float20;Float 0;11;0;Create;True;0;0;False;0;12;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-2752,128;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT;12;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;116;-2608,128;Inherit;False;Normal Reconstruct Z (Microsplat);-1;;91;6baaf20447a811e40a787da81b4f4de5;0;1;2;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-2290,264;Inherit;False;Constant;_Float24;Float 0;11;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-4352,-192;Inherit;True;Property;_BumpMap;BumpMap;6;0;Create;True;0;0;False;0;-1;None;f13e968a362e25a4a81aa2abd1f26a5e;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;125;-2304,128;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;166;-2290,344;Inherit;False;Constant;_Float23;Float 0;11;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;117;-2067,124;Inherit;True;Times X Plus Y;-1;;92;ddb4d2db804c78945a1752632d02d0ca;0;3;5;FLOAT3;2,0,0;False;2;FLOAT;0.5;False;4;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;15;-4018,-197;Inherit;True;Normal Pack;-1;;93;505170751a054f248b165a75f7b6efb7;0;1;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;76;-1782.349,-183.5564;Inherit;True;Property;_NormalMapPresent;Normal Map Present;3;0;Create;True;0;0;False;0;0;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;18;-1437.5,-178.2858;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;41;-4464,-792.3465;Inherit;True;Property;_asdf;asdf;0;0;Create;True;0;0;False;0;-1;None;a6e4d8638bc5d664b8ae87acb889e55b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;75;-1544.177,-352.8441;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-720,-928;Inherit;False;Property;_TransmissionHueLow;Transmission Hue Low;9;0;Create;True;0;0;False;0;0.2058824;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;23;-768,-416;Inherit;False;Blend Overlay;-1;;95;72301a50f5a0bcc4a96e9c03db9e571c;0;2;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;73;-1301.348,-891.7399;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-720,-832;Inherit;False;Property;_TransmissionHueHigh;Transmission Hue High;10;0;Create;True;0;0;False;0;0.3109002;0.08350229;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;60;-768,-688;Inherit;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WireNode;74;-544,432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-592,288;Inherit;False;Property;_SmoothnessStrength;Smoothness Strength;11;0;Create;True;0;0;False;0;0.075;0.075;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;24;-560,-416;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;72;-544,-992;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;69;-368,-704;Inherit;False;Safe MinMax;-1;;97;439057af5bb991b41a2af5b820f0984c;0;2;3;FLOAT;0;False;4;FLOAT;0;False;3;FLOAT;0;FLOAT;5;FLOAT2;6
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-208,272;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareWithRange;70;-186,-509;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-96,-752;Inherit;False;Property;_TransmissionStrength;Transmission Strength;8;0;Create;True;0;0;False;0;0.85;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-384,-112;Inherit;False;Property;_AOStrength;AO Strength;7;0;Create;True;0;0;False;0;0.85;0.85;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;71;-144,-656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;240,-544;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;39;288,256;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-80,-240;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;16;560,-256;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector4Node;25;624,-560;Inherit;False;Property;_Levels;Levels;12;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;864,-320;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;77;-160,608;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LengthOpNode;28;865,-446;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;121;268.4507,836.2687;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCCompareGreater;29;1072,-368;Inherit;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ToggleSwitchNode;122;1848.57,-363.3901;Inherit;False;Property;_GenerateSurface;Generate Surface;4;0;Create;True;0;0;False;0;1;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;2563.796,-368;Float;False;True;-1;2;ASEMaterialInspector;100;1;appalachia/core/trees/TextureGenerator;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;0
WireConnection;100;0;93;0
WireConnection;95;0;93;0
WireConnection;96;0;93;0
WireConnection;94;0;93;0
WireConnection;97;0;93;0
WireConnection;98;0;93;0
WireConnection;99;0;93;0
WireConnection;157;0;154;0
WireConnection;158;0;153;0
WireConnection;180;6;98;0
WireConnection;180;1;82;0
WireConnection;180;2;144;0
WireConnection;180;3;143;0
WireConnection;180;4;150;0
WireConnection;160;0;151;0
WireConnection;156;0;150;0
WireConnection;161;0;150;0
WireConnection;177;6;99;0
WireConnection;177;1;82;0
WireConnection;177;2;145;0
WireConnection;177;3;142;0
WireConnection;177;4;149;0
WireConnection;181;6;100;0
WireConnection;181;1;82;0
WireConnection;181;2;146;0
WireConnection;181;3;141;0
WireConnection;181;4;148;0
WireConnection;176;6;95;0
WireConnection;176;1;82;0
WireConnection;176;2;141;0
WireConnection;176;3;146;0
WireConnection;176;4;153;0
WireConnection;178;6;94;0
WireConnection;178;1;82;0
WireConnection;178;2;140;0
WireConnection;178;3;147;0
WireConnection;178;4;154;0
WireConnection;179;6;96;0
WireConnection;179;1;82;0
WireConnection;179;2;142;0
WireConnection;179;3;145;0
WireConnection;179;4;152;0
WireConnection;175;6;97;0
WireConnection;175;1;82;0
WireConnection;175;2;143;0
WireConnection;175;3;144;0
WireConnection;175;4;151;0
WireConnection;159;0;152;0
WireConnection;105;0;176;0
WireConnection;105;1;158;0
WireConnection;104;0;179;0
WireConnection;104;1;159;0
WireConnection;108;0;177;0
WireConnection;106;0;178;0
WireConnection;106;1;157;0
WireConnection;101;0;181;0
WireConnection;101;1;156;0
WireConnection;102;0;180;0
WireConnection;102;1;161;0
WireConnection;103;0;175;0
WireConnection;103;1;160;0
WireConnection;107;0;106;0
WireConnection;107;1;105;0
WireConnection;107;2;104;0
WireConnection;107;3;103;0
WireConnection;107;4;102;0
WireConnection;107;5;108;0
WireConnection;107;6;101;0
WireConnection;109;0;107;0
WireConnection;109;1;162;0
WireConnection;112;0;109;0
WireConnection;124;5;112;0
WireConnection;124;2;165;0
WireConnection;124;4;164;0
WireConnection;114;0;124;0
WireConnection;114;1;163;0
WireConnection;114;2;113;0
WireConnection;116;2;114;0
WireConnection;2;1;82;0
WireConnection;125;0;116;0
WireConnection;117;5;125;0
WireConnection;117;2;167;0
WireConnection;117;4;166;0
WireConnection;15;2;2;0
WireConnection;76;0;117;0
WireConnection;76;1;15;0
WireConnection;18;0;76;0
WireConnection;41;0;93;0
WireConnection;75;0;41;4
WireConnection;23;2;18;1
WireConnection;23;3;18;0
WireConnection;73;0;41;4
WireConnection;60;0;41;0
WireConnection;74;0;75;0
WireConnection;24;0;23;0
WireConnection;72;0;73;0
WireConnection;69;3;34;0
WireConnection;69;4;66;0
WireConnection;19;0;18;2
WireConnection;19;1;21;0
WireConnection;19;2;74;0
WireConnection;70;0;60;1
WireConnection;70;1;69;0
WireConnection;70;2;69;5
WireConnection;70;3;24;0
WireConnection;71;0;72;0
WireConnection;40;0;33;0
WireConnection;40;1;70;0
WireConnection;40;2;71;0
WireConnection;39;0;19;0
WireConnection;17;0;18;2
WireConnection;17;1;18;2
WireConnection;17;2;32;0
WireConnection;16;1;17;0
WireConnection;16;2;40;0
WireConnection;16;3;39;0
WireConnection;26;0;25;0
WireConnection;26;1;16;0
WireConnection;77;0;76;0
WireConnection;28;0;25;0
WireConnection;121;0;117;0
WireConnection;29;0;28;0
WireConnection;29;2;26;0
WireConnection;29;3;77;0
WireConnection;122;0;121;0
WireConnection;122;1;29;0
WireConnection;0;0;122;0
ASEEND*/
//CHKSM=BC62F532EC94F988ECDDDF99BBA9265B4BC27DE0