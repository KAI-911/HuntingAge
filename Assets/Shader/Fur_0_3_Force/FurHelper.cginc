#pragma target 3.0

#include "Lighting.cginc"
#include "UnityCG.cginc"




struct v2f
{
    float4 pos: SV_POSITION;
    half4 uv: TEXCOORD0;
    half3 normal : TEXCOORD1; //法線
    float3 worldPos: TEXCOORD4;
};

fixed4 _Color;
fixed4 _Specular;
half _Shininess;

sampler2D _MainTex;
half4 _MainTex_ST;
sampler2D _FurTex;
half4 _FurTex_ST;
sampler2D _DisolveTex;
half _Threshold;
sampler2D _NormalTex;
half _BumpScale;
fixed _FurLength;
fixed _FurDensity;
fixed _FurThinness;
fixed _FurShading;

float4 _ForceGlobal;
float4 _ForceLocal;

v2f vert_surface(appdata_base v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

    return o;
}

v2f vert_base(appdata_base v)
{
    v2f o;
    float3 P = v.vertex.xyz + v.normal * _FurLength * FURSTEP;
    P += clamp(mul(unity_WorldToObject, _ForceGlobal).xyz + _ForceLocal.xyz, -1, 1) * pow(FURSTEP, 3) * _FurLength;
    o.pos = UnityObjectToClipPos(float4(P, 1.0));
    o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.uv.zw = TRANSFORM_TEX(v.texcoord, _FurTex);
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

    return o;
}

v2f cutom_vert_surface(appdata_full v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
    
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

    return o;
}

v2f cutom_vert_base(appdata_base v)
{
    v2f o;
    float3 P = v.vertex.xyz + v.normal * _FurLength * FURSTEP;
    P += clamp(mul(unity_WorldToObject, _ForceGlobal).xyz + _ForceLocal.xyz, -1, 1) * pow(FURSTEP, 3) * _FurLength;
    o.pos = UnityObjectToClipPos(float4(P, 1.0));
    o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.uv.zw = TRANSFORM_TEX(v.texcoord, _FurTex);
    o.normal = UnityObjectToWorldNormal(v.normal);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

    return o;
}
fixed4 frag_surface(v2f i): SV_Target
{
    
    fixed3 worldNormal = normalize(i.normal);
    fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
    fixed3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
    fixed3 worldHalf = normalize(worldView + worldLight);

    fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color;
    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
    fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(worldNormal, worldLight));
    fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal, worldHalf)), _Shininess);

    fixed3 color = ambient + diffuse + specular;
    
    return fixed4(color, 1.0);
}

fixed4 frag_base(v2f i): SV_Target
{
    fixed3 worldNormal = normalize(i.normal);
    fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
    fixed3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
    fixed3 worldHalf = normalize(worldView + worldLight);

    fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color;
    albedo -= (pow(1 - FURSTEP, 3)) * _FurShading;

    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
    fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(worldNormal, worldLight));
    fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal, worldHalf)), _Shininess);

    fixed3 color = ambient + diffuse + specular;
    fixed3 noise = tex2D(_FurTex, i.uv.zw * _FurThinness).rgb;
    fixed alpha = clamp(noise - (FURSTEP * FURSTEP) * _FurDensity, 0, 1);
    
    return fixed4(color, alpha);
}
fixed4 cutom_frag_surface(v2f i) : SV_Target
{
    
    fixed3 worldNormal = normalize(i.normal);
    fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
    fixed3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
    fixed3 worldHalf = normalize(worldView + worldLight);
    
    fixed3 m = tex2D(_DisolveTex, i.uv.zw * _FurThinness).rgb;
    fixed alpha = clamp(m - _Threshold * _FurDensity, 0, 1);
    half g = m.r * 0.2 + m.g * 0.7 + m.b * 0.1;
    if (g < _Threshold)
    {
        alpha = 0;
    }
    
    fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color;
    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
    fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(worldNormal, worldLight));
    fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal, worldHalf)), _Shininess);

    fixed3 color = ambient + diffuse + specular;
    
    
    return fixed4(color, alpha);
}

fixed4 cutom_frag_base(v2f i) : SV_Target
{
    fixed3 worldNormal = normalize(i.normal);
    fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
    fixed3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
    fixed3 worldHalf = normalize(worldView + worldLight);

    fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color;
    albedo -= (pow(1 - FURSTEP, 3)) * _FurShading;

    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
    fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(worldNormal, worldLight));
    fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(worldNormal, worldHalf)), _Shininess);

    fixed3 color = ambient + diffuse + specular;
    fixed3 noise = tex2D(_FurTex, i.uv.zw * _FurThinness).rgb;
    fixed alpha = clamp(noise - (FURSTEP * FURSTEP) * _FurDensity, 0, 1);
    
    fixed3 m = tex2D(_DisolveTex, i.uv.zw * _FurThinness).rgb;
    half g = m.r * 0.2 + m.g * 0.7 + m.b * 0.1;
    if (g < _Threshold)
    {
        discard;
    }
    return fixed4(color, alpha);
}