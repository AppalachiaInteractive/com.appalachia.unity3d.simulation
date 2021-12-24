// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Snapshot Sample Replacement"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		_MetallicGlossMap("MetallicGlossMap", 2D) = "white" {}
		_BumpMap("BumpMap", 2D) = "white" {}
		[HideInInspector]_Internal_Translucency("Internal_Translucency", Range( 0 , 1)) = 0
		_Glossiness("Glossiness", Float) = 0.1
		_Transmission("Transmission", Float) = 0.1
		_Occlusion("Occlusion", Float) = 0.1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustom keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Transmission;
		};

		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _MetallicGlossMap;
		uniform float4 _MetallicGlossMap_ST;
		uniform float _Glossiness;
		uniform float _Occlusion;
		uniform float _Transmission;
		uniform half _Internal_Translucency;
		uniform float _Cutoff = 0.5;


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			half3 transmission = max(0 , -dot(s.Normal, gi.light.dir)) * gi.light.color * s.Transmission;
			half4 d = half4(s.Albedo * transmission , 0);

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + d;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			float3 tex2DNode21 = UnpackScaleNormal( tex2D( _BumpMap, uv_BumpMap ), 2.0 );
			o.Normal = tex2DNode21;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode4 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = tex2DNode4.rgb;
			float2 uv_MetallicGlossMap = i.uv_texcoord * _MetallicGlossMap_ST.xy + _MetallicGlossMap_ST.zw;
			float4 break52_g377 = tex2D( _MetallicGlossMap, uv_MetallicGlossMap );
			float maohs_a97_g377 = break52_g377.a;
			float4 albedo_opacity101_g377 = tex2DNode4;
			float temp_output_81_0_g377 = (albedo_opacity101_g377).a;
			float3 break3_g377 = ( ( tex2DNode21 * 0.5 ) + 0.5 );
			float normal_z93_g377 = break3_g377.z;
			float SMOOTHNESS76_g377 = _Glossiness;
			float temp_output_51_0_g377 = (( maohs_a97_g377 > 0.0001 ) ? ( temp_output_81_0_g377 * maohs_a97_g377 ) :  ( temp_output_81_0_g377 * normal_z93_g377 * SMOOTHNESS76_g377 ) );
			o.Smoothness = temp_output_51_0_g377;
			float maohs_g100_g377 = break52_g377.g;
			float OCCLUSION77_g377 = _Occlusion;
			float temp_output_53_0_g377 = (( maohs_g100_g377 > 0.0001 ) ? maohs_g100_g377 :  ( normal_z93_g377 * normal_z93_g377 * OCCLUSION77_g377 ) );
			o.Occlusion = temp_output_53_0_g377;
			float maohs_b98_g377 = break52_g377.b;
			float3 hsvTorgb8_g377 = RGBToHSV( albedo_opacity101_g377.rgb );
			float temp_output_3_0_g378 = 0.1941177;
			float temp_output_1_0_g378 = min( temp_output_3_0_g378 , temp_output_3_0_g378 );
			float temp_output_2_0_g378 = max( temp_output_3_0_g378 , 0.4520767 );
			float normal_y92_g377 = break3_g377.y;
			float temp_output_2_0_g379 = normal_y92_g377;
			float normal_x91_g377 = break3_g377.x;
			float temp_output_3_0_g379 = normal_x91_g377;
			float TRANSMISSION73_g377 = _Transmission;
			float transmission_calculated109_g377 = ( (albedo_opacity101_g377).a * (( hsvTorgb8_g377.x >= temp_output_1_0_g378 && hsvTorgb8_g377.x <= temp_output_2_0_g378 ) ? saturate( (( temp_output_2_0_g379 < 0.5 ) ? ( temp_output_2_0_g379 * temp_output_3_0_g379 * 2.0 ) :  ( 1.0 - ( 2.0 * ( 1.0 - temp_output_2_0_g379 ) * ( 1.0 - temp_output_3_0_g379 ) ) ) ) ) :  0.0 ) * TRANSMISSION73_g377 );
			float temp_output_85_0_g377 = ( (( maohs_b98_g377 > 0.0001 ) ? maohs_b98_g377 :  transmission_calculated109_g377 ) * _Internal_Translucency );
			float3 temp_cast_2 = (temp_output_85_0_g377).xxx;
			o.Transmission = temp_cast_2;
			o.Alpha = 1;
			clip( tex2DNode4.a - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "AppalachiaShaderGUI"
}
/*ASEBEGIN
Version=17500
0;-864;1536;843;750.2267;259.3265;1;True;False
Node;AmplifyShaderEditor.SamplerNode;21;-1918.7,-49.3;Inherit;True;Property;_BumpMap;BumpMap;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;2;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1920,-256;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;48;-1263.137,-835.0345;Inherit;False;Property;_Occlusion;Occlusion;7;0;Create;True;0;0;True;0;0.1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-1256.612,-508.5168;Half;False;Property;_Internal_Translucency;Internal_Translucency;4;1;[HideInInspector];Create;True;0;0;True;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-910.7372,-800.4345;Inherit;False;Property;_Transmission;Transmission;6;0;Create;True;0;0;True;0;0.1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-1387.576,-429.893;Inherit;False;Property;_Glossiness;Glossiness;5;0;Create;True;0;0;True;0;0.1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;24;-1153.2,45.70001;Inherit;False;Normal Pack;-1;;371;505170751a054f248b165a75f7b6efb7;0;1;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;10;-1476.9,-749.9;Inherit;True;Property;_MetallicGlossMap;MetallicGlossMap;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewMatrixNode;19;-1931.986,174.3215;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.FunctionNode;56;-1949.498,325.2147;Inherit;False;Pivot Billboard;-1;;373;50ed44a1d0e6ecb498e997b8969f8558;3,431,0,432,0,433,0;0;3;FLOAT3;370;FLOAT3;369;FLOAT4;371
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-1699.985,189.3215;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;54;-479.4908,-493.473;Inherit;True;Generate SAOT;-1;;377;f72dfa94cfdbed046bbdf27dde2d2559;0;7;28;COLOR;0.3433473,0.509434,0.1658063,0.509804;False;29;FLOAT3;0.5,0.5,1;False;50;COLOR;0,0,0,0;False;44;FLOAT;0.85;False;45;FLOAT;0.85;False;39;FLOAT;0;False;46;FLOAT;0.1;False;4;FLOAT4;0;FLOAT;47;FLOAT;48;FLOAT;49
Node;AmplifyShaderEditor.FunctionNode;23;-1448.1,138.6;Inherit;False;Blend Normals (Reorient);-1;;381;e4ff9c08d8f34a64f9089d40151952d2;0;2;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;3;470.3,8.1;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Snapshot Sample Replacement;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;TransparentCutout;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;14;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;16;FLOAT3;0,0,0;False;0
WireConnection;24;2;21;0
WireConnection;22;0;19;0
WireConnection;22;1;56;369
WireConnection;54;28;4;0
WireConnection;54;29;24;0
WireConnection;54;50;10;0
WireConnection;54;44;48;0
WireConnection;54;45;47;0
WireConnection;54;39;17;0
WireConnection;54;46;36;0
WireConnection;23;1;22;0
WireConnection;23;2;21;0
WireConnection;3;0;4;0
WireConnection;3;1;21;0
WireConnection;3;4;54;49
WireConnection;3;5;54;47
WireConnection;3;6;54;48
WireConnection;3;10;4;4
ASEEND*/
//CHKSM=0CED190AB4C76D8CBC018BE4194411F0F80B621A