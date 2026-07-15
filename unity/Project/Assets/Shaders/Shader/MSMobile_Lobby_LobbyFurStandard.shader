Shader "MSMobile/Lobby/LobbyFurStandard" {
	Properties {
		_TintColor ("_Tint Color", Vector) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_NormalTex ("NormalTex", 2D) = "white" {}
		_FurTex ("FurTex", 2D) = "white" {}
		_FurLength ("FurLength", Range(0, 0.5)) = 0.1
		_BackLight ("BackLight", Range(0, 1)) = 0
		_BackLightColor ("BackLight Color", Vector) = (1,1,1,1)
		_FurInsideLight ("Fur Inside Light", Range(0, 5)) = 1
		_LowQualityLightScale ("Low Quality Light Override", Range(0, 1)) = 1
		_UVScale ("UVScale", Range(0, 20)) = 1
		_UVOffset ("UVOffset", Vector) = (0,0,0,0)
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
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy);
			}

			ENDHLSL
		}
	}
}