#include "Macros.fxh"

float4x4 World;
float4x4 View;
float4x4 Projection;
float specularIntensity = 0.8f;
float specularPower = 0.5f; 

DECLARE_TEXTURE(Diffuse, 0) = sampler_state
{
    //Texture = (Diffuse);
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

DECLARE_TEXTURE(SpecularMap, 1) = sampler_state
{
    //Texture = (SpecularMap);
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

DECLARE_TEXTURE(NormalMap, 2) = sampler_state
{
    //Texture = (NormalMap);
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    float3x3 tangentToWorld : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(float4(input.Position.xyz,1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors
    output.tangentToWorld[0] = mul(input.Tangent, World);
    output.tangentToWorld[1] = mul(input.Binormal, World);
    output.tangentToWorld[2] = mul(input.Normal, World);

    return output;
}
struct PixelShaderOutput
{
    half4 Color : COLOR0;
    half4 Normal : COLOR1;
    half4 Depth : COLOR2;
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
    PixelShaderOutput output;
    output.Color = SAMPLE_TEXTURE(Diffuse, input.TexCoord);
    
    float4 specularAttributes = SAMPLE_TEXTURE(SpecularMap, input.TexCoord);
    //specular Intensity
    output.Color.a = specularAttributes.r;
    
    // read the normal from the normal map
    float3 normalFromMap = SAMPLE_TEXTURE(NormalMap, input.TexCoord);
    //tranform to [-1,1]
    normalFromMap = 2.0f * normalFromMap - 1.0f;
	normalFromMap = float3(0,0,1); //if we don't have a normalMap do this!
    //transform into world space
    normalFromMap = mul(normalFromMap, input.tangentToWorld);
    //normalize the result
    normalFromMap = normalize(normalFromMap);
    //output the normal, in [0,1] space
    output.Normal.rgb = 0.5f * (normalFromMap + 1.0f);
	
    //specular Power
    output.Normal.a = specularAttributes.a;

    output.Depth = input.Depth.x / input.Depth.y;
    return output;
}

TECHNIQUE( Standard, VertexShaderFunction, PixelShaderFunction );
