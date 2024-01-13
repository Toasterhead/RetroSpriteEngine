using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RetroSpriteEngine
{
    public struct STransformation
    {
        public Vector2 origin;
        public float scale;
        public float rotation;
        public SpriteEffects flip;

        public STransformation(Vector2 origin, float scale = 1.0f, float rotation = 0.0f, SpriteEffects flip = SpriteEffects.None)
        {
            this.origin = origin; //To be used relative to an absolute position.
            this.scale = scale;
            this.rotation = rotation;
            this.flip = flip;
        }
    }
}
