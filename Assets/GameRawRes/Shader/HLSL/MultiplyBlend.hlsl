#ifndef MULTIPLY_BLEND_INCLUDED
#define MULTIPLY_BLEND_INCLUDED

struct Attributes
{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float2 uv : TEXCOORD0;
    float4 positionHCS : SV_POSITION;
};

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
    OUT.uv = IN.uv;
    return OUT;
}

half4 frag(Varyings IN) : SV_Target
{
    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
}

#endif