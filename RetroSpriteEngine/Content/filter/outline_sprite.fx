#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D s0 = sampler_state
{
	Texture = <SpriteTexture>;
};

//Using floating-point rather than integer to improve performance.

float 	grandPaletteSize;
float 	colorIndex;
float 	imageWidth;
float	imageHeight;

struct VertexShaderOutput
{
	float4 Position 			: SV_POSITION;
	float4 Color 				: COLOR0;
	float2 TextureCoordinates 	: TEXCOORD0;
};

bool PositiveAdjacent(sampler2D s0, float2 origin, float displacementX, float displacementY)
{
	return tex2D(s0, origin + float2(displacementX, displacementY)).a >= 0.5;
}

float4 OutlineSprite(VertexShaderOutput input) : COLOR
{
	float shade = colorIndex / grandPaletteSize;
	float4 pixelColor = tex2D(s0, input.TextureCoordinates);
	float4 outputColorPositive = float4(shade, shade, shade, 1.0);
	float4 outputColorNegative = float4(0.0, 0.0, 0.0, 0.0);
	
	float pixelWidth 	= 1.0 / imageWidth;
	float pixelHeight 	= 1.0 / imageHeight;
	
	if (pixelColor.a < 0.5)
	{
		if (PositiveAdjacent(s0, input.TextureCoordinates, -pixelWidth, -pixelHeight) 	||
			PositiveAdjacent(s0, input.TextureCoordinates,  0,			-pixelHeight) 	||
			PositiveAdjacent(s0, input.TextureCoordinates,  pixelWidth, -pixelHeight) 	||
			PositiveAdjacent(s0, input.TextureCoordinates, -pixelWidth,  0)				||
			PositiveAdjacent(s0, input.TextureCoordinates,  pixelWidth,  0)				||
			PositiveAdjacent(s0, input.TextureCoordinates, -pixelWidth,  pixelHeight)	||
			PositiveAdjacent(s0, input.TextureCoordinates,  0,			 pixelHeight)	||
			PositiveAdjacent(s0, input.TextureCoordinates,  pixelWidth,  pixelHeight))
			
			return outputColorPositive;
			
		//float4 adjacentColor = tex2D(s0, input.TextureCoordinates + float2(-pixelWidth, -pixelHeight));
		//if (adjacentColor.a >= 0.5) return outputColorPositive;
	}
	
	return outputColorNegative;
}

technique OutlineSpriteTechnique
{
	pass P0 { PixelShader = compile PS_SHADERMODEL OutlineSprite(); }
};