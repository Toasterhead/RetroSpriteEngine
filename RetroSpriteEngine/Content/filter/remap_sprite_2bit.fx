#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//Using floating-point rather than integer to improve performance.

extern float grandPaletteSize;
extern float palette0;
extern float palette1;
extern float palette2;
extern float palette3;

Texture2D SpriteTexture;

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

float4 RemapSprite(VertexShaderOutput input) : COLOR
{
	const float TOTAL_SPRITE_COLORS = 4.0;
	const float INTERVAL			= 1.0 / TOTAL_SPRITE_COLORS;
	
	float4 pixelColor = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	float4 outputColor;
	bool isTransparent = false;

	float shade = (pixelColor.r + pixelColor.g + pixelColor.b) / 3.0;
	float outputShade;
	
	if (shade < 1.0 * INTERVAL)
	{
		if (palette0 == -1) isTransparent = true;
		outputShade = palette0 / grandPaletteSize;
	}
	else if (shade < 2.0 * INTERVAL)
	{
		if (palette1 == -1) isTransparent = true;
		outputShade = palette1 / grandPaletteSize;
	}
	else if (shade < 3.0 * INTERVAL)
	{
		if (palette2 == -1) isTransparent = true;
		outputShade = palette2 / grandPaletteSize;
	}
	else 
	{
		if (palette3 == -1) isTransparent = true;
		outputShade = palette3 / grandPaletteSize;
	}
	
	if (isTransparent == true)
		outputColor = float4(0.0, 0.0, 0.0, 0.0);
	else outputColor = float4(outputShade, outputShade, outputShade, 1.0);
	
	return outputColor;
}

technique RemapSpriteTechnique
{
	pass P0 { PixelShader = compile PS_SHADERMODEL RemapSprite(); }
};