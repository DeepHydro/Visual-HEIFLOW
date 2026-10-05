#version 140

uniform vec4 ColorUniform;
uniform float AlphaMultiplyUniform; 

out vec4 OutColor;

void main()
{
	OutColor = vec4(ColorUniform.xyz, ColorUniform.w * AlphaMultiplyUniform);
}