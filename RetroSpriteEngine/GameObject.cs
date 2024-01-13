using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RetroSpriteEngine
{
    public interface IGameObject : IAnimated
    {
        Texture2D Image { get; }
        RenderTarget2D TargetCanvas { get; }
        int PaletteIndex { get; }

        void Update(int timestamp);
        void Update();
        bool Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Effect filterPalette);
    }

    public interface IRenderable : IPositional { RenderTarget2D TargetCanvas { get; } }

    public interface IAnimated
    {
        Point Frame { get; }
        uint FrameDelay { get; }
        uint FrameDelayTimer { get; } //Internal timer for animation. Not to be used when Animate() receives a universal time-stamp value.

        void Animate(int timestamp);
        void Animate();

    }

    public interface IPositional { Vector3 Position { get; } }
}
