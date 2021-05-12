struct PS_INPUT
{
    float4 pos : SV_POSITION;
    float3 uv : TEXCOORD;
};

TextureCube tex : register(t0);
SamplerState filter : register(s0);

float4 main(PS_INPUT input) : SV_TARGET
{
    return tex.Sample(filter, input.uv);
}