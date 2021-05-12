cbuffer SHADER_VARS : register(b0)
{
    float4x4 worldMatrix;
    float4x4 viewMatrix;
    float4x4 projectionMatrix;
};

struct VS_INPUT
{
    float3 pos : POSITION;
    float3 uvw : TEXCOORD;
    float3 nrm : NORMAL;
};

struct VS_OUTPUT
{
    float4 pos : SV_POSITION;
    float3 uvw : TEXCOORD;
};



VS_OUTPUT main(VS_INPUT input) 
{
    VS_OUTPUT output;
    float4 pos = float4(input.pos, 1.0f);
    pos = mul(worldMatrix, pos);
    pos = mul(viewMatrix, pos);
    pos = mul(projectionMatrix, pos);
    output.pos = pos;
    output.uvw = input.pos;
	return output;
}