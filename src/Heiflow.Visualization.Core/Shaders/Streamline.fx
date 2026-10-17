//-----------------------------------------------------------------------------
// Streamline.fx
//
// A line traced through a flow is drawn as a ribbon, two triangles for every
// piece of it, and every vertex of the ribbon carries three numbers besides its
// colour: how far along the line it lies, counted from the cell the line was
// started in, how fast the flow is there, and which side of the line it is on.
// The colour of the ramp still comes from the vertex, so a line says the same
// thing an arrow did.
//
// The width is put on in the vertex shader and not asked of the card: a vertex
// walks a short way along the direction its line runs in, both ends are put on
// the screen, and the vertex is pushed sideways from that, by half the width,
// in pixels. A line list would have been simpler, but a card is free to ignore
// the width asked of a line and most of them do, so the ribbon is what makes
// the width mean anything.
//
// What moves is not the geometry. One float a frame tells the shader where the
// head of a dash has run to, and it works out from the distance of a vertex how
// far behind that head the vertex is: a dash is brightest at its head, fades
// over the length of its trail and is gone until the next head comes by. Nothing
// is written to a vertex again while the picture moves, which is what a field
// of a hundred thousand vertices needs.
//-----------------------------------------------------------------------------

float4x4 WorldViewProjection;

// How wide and how high the picture is, in pixels, which is what the width of a
// line is counted in.
float2 Viewport;

// How wide a line is, in pixels.
float Width;

// How far along its own direction a vertex is looked for a second time, in the
// units the scene is drawn in.
float TangentOffset;

// Where the head of a dash has run to, counted along a line and folded into the
// distance between two heads, so between 0 and Spacing.
float Head;

// How far apart the heads of two dashes are, in the units the scene is drawn in.
float Spacing;

// How long the trail behind the head of a dash is, in the same units. Where the
// trail ends the dash is gone, and it stays gone until the next head comes by.
float Trail;

// How quickly the light falls off behind the head of a dash. At 1 the trail is
// lit evenly from head to end, a higher number leaves a short bright head and a
// long dim tail behind it.
float TailSharpness;

// The share of the colour the line keeps between two dashes, so that the line a
// dash runs along stays in sight while the dash is somewhere else.
float Floor;

// The share of the colour that is left where the flow is at its slowest, so
// that a fast part of the field stands out against a slow one.
float Slowest;

float Opacity;

struct VertexInput
{
    float3 Position : POSITION;

    // Which way the line runs at this vertex. A vertex of a ribbon sits on one
    // side or the other of the line, and both sides are pushed the same way, so
    // the two pieces of the ribbon that meet at a vertex stay joined.
    float3 Tangent : NORMAL;

    float4 Colour : COLOR0;

    // x is how far along the line the vertex lies, y is how fast the flow is
    // there, between 0 and 1, and z is which side of the line it lies on.
    float3 Flow : TEXCOORD0;
};

struct VertexOutput
{
    float4 Position : POSITION;
    float4 Colour : COLOR0;
    float2 Flow : TEXCOORD0;
};

// Where a vertex lands on the screen, in pixels, with the origin in the middle.
float2 ToScreen(float4 clip)
{
    return clip.xy / max(1e-6, clip.w) * Viewport * 0.5;
}

VertexOutput Transform(VertexInput input)
{
    VertexOutput output;

    float4 here = mul(float4(input.Position, 1.0), WorldViewProjection);
    float4 ahead = mul(float4(input.Position + input.Tangent * TangentOffset, 1.0), WorldViewProjection);

    float2 start = ToScreen(here);
    float2 end = ToScreen(ahead);
    float2 along = end - start;
    float reach = length(along);

    // Sideways across the screen is a quarter turn from the way the line runs.
    // Where the two ends land on the same pixel there is no way for the line to
    // run, and the vertex stays put rather than being pushed somewhere arbitrary.
    float2 across = reach > 1e-6 ? float2(-along.y, along.x) / reach : float2(0.0, 0.0);

    start += across * input.Flow.z * Width * 0.5;

    // Back from pixels to the corner the card clips against.
    output.Position = float4(start / (Viewport * 0.5) * here.w, here.z, here.w);
    output.Colour = input.Colour;
    output.Flow = input.Flow.xy;
    return output;
}

float4 Shade(VertexOutput input) : COLOR0
{
    float travelled = input.Flow.x;
    float speed = saturate(input.Flow.y);

    // How far behind the head of the dash that is passing over it this piece of
    // the line lies. It is 0 at the head and grows along the trail, so the head
    // is the end that runs forward as Head grows and the dim end trails behind.
    float behind = frac((Head - travelled) / Spacing) * Spacing;

    // 1 at the head, down to 0 where the trail has run out, and 0 the rest of
    // the way to the next head, which is the dark stretch between two dashes.
    float lit = saturate(1.0 - behind / max(1.0, Trail));
    float glow = pow(lit, TailSharpness);

    float strength = Floor + (1.0 - Floor) * glow;
    strength *= lerp(Slowest, 1.0, speed);

    float3 colour = input.Colour.rgb * strength;

    // A white tip on the head of a dash, so that the way it runs is plain even
    // where the ramp gives neighbouring cells nearly the same colour.
    colour += pow(glow, 8.0) * speed;

    return float4(colour, Opacity * saturate(strength));
}

technique RenderStreamline
{
    pass P0
    {
        VertexShader = compile vs_2_0 Transform();
        PixelShader = compile ps_2_0 Shade();
    }
}
