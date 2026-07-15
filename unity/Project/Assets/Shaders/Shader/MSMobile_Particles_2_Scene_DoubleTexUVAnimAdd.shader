Shader "MSMobile/Particles_2/Scene/DoubleTexUVAnimAdd" {
	Properties {
		_MainTex1 ("Main Tex1", 2D) = "white" {}
		_MainTex2 ("Main Tex2", 2D) = "white" {}
		_ScrollX ("Main Tex1 UVSpeed X", Float) = 1
		_ScrollY ("Main Tex1 UVSpeed Y", Float) = 0
		_Scroll2X ("Main Tex2 UVSpeed X", Float) = 1
		_Scroll2Y ("Main Tex2 UVSpeed Y", Float) = 0
		_Color ("Color", Vector) = (1,1,1,1)
		_UVXX ("Tex2 Addition Offset", Vector) = (0.3,1,1,1)
		_MMultiplier ("Brightness", Float) = 2
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 _Color;

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return _Color; // RGBA
			}

			ENDHLSL
		}
	}
}