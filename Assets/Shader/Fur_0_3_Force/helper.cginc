sampler2D _MainTex;
sampler2D _DissolveTex;
sampler2D _FurTex;
sampler2D _FurMaskTex;

struct Input
{
    float2 uv_MainTex;
    float2 uv2_FurTex;
};
half _Glossiness;
half _Metallic;
fixed4 _Color;
float _FurLength;
float _FurThinness;
float _FurDensity;
float _FurClip;
float _Dissolve;
UNITY_INSTANCING_BUFFER_START(Props)
UNITY_INSTANCING_BUFFER_END(Props)


void vert(inout appdata_full v)
{
    v.vertex.xyz += v.normal * (_FurLength * FURSTEP);
}
void surf(Input IN, inout SurfaceOutputStandard o)
{
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
    o.Albedo = c.rgb;
    o.Metallic = _Metallic;
    o.Smoothness = _Glossiness;
    
    //毛皮マスク
    float mask = tex2D(_FurMaskTex, IN.uv_MainTex).r;
    clip(mask - 0.5);

    
    
    //毛皮
    fixed3 noise = tex2D(_FurTex, IN.uv2_FurTex * _FurThinness).rgb;
    fixed4 m = tex2D(_FurTex, IN.uv_MainTex);
    float fur = clamp(noise - (FURSTEP * FURSTEP) * _FurDensity, 0, 1);
    o.Alpha = fur;
    fur = fur * (fur - (FURSTEP * FURSTEP) * _FurDensity);
    clip(fur - _FurClip);

	//ディゾルブ
    float dissolve = tex2D(_DissolveTex, IN.uv_MainTex).r;
    dissolve = dissolve * 0.999;
    clip(dissolve - _Dissolve);
}