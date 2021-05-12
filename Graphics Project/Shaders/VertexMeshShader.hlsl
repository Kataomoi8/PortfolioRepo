//rule of 3
//3 things must match
//1. c++ certex sturct
//2. input layout
//3. hlsl vertex struct


struct InputVertex
{
    float3 xyz : POSITION;
    float3 uvw : TEXCOORD;
    float3 nrm : NORMAL;
    float3 instancePos : INSTANCEPOS;
    //float4 tangent : TANGENT;
};

struct OutputVertex
{
    float4 xyzw : SV_POSITION;
    float3 uvw  : TEXCOORD;
    float3 Norm : NORMAL;
    float3 wPos : WORLDPOS;
    float4 tangent : TANGENT;
};

cbuffer SHADER_VARS : register(b0)
{
    float4x4 worldMatrix;
    float4x4 viewMatrix;
    float4x4 projectionMatrix;
    float4   cameraPosition;
    float4 isInstance;
};

OutputVertex main(InputVertex input)
{

    OutputVertex output = (OutputVertex) 0;
    if (isInstance.x == 1)
    {
        input.xyz += input.instancePos;
    }
    output.xyzw = float4(input.xyz,1);
    output.Norm = input.nrm;
    output.uvw = input.uvw;

    output.xyzw = float4(input.xyz, 1);
    output.Norm = mul(worldMatrix, float4(input.nrm, 0)).xyz;
    output.uvw = input.uvw;
    //do math here
    output.xyzw = mul(worldMatrix, output.xyzw);
    output.wPos = output.xyzw.xyz;
    output.xyzw = mul(viewMatrix, output.xyzw);
    output.xyzw = mul(projectionMatrix, output.xyzw);
    //dont do perspective divide its done automatically
    
    return output;
}