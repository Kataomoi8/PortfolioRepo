#pragma pack_matrix(row_major)

cbuffer SHADER_VARS : register(b0)
{
    float4     vLightDir[2];
    float4     vLightColor[2];
    float4     vOutputColor;
    float4     pointRadius[30];
    //float4     pointRange;
    float4     pointPosition[30];
    float4     ambient;
    float4     pLightColor[30];

    float4     spotPosition[2];
    float4     coneDir[2];
    float4     spLightColor[2];
    float4     coneRatio[2];

    matrix     wCamPos;
};
Texture2D mytex : register(t0);
sampler quality : register(s0);



struct OutputVertex
{
    float4 xyzw : SV_POSITION;
    float3 uvw  : TEXCOORD;
    float3 Norm : NORMAL;
    float3 wPos : WORLDPOS;

};

float4 main(OutputVertex inputPixel) : SV_TARGET
{
    inputPixel.Norm = normalize(inputPixel.Norm);
    float4 textureColor = mytex.Sample(quality, inputPixel.uvw.xy);
    float4 finalColor = { 0,0,0,0 };
    float4 finalDirColor;
    float4 finalPointColor;
    //directional light
    float dLightRatio = dot(normalize(-vLightDir[0]),inputPixel.Norm);
    finalDirColor = dLightRatio * vLightColor[0] * textureColor;
    finalColor = saturate(ambient*textureColor);
    


    //point light

    for (int i = 0; i < 30; i++)
    {
        float3 plightdir = normalize(pointPosition[i] - inputPixel.wPos);
        float pLightRatio = dot(plightdir, inputPixel.Norm);
        finalPointColor = pLightRatio * pLightColor[i] * textureColor;
        float attenuation = 1.0f - saturate(length(pointPosition[i] - inputPixel.wPos) / pointRadius[i].x);
        finalPointColor *= attenuation;
        finalColor += finalPointColor;
    }


    //spot light
    float3 spotDirection;
    spotDirection = normalize(spotPosition[0] - inputPixel.wPos);
    float  surfaceRatio;
    surfaceRatio = saturate(dot(-spotDirection, normalize(coneDir[0])));
    float fallOffCone;
    fallOffCone = 1.0f - saturate((coneRatio[0].x - surfaceRatio) / (coneRatio[0].x - coneRatio[0].y));
    float spAttenuation;
    spAttenuation = 1.0f - saturate(length(spotPosition[0] - inputPixel.wPos) / coneRatio[0].z);
    float spotFactor;
    if (surfaceRatio > coneRatio[0].y)
    {
        spotFactor = 1;
    }
    else
    {
        spotFactor = 0;
    }
    float spLightRatio;
    spLightRatio = saturate(dot(spotDirection, inputPixel.Norm));
    float4 finalSpotColor;
    finalSpotColor = spotFactor * spLightRatio * spLightColor[1] * textureColor;
    finalSpotColor *= spAttenuation;
    finalSpotColor *= fallOffCone;




    //specular
   float4 specular;
   float4 lightDir;
   float3 reflection;
   float3 halfVec;
   float3 viewDirection;
   float  lightIntensity;
   float x;
   viewDirection = normalize((wCamPos[3] - inputPixel.wPos));
   specular = float4(0.0f, 0.0f, 0.0f, 0.0f);
   lightDir = normalize(-vLightDir[0]);
   halfVec = normalize(lightDir + viewDirection);
   lightIntensity = saturate((pow(dot(inputPixel.Norm, halfVec),512)));
   specular = vLightColor[0] * 0.4f * lightIntensity;
   finalDirColor += specular;


   finalColor += finalDirColor;
   return finalColor;
}