//-----------------------------------------------------------------------------
// ModelColorNodes.fx
//
// The colours of a mesh whose vertices are only numbered, one that is not laid
// out in rows and columns. The class or the shade of a vertex travels in a
// texture of its own and is read here in the vertex shader, one texel for the
// vertex itself, and the colour of the ramp that class stands for is found
// there too. What reaches the pixel shader is a colour, and the card spreads it
// over the triangle exactly as it spread the colour of a vertex before the
// textures were brought in.
//
// ModelColor.fx reads the same texture in the pixel shader, from a coordinate
// it found between the corners of a triangle. On a grid of rows and columns
// that is the cell between them, which is what is wanted. Here it is not: the
// vertices are numbered, so between the texel of one of them and the texel of
// the next lie thousands of others that have nothing to do with the triangle,
// and a face comes out filled with stripes of colours that belong all over the
// grid. Reading the texel of the vertex itself is the only place where that
// cannot happen, and it costs three fetches a vertex instead of one a pixel.
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

// The colour of one class and not a blend of two of them, which is the colour the card was handed
// while the classes were still put on the vertices. The colour is mixed over the triangle below,
// so a face still runs from one shade into the next.
sampler RampSampler = sampler_state
{
    Texture = <Ramp>;
    MipFilter = NONE;
    MinFilter = POINT;
    MagFilter = POINT;
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
    float3 Colour : TEXCOORD1;
};

VertexOutput Transform(VertexInput input)
{
    VertexOutput output;
    output.Position = mul(float4(input.Position, 1.0), WorldViewProjection);
    output.Normal = input.Normal;

    // The texel of this vertex and not of the pixel: a coordinate found between two corners of this
    // grid runs over texels that belong anywhere but between them.
    float before = tex2Dlod(StepBeforeSampler, float4(input.TexCoord, 0.0, 0.0)).r;
    float now = tex2Dlod(StepNowSampler, float4(input.TexCoord, 0.0, 0.0)).r;
    float level = lerp(before, now, Blend);

    output.Colour = tex2Dlod(RampSampler, float4(level, 0.5, 0.0, 0.0)).rgb;
    return output;
}

float4 Shade(VertexOutput input) : COLOR0
{
    // The same numbers the fixed pipeline was given, only worked out here: the
    // light of the sun on the normal over the ambient floor.
    float3 light = Ambient + SunLight * saturate(dot(normalize(input.Normal), SunDirection));

    return float4(input.Colour * light, Opacity);
}

// A vertex shader that reads a texture is one of the third version and no earlier one, so a card
// that cannot run it is told so and the layer draws the mesh the way it always did, see
// ModelColorShader.SupportsShaders.
technique RenderModelColorNodes
{
    pass P0
    {
        VertexShader = compile vs_3_0 Transform();
        PixelShader = compile ps_3_0 Shade();
    }
}
