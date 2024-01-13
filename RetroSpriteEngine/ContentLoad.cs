using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RetroSpriteEngine
{
    public partial class Game1: Game
    {
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            GameContent.Images.EXAMPLE_A = Content.Load<Texture2D>("image/example/block");
            GameContent.Images.EXAMPLE_B = Content.Load<Texture2D>("image/example/monitor");
            GameContent.Images.EXAMPLE_C = Content.Load<Texture2D>("image/example/arrow");
            GameContent.Images.EXAMPLE_D = Content.Load<Texture2D>("image/example/ball");
            GameContent.Images.EXAMPLE_E = Content.Load<Texture2D>("image/example/block_small");
            GameContent.Images.SPRITE_SHEET_A = Content.Load<Texture2D>("image/sprite_sheet_a");

            //GameContent.Filters.EXAMPLE_1 = Content.Load<Effect>("filter/TintShader");
            GameContent.Filters.COLOR_SPRITE = Content.Load<Effect>("filter/color_sprite");
            GameContent.Filters.REMAP_SPRITE_2BIT = Content.Load<Effect>("filter/remap_sprite_2bit");
            GameContent.Filters.REMAP_SPRITE_4BIT = Content.Load<Effect>("filter/remap_sprite_4bit");
            GameContent.Filters.OUTLINE_SPRITE = Content.Load<Effect>("filter/outline_sprite");
            GameContent.Filters.DRAW_LINE = Content.Load<Effect>("filter/draw_line");
        }
    }
}
