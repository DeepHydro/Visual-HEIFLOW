#version 140

out vec4 OutColor;

smooth in vec4 FragmentColor; 

void main()
{
    OutColor = FragmentColor;
}