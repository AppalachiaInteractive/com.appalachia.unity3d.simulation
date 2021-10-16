// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Snapshot Surface Replacement"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_MetallicGlossMap("MetallicGlossMap", 2D) = "white" {}
		_BumpMap("BumpMap", 2D) = "bump" {}
		[HideInInspector][Toggle]_IsBark("Is Bark", Range( 0 , 1)) = 1
		_Glossiness("Glossiness", Range( 0 , 1)) = 0.05
		_Occlusion("Occlusion", Range( 0 , 3)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MetallicGlossMap;
		uniform float4 _MetallicGlossMap_ST;
		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform float _Occlusion;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform half _IsBark;
		uniform float _Glossiness;


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MetallicGlossMap = i.uv_texcoord * _MetallicGlossMap_ST.xy + _MetallicGlossMap_ST.zw;
			float4 break52_g390 = tex2D( _MetallicGlossMap, uv_MetallicGlossMap );
			float maohs_g100_g390 = break52_g390.g;
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			float3 break3_g390 = ( ( UnpackScaleNormal( tex2D( _BumpMap, uv_BumpMap ), 2.0 ) * 0.5 ) + 0.5 );
			float normal_z93_g390 = break3_g390.z;
			float OCCLUSION77_g390 = _Occlusion;
			float temp_output_53_0_g390 = (( maohs_g100_g390 > 0.0001 ) ? maohs_g100_g390 :  ( normal_z93_g390 * normal_z93_g390 * OCCLUSION77_g390 ) );
			float maohs_b98_g390 = break52_g390.b;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode4 = tex2D( _MainTex, uv_MainTex );
			float4 albedo_opacity101_g390 = tex2DNode4;
			float3 hsvTorgb8_g390 = RGBToHSV( albedo_opacity101_g390.rgb );
			float temp_output_3_0_g393 = 0.1941177;
			float temp_output_1_0_g393 = min( temp_output_3_0_g393 , temp_output_3_0_g393 );
			float temp_output_2_0_g393 = max( temp_output_3_0_g393 , 0.4520767 );
			float normal_y92_g390 = break3_g390.y;
			float temp_output_2_0_g391 = normal_y92_g390;
			float normal_x91_g390 = break3_g390.x;
			float temp_output_3_0_g391 = normal_x91_g390;
			float TRANSMISSION73_g390 = 3.0;
			float transmission_calculated109_g390 = ( (albedo_opacity101_g390).a * (( hsvTorgb8_g390.x >= temp_output_1_0_g393 && hsvTorgb8_g390.x <= temp_output_2_0_g393 ) ? saturate( (( temp_output_2_0_g391 < 0.5 ) ? ( temp_output_2_0_g391 * temp_output_3_0_g391 * 2.0 ) :  ( 1.0 - ( 2.0 * ( 1.0 - temp_output_2_0_g391 ) * ( 1.0 - temp_output_3_0_g391 ) ) ) ) ) :  0.0 ) * TRANSMISSION73_g390 );
			float temp_output_85_0_g390 = ( (( maohs_b98_g390 > 0.0001 ) ? maohs_b98_g390 :  transmission_calculated109_g390 ) * ( 1.0 - _IsBark ) );
			float3 appendResult16 = (float3(0.0 , temp_output_53_0_g390 , temp_output_85_0_g390));
			clip( tex2DNode4.a - 0.001);
			o.Emission = appendResult16;
			float maohs_a97_g390 = break52_g390.a;
			float temp_output_81_0_g390 = (albedo_opacity101_g390).a;
			float SMOOTHNESS76_g390 = _Glossiness;
			float temp_output_51_0_g390 = (( maohs_a97_g390 > 0.0001 ) ? ( temp_output_81_0_g390 * maohs_a97_g390 ) :  ( temp_output_81_0_g390 * normal_z93_g390 * SMOOTHNESS76_g390 ) );
			o.Alpha = temp_output_51_0_g390;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
0;-864;1536;843;1841.712;627.0446;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;17;-1415.312,-126.5167;Half;False;Property;_IsBark;Is Bark;4;2;[HideInInspector];[Toggle];Create;True;0;0;True;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;21;-1917.4,67.7;Inherit;True;Property;_BumpMap;BumpMap;2;0;Create;True;0;0;False;0;-1;None;b5d277dcadf18544c87342d4c805a3a0;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;2;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1920,-256;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;-1;None;a4a2b54dd7cc8344eae43cbbb8fc1b05;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;69;-1472.259,-59.1867;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-1347.076,1.507;Inherit;False;Property;_Glossiness;Glossiness;5;0;Create;True;0;0;True;0;0.05;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-1291.737,-209.6345;Inherit;False;Constant;_Transmission;Transmission;5;0;Create;True;0;0;True;0;3;0.5;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;24;-1142.925,131.0448;Inherit;False;Normal Pack;-1;;272;505170751a054f248b165a75f7b6efb7;0;1;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;10;-1478.2,-749.9;Inherit;True;Property;_MetallicGlossMap;MetallicGlossMap;1;0;Create;True;0;0;False;0;-1;None;47a4207b801980844a5a7f90c3777533;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;59;-1094.771,-137.6392;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;58;-1365.508,-292.0742;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-1281.437,-377.2345;Inherit;False;Property;_Occlusion;Occlusion;6;0;Create;True;0;0;True;0;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;68;-1091.359,-16.2867;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;70;-798.5782,-231.4475;Inherit;True;Generate SAOT;-1;;390;f72dfa94cfdbed046bbdf27dde2d2559;0;7;28;COLOR;0.3433473,0.509434,0.1658063,0.509804;False;29;FLOAT3;0.5,0.5,1;False;50;COLOR;0,0,0,0;False;44;FLOAT;0.85;False;45;FLOAT;0.85;False;39;FLOAT;0;False;46;FLOAT;0.1;False;4;FLOAT4;0;FLOAT;47;FLOAT;48;FLOAT;49
Node;AmplifyShaderEditor.DynamicAppendNode;16;-84.56199,-161.9636;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-278.0791,468.9829;Inherit;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;False;0;0.001;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;67;-737.7584,120.2133;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-391.2542,351.5498;Inherit;False;Property;_Cutoff;_Cutoff;3;0;Fetch;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClipNode;65;255.3931,39.80878;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;3;598.3,140;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Snapshot Surface Replacement;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;14;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;16;FLOAT3;0,0,0;False;0
WireConnection;69;0;4;4
WireConnection;24;2;21;0
WireConnection;59;0;17;0
WireConnection;58;0;4;0
WireConnection;68;0;69;0
WireConnection;70;28;58;0
WireConnection;70;29;24;0
WireConnection;70;50;10;0
WireConnection;70;44;48;0
WireConnection;70;45;47;0
WireConnection;70;39;59;0
WireConnection;70;46;36;0
WireConnection;16;1;70;47
WireConnection;16;2;70;48
WireConnection;67;0;68;0
WireConnection;65;0;16;0
WireConnection;65;1;67;0
WireConnection;65;2;66;0
WireConnection;3;2;65;0
WireConnection;3;9;70;49
ASEEND*/
//CHKSM=5A67BF676866FE59B4DCFA0793FEA76F2EF0D032