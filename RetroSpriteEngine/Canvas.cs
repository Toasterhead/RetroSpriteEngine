using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using static RetroSpriteEngine.Canvas;
using static System.Net.Mime.MediaTypeNames;

namespace RetroSpriteEngine
{
    public static class Canvas
    {
        public class RenderableCanvas : IRenderable
        {
            public RenderTarget2D TargetCanvas { get; }
            public Vector3 Position { get; }

            public RenderableCanvas(RenderTarget2D targetCanvas, Vector3 position)
            {
                TargetCanvas = targetCanvas;
                Position = position;
            }
        }

        public struct SBuffer
        {
            public RenderTarget2D targetCanvas;
            public RenderTarget2D bufferCanvas;

            public SBuffer(RenderTarget2D targetCanvas, RenderTarget2D bufferCanvas)
            {
                this.targetCanvas = targetCanvas;
                this.bufferCanvas = bufferCanvas;
            }
        }

        public struct SLine
        {
            public int colorIndex;
            public Vector2 start;
            public Vector2 end;

            public SLine(int colorIndex, Vector2 start, Vector2 end)
            {
                this.colorIndex = colorIndex;
                this.start = start;
                this.end = end;
            }
        }

        public static Point SpriteSheetDimensions { get; private set; }

        public static int TileMapWidth { get; private set; } = 40;
        public static int TileMapHeight { get; private set; } = 40;
        public static int TileMapDepth { get; private set; } = 120;

        public static int MaximumVisibleTiles { get; private set; } = 16 * 16; //256

        public static int Multiplier = 4;
        public static Point DimensionsScreen { get; private set; } = new Point(400, 240);
        public static Point DimensionsAction { get; private set; } = new Point(320, 240);
        public static Point DimensionsHud { get; private set; } = new Point(80, 240);
        public static Point PositionAction { get; private set; } = new Point(0, 0);
        public static Point PositionHud { get; private set; } = new Point(320, 0);

        public static int?[,,] TileMap { get; private set; } //Three-dimensional array of indices of active tiles.
        public static Texture2D SpriteSheet { get; private set; } //Analogous to the Pattern Table of an NES.
        public static List<Tile> ActiveTile { get; private set; }

        public static RenderTarget2D[] TileMapLayerCanvas { get; private set; }
        public static RenderTarget2D[] TileCanvas { get; private set; }
        public static Stack<RenderTarget2D> AvailableTileCanvas { get; private set; }

        public static RenderTarget2D CanvasAction { get; private set; }
        public static RenderTarget2D CanvasHUD { get; private set; }
        public static RenderTarget2D CanvasScreen { get; private set; } //For use only in full-screen mode.

        public static int SpriteCanvasSmallCount { get; private set; } = 16;
        public static int SpriteCanvasMediumCount { get; private set; } = 16;
        public static int SpriteCanvasLargeCount { get; private set; } = 4;

        public static RenderTarget2D[] SpriteCanvasSmall { get; private set; }
        public static RenderTarget2D[] SpriteCanvasMedium { get; private set; }
        public static RenderTarget2D[] SpriteCanvasLarge { get; private set; }

        public static Stack<RenderTarget2D> AvailableSpriteCanvasSmall { get; private set; }
        public static Stack<RenderTarget2D> AvailableSpriteCanvasMedium { get; private set; }
        public static Stack<RenderTarget2D> AvailableSpriteCanvasLarge { get; private set; }

        public static List<SBuffer> BufferList { get; private set; } //Consider using a dictionary instead.

        static Canvas()
        {
            //
        }

