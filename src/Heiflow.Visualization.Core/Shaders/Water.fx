// A sheet of triangles that is moved up and down by a handful of waves running across it, and that
// is lit the way water is: mostly what it mirrors of the sky, and one bright spot of the sun on the
// side of every wave that faces it.
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
    float3 Surface   : TEXCOORD3;   // metres east and north, and how near the eye the water is
};

// One wave: how far it lifts the water here, and how steeply it leans there. The slope is what the
// light is worked out from, and it grows with the size of the wave, so a short one tilts the water
// more than its height alone would say.
void OneWave(float2 uv, float2 dir, float k, float amp, float phase,
             inout float height, inout float2 slope)
{
    float p = dot(uv, dir) * k + phase;

    height += sin(p) * amp;
    slope  += dir * (cos(p) * amp * k);
}

// Six waves of one shape, in six directions and of six sizes.
//
// They all run within some forty degrees of one another, the way waves under one wind do, and none
// of them is a round number of times any other, which is what keeps the water from coming out in
// rows. Each runs at the speed a wave of its own size runs at, the long ones outdoing the short, so
// the pattern never travels across the sheet as one piece: it is never the same twice.
//
// The amplitudes fall off with size as the sea's do, the long swell carrying most of the height and
// the short waves no more than a ripple of it.
//
// The shortest of them is kept to a third of the length of the longest, for the sheet is cut into
// squares a sixth of that length across and a wave shorter than twice a square has nothing to stand
// on: it comes out as noise rather than as water.
void Waves(float2 uv, out float height, out float2 slope)
{
    height = 0.0;
    slope = float2(0.0, 0.0);

    float k = 6.2831853 / max(WaveLength, 1.0);       // radians a metre

    float2 dir0 = normalize(float2( 1.000,  0.000));
    float2 dir1 = normalize(float2( 0.921,  0.391));
    float2 dir2 = normalize(float2( 0.951, -0.309));
    float2 dir3 = normalize(float2( 0.755,  0.656));
    float2 dir4 = normalize(float2( 0.819, -0.574));
    float2 dir5 = normalize(float2( 0.988,  0.156));

    // A crest of the longest of them runs one wavelength in this many seconds, and the shorter ones
    // run slower by the square root of how much shorter they are, which is how deep water behaves.
    float speed = WaveSpeed * k;

    OneWave(uv, dir0, k * 1.00, 0.55, Time * speed * 1.000 + 0.00, height, slope);
    OneWave(uv, dir1, k * 1.31, 0.42, Time * speed * 0.874 + 1.73, height, slope);
    OneWave(uv, dir2, k * 1.64, 0.31, Time * speed * 0.781 + 3.41, height, slope);
    OneWave(uv, dir3, k * 2.03, 0.21, Time * speed * 0.702 + 5.12, height, slope);
    OneWave(uv, dir4, k * 2.52, 0.13, Time * speed * 0.630 + 2.29, height, slope);
    OneWave(uv, dir5, k * 3.13, 0.07, Time * speed * 0.565 + 4.63, height, slope);

    // A gust passing over the water: the crests stand higher in one place than in another and the
    // patches drift, so the sheet is not the same height from one shore to the other.
    float gust = 0.78 + 0.22 * sin(uv.x * 0.0017 + uv.y * 0.0011 + Time * 0.06);

    height *= WaveHeight * gust;
    slope  *= WaveHeight * gust;
}

VS_OUTPUT WaterVS(VS_INPUT IN)
{
    VS_OUTPUT OUT;

    float2 uv = IN.TexCoord;

    float height;
    float2 slope;
    Waves(uv, height, slope);

    // The normal of a vertex points away from the middle of the earth, so the waves are lifted
    // along it and the water stays level however the polygon lies.
    float3 up = normalize(IN.Normal);
    float3 moved = IN.Position + up * height;

    OUT.World = moved;
    OUT.Normal = normalize(up - LocalEast * slope.x - LocalNorth * slope.y);
    OUT.Height = height / max(WaveHeight, 0.0001);

    // How near the eye this water is, from nothing far off to one close by, and already carrying how
    // tall the ripples stand here. The pixel shader has barely room for them as it is, so it is
    // worked out here, once for a corner, rather than there for every pixel.
    float near = saturate(1.0 - length(EyePosition - moved) / max(WaveLength * 300.0, 1.0));
    OUT.Surface = float3(uv, near * WaveHeight * 0.5);

    OUT.Position = mul(float4(moved, 1.0), WorldViewProjection);

    return OUT;
}

// A wave of the cheapest shape there is: up one side and down the other, over and over. It is what
// the ripples are made of, see WaterPS.
float Chop(float phase)
{
    float turn = frac(phase * 0.15915494);      // turns, nothing to one
    return abs(turn * 4.0 - 2.0) - 1.0;         // up and down, nothing to one either side
}

float4 WaterPS(VS_OUTPUT IN) : COLOR
{
    // The vertex shader sent these out of one length, and the sun comes in of one as well, so none
    // of the three is straightened here: the room to do it in is needed elsewhere.
    float3 normal = IN.Normal;
    float3 eye = normalize(EyePosition - IN.World);
    float3 sun = SunDirection;

    // Ripples: two waves far shorter than any the sheet could carry as height, for the corners of
    // the triangles are metres apart and these are not. They are put into the way the water is lit
    // instead, which is where the sparkle of a real surface comes from. A sheet without them reads
    // as glass however many waves run over it.
    //
    // They are triangles rather than cosines. A card of this age spends some ten instructions on a
    // cosine and four on this, and at the size of a ripple the eye does not know one from the other.
    //
    // They die away in the distance, where one of them would fall between two pixels and shimmer,
    // which is the number the vertex shader worked out.
    float k = 6.2831853 / max(WaveLength, 1.0);
    float speed = Time * WaveSpeed * k;
    float2 uv = IN.Surface.xy;

    float2 fine = float2( 0.944,  0.330) * Chop(dot(uv, float2( 0.944,  0.330)) * k *  7.3 + speed * 2.6) * 0.35
                + float2(-0.447,  0.894) * Chop(dot(uv, float2(-0.447,  0.894)) * k * 13.7 + speed * 3.4) * 0.22;

    normal = normalize(normal - (LocalEast * fine.x + LocalNorth * fine.y) * IN.Surface.z);

    // The further the eye is from looking straight down, the more of the sky it sees in the water,
    // which is the one thing that makes a sheet of colour read as water.
    float facing = saturate(dot(normal, eye));
    float off = 1.0 - facing;
    float off2 = off * off;
    float fresnel = 0.02 + 0.98 * (off2 * off2);

    // A crest is the shallow colour and a trough the deep one.
    float rise = saturate(0.5 + 0.5 * IN.Height);
    float3 body = lerp(DeepColor.rgb, ShallowColor.rgb, rise);

    // One bright spot on the flank of a wave, and nothing on the rest of it. The ripples break it
    // into a field of them, which is what the sun on water looks like.
    //
    // The way between the sun and the eye is halved rather than straightened: both are of one
    // length, so halving is right when they point the same or opposite ways and a little narrow in
    // between, and it costs one instruction where straightening costs four. The power of thirty two
    // is five squarings, which a card of this age does faster than it does a power of its own.
    float shine = saturate(dot(normal, (sun + eye) * 0.5));
    float shine2 = shine * shine;
    float shine4 = shine2 * shine2;
    float shine8 = shine4 * shine4;
    float specular = shine8 * shine8 * shine8;

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
