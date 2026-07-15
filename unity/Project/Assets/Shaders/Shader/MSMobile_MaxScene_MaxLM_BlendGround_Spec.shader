Shader "MSMobile/MaxScene/MaxLM_BlendGround_Spec" {
	Properties {
		_MainColor ("Main Color", Vector) = (0.5,0.5,0.5,1)
		_Tex1 ("Tex 1", 2D) = "white" {}
		_Tex2 ("Tex 2", 2D) = "white" {}
		_LightMap ("LightMap", 2D) = "white" {}
		_FakeSpecDir ("Fake Specular Dir", Vector) = (0.9,2,0.85,0)
		_FakeSpecColor ("Fake Spec Color", Vector) = (1,1,1,1)
		_FakeSpecPower ("Fake Spec Power", Range(0, 5)) = 1
		_FakeSpecSharp ("Fake Spec Sharp", Range(0, 1000)) = 100
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