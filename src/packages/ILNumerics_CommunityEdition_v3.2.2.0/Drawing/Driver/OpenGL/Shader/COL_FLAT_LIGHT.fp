#version 140

in vec3 FragmentCamPosition; 
in vec3 FragmentNormal; 

out vec4 OutColor;

uniform vec4 ColorUniform;
uniform vec4 SpecularUniform; 
uniform vec4 EmissionUniform; 
uniform float ShininessUniform; 
uniform float AlphaMultiplyUniform; 

// caution! due to a strange, ugly bug in some drivers (eg. acer notebook with GT 330M) 
// the naming of the uniform blocks is important! Do not change them here! 
layout(std140) uniform A1LightParameterExt {
	vec4 AmbientColor; 
	float GlobalAttenuation;
	int NumberOfLights; 
};
struct L {
	vec4 LightPosition; 
	vec3 LightColor; 
	float LightIntensity; 
};
layout(std140) uniform A2LightsArray {
	L Li[20]; 
};

vec4 gamma = vec4(1.0/2.2, 1.0/2.2, 1.0/2.2, 1.0); 

void main()
{
	vec4 accumLight = ColorUniform * AmbientColor;
	//float Shininess = 0.30; 

	float AOI = 0;  
	vec3 normFragmentNormal = normalize(FragmentNormal); 
	vec3 normCam2Fragment = normalize(FragmentCamPosition); 
	vec3 halfAngle; 
	for (int i = NumberOfLights; i --> 0;) {
		// diffuse
		vec3 direction2light = (Li[i].LightPosition).xyz - FragmentCamPosition; 
		float lightDistSqr = dot(direction2light, direction2light); 
		direction2light = direction2light * inversesqrt(lightDistSqr); 
		AOI = dot(direction2light, normFragmentNormal); 
		
		// attenuation
		float att = 1 / (1.0 + lightDistSqr);

		// specular
		halfAngle = normalize(direction2light - normCam2Fragment);
		float expnt = acos(dot(halfAngle, normFragmentNormal)) / ShininessUniform;
		float specAtt = exp(-(expnt * expnt));
		specAtt = (AOI >= 0.0) ? specAtt : 0.0;

		// .. all together 
		AOI= (AOI > 0.001) ? clamp(AOI,0,1) : 0.0; 
		vec4 thisLight = (vec4(Li[i].LightColor,1) * (ColorUniform * AOI * att 
							+ SpecularUniform * specAtt)) * Li[i].LightIntensity // * dot(vec4(Li[i].LightColor,1), ColorUniform);
							+ EmissionUniform; 
		accumLight += thisLight; 
	}
    
	accumLight= pow(accumLight * GlobalAttenuation, gamma); 
	accumLight.w = ColorUniform.w * AlphaMultiplyUniform; 
	OutColor = accumLight; 

}