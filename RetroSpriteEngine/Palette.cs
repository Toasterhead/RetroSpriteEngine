using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RetroSpriteEngine
{
    public static class Palette
    {
        private static class ColorComponents
        {
            public enum HSL { Hue = 0, Saturation = 1, Lightness = 2, EnumSize = 3 };
            public enum RGB { Red = 0, Green = 1, Blue = 2, EnumSize = 3 };
        }

        public static int BackgroundColorIndex = 0;
        public static int BlackIndex = 0;
        public static int WhiteIndex = 1;

        public static int PaletteSizeGrand { get; private set; } = 40;
        public static int BitDepthSub { get; private set; } = 4;
        public static int BitDepthTile { get; private set; } = 4;
        public static int BitDepthSprite { get; private set; } = 2;
        public static int BitDepthPrimatives { get; private set; } = 2;
        public static int NumPalettesTile { get; private set; } = 1;
        public static int NumPalettesSprite { get; private set; } = 4;

        public static int PaletteSizeTile { get; private set; }
        public static int PaletteSizeSprite { get; private set; }
        public static int PaletteSizePrimatives { get; private set; }

        public static int[,] Sprite { get; private set; }
        public static  int[,] Tile { get; private set; }
        public static int[] Primatives { get; private set; }
        public static Color[] Grand { get; private set; }

        static Palette()
        {
            //
        }

        public static void Initialize()
        {
            PaletteSizeSprite = (int)Math.Pow(2, BitDepthSprite);
            PaletteSizeTile = (int)Math.Pow(2, BitDepthTile);
            PaletteSizePrimatives = (int)Math.Pow(2, BitDepthPrimatives);

            Sprite = new int[NumPalettesSprite, PaletteSizeSprite];
            Tile = new int[NumPalettesTile, PaletteSizeTile];
            Primatives = new int[PaletteSizePrimatives];

            SetDefaultPaletteA();
        }

        public static void SetDefaultPaletteA() { SetDefaultPalette(3, 20, 180, 60, 60); } //Equidistant hues and brightness.

        public static void SetDefaultPaletteB() { SetDefaultPalette(3, 16, 200, 60, 30); } //Deliberately skips heavily violet hues.

        public static void SetDefaultPaletteC() { SetDefaultPalette(3, 16, 120, 40, 50); } //Deliberately skips heavily violet hues.

        private static void SetDefaultPalette(int hueGroupSize, int hueInterval, int saturation, int darkValue, int brightnessInterval)
        {
            Color[] grandPallette = new Color[PaletteSizeGrand];

            for (int i = 0; i < grandPallette.Length; i++)

                if (i == PaletteSizeGrand - 4)
                    grandPallette[i] = HslToRgb(0, 0, 80);
                else if (i == PaletteSizeGrand - 3)
                    grandPallette[i] = HslToRgb(0, 0, 160);
                else if (i == PaletteSizeGrand - 2)
                    grandPallette[i] = new Color(255, 255, 255);
                else if (i == PaletteSizeGrand - 1)
                    grandPallette[i] = HslToRgb(0, 0, 0);
                else grandPallette[i] = HslToRgb((i / hueGroupSize) * hueInterval, saturation, darkValue + ((i % 3) * brightnessInterval));

            SetGrandPalette(grandPallette);

            BlackIndex = PaletteSizeGrand - 1;
            WhiteIndex = PaletteSizeGrand - 2;
            //BackgroundColorIndex = BlackIndex;
        }

        public static void SetGrandPalette(Color[] colors)
        {
            if (Grand == null) Grand = new Color[PaletteSizeGrand];

            for (int i = 0; i < PaletteSizeGrand && i < colors.Length; i++)
                Grand[i] = colors[i];
        }

        public static int[] GetSpritePalette(int index)
        {
            int[] spritePallette = new int[PaletteSizeSprite];

            for (int i = 0; i < spritePallette.Length; i++)
                spritePallette[i] = Sprite[index, i];

            return spritePallette;
        }

        public static void SetSpritePalette(int index, int[] colorIndices)
        {
            if (colorIndices.Length != PaletteSizeSprite)
                throw new Exception("Error - the sprite palette must have a size of " + PaletteSizeSprite + ".");

            for (int i = 0; i < colorIndices.Length; i++)
                Sprite[index, i] = colorIndices[i];
        }

        public static int[] GetTilePalette(int index)
        {
            int[] tilePallette = new int[PaletteSizeTile];

            for (int i = 0; i < tilePallette.Length; i++)
                tilePallette[i] = Tile[index, i];

            return tilePallette;
        }

        public static void SetTilePalette(int index, int[] colorIndices)
        {
            if (colorIndices.Length != PaletteSizeTile)
                throw new Exception("Error - the tile palette must have a size of " + PaletteSizeTile + ".");

            for (int i = 0; i < colorIndices.Length; i++)
                Tile[index, i] = colorIndices[i];
        }

        public static int GetPrimativeColorIndex(int index) { return Primatives[index]; }

        public static void SetPrimativesPalette(int[] colorIndices) 
        {
            if (colorIndices.Length != PaletteSizePrimatives)
                throw new Exception("Error - the palette for geometric primatives must have a size of " + PaletteSizePrimatives + ".");

            for (int i = 0; i < colorIndices.Length; i++)
                Primatives[i] = colorIndices[i];
        }

        public static Vector3[] CreateVectorizedGrandPallele()
        {
            List<Vector3> vectorized = new List<Vector3>();

            for (int i = 0; i < Grand.Length; i++)
                vectorized.Add(Grand[i].ToVector3());

            return vectorized.ToArray();
        }

        public static Color[] AutoBrite(Color[] grandPalette, int hueGroupSize, int adjustment, bool useBlackIndex = true, bool useWhiteIndex = true)
        {
            //Doesn't properly process black. Fix later.

            Color[] adjustedPalette = new Color[grandPalette.Length];

            for (int i = 0; i < grandPalette.Length / hueGroupSize; i++)

                for (int j = 0; j < hueGroupSize; j++)
                {
                    int initialIndex = (i * hueGroupSize) + j;
                    int adjustedIndex = j + adjustment;

                    if (adjustedIndex < 0 && useBlackIndex)
                        adjustedPalette[initialIndex] = grandPalette[BlackIndex];
                    else if (adjustedIndex >= hueGroupSize && useWhiteIndex)
                        adjustedPalette[initialIndex] = grandPalette[WhiteIndex];
                    else
                    {
                        if (adjustedIndex < 0)
                            adjustedIndex = 0;
                        else if (adjustedIndex >= hueGroupSize)
                            adjustedIndex = hueGroupSize - 1;

                        adjustedIndex += (i * hueGroupSize);
                        adjustedPalette[initialIndex] = grandPalette[adjustedIndex];
                    }
                }

            return adjustedPalette;
        }

        public static Color[] AutoInvert(Color[] grandPalette, int hueGroupSize, float adjustmentRatio, int grayscaleCutoff = -1)
        {
            if (grayscaleCutoff < 0) grayscaleCutoff = grandPalette.Length;

            Color[] adjustedPalette = new Color[grandPalette.Length];

            adjustmentRatio %= 1.0f;
            int hueCount = grayscaleCutoff / hueGroupSize;
            int adjustment = (int)(adjustmentRatio * hueCount);

            for (int i = 0; i < grandPalette.Length; i++)

                if (i < grayscaleCutoff)
                    adjustedPalette[i] = grandPalette[(i + (adjustment * hueGroupSize)) % grayscaleCutoff];
                else adjustedPalette[i] = grandPalette[i];

            return adjustedPalette; 
        }

        public static Color HslToRgb(int h, int s, int l)
        {
            const int DEGREES_PER_ROTATION = 360;
            const int MAX_INPUT_VALUE = 240;
            const int NUM_COLOR_COMPONENTS = (int)ColorComponents.RGB.EnumSize;
            const int H = (int)ColorComponents.HSL.Hue;
            const int S = (int)ColorComponents.HSL.Saturation;
            const int L = (int)ColorComponents.HSL.Lightness;

            if (h < 0) h = MAX_INPUT_VALUE - h;
            if (s < 0) s = MAX_INPUT_VALUE - s;
            if (l < 0) l = MAX_INPUT_VALUE - l;

            h %= MAX_INPUT_VALUE;
            s %= MAX_INPUT_VALUE;
            l %= MAX_INPUT_VALUE;

            float[] color = new float[NUM_COLOR_COMPONENTS] 
            { 
                (h / (float)MAX_INPUT_VALUE) * DEGREES_PER_ROTATION, 
                s / (float)MAX_INPUT_VALUE, 
                l / (float)MAX_INPUT_VALUE 
            };

            float c = (1.0f - Math.Abs((2.0f * color[L]) - 1.0f)) * color[S];
            float x = c * (1.0f - Math.Abs(((color[H] / 60.0f) % 2.0f) - 1.0f));
            float m = color[L] - (c / 2.0f);

            float[] prime;

            if (color[H] >= 0.0f && color[H] < 60.0f) prime = new float[NUM_COLOR_COMPONENTS] { c, x, 0.0f };
            else if (color[H] >= 60.0f && color[H] < 120.0f) prime = new float[NUM_COLOR_COMPONENTS] { x, c, 0.0f };
            else if (color[H] >= 120.0f && color[H] < 180.0f) prime = new float[NUM_COLOR_COMPONENTS] { 0.0f, c, x };
            else if (color[H] >= 180.0f && color[H] < 240.0f) prime = new float[NUM_COLOR_COMPONENTS] { 0.0f, x, c };
            else if (color[H] >= 240.0f && color[H] < 300.0f) prime = new float[NUM_COLOR_COMPONENTS] { x, 0.0f, c };
            else prime = new float[NUM_COLOR_COMPONENTS] { c, 0.0f, x };

            return new Color(
                prime[(int)ColorComponents.RGB.Red] + m, 
                prime[(int)ColorComponents.RGB.Green] + m, 
                prime[(int)ColorComponents.RGB.Blue] + m);
        }
    }
}
