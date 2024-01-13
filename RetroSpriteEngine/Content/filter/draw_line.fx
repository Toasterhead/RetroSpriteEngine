#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//Using floating-point rather than integer to improve performance.

extern float 	imageWidth;
extern float 	imageHeight;
extern float	xA0;
extern float	yA0;
extern float	xB0;
extern float	yB0;
extern float3 	color0;
extern float	xA1;
extern float	yA1;
extern float	xB1;
extern float	yB1;
extern float3 	color1;
extern float	xA2;
extern float	yA2;
extern float	xB2;
extern float	yB2;
extern float3 	color2;
extern float	xA3;
extern float	yA3;
extern float	xB3;
extern float	yB3;
extern float3 	color3;


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

bool Intersects(float xA, float yA, float xB, float yB, VertexShaderOutput input)
{
	float pixelWidth 	= 1.0 / imageWidth;
	float pixelHeight 	= 1.0 / imageHeight;
	
	float2 	v 			= float2(xB - xA, yB - yA);
	float 	magnitude 	= sqrt((v.x * v.x) + (v.y * v.y));
	float2 	normalized 	= v / magnitude;
	float2 	vPixelized	= float2(normalized.x * pixelWidth, normalized.y * pixelHeight);
	
	for (int i = 0; i < (int)(xB * imageWidth) - (int)(xA * imageWidth); i++)
	{
		float2 linePosition = float2(xA, yA) + (i * vPixelized);
		float2 position 	= input.TextureCoordinates;
		
		float coordinateX = linePosition.x - (linePosition.x % pixelWidth);
		float coordinateY = linePosition.y - (linePosition.y % pixelHeight);
		
		if (position.x >= coordinateX 				&& 
			position.x <  coordinateX + pixelWidth 	&&
			position.y >= coordinateY				&&
			position.y <  coordinateY + pixelHeight)
			
			return true;
	}
	
	return false;
}

float4 GetLineColor(float3 color) { return float4(color.r, color.g, color.b, 1.0); }

float4 DrawLine(VertexShaderOutput input) : COLOR
{
	//Still needs to account for perfectly vertical lines. Modify later.
	
	float4 pixelColor = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	
	if (		Intersects(xA0, yA0, xB0, yB0, input)) return GetLineColor(color0);
	else if (	Intersects(xA1, yA1, xB1, yB1, input)) return GetLineColor(color1); 
	else if (	Intersects(xA2, yA2, xB2, yB2, input)) return GetLineColor(color2);
	else if (	Intersects(xA3, yA3, xB3, yB3, input)) return GetLineColor(color3);
	
	return pixelColor;
}

technique DrawLineTechnique
{
	pass P0 { PixelShader = compile PS_SHADERMODEL DrawLine(); }
};