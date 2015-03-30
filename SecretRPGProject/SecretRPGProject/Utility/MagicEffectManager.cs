using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Dungeon_Tile_Engine;
using PoorlyDrawnDungeons.Graphics;


namespace PoorlyDrawnDungeons.Utility
{
    public static class MagicEffectManager
    {
        #region Variables

        private static List<Sprite> effects;
        private static List<Vector2> locations;

        #endregion

        #region Initialise

        public static void Initialise()
        {
            effects = new List<Sprite>();
            locations = new List<Vector2>();
        }

        #endregion

        #region AddEffect

        public static void AddEffect(Sprite sprite, Vector2 location)
        {
            effects.Add(sprite);
            locations.Add(location);
        }

        #endregion

        #region Update

        public static void Update(GameTime gameTime)
        {
            for (int i = effects.Count - 1; i >= 0; i--)
            {
                if (!effects[i].FinishedPlaying)
                {
                    effects[i].Update(gameTime);
                }
                else
                {
                    effects.RemoveAt(i);
                    locations.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Draw

        public static void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].Draw(spriteBatch, Camera.LocationToScreen(locations[i]), 0.7f);
            }
        }

        #endregion
    }
}
