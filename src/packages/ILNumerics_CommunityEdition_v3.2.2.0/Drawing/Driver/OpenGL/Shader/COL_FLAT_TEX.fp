#version 140

in vec2 fragTexCoord;
flat in vec4 fragColor; 

uniform sampler2D colorTexture;
//uniform vec4 Color; 

out vec4 OutColor;

void main()
{
	vec4 col = texture(colorTexture, fragTexCoord);
	if(col.a < 0.01)
       discard;
    OutColor = vec4(fragColor.rgb, col.a); // vec4(1,0,0,1); // Color * col;
}
