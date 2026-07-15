Shader "MSMobile/BlendGround" {
	Properties {
		_Tex1 ("Tex 1", 2D) = "white" {}
		_Tex2 ("Tex 2", 2D) = "white" {}
		_AlphaMask ("Alpha Mask", 2D) = "white" {}
		_Sharpness ("Alpha Blend Sharpness", Float) = 1.7
		_Tex2BaseAlpha ("Higher Value Makes Tex 2 More Visible", Float) = 0.7
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
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

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
}