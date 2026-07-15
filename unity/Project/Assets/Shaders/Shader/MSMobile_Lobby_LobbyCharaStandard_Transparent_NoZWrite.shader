Shader "MSMobile/Lobby/LobbyCharaStandard_Transparent_NoZWrite" {
	Properties {
		_Diffuse ("Diffuse", Vector) = (1,1,1,1)
		_MainTex ("MainTex", 2D) = "white" {}
		_NormalTex ("NormalTex", 2D) = "bump" {}
		_PBRTex ("PBRTex R=Rough G=Metal B=AO", 2D) = "white" {}
		_Metal ("Metal", Range(0, 1)) = 0
		_Smooth ("Smooth", Range(0, 1)) = 0
		_AO ("AO", Range(0, 1)) = 0
		_EffectTex ("EffectTex", 2D) = "white" {}
		_SubSurfaceColor ("SubSurface Color", Vector) = (1,1,1,1)
		_SubSurfaceColorScale ("SubSurface Scale", Range(0, 1)) = 0
		_EmitColor ("Emit Color", Vector) = (0,0,0,1)
		_SpecStrength ("Spec Strength", Range(0, 5)) = 0
		_ShadowColor ("ShadowColor", Vector) = (0,0,0,1)
		_Light2Direction ("_Light2Direction", Vector) = (0,0,0,1)
		_Light2Color ("Light2Color", Vector) = (1,1,1,1)
		_EnvTex ("Env Tex", 2D) = "white" {}
		_EnvReflectScale ("Env Reflect Scale", Range(0, 8)) = 0
		_EnvRotY ("Env RotY", Range(0, 1)) = 0
		_BackLightScale ("Back Light Scale", Range(0, 2)) = 0
		_BackLightColor ("Back Light Color", Vector) = (1,1,1,1)
		_FogColor ("FogColor", Vector) = (1,1,1,1)
		_FogMaxDistance ("Fog Max Distance", Float) = 2
		_FogMinDistance ("Fog Min Distance", Float) = 0
		_FogDirection ("FogDirection", Vector) = (0,0,0,1)
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
	Fallback "Specular"
}