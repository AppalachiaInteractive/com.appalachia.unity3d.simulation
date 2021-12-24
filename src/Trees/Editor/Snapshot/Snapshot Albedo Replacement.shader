// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Snapshot Albedo Replacement"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Cutoff("_Cutoff", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma shader_feature _GAMMA_TO_LINEAR
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Cutoff;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode4 = tex2D( _MainTex, uv_MainTex );
			float3 linearToGamma10 = LinearToGammaSpace( tex2DNode4.rgb );
			#ifdef _GAMMA_TO_LINEAR
				float4 staticSwitch7 = float4( linearToGamma10 , 0.0 );
			#else
				float4 staticSwitch7 = tex2DNode4;
			#endif
			clip( tex2DNode4.a - _Cutoff);
			o.Emission = staticSwitch7.rgb;
			o.Alpha = 1;
			float temp_output_12_0 = _Cutoff;
			clip( temp_output_12_0 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "AppalachiaShaderGUI"
}
/*ASEBEGIN
Version=17500
30.66667;54.66667;1280;659;793.5806;428.7877;1.3;True;False
Node;AmplifyShaderEditor.SamplerNode;4;-643,-268;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LinearToGammaNode;10;-289.6666,-152.8056;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;7;-29.16659,-180.5056;Inherit;True;Property;_GAMMA_TO_LINEAR;GAMMA_TO_LINEAR;4;0;Create;True;0;0;False;0;0;0;0;False;;Toggle;2;Key0;Key1;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;14;-189.4806,5.012322;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-220.8115,206.982;Inherit;False;Property;_Cutoff;_Cutoff;1;0;Fetch;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;11;-638,250;Inherit;False;Pivot Billboard;-1;;8;50ed44a1d0e6ecb498e997b8969f8558;3,431,0,432,0,433,0;0;3;FLOAT3;370;FLOAT3;369;FLOAT4;371
Node;AmplifyShaderEditor.ClipNode;13;254.2193,-19.28767;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;3;470.3,8.1;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Snapshot Albedo Replacement;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;12;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;14;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;16;FLOAT3;0,0,0;False;0
WireConnection;10;0;4;0
WireConnection;7;1;4;0
WireConnection;7;0;10;0
WireConnection;14;0;4;4
WireConnection;13;0;7;0
WireConnection;13;1;14;0
WireConnection;13;2;12;0
WireConnection;3;2;13;0
WireConnection;3;10;12;0
ASEEND*/
//CHKSM=9D8689D33ADF5717DE513961EB5EF0FF5F7B22B7