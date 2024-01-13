using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RetroSpriteEngine
{
    public class MetaTile
    {
        public Tile[] TileSet { get; protected set; }
        public Point Frame { get; protected set; }
        public uint FrameDelay { get; protected set; }
        public uint FrameDelayTimer { get; protected set; }

        public MetaTile(Tile[] tileSet)
        {
            TileSet = tileSet;

            foreach (Tile tile in TileSet) tile.Parent = this;
        }
    }
}
