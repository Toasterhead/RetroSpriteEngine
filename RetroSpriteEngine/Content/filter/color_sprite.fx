#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

extern int 		transparency;
extern float4 	palette0;
extern float4 	palette1;
extern float4 	palette2;
extern float4 	palette3;

//	The texture that we'll be sampling
Texture2D SpriteTexture;

//	The sampler object we'll use to sample the texture
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 ColorSprite(VertexShaderOutput input) : COLOR
{
	//	First we use the tex2D function to get the color of the pixel
	float4 pixelColor = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
	float4 outputColor;
	bool isTransparent = false;

	float shade = (pixelColor.r + pixelColor.g + pixelColor.b) / 3.0;
	
	if (shade < 0.25)
	{
		if (transparency == 0) isTransparent = true;
		outputColor = palette0;
	}
	else if (shade < 0.5)
	{
		if (transparency == 1) isTransparent = true;
		outputColor = palette1;
	}
	else if (shade < 0.75)
	{
		if (transparency == 2) isTransparent = true;
		outputColor = palette2;
	}
	else 
	{
		if (transparency == 3) isTransparent = true;
		outputColor = palette3;
	}
	
	if (isTransparent == true) outputColor = float4(0.0, 0.0, 0.0, 0.0);
	
	return outputColor;
}

//	Here, I've renamed the technique to match what it is actually doing
technique ColorSpriteTechnique
{
	pass P0 { PixelShader = compile PS_SHADERMODEL ColorSprite(); }
};