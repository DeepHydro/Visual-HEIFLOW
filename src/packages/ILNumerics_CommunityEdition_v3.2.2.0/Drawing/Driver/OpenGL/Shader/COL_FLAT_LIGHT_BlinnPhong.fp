#version 140

in vec4 FragmentCamPosition; 
in vec4 FragmentNormal; 

out vec4 OutColor;

uniform vec4 ColorUniform;
// caution! due to a strange and ugly bug in some drivers (eg. acer notbook on GT 330M) 
// the naming of the uniform blocks is important! Do not change them here! 
uniform A1LightParameterExt {
	vec4 AmbientColor; 
	float Attenuation;
	int NumberOfLights; 
};
struct L {
	vec4 Position; 
	vec3 Color; 
	float Intensity; 
};
uniform A2LightsArray {
	L Li[20]; 
};

void main()
{
	vec4 accumLight = vec4(0,0,0,0);
	vec4 SpecularColor = vec4(1,1,1,1); 
	float Shininess = 23; 

	float AOI = 0;  
	vec4 normFragmentNormal = normalize(FragmentNormal); 
	vec4 normCam2Fragment = normalize(FragmentCamPosition); 
	vec4 halfAngle; 
	for (int i = NumberOfLights; i --> 0;) {
		// diffuse term
		vec4 direction2light = Li[i].Position - FragmentCamPosition; 
		float lightDistSqr = dot(direction2light, direction2light); 
		direction2light = direction2light / sqrt(lightDistSqr); 
		AOI = dot(direction2light, normFragmentNormal); 
		AOI = clamp(AOI,0,1); 
		// attenuation
		float att = Li[i].Intensity * (1 / (1.0 + Attenuation * sqrt(lightDistSqr)));
		// specular
		halfAngle = normalize(direction2light - normCam2Fragment);
		float specAtt = dot(normFragmentNormal, halfAngle); 
		specAtt = clamp(specAtt,0,1); 
		specAtt = (AOI != 0.0) ? specAtt : 1;
		specAtt = pow(specAtt,Shininess); 
		// put it all together 
		accumLight += (ColorUniform * AOI * att * normalize(dot(ColorUniform,vec4(Li[i].Color,1))) + SpecularColor * specAtt * 10000) ; 
	}
    OutColor = accumLight + ColorUniform * dot(AmbientColor, ColorUniform); // + ColorUniform * clamp(sqrt(dot(ColorUniform, AmbientColor)),0,0.001); 
}