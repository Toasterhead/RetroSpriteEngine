using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

//Modify to use SpriteSheet in static Canvas class rather than local Image. Work on later.

namespace RetroSpriteEngine
{
    public class Sprite : IGameObject, IRenderable
    {
        public readonly Point SheetDimensions;

        public Texture2D Image { get; protected set; }
        public RenderTarget2D TargetCanvas { get; protected set; }
        public STransformation Transformation { get; set; }
        public bool Outlined { get; protected set; }
        public int PaletteIndex { get; protected set; }
        public Point Frame { get; protected set; }
        public Point FrameSize { get; protected set; }
        public uint FrameDelay { get; protected set; }
        public uint FrameDelayTimer { get; protected set; }
        public Vector3 Position { get; protected set; }
        public Sprite Subject { get; protected set; }
        public Sprite Peripheral { get; protected set; }

        public Sprite(
            Texture2D image, 
            float x, 
            float y,
            float z,
            int frameWidth, 
            int frameHeight, 
            uint frameDelay = 5, 
            int paletteIndex = 0,
            bool outlined = false)
        {
            if (frameWidth % Tile.Size != 0)
                throw new Exception("Error - The width of the sprite frame must be evenly divisible by the tile size.");
            else if (frameHeight % Tile.Size != 0)
                throw new Exception("Error - The height of the sprite frame must be evenly divisible by the tile size.");

            if (image.Width % frameWidth != 0)
                throw new Exception("Error - The width of the sprite image must be evenly divisible by the sprite-size.");
            else if (image.Height % frameHeight != 0)
                throw new Exception("Error - The height of the sprite image must be evenly divisible by the sprite-size.");

            SheetDimensions = new Point(image.Width / frameWidth, image.Height / frameHeight);
            Transformation = new STransformation(new Vector2());

            Image = image;
            PaletteIndex = paletteIndex;
            Outlined = outlined;
            FrameSize = new Point(frameWidth, frameHeight);
            FrameDelay = frameDelay;
            Position = new Vector3(x, y, z);
            Subject = null; 
            Peripheral = null;
        }

        public virtual void Animate() { if (++FrameDelayTimer >= FrameDelay) AdvanceFrameX(); }

        public virtual void Animate(int timestamp) { if (timestamp % FrameDelay == 0) AdvanceFrameX(); }

        protected void AdvanceFrameX()
        {
            Frame = new Point(Frame.X + 1, Frame.Y);

            if (Frame.X >= SheetDimensions.X) Frame = new Point(0, Frame.Y);
        }

        protected void AdvanceFrameY()
        {
            Frame = new Point(Frame.X, Frame.Y + 1);

            if (Frame.Y >= SheetDimensions.Y) Frame = new Point(Frame.X, 0);
        }

        public virtual void Update() { Animate(); }

        public virtual void Update(int timestamp) { Animate(timestamp); }

        public virtual bool Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Effect filterPalette)
        {
            TargetCanvas = Canvas.GetAvailableSpriteCanvas(this);

            if (TargetCanvas == null)

                return false;

            else
            {
                graphicsDevice.SetRenderTarget(TargetCanvas);
                graphicsDevice.Clear(Color.Transparent);

                SubDraw(spriteBatch, filterPalette);
            }

            return true;
        }

        public virtual bool Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Effect filterPalette, Effect filterOutline, int? outlineColorIndex = null)
        {
            const string PARAMETER_GRAND_PALETTE_SIZE = "grandPaletteSize";
            const string PARAMETER_COLOR_INDEX = "colorIndex";
            const string PARAMETER_IMAGE_WIDTH = "imageWidth";
            const string PARAMETER_IMAGE_HEIGHT = "imageHeight";

            TargetCanvas = Canvas.GetAvailableSpriteCanvas(this);

            if (TargetCanvas == null)

                return false;

            else
            {
                graphicsDevice.SetRenderTarget(TargetCanvas);
                graphicsDevice.Clear(Color.Transparent);

                SubDraw(spriteBatch, filterPalette);

                spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: filterOutline);
                filterOutline.Parameters[PARAMETER_GRAND_PALETTE_SIZE].SetValue((float)Palette.Grand.Length);
                filterOutline.Parameters[PARAMETER_COLOR_INDEX].SetValue((float)(outlineColorIndex == null ? Palette.BlackIndex : outlineColorIndex));
                filterOutline.Parameters[PARAMETER_IMAGE_WIDTH].SetValue((float)FrameSize.X);
                filterOutline.Parameters[PARAMETER_IMAGE_HEIGHT].SetValue((float)FrameSize.Y);
                spriteBatch.Draw(TargetCanvas, new Vector2(), Color.White);
                spriteBatch.End();
            }

            return true;
        }

        private void SubDraw(SpriteBatch spriteBatch, Effect filterPalette)
        {
            const int OUTLINE_MARGIN = 1;
            const string PARAMETER_GRAND_PALETTE_SIZE = "grandPaletteSize";
            const string PARAMETER_PALETTE = "palette";

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: filterPalette);
            filterPalette.Parameters[PARAMETER_GRAND_PALETTE_SIZE].SetValue((float)Palette.Grand.Length);
            for (int i = 0; i < Palette.PaletteSizeSprite; i++)
                filterPalette.Parameters[PARAMETER_PALETTE + i.ToString()].SetValue((float)Palette.GetSpritePalette(PaletteIndex)[i]);
            spriteBatch.Draw(
                Image, 
                new Vector2(OUTLINE_MARGIN, OUTLINE_MARGIN), 
                new Rectangle(Frame.X * FrameSize.X, Frame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y),
                Color.White);
            spriteBatch.End();
        }
    }
}
