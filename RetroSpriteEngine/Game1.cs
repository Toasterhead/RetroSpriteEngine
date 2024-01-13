using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RetroSpriteEngine
{
    public partial class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        delegate float DetermineRenderingLayer(IRenderable renderable);
        delegate List<IRenderable> CurateTiles(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, List<Tile> activeTile, int?[,,] tileMap, Camera camera);

        private int elapsedTime = 0;//
        private int currentColorIndex = 0;//
        private Camera camera;//
        private Sprite exampleA;//
        private Sprite exampleB;//
        private Sprite exampleC;//

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Canvas.Initialize(GraphicsDevice, GameContent.Images.SPRITE_SHEET_A);

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = Canvas.Multiplier * Canvas.DimensionsScreen.X;
            graphics.PreferredBackBufferHeight = Canvas.Multiplier * Canvas.DimensionsScreen.Y;
            graphics.ApplyChanges();

            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 30.0f); // --- Adjust framerate.

            Palette.Initialize();
            Palette.SetDefaultPaletteA();
            Palette.SetSpritePalette(0, new int[] { -1, 0, 1, 2 });
            Palette.SetSpritePalette(1, new int[] { -1, 3, 4, 5 });
            Palette.SetSpritePalette(2, new int[] { -1, 6, 7, 8 });
            Palette.SetSpritePalette(3, new int[] { -1, 9, 10, 11 });
            Palette.SetTilePalette(0, new int[] { -1, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 });
            Palette.SetPrimativesPalette(new int[] { 4, 8, 12, 16 });

            Canvas.ActiveTile.Add(new Tile(GameContent.Images.EXAMPLE_D));
            Canvas.ActiveTile.Add(new Tile(GameContent.Images.EXAMPLE_E));
            Canvas.TileMap[0, 0, 0] = 0;
            Canvas.TileMap[1, 0, 0] = 1;
            Canvas.TileMap[2, 1, 0] = 0;
            Canvas.TileMap[6, 4, 1] = 1;

            camera = new Camera(new Rectangle(-4 * Tile.Size, -4 * Tile.Size, Canvas.CanvasAction.Width + (8 * Tile.Size), Canvas.CanvasAction.Height + (8 * Tile.Size)));
            exampleA = new Sprite(GameContent.Images.EXAMPLE_A, 50, 50, 0, 16, 16, paletteIndex: 3);//
            exampleB = new Sprite(GameContent.Images.EXAMPLE_B, 100, 100, 0, 16, 16, paletteIndex: 0);//
            exampleC = new Sprite(GameContent.Images.EXAMPLE_C, 120, 100, 0, 16, 16, paletteIndex: 3, outlined: true);//
        }

        protected override void Dispose(bool disposing)
        {
            Canvas.DisposeAll();
            base.Dispose(disposing);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (elapsedTime++ % 30 == 0)
            {
                currentColorIndex++;
                if (currentColorIndex >= Palette.PaletteSizeGrand) currentColorIndex = 0;
            }

            //camera.X = elapsedTime / 10.0f;//
            //camera.Y = elapsedTime / 5.0f;//
            exampleA.Update(elapsedTime);//
            exampleB.Update(elapsedTime);//

            base.Update(gameTime);
        }

        private void DrawViewport(
            GraphicsDevice graphicsDevice,
            SpriteBatch spriteBatch,
            RenderTarget2D canvas,
            Effect filterSpritePalette,
            Effect filterTilePalette,
            Effect filterOutline,
            int outlineColorIndex,
            List<Sprite> spriteSet,
            int?[,,] tileMap,
            Camera camera,
            DetermineRenderingLayer determineLayer,
            CurateTiles curateTiles)
        {
            const int OUTLINE_MARGIN = 1;

            Canvas.ResetAvailableSpriteCanvases();
            Canvas.ResetAvailableTileCanvases();

            foreach (Sprite sprite in spriteSet)

                if (sprite.Outlined)
                    sprite.Draw(graphicsDevice, spriteBatch, filterSpritePalette, filterOutline, outlineColorIndex);
                else sprite.Draw(graphicsDevice, spriteBatch, filterSpritePalette);

            foreach (Tile tile in Canvas.ActiveTile)
                tile.Draw(graphicsDevice, spriteBatch, filterTilePalette);

            List<IRenderable> fullSet = new List<IRenderable>(spriteSet);
            List<IRenderable> tileMapLayer = curateTiles(graphicsDevice, spriteBatch, Canvas.ActiveTile, tileMap, camera); //Result of rendering tile-map layers needs testing.
            fullSet.AddRange(tileMapLayer);

            GraphicsDevice.SetRenderTarget(canvas);
            GraphicsDevice.Clear(Palette.Grand[Palette.BackgroundColorIndex]);

            spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);

            foreach (IRenderable renderable in fullSet)

                if (renderable is Sprite)
                {
                    Sprite sprite = renderable as Sprite;

                    spriteBatch.Draw( //Results of transformation and layering arguments need testing.
                        sprite.TargetCanvas,
                        new Vector2(
                            (sprite.Position.X - OUTLINE_MARGIN) - camera.Position.X,
                            (sprite.Position.Y - OUTLINE_MARGIN) - camera.Position.Y),
                        null,
                        Color.White,
                        sprite.Transformation.rotation,
                        sprite.Transformation.origin, //This is probably being used incorrectly.
                        sprite.Transformation.scale,
                        sprite.Transformation.flip,
                        determineLayer(sprite));
                }
                else spriteBatch.Draw(
                    renderable.TargetCanvas,
                    new Vector2(
                        renderable.Position.X - camera.Position.X,
                        renderable.Position.Y - camera.Position.Y),
                    null,
                    Color.White,
                    0.0f,
                    new Vector2(),
                    1.0f,
                    SpriteEffects.None,
                    determineLayer(renderable));

            spriteBatch.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            Canvas.ResetAvailableSpriteCanvases();

            List<Sprite> spriteSet = new List<Sprite>();
            spriteSet.Add(exampleA);
            spriteSet.Add(exampleB);
            spriteSet.Add(exampleC);

            DrawViewport(
                GraphicsDevice,
                spriteBatch,
                Canvas.CanvasAction,
                GameContent.Filters.REMAP_SPRITE_2BIT,
                GameContent.Filters.REMAP_SPRITE_4BIT,
                GameContent.Filters.OUTLINE_SPRITE,
                Palette.BlackIndex,
                spriteSet,
                Canvas.TileMap,
                camera,
                renderable => renderable.Position.Z / 1000.0f,
                (graphicsDevice, spriteBatch, activeTile, tileMap, camera) => 
                {
                    float zInterval = Tile.Size;

                    List<IRenderable> renderableLayer = new List<IRenderable>();
                    float tileSize = Tile.Size;

                    for (int k = 0; k < Canvas.TileMapDepth; k++)
                    {
                        GraphicsDevice.SetRenderTarget(Canvas.TileMapLayerCanvas[k]);
                        GraphicsDevice.Clear(Color.Transparent);

                        spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

                        for (int i = 0; i < Canvas.TileMapWidth; i++)

                            for (int j = 0; j < Canvas.TileMapHeight; j++)
                            {
                                int? index = tileMap[i, j, k];

                                float renderedPositionX = (i * Tile.Size) - camera.X;
                                float renderedPositionY = (j * Tile.Size) - camera.Y;

                                if (index != null && 
                                    index < activeTile.Count && 
                                    activeTile[(int)index].Image != null &&
                                    renderedPositionX >= camera.Boundary.X &&
                                    renderedPositionY >= camera.Boundary.Y &&
                                    renderedPositionX < camera.Boundary.X + camera.Boundary.Width &&
                                    renderedPositionY < camera.Boundary.Y + camera.Boundary.Height)

                                    spriteBatch.Draw(
                                        activeTile[(int)index].TargetCanvas,
                                        new Vector2(
                                            renderedPositionX,
                                            renderedPositionY),
                                        Color.White);
                            }

                        spriteBatch.End();

                        renderableLayer.Add(new Canvas.RenderableCanvas(Canvas.TileMapLayerCanvas[k], new Vector3(0.0f, 0.0f, k * zInterval)));
                    }

                    return renderableLayer;
                });

            GraphicsDevice.SetRenderTarget(Canvas.CanvasHUD);
            GraphicsDevice.Clear(Palette.AutoBrite(Palette.Grand, 3, 0, true, true)[currentColorIndex]);
            //GraphicsDevice.Clear(Pallette.AutoInvert(Pallette.Grand, 3, 0.5f, 36)[currentColorIndex]);

            Queue<Canvas.SLine> lines = new Queue<Canvas.SLine>();
            const float INTERVAL = 3.0f;
            for (int i  = 0; i < 14; i++)
            {
                float interval = INTERVAL * i;
                lines.Enqueue(
                    new Canvas.SLine(
                        i % Palette.Primatives.Length,
                        new Vector2(0.2f * Canvas.DimensionsAction.X, 0.3f * Canvas.DimensionsAction.Y + interval), 
                        new Vector2(0.75f * Canvas.DimensionsAction.X, 0.6f * Canvas.DimensionsAction.Y + interval)));
            }
            Canvas.DrawLines(GraphicsDevice, spriteBatch, GameContent.Filters.DRAW_LINE, Canvas.CanvasAction, lines);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate,samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(
                Canvas.CanvasAction, 
                new Rectangle(
                    Canvas.Multiplier * Canvas.PositionAction.X,
                    Canvas.Multiplier * Canvas.PositionAction.Y, 
                    Canvas.Multiplier * Canvas.DimensionsAction.X,
                    Canvas.Multiplier * Canvas.DimensionsAction.Y),
                Color.White);
            spriteBatch.Draw(
                Canvas.CanvasHUD,
                new Rectangle(
                    Canvas.Multiplier * Canvas.PositionHud.X,
                    Canvas.Multiplier * Canvas.PositionHud.Y,
                    Canvas.Multiplier * Canvas.DimensionsHud.X,
                    Canvas.Multiplier * Canvas.DimensionsHud.Y),
                Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}