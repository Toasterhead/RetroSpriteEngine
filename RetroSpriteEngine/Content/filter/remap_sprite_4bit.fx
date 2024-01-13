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
extern float palette4;
extern float palette5;
extern float palette6;
extern float palette7;
extern float palette8;
extern float palette9;
extern float palette10;
extern float palette11;
extern float palette12;
extern float palette13;
extern float palette14;
extern float palette15;

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
	else if (shade < 4.0 * INTERVAL)
	{
		if (palette3 == -1) isTransparent = true;
		outputShade = palette3 / grandPaletteSize;
	}
	else if (shade < 5.0 * INTERVAL)
	{
		if (palette4 == -1) isTransparent = true;
		outputShade = palette4 / grandPaletteSize;
	}
	else if (shade < 6.0 * INTERVAL)
	{
		if (palette5 == -1) isTransparent = true;
		outputShade = palette5 / grandPaletteSize;
	}
	else if (shade < 7.0 * INTERVAL)
	{
		if (palette6 == -1) isTransparent = true;
		outputShade = palette6 / grandPaletteSize;
	}
	else if (shade < 8.0 * INTERVAL)
	{
		if (palette7 == -1) isTransparent = true;
		outputShade = palette7 / grandPaletteSize;
	}
	else if (shade < 9.0 * INTERVAL)
	{
		if (palette8 == -1) isTransparent = true;
		outputShade = palette8 / grandPaletteSize;
	}
	else if (shade < 10.0 * INTERVAL)
	{
		if (palette9 == -1) isTransparent = true;
		outputShade = palette9 / grandPaletteSize;
	}
	else if (shade < 11.0 * INTERVAL)
	{
		if (palette10 == -1) isTransparent = true;
		outputShade = palette10 / grandPaletteSize;
	}
	else if (shade < 12.0 * INTERVAL)
	{
		if (palette11 == -1) isTransparent = true;
		outputShade = palette11 / grandPaletteSize;
	}
	else if (shade < 13.0 * INTERVAL)
	{
		if (palette12 == -1) isTransparent = true;
		outputShade = palette12 / grandPaletteSize;
	}
	else if (shade < 14.0 * INTERVAL)
	{
		if (palette13 == -1) isTransparent = true;
		outputShade = palette13 / grandPaletteSize;
	}
	else if (shade < 15.0 * INTERVAL)
	{
		if (palette14 == -1) isTransparent = true;
		outputShade = palette14 / grandPaletteSize;
	}
	else
	{
		if (palette15 == -1) isTransparent = true;
		outputShade = palette15 / grandPaletteSize;
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