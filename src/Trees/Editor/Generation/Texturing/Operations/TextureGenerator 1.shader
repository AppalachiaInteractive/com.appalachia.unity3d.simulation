// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "appalachia/core/trees/TextureGenerator"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
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

			uniform float4 _Levels;
			uniform sampler2D _BumpMap;
			uniform float4 _BumpMap_ST;
			uniform float _AOStrength;
			uniform float _TransmissionStrength;
			uniform sampler2D _MainTex;
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
				float2 uv_BumpMap = i.ase_texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
				float3 temp_output_15_0 = ( ( UnpackNormal( tex2D( _BumpMap, uv_BumpMap ) ) * 0.5 ) + 0.5 );
				float3 break18 = temp_output_15_0;
				float2 uv_MainTex = i.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode41 = tex2D( _MainTex, uv_MainTex );
				float3 hsvTorgb60 = RGBToHSV( tex2DNode41.rgb );
				float temp_output_3_0_g13 = _TransmissionHueLow;
				float temp_output_1_0_g13 = min( temp_output_3_0_g13 , temp_output_3_0_g13 );
				float temp_output_2_0_g13 = max( temp_output_3_0_g13 , _TransmissionHueHigh );
				float temp_output_2_0_g11 = break18.y;
				float temp_output_3_0_g11 = break18.x;
				float4 appendResult16 = (float4(0.0 , ( break18.z * break18.z * _AOStrength ) , ( _TransmissionStrength * (( hsvTorgb60.x >= temp_output_1_0_g13 && hsvTorgb60.x <= temp_output_2_0_g13 ) ? saturate( (( temp_output_2_0_g11 < 0.5 ) ? ( temp_output_2_0_g11 * temp_output_3_0_g11 * 2.0 ) :  ( 1.0 - ( 2.0 * ( 1.0 - temp_output_2_0_g11 ) * ( 1.0 - temp_output_3_0_g11 ) ) ) ) ) :  0.0 ) * tex2DNode41.a ) , ( break18.z * _SmoothnessStrength * tex2DNode41.a )));
				
				
				finalColor = (( length( _Levels ) > 0.0 ) ? ( _Levels * appendResult16 ) :  float4( temp_output_15_0 , 0.0 ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "AppalachiaShaderGUI"
	
	
}
/*ASEBEGIN
Version=17500
0;0;1280;659;1669.476;1256.172;1;True;False
Node;AmplifyShaderEditor.SamplerNode;2;-2304,-256;Inherit;True;Property;_BumpMap;BumpMap;1;0;Create;True;0;0;False;0;-1;None;f13e968a362e25a4a81aa2abd1f26a5e;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;15;-1984,-256;Inherit;False;Normal Pack;-1;;7;505170751a054f248b165a75f7b6efb7;0;1;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;18;-1792,-256;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;41;-2265.001,-545.8001;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;-1;None;a6e4d8638bc5d664b8ae87acb889e55b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;38;-1100.09,170.2235;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-1152,-944;Inherit;False;Property;_TransmissionHueLow;Transmission Hue Low;4;0;Create;True;0;0;False;0;0.2058824;0.2058824;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;23;-1200,-432;Inherit;False;Blend Overlay;-1;;11;72301a50f5a0bcc4a96e9c03db9e571c;0;2;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-1152,-848;Inherit;False;Property;_TransmissionHueHigh;Transmission Hue High;5;0;Create;True;0;0;False;0;0.3109002;0.3109002;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;75;-1544.177,-352.8441;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;73;-1301.348,-891.7399;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;74;-978.1427,417.9767;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;72;-973.5854,-1010.76;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;37;-742.8889,216.1025;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;60;-1195.8,-713.3;Inherit;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;24;-992,-432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;69;-800,-720;Inherit;False;Safe MinMax;-1;;13;439057af5bb991b41a2af5b820f0984c;0;2;3;FLOAT;0;False;4;FLOAT;0;False;3;FLOAT;0;FLOAT;5;FLOAT2;6
Node;AmplifyShaderEditor.RangedFloatNode;21;-1024,272;Inherit;False;Property;_SmoothnessStrength;Smoothness Strength;6;0;Create;True;0;0;False;0;0.075;0.075;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareWithRange;70;-512,-528;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-532.6,-776.4001;Inherit;False;Property;_TransmissionStrength;Transmission Strength;3;0;Create;True;0;0;False;0;0.85;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-640,256;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;71;-570.7484,-670.1794;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-816,-128;Inherit;False;Property;_AOStrength;AO Strength;2;0;Create;True;0;0;False;0;0.85;0.85;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-190,-555;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-512,-256;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;39;-144,240;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;30;-1296,464;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;16;128,-272;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector4Node;25;187.2,-581.7999;Inherit;False;Property;_Levels;Levels;7;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;432,-336;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;31;528,272;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LengthOpNode;28;496,-416;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareGreater;29;640,-384;Inherit;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;896,-384;Float;False;True;-1;2;ASEMaterialInspector;100;1;appalachia/core/trees/TextureGenerator;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;0
WireConnection;15;2;2;0
WireConnection;18;0;15;0
WireConnection;38;0;18;2
WireConnection;23;2;18;1
WireConnection;23;3;18;0
WireConnection;75;0;41;4
WireConnection;73;0;41;4
WireConnection;74;0;75;0
WireConnection;72;0;73;0
WireConnection;37;0;38;0
WireConnection;60;0;41;0
WireConnection;24;0;23;0
WireConnection;69;3;34;0
WireConnection;69;4;66;0
WireConnection;70;0;60;1
WireConnection;70;1;69;0
WireConnection;70;2;69;5
WireConnection;70;3;24;0
WireConnection;19;0;37;0
WireConnection;19;1;21;0
WireConnection;19;2;74;0
WireConnection;71;0;72;0
WireConnection;40;0;33;0
WireConnection;40;1;70;0
WireConnection;40;2;71;0
WireConnection;17;0;18;2
WireConnection;17;1;18;2
WireConnection;17;2;32;0
WireConnection;39;0;19;0
WireConnection;30;0;15;0
WireConnection;16;1;17;0
WireConnection;16;2;40;0
WireConnection;16;3;39;0
WireConnection;26;0;25;0
WireConnection;26;1;16;0
WireConnection;31;0;30;0
WireConnection;28;0;25;0
WireConnection;29;0;28;0
WireConnection;29;2;26;0
WireConnection;29;3;31;0
WireConnection;0;0;29;0
ASEEND*/
//CHKSM=814936FBB2B8225F6BE08A0C6DBAE627E39CAD70