using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RetroSpriteEngine
{
    public class Camera : IPositional
    {
        public Vector3 Position { get; set; }
        public Rectangle Boundary { get; protected set; } //Will typically be the same size as the screen or action canvas.
        public float X
        {
            get { return Position.X; }
            set { Position = new Vector3(value, Position.Y, Position.Z); }
        }
        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector3(Position.X, value, Position.Z); }
        }
        public float Z
        {
            get { return Position.Y; }
            set { Position = new Vector3(Position.X, Position.Y, value); }
        }

        public Camera(Rectangle boundary) 
        { 
            Position = new Vector3();
            Boundary = boundary;
        }

        public Camera(Vector3 position, Rectangle boundary) 
        { 
            Position = position;
            Boundary = boundary;
        }
    }
}
