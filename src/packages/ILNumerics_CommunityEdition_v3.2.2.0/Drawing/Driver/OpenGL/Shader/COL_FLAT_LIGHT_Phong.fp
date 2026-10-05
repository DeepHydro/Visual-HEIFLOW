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
	float Shininess = 4; 

	float fac = 0;  
	for (int i = NumberOfLights; i --> 0;) {
		// diffuse term
		vec4 direction2light = Li[i].Position - FragmentCamPosition; 
		float lightDistSqr = dot(direction2light, direction2light); 
		direction2light = direction2light / sqrt(lightDistSqr); 
		vec4 normFragmentNormal = normalize(FragmentNormal); 
		fac = dot(direction2light, normFragmentNormal); 
		fac = clamp(fac,0,1); 
		// attenuation
		float att = Li[i].Intensity * (1 / (1.0 + Attenuation * lightDistSqr));
		// specular
		vec4 normCam2Fragment = -normalize(FragmentCamPosition); 
		vec4 reflectDir = reflect(direction2light,normFragmentNormal);
		float specAtt = (fac > 0) ? clamp(dot(normCam2Fragment,reflectDir),0,1) : 0;
		specAtt = pow(specAtt,Shininess); 
		// put it all together 
		accumLight += ColorUniform * fac * att + SpecularColor * specAtt * att; 
	}
    OutColor = accumLight  + ColorUniform * dot(AmbientColor, ColorUniform); // + ColorUniform * clamp(sqrt(dot(ColorUniform, AmbientColor)),0,0.001); 
}