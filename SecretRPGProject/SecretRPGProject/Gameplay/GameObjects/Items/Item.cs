using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorlyDrawnDungeons.Gameplay;
using PoorlyDrawnDungeons.Gameplay.Auras;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters;
using PoorlyDrawnDungeons.Graphics;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.Items
{
    [Serializable]
    public class Item: GameObject
    {
        #region Variables

        protected String name;        //The item name;
        protected int price;          //How many gold pieces the item is worth
        protected Texture2D image;    //The image that represents this item, for shops and inventory
        protected bool drawSprite;    //Should this item be drawn into the world

        //Stack Variables
        protected int maxStack;
        protected int stack;

        //Special Variable for potions and other timed effects but instant use items
        protected bool active;

        #endregion

        #region Constructor

        public Item(String name, int price, Texture2D image, String filename)
            :base()
        {
            Sprite[] sprites = new Sprite[1];
            sprites[0] = new Sprite(image, filename);

            this.name = name;
            this.price = price;
            this.image = image;
            this.image.Name = filename;
            drawSprite = false;
            sprite = sprites;
            spriteNum = 0;
            position = Vector2.Zero; //Position on grid
            alive = true;
            direction = 0;

            maxStack = 1;
            stack = 1;

            CalcBounds();

            active = true;
        }

        public Item(String name, int price, Texture2D image, Sprite[] sprite, bool drawSprite, int maxStack, int stack, String filename)
            : this(name, price, image, sprite, drawSprite, Vector2.Zero, maxStack, stack, filename)
        {
        }

        public Item(String name, int price, Texture2D image, Sprite[] sprite, bool drawSprite, Vector2 position, int maxStack, int stack, String filename)
            :base(sprite, position, 0)
        {
            this.name = name;
            this.price = price;
            this.image = image;
            this.image.Name = filename;
            this.drawSprite = drawSprite;
            spriteNum = 0;
            alive = true;

            this.maxStack = maxStack;
            this.stack = stack;

            active = true;

            CalcBounds();
        }

        #endregion

        #region Properties

        public String Name
        {
            get { return name; }
        }

        public int Price
        {
            get { return price; }
        }

        public Texture2D Image
        {
            get { return image; }
        }

        public bool DrawSprite
        {
            get { return drawSprite; }
            set { drawSprite = value; }
        }

        public int MaxStack
        {
            get { return maxStack; }
        }

        public int Stack
        {
            get { return stack; }
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (drawSprite)
            {
                base.Draw(spriteBatch);
            }
        }

        public void DrawImage(SpriteBatch spriteBatch, Vector2 location)
        {
            if(alive)
            {
                Color colour;
                if (active)
                {
                    colour = Color.White;
                }
                else
                {
                    colour = Color.DarkGray;
                }
                spriteBatch.Draw(image, location, image.Bounds,colour, 0f, Vector2.Zero, 1f,SpriteEffects.None, 0.4f);
            }
        }

        #endregion

        #region Inc & Dec Stack

        public void IncStack(int amount)
        {
            stack += amount;

            if (stack > maxStack)
            {
                stack = maxStack;
            }
        }

        public void DecStack(int amount)
        {
            stack -= amount;

            if (stack < 0)
            {
                stack = 0;
            }
        }

        #endregion

        #region OnClick

        //What happens when we click on this item in the inventory?
        public virtual void OnClick(Player player)
        {
            //Basic object does nothing yet
        }

        #endregion

        #region Serializable Methods

        public Item(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {
            this.name = (String)info.GetValue("Name", typeof(String));
            this.price = (int)info.GetValue("Price", typeof(int));
            this.image = GlobalVar.content.Load<Texture2D>((String)info.GetValue("Image", typeof(String)));
            this.drawSprite = (bool)info.GetValue("DrawSprite", typeof(bool));
            this.maxStack = (int)info.GetValue("MaxStack", typeof(int));
            this.stack = (int)info.GetValue("Stack", typeof(int));
            this.active = (bool)info.GetValue("Active", typeof(bool));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Name", this.name);
            info.AddValue("Price", this.price);
            info.AddValue("Image", this.image.Name);
            info.AddValue("DrawSprite", this.drawSprite);
            info.AddValue("MaxStack", this.maxStack);
            info.AddValue("Stack", this.stack);
            info.AddValue("Active", this.active);

            base.GetObjectData(info, ctxt);
        }

        #endregion
    }
}
