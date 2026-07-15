Shader "MSMobile/MaxScene/MaxLM_BlendGround_Water" {
	Properties {
		_MainColor ("Main Color", Vector) = (0.5,0.5,0.5,1)
		_Tex1 ("Tex 1", 2D) = "white" {}
		_Tex2 ("Tex 2", 2D) = "white" {}
		_LightMap ("LightMap", 2D) = "white" {}
		_WaterTex ("Tex Water", 2D) = "white" {}
		_Theta ("Theta Radian", Range(0, 7)) = 0
		_SkyColor ("Sky DarkSide Color", Vector) = (1,1,1,1)
		_SunColor ("Sky BrightSide Color", Vector) = (1,1,1,1)
		_WaterOffsetScale ("Water Normal Offset Scale", Float) = 0.2
		_WaterUVVect ("Water UV Tiling: x1 y1 x2 y2", Vector) = (1,1,1,1)
		_UVSpeed ("UV Anim Speed X1 Y1 X2 Y2", Vector) = (1,1,1,1)
		_WaterMask ("Water Mask", 2D) = "white" {}
		_SunDir ("Sun Dir", Vector) = (1,1,1,1)
		_SunPow ("Sky BrightSide Scale", Range(0.1, 5)) = 2
		_ReflectScale ("Reflect Scale", Float) = 0.2
		_ReflectPow ("Reflect Pow", Range(0.1, 10)) = 2
		_MotionScale ("Motion Scale", Range(0, 2)) = 1
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