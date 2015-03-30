using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters;
using PoorlyDrawnDungeons.Graphics;

namespace PoorlyDrawnDungeons.Gameplay.Magic
{
    [Serializable]
    public abstract class Magic : ISerializable
    {
        #region Variables

        protected int manaCost;     //How much mana is needed to cast this spell/prayer
        protected string name;      //The name of the spell/prayer
        protected Texture2D image;
        protected Sprite effect;

        #endregion

        #region Constructor

        public Magic(int manaCost, string name, Texture2D image, string filename, Sprite effect)
        {
            this.manaCost = manaCost;
            this.name = name;
            this.image = image;
            this.image.Name = filename;
            this.effect = effect;
        }

        #endregion

        #region Properties

        public int ManaCost
        {
            get { return manaCost; }
        }

        public String Name
        {
            get { return name; }
        }

        public Texture2D Image
        {
            get { return image; }
        }

        public Sprite Effect
        {
            get { return effect; }
        }

        #endregion

        #region Update

        public virtual void Update(GameTime gameTime)
        {
            //For inheritance
        }

        #endregion

        #region Cast

        public virtual void Cast()
        {

        }

        public bool CanCast()
        {
            //If the caster has enough mana allow him to cast otherwise return false
            if (GlobalVar.hero.CurrentMana >= manaCost)
            {
                GlobalVar.hero.CurrentMana -= manaCost;
                return true;
            }
            else
            {
                GlobalVar.magicMsg = "Low MP";
                GlobalVar.drawMagicMsg = true;
                return false;
            }
        }

        #endregion

        #region Draw

        public void DrawImage(SpriteBatch spriteBatch, Vector2 location)
        {
            spriteBatch.Draw(image, location, image.Bounds, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.4f);
        }

        #endregion

        #region Serializable Methods

        public Magic(SerializationInfo info, StreamingContext ctxt)
        {
            this.manaCost = (int)info.GetValue("ManaCost", typeof(int));
            this.name = (string)info.GetValue("Name", typeof(string));
            this.image = GlobalVar.content.Load<Texture2D>((String)info.GetValue("Image", typeof(String)));
            this.effect = (Sprite)info.GetValue("Sprite", typeof(Sprite));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("ManaCost", this.manaCost);
            info.AddValue("Name", this.name);
            info.AddValue("Image", this.image.Name);
            info.AddValue("Sprite", this.effect);
        }

        #endregion
    }
}