        public static void Initialize(GraphicsDevice graphicsDevice, Texture2D spriteSheet)
        {
            SetSpriteSheet(spriteSheet);
            Initialize(graphicsDevice);
        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            const int TOTAL_OUTLINE_MARGIN = 2;

            CanvasScreen = CreateCanvas(graphicsDevice, DimensionsScreen.X, DimensionsScreen.Y);
            CanvasAction = CreateCanvas(graphicsDevice, DimensionsAction.X, DimensionsAction.Y);
            CanvasHUD = CreateCanvas(graphicsDevice, DimensionsHud.X, DimensionsHud.Y);

            List<RenderTarget2D> spriteCanvasSmallList = new List<RenderTarget2D>();
            List<RenderTarget2D> spriteCanvasMediumList = new List<RenderTarget2D>();
            List<RenderTarget2D> spriteCanvasLargeList = new List<RenderTarget2D>();

            for (int i = 0; i < SpriteCanvasSmallCount; i++)
                spriteCanvasSmallList.Add(CreateCanvas(graphicsDevice, Tile.Size + TOTAL_OUTLINE_MARGIN, Tile.Size + TOTAL_OUTLINE_MARGIN));
            for (int i = 0; i < SpriteCanvasMediumCount; i++)
                spriteCanvasMediumList.Add(CreateCanvas(graphicsDevice, 2 * Tile.Size + TOTAL_OUTLINE_MARGIN, 2 * Tile.Size + TOTAL_OUTLINE_MARGIN));
            for (int i = 0; i < SpriteCanvasLargeCount; i++)
                spriteCanvasLargeList.Add(CreateCanvas(graphicsDevice, 4 * Tile.Size + TOTAL_OUTLINE_MARGIN, 4 * Tile.Size + TOTAL_OUTLINE_MARGIN));

            SpriteCanvasSmall = spriteCanvasSmallList.ToArray();
            SpriteCanvasMedium = spriteCanvasMediumList.ToArray();
            SpriteCanvasLarge = spriteCanvasLargeList.ToArray();

            AvailableSpriteCanvasSmall = new Stack<RenderTarget2D>(SpriteCanvasSmall);
            AvailableSpriteCanvasMedium = new Stack<RenderTarget2D>(SpriteCanvasMedium);
            AvailableSpriteCanvasLarge = new Stack<RenderTarget2D>(SpriteCanvasLarge);

            TileMap = new int?[TileMapWidth, TileMapHeight, TileMapDepth];

            TileMapLayerCanvas = new RenderTarget2D[TileMapDepth];
            TileCanvas = new RenderTarget2D[MaximumVisibleTiles];
            ActiveTile = new List<Tile>();

            for (int i = 0; i < TileMapDepth; i++)
                TileMapLayerCanvas[i] = CreateCanvas(graphicsDevice, CanvasAction.Width, CanvasAction.Height);
            for (int i = 0; i < MaximumVisibleTiles; i++)
                TileCanvas[i] = CreateCanvas(graphicsDevice, Tile.Size, Tile.Size);

            AvailableTileCanvas = new Stack<RenderTarget2D>(TileCanvas);

            BufferList = new List<SBuffer>();
        }

