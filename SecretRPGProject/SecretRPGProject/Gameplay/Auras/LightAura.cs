using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Dungeon_Tile_Engine;

namespace PoorlyDrawnDungeons.Gameplay.Auras
{
    [Serializable]
    public class LightAura : ISerializable
    {
        #region Variables

        Texture2D mask; //The aura's texture
        Vector2 position;

        #endregion

        #region Constructor

        public LightAura(Texture2D mask, Vector2 position, String filename)
        {
            this.mask = mask;
            this.mask.Name = filename;
            this.position = position;
        }

        #endregion

        #region Properties

        public Texture2D Mask
        {
            get { return mask; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mask, Camera.LocationToScreen(position), Color.White);
        }

        #endregion

        #region Other

        public void MoveTo(Vector2 position)
        {
            this.position = position;
        }

        #endregion

        #region Serializable Methods

        public LightAura(SerializationInfo info, StreamingContext ctxt)
        {
            this.mask = GlobalVar.content.Load<Texture2D>((String)info.GetValue("Mask", typeof(String)));
            this.position = (Vector2)info.GetValue("Position", typeof(Vector2));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Mask", this.mask.Name);
            info.AddValue("Position", this.position);
        }

        #endregion
    }
}
