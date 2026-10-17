// A sheet of triangles that is moved up and down by three waves running across it, and that is lit
// the way water is: mostly what it mirrors of the sky, and one bright spot of the sun on the side of
// every wave that faces it.
//
// The vertices come in as geocentric cartesian, and the two texture coordinates are metres east and
// north of the middle of the water body, so a wave is the same size whatever shape the polygon is
// and wherever on earth it lies.

float4x4 WorldViewProjection;

float3 EyePosition;      // the camera, in the same coordinates as the vertices
float3 SunDirection;     // towards the sun
float3 LocalEast;        // unit vector along the u of the texture coordinates
float3 LocalNorth;       // unit vector along the v of them

float Time;              // seconds, it is what makes the water move
float WaveHeight;        // metres from a trough to a crest
float WaveLength;        // metres between two crests
float WaveSpeed;         // how fast a crest runs
float Opacity;           // 0 see through, 1 solid

float4 ShallowColor;     // colour of a crest
float4 DeepColor;        // colour of a trough
float4 SkyColor;         // what the water mirrors
float4 SunColor;         // the sun on a crest

struct VS_INPUT
{
    float3 Position : POSITION;
    float3 Normal   : NORMAL;
    float2 TexCoord : TEXCOORD0;
};

struct VS_OUTPUT
{
    float4 Position  : POSITION;
    float3 World     : TEXCOORD0;
    float3 Normal    : TEXCOORD1;
    float  Height    : TEXCOORD2;
};

// Three waves of one shape, in three directions and of three sizes. None of them is a round number
// of times the others, which is what keeps the surface from looking like a sheet of corrugated iron.
void Waves(float2 uv, out float height, out float2 slope)
{
    float2 dir0 = normalize(float2( 1.00,  0.15));
    float2 dir1 = normalize(float2(-0.60,  0.80));
    float2 dir2 = normalize(float2( 0.25, -0.95));

    float k = 6.2831853 / max(WaveLength, 1.0);       // radians a metre
    float k0 = k * 1.00;
    float k1 = k * 1.87;
    float k2 = k * 3.31;

    float amp0 = 1.00;
    float amp1 = 0.45;
    float amp2 = 0.22;

    float phase0 = dot(uv, dir0) * k0 + Time * WaveSpeed * 1.00;
    float phase1 = dot(uv, dir1) * k1 + Time * WaveSpeed * 1.35;
    float phase2 = dot(uv, dir2) * k2 + Time * WaveSpeed * 1.75;

    // How high the water is here.
    height = (sin(phase0) * amp0 + sin(phase1) * amp1 + sin(phase2) * amp2) * WaveHeight;

    // Which way it leans here, which is the slope of the three waves taken together.
    slope = (dir0 * (cos(phase0) * amp0 * k0)
           + dir1 * (cos(phase1) * amp1 * k1)
           + dir2 * (cos(phase2) * amp2 * k2)) * WaveHeight;
}

VS_OUTPUT WaterVS(VS_INPUT IN)
{
    VS_OUTPUT OUT;

    float height;
    float2 slope;
    Waves(IN.TexCoord, height, slope);

    // The normal of a vertex points away from the middle of the earth, so the waves are lifted
    // along it and the water stays level however the polygon lies.
    float3 up = normalize(IN.Normal);
    float3 moved = IN.Position + up * height;

    OUT.World = moved;
    OUT.Normal = normalize(up - LocalEast * slope.x - LocalNorth * slope.y);
    OUT.Height = height / max(WaveHeight, 0.0001);
    OUT.Position = mul(float4(moved, 1.0), WorldViewProjection);

    return OUT;
}

float4 WaterPS(VS_OUTPUT IN) : COLOR
{
    float3 normal = normalize(IN.Normal);
    float3 eye = normalize(EyePosition - IN.World);
    float3 sun = normalize(SunDirection);

    // The further the eye is from looking straight down, the more of the sky it sees in the water,
    // which is the one thing that makes a sheet of colour read as water.
    float facing = saturate(dot(normal, eye));
    float fresnel = 0.02 + 0.98 * pow(1.0 - facing, 5.0);

    // A crest is the shallow colour and a trough the deep one.
    float rise = saturate(0.5 + 0.5 * IN.Height);
    float3 body = lerp(DeepColor.rgb, ShallowColor.rgb, rise);

    // One bright spot on the flank of a wave, and nothing on the rest of it.
    float3 halfway = normalize(sun + eye);
    float specular = pow(saturate(dot(normal, halfway)), 90.0);

    float3 color = lerp(body, SkyColor.rgb, fresnel) + SunColor.rgb * specular;

    return float4(color, Opacity);
}

technique Water
{
    pass P0
    {
        VertexShader = compile vs_2_0 WaterVS();
        PixelShader  = compile ps_2_0 WaterPS();
    }
}
