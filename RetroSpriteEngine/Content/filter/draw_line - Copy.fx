#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//Using floating-point rather than integer to improve performance.

extern float3 	color;
extern float 	imageWidth;
extern float 	imageHeight;
extern float 	x1;
extern float 	x2;
extern float 	y1;
extern float 	y2;

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position 			: SV_POSITION;
	float4 Color 				: COLOR0;
	float2 TextureCoordinates 	: TEXCOORD0;
};

float4 DrawLine(VertexShaderOutput input) : COLOR
{
	//Still needs to account for perfectly vertical lines. Modify later.

	float pixelWidth 	= 1.0 / imageWidth;
	float pixelHeight 	= 1.0 / imageHeight;
	
	float4 pixelColor 	= tex2D(SpriteTextureSampler, input.TextureCoordinates);
	float4 lineColor 	= float4(color.r, color.g, color.b, 1.0);
	
	float2 	v 			= float2(x2 - x1, y2 - y1);
	float 	magnitude 	= sqrt((v.x * v.x) + (v.y * v.y));
	float2 	normalized 	= v / magnitude;
	float2 	vPixelized	= float2(normalized.x * pixelWidth, normalized.y * pixelHeight);
	
	for (int i = 0; i < (int)(x2 * imageWidth) - (int)(x1 * imageWidth); i++)
	{
		float2 linePosition = float2(x1, y1) + (i * vPixelized);
		float2 position 	= input.TextureCoordinates;
		
		float coordinateX = linePosition.x - (linePosition.x % pixelWidth);
		float coordinateY = linePosition.y - (linePosition.y % pixelHeight);
		
		if (position.x >= coordinateX 				&& 
			position.x <  coordinateX + pixelWidth 	&&
			position.y >= coordinateY				&&
			position.y <  coordinateY + pixelHeight)
			
			return lineColor;
	}
	
	return pixelColor;
}

technique DrawLineTechnique
{
	pass P0 { PixelShader = compile PS_SHADERMODEL DrawLine(); }
};