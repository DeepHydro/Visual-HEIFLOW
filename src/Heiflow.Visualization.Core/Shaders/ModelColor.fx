//-----------------------------------------------------------------------------
// ModelColor.fx
//
// The colour of a vertex is no longer carried in the vertex itself. It lives in
// a texture that holds one texel for every vertex of a step, and a second, thin
// texture carries the colours of the ramp. The pixel shader reads the class or
// the shade of the vertex out of the first and looks its colour up in the
// second.
//
// Two steps are kept at once, the one the animation came from and the one it is
// at, and while it is between them both are read and mixed. That is what makes
// the picture move on without a single vertex being written again: a frame costs
// one float, where it used to cost an array as long as the grid.
//-----------------------------------------------------------------------------

float4x4 WorldViewProjection;

// The sun, as the fixed pipeline was asked for it: one directional light over an
// ambient floor. SunLight is 0 while the sun shading is switched off, which
// leaves the ambient alone.
float3 SunDirection;
float SunLight;

// The ambient is a colour and not one number, the fixed pipeline puts a colour on a vertex, and a
// layer that is not lit at all is given white so that the ramp comes out as it was drawn.
float3 Ambient;

float Opacity;

// 0 shows the step the animation came from, 1 the step it is at.
float Blend;

texture StepBefore;
texture StepNow;
texture Ramp;

sampler StepBeforeSampler = sampler_state
{
    Texture = <StepBefore>;
    MipFilter = NONE;
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

sampler StepNowSampler = sampler_state
{
    Texture = <StepNow>;
    MipFilter = NONE;
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

// The same two steps read with an eye to the neighbours. A grid that is laid out in rows and columns
// keeps them next to each other in the texture, so mixing them gives the colour between two vertices
// that really are next to each other, which is what the vertex colours of the fixed pipeline did.
sampler StepBeforeSmooth = sampler_state
{
    Texture = <StepBefore>;
    MipFilter = NONE;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

sampler StepNowSmooth = sampler_state
{
    Texture = <StepNow>;
    MipFilter = NONE;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

sampler RampSampler = sampler_state
{
    Texture = <Ramp>;
    MipFilter = NONE;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

struct VertexInput
{
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float2 TexCoord : TEXCOORD0;
};

struct VertexOutput
{
    float4 Position : POSITION;
    float3 Normal : TEXCOORD0;
    float2 TexCoord : TEXCOORD1;
};

VertexOutput Transform(VertexInput input)
{
    VertexOutput output;
    output.Position = mul(float4(input.Position, 1.0), WorldViewProjection);
    output.Normal = input.Normal;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 Shade(VertexOutput input) : COLOR0
{
    float before = tex2D(StepBeforeSampler, input.TexCoord).r;
    float now = tex2D(StepNowSampler, input.TexCoord).r;
    float level = lerp(before, now, Blend);

    float3 colour = tex2D(RampSampler, float2(level, 0.5)).rgb;

    // The same numbers the fixed pipeline was given, only worked out here: the
    // light of the sun on the normal over the ambient floor.
    float3 light = Ambient + SunLight * saturate(dot(normalize(input.Normal), SunDirection));

    return float4(colour * light, Opacity);
}

float4 ShadeSmooth(VertexOutput input) : COLOR0
{
    float before = tex2D(StepBeforeSmooth, input.TexCoord).r;
    float now = tex2D(StepNowSmooth, input.TexCoord).r;
    float level = lerp(before, now, Blend);

    float3 colour = tex2D(RampSampler, float2(level, 0.5)).rgb;
    float3 light = Ambient + SunLight * saturate(dot(normalize(input.Normal), SunDirection));

    return float4(colour * light, Opacity);
}

// The colour between two vertices of a grid that has no shape, one that is only a list of them, would
// be read from texels that sit next to each other in the strip without being next to each other on
// the grid, so there the nearer texel is taken as it is.
technique RenderModelColor
{
    pass P0
    {
        VertexShader = compile vs_2_0 Transform();
        PixelShader = compile ps_2_0 Shade();
    }
}

technique RenderModelColorSmooth
{
    pass P0
    {
        VertexShader = compile vs_2_0 Transform();
        PixelShader = compile ps_2_0 ShadeSmooth();
    }
}
