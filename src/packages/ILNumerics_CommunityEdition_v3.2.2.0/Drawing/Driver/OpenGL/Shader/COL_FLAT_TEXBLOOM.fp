#version 140

in vec2 fragTexCoord;
flat in vec4 fragColor; 

uniform sampler2D ColorTextureSampler;
uniform int FringeSizeUniform; 
uniform vec3 FringeColorUniform;

out vec4 OutColor;

void main()
{
	if (FringeSizeUniform > 0) {
		
		vec4 col = vec4(0,0,0,0); 
		for (int c = -FringeSizeUniform; c <= FringeSizeUniform; c++) {
			for (int r = -FringeSizeUniform; r <= FringeSizeUniform; r++) {
				col += texture(ColorTextureSampler, vec2(fragTexCoord.x + r / 1024.0, fragTexCoord.y + c / 1024.0));
			}
		}
		col.a /= (FringeSizeUniform * FringeSizeUniform);
		 
		//if (col.a < 0.001) 
			//discard;
		//
		OutColor = vec4(FringeColorUniform, clamp(col.a,0,1));

	} else {
		vec4 col = texture(ColorTextureSampler, fragTexCoord); 
		if (col.a < 0.01) discard;

		OutColor = vec4(fragColor.rgb, col.a); // vec4(1,0,0,1); // Color * col;
	}
}
