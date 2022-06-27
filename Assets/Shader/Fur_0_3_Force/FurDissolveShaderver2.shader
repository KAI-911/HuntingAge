Shader "Custom/FurDissolveShaderver2"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_DissolveTex("Dissolve", 2D) = "white" {}
		_DitherTex("Dither", 2D) = "white" {}
		_FurTex("Fur", 2D) = "white" {}
		_FurMaskTex("FurMask", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_FurLength("_FurLength", Range(0,1)) = 0.0
		_FurDensity("Fur Density", Range(0, 2)) = 0.11
		_FurThinness("Fur Thinness", Range(0.01, 10)) = 1
		_FurClip("Fur Clip", Range(0.01, 1)) = 0.01
		_Dissolve("Dissolve", Range(0,1)) = 0.0
		_Dither("Dither", Range(0,1)) = 0.0
	}
		SubShader
		{
				Tags { "RenderType" = "Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent" }
				LOD 200

				Cull Off
				ZWrite On


				CGPROGRAM
				#pragma surface surf Standard vertex:vert
				#pragma target 3.0
				sampler2D _MainTex;
				sampler2D _DissolveTex;
				sampler2D _DitherTex;
				struct Input
				{
					float2 uv_MainTex;
				};
				half _Glossiness;
				half _Metallic;
				fixed4 _Color;
				float _Scale;
				float _Dissolve;
				float _Dither;
				UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_INSTANCING_BUFFER_END(Props)
				void vert(inout appdata_full v)
				{
				}
				void surf(Input IN, inout SurfaceOutputStandard o)
				{
					fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
					o.Albedo = c.rgb;
					o.Metallic = _Metallic;
					o.Smoothness = _Glossiness;
					o.Alpha = c.a;

					//ディゾルブ
					float dissolve = tex2D(_DissolveTex, IN.uv_MainTex).r;
					dissolve = dissolve * 0.999;
					clip(dissolve - _Dissolve);

					//ディザ抜き
					float dither = tex2D(_DitherTex, IN.uv_MainTex).r;
					dither = dither * 0.999;
					clip(dither - _Dither);

				}
				ENDCG


				//毛の表示
				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.01
					#include "helper.cginc"
				ENDCG
				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.02
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.03
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.04
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.05
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.06
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.07
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.08
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.09
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.10
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.11
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.12
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.13
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.14
					#include "helper.cginc"
				ENDCG

				CGPROGRAM
					#pragma surface surf Standard vertex:vert
					#pragma target 3.0			
					#define FURSTEP 0.15
					#include "helper.cginc"
				ENDCG


		}

	FallBack "Diffuse"
}