        private static RenderTarget2D CreateCanvas(GraphicsDevice graphicsDevice, int width, int height)
        {
            return new RenderTarget2D(
                graphicsDevice,
                width,
                height,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }
        
        public static void DisposeAll()
        {
            CanvasAction.Dispose();
            CanvasHUD.Dispose();
            CanvasScreen.Dispose();

            foreach (RenderTarget2D surface in SpriteCanvasSmall) surface.Dispose();
            foreach (RenderTarget2D surface in SpriteCanvasMedium) surface.Dispose();
            foreach (RenderTarget2D surface in SpriteCanvasLarge) surface.Dispose();

            for (int i = 0; i < SpriteCanvasSmall.Length; i++) SpriteCanvasSmall[i].Dispose();
            for (int i = 0; i < SpriteCanvasMedium.Length; i++) SpriteCanvasMedium[i].Dispose();
            for (int i = 0; i < SpriteCanvasLarge.Length; i++) SpriteCanvasLarge[i].Dispose();

            foreach (SBuffer i in BufferList) i.bufferCanvas.Dispose();
        }

        public static void SetSpriteSheet(Texture2D spriteSheet)
        {
            SpriteSheet = spriteSheet;
            SpriteSheetDimensions = new Point(spriteSheet.Width / Tile.Size, spriteSheet.Height / Tile.Size);
        }

        public static void ResetAvailableSpriteCanvases()
        {
            AvailableSpriteCanvasSmall.Clear(); //Is it more efficient to declare new stacks and load them with the collection?
            AvailableSpriteCanvasMedium.Clear();
            AvailableSpriteCanvasLarge.Clear();

            foreach (RenderTarget2D canvas in SpriteCanvasSmall) AvailableSpriteCanvasSmall.Push(canvas);
            foreach (RenderTarget2D canvas in SpriteCanvasMedium) AvailableSpriteCanvasMedium.Push(canvas);
            foreach (RenderTarget2D canvas in SpriteCanvasLarge) AvailableSpriteCanvasLarge.Push(canvas);
        }

        public static void ResetAvailableTileCanvases()
        {
            AvailableTileCanvas.Clear();

            foreach (RenderTarget2D canvas in TileCanvas) AvailableTileCanvas.Push(canvas);
        }

        public static RenderTarget2D GetAvailableSpriteCanvas(Sprite sprite)
        {
            
            Stack<RenderTarget2D> availableCanvas;

            if (MatchesCanvasCollection(sprite, SpriteCanvasSmall))
                availableCanvas = AvailableSpriteCanvasSmall;
            else if (MatchesCanvasCollection(sprite, SpriteCanvasMedium))
                availableCanvas = AvailableSpriteCanvasMedium;
            else if (MatchesCanvasCollection(sprite, SpriteCanvasLarge))
                availableCanvas = AvailableSpriteCanvasLarge;
            else return null;

            if (availableCanvas.Count > 0)

                return availableCanvas.Pop();

            return null;

            //return availableCanvas.Count > 0 ? availableCanvas.Pop() : null; //Why doesn't this work?
        }

        public static RenderTarget2D GetAvailableTileCanvas()
        {
            if (AvailableTileCanvas.Count > 0)

                return AvailableTileCanvas.Pop();

            return null;
        }

        private static bool MatchesCanvasCollection(Sprite sprite, RenderTarget2D[] canvases)
        {
            const int OUTLINE_MARGIN = 1;

            int outlineSize = 2 * OUTLINE_MARGIN;

            return
                canvases != null &&
                canvases.Length > 0 &&
                sprite.FrameSize.X + outlineSize == canvases[0].Width &&
                sprite.FrameSize.Y + outlineSize == canvases[0].Height;
        }

        public static void DrawLines(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Effect drawLine, RenderTarget2D canvas, Queue<SLine> line)

        {
            //Modify SLine to use a color index from the grand palette rather than an arbitrary color later.
            //Should geometric primatives have their own reserved color or palette?
            //Consider requiring lines in collection to be connected, much like a vector display.

            const int CAPACITY = 4;

            SLine[] lineBuffer = new SLine[CAPACITY];

            while (line.Count > 0)
            {
                int count = line.Count < CAPACITY ? line.Count : CAPACITY;

                for (int i = 0; i < count; i++)
                    lineBuffer[i] = line.Dequeue();

                SubDrawLines(graphicsDevice, spriteBatch, drawLine, canvas, lineBuffer);
            }
        }

        private static void SubDrawLines(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Effect drawLine, RenderTarget2D canvas, SLine[] lineBuffer)
        {
            const string PARAMETER_IMAGE_WIDTH = "imageWidth";
            const string PARAMETER_IMAGE_HEIGHT = "imageHeight";
            const string PARAMETER_COLOR = "color";
            const string PARAMETER_X_A = "xA";
            const string PARAMETER_Y_A = "yA";
            const string PARAMETER_X_B = "xB";
            const string PARAMETER_Y_B = "yB";

            drawLine.Parameters[PARAMETER_IMAGE_WIDTH].SetValue((float)canvas.Width);
            drawLine.Parameters[PARAMETER_IMAGE_HEIGHT].SetValue((float)canvas.Height);

            for (int i = 0; i < lineBuffer.Length; i++)
            {
                drawLine.Parameters[PARAMETER_COLOR + i.ToString()].SetValue(Palette.Grand[Palette.Primatives[lineBuffer[i].colorIndex]].ToVector3());
                drawLine.Parameters[PARAMETER_X_A + i.ToString()].SetValue(lineBuffer[i].start.X / canvas.Width);
                drawLine.Parameters[PARAMETER_Y_A + i.ToString()].SetValue(lineBuffer[i].start.Y / canvas.Height);
                drawLine.Parameters[PARAMETER_X_B + i.ToString()].SetValue(lineBuffer[i].end.X / canvas.Width);
                drawLine.Parameters[PARAMETER_Y_B + i.ToString()].SetValue(lineBuffer[i].end.Y / canvas.Height);
            }

            RenderTarget2D bufferCanvas = GetBuffer(graphicsDevice, canvas);

            graphicsDevice.SetRenderTarget(bufferCanvas);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: drawLine);
            spriteBatch.Draw(canvas, new Vector2(), Color.White);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(canvas);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(bufferCanvas, new Vector2(), Color.White);
            spriteBatch.End();
        }

        private static RenderTarget2D GetBuffer(GraphicsDevice graphicsDevice, RenderTarget2D targetCanvas)
        {
            foreach (SBuffer i in BufferList)

                if (i.targetCanvas.Equals(targetCanvas)) return i.bufferCanvas;

            RenderTarget2D bufferCanvas = CreateCanvas(graphicsDevice, targetCanvas.Width, targetCanvas.Height);
            SBuffer buffer = new SBuffer(targetCanvas, bufferCanvas);

            BufferList.Add(buffer);

            return buffer.bufferCanvas;
        }
    }
}