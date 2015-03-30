using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Dungeon_Tile_Engine;
using PoorlyDrawnDungeons.Graphics;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects
{
    [Serializable]
    public abstract class GameObject : ISerializable
    {
        #region Variables

        protected Sprite[] sprite;
        protected int spriteNum;
        protected Vector2 position; //Position on grid
        protected bool alive;
        protected float depth = 0.8f;
        protected Rectangle bounds; //A bounding box around the object, used for collision
        protected int direction;

        #endregion

        #region Constructor

        public GameObject()
        {
        }

        public GameObject(Sprite[] sprite, Vector2 position, int direction)
        {
            this.sprite = sprite;
            spriteNum = 0;

            this.position = position;

            this.direction = direction;
        }

        #endregion

        #region Properties

        public Sprite Sprite
        {
            get { return sprite[spriteNum]; }
        }

        public Sprite[] Sprites
        {
            get { return sprite; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 PositionCenter
        {
            get { return new Vector2(position.X + (int)(sprite[spriteNum].Width / 2), position.Y + (int)(sprite[spriteNum].Height / 2)); }
        }

        public bool Alive
        {
            get { return alive; }
        }

        public Rectangle Bounds
        {
            get { return bounds; }
        }

        public int Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        #endregion

        #region Collisions

        //Calculate the bounds
        public void CalcBounds()
        {
            bounds = new Rectangle((int)position.X, (int)position.Y, sprite[spriteNum].Width, sprite[spriteNum].Height);
        }

        //Check for collision
        public bool Collided(Rectangle rect)
        {
            return bounds.Intersects(rect);
        }

        #endregion

        #region Kill, Revive

        public virtual void Kill()
        {
            alive = false;
        }

        public virtual void Revive()
        {
            alive = true;
        }
        #endregion

        #region Update

        public virtual void Update(GameTime gameTime)
        {
             //Do nothing if object is dead
            if (!alive)
                return;

            if (!GlobalVar.paused)
            {
                //Update the sprite
                sprite[spriteNum].Update(gameTime);
            }

            //Object has moved so move the collision box to ensure collisions work correctly
            CalcBounds();
        }

        #endregion

        #region Draw

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (alive || (this is Character && GlobalVar.battleMode))
            {
                if (Sprite.Direction != direction)
                {
                    Sprite.Direction = direction;
                    Sprite.CurrentState = 0;
                }

                Sprite.Draw(spriteBatch, Camera.LocationToScreen(position), depth);
            }
        }

        #endregion

        #region Serializable Methods

        public GameObject(SerializationInfo info, StreamingContext ctxt)
        {
            this.sprite = (Sprite[])info.GetValue("Sprite", typeof(Sprite[]));
            this.spriteNum = (int)info.GetValue("SpriteNum", typeof(int));
            this.position = (Vector2)info.GetValue("Position", typeof(Vector2));
            this.alive = (bool)info.GetValue("Alive", typeof(bool));
            this.depth = (float)info.GetValue("Depth", typeof(float));
            this.bounds = (Rectangle)info.GetValue("Bounds", typeof(Rectangle));
            this.direction = (int)info.GetValue("Direction", typeof(int));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Sprite", this.sprite);
            info.AddValue("SpriteNum", this.spriteNum);
            info.AddValue("Position", this.position);
            info.AddValue("Alive", this.alive);
            info.AddValue("Depth", this.depth);
            info.AddValue("Bounds", this.bounds);
            info.AddValue("Direction", this.direction);
        }

        #endregion
    }
}
