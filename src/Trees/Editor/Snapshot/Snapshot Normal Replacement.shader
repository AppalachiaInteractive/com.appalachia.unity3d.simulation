// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Snapshot Normal Replacement"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_BumpMap("BumpMap", 2D) = "bump" {}
		_Cutoff("_Cutoff", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			half ASEVFace : VFACE;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Cutoff;


		float3x3 Inverse3x3(float3x3 input)
		{
			float3 a = input._11_21_31;
			float3 b = input._12_22_32;
			float3 c = input._13_23_33;
			return float3x3(cross(b,c), cross(c,a), cross(a,b)) * (1.0 / dot(a,cross(b,c)));
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			float3 temp_output_13_0_g24 = UnpackNormal( tex2D( _BumpMap, uv_BumpMap ) );
			float3 switchResult20_g24 = (((i.ASEVFace>0)?(temp_output_13_0_g24):(-temp_output_13_0_g24)));
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3x3 ase_tangentToWorldPrecise = Inverse3x3( ase_worldToTangent );
			float3 tangentToViewDir22 = normalize( mul( UNITY_MATRIX_V, float4( mul( ase_tangentToWorldPrecise, switchResult20_g24 ), 0 ) ).xyz );
			float3 myVarName32 = ( ( tangentToViewDir22 * 0.5 ) + 0.5 );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			clip( tex2D( _MainTex, uv_MainTex ).a - _Cutoff);
			o.Emission = myVarName32;
			o.Alpha = 1;
			float temp_output_16_0 = _Cutoff;
			clip( temp_output_16_0 - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
0;0;1280;659;-1530.242;-503.8518;1;True;False
Node;AmplifyShaderEditor.SamplerNode;5;-237.0834,287.0987;Inherit;True;Property;_BumpMap;BumpMap;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;37;98.1135,297.7757;Inherit;False;Normal BackFace;-1;;24;121446c878db06f4c847f9c5afed7cfe;0;1;13;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TransformDirectionNode;22;340.7625,294.0731;Inherit;False;Tangent;View;True;Precise;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;8;613.6129,308.6275;Inherit;False;Normal Pack;-1;;25;505170751a054f248b165a75f7b6efb7;0;1;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;857.071,294.8635;Inherit;False;myVarName;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;1975.673,699.0903;Inherit;False;32;myVarName;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;4;1771.939,861.3538;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;16;1905.802,1087.871;Inherit;False;Property;_Cutoff;_Cutoff;2;0;Fetch;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClipNode;25;2297.343,759.3315;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;26;1985.86,782.8845;Inherit;False;Constant;_Float2;Float 1;7;0;Create;True;0;0;False;0;0.001;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;3;2596.645,692.2361;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Snapshot Normal Replacement;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;TransparentCutout;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;16;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;14;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;16;FLOAT3;0,0,0;False;0
WireConnection;37;13;5;0
WireConnection;22;0;37;0
WireConnection;8;2;22;0
WireConnection;32;0;8;0
WireConnection;25;0;33;0
WireConnection;25;1;4;4
WireConnection;25;2;16;0
WireConnection;3;2;25;0
WireConnection;3;10;16;0
ASEEND*/
//CHKSM=7734260BFB37E41651B9C3712785F1CA356B02B3