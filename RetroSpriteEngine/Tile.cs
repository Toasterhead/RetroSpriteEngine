using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RetroSpriteEngine
{
    public class Tile : IGameObject
    {
        public static int Size = 8;
        
        public readonly Point SheetDimensions;

        public Texture2D Image { get; protected set; }
        public RenderTarget2D TargetCanvas { get; protected set; }
        public int PaletteIndex { get; protected set; }
        public Point Frame { get; protected set; }
        public uint FrameDelay { get; protected set; }
        public uint FrameDelayTimer { get; protected set; }
        public MetaTile Parent { get; set; }

        public Tile(Texture2D image, uint frameDelay = 5, int paletteIndex = 0)
        {
            if (image.Width % Size != 0)
                throw new Exception("Error - The width of the tile image must be evenly divisible by the tile-size.");
            else if (image.Height % Size != 0)
                throw new Exception("Error - The height of the tile image must be evenly divisible by the tile-size.");

            SheetDimensions = new Point(image.Width / Size,  image.Height / Size);

            Image = image;
            PaletteIndex = paletteIndex;
            FrameDelay = frameDelay;
            Parent = null;
        }

        public Sprite ConvertToSprite(Vector3 Position)
        {
            return new Sprite(Image, Position.X, Position.Y, Position.Z, Size, Size, FrameDelay, PaletteIndex);
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

        public virtual void Update() { Animate();  }

        public virtual void Update(int timestamp) { Animate(timestamp); }

        public virtual bool Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Effect filterPalette)
        {
            TargetCanvas = Canvas.GetAvailableTileCanvas();

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

        private void SubDraw(SpriteBatch spriteBatch, Effect filterPalette)
        {
            const string PARAMETER_GRAND_PALETTE_SIZE = "grandPaletteSize";
            const string PARAMETER_PALETTE = "palette";

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: filterPalette);
            filterPalette.Parameters[PARAMETER_GRAND_PALETTE_SIZE].SetValue((float)Palette.Grand.Length);
            for (int i = 0; i < Palette.PaletteSizeSprite; i++)
                filterPalette.Parameters[PARAMETER_PALETTE + i.ToString()].SetValue((float)Palette.GetTilePalette(PaletteIndex)[i]);
            spriteBatch.Draw(
                Image,
                new Vector2(),
                new Rectangle(Frame.X * Size, Frame.Y * Size, Size, Size),
                Color.White);
            spriteBatch.End();
        }
    }
}
