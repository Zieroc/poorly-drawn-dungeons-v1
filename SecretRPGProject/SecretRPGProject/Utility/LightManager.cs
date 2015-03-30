using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorlyDrawnDungeons.Gameplay;
using PoorlyDrawnDungeons.Gameplay.Auras;
using PoorlyDrawnDungeons.Gameplay.GameObjects;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Items;

namespace PoorlyDrawnDungeons.Utility
{
    public static class LightManager
    {
        #region Variables

        public static List<Light> lights = new List<Light>();

        #endregion

        #region Properties

        public static List<Light> Lights
        {
            get { return lights; }
        }

        #endregion

        #region Add and Remove and Set

        public static void AddLight(Light light)
        {
            lights.Add(light);
        }

        private static void RemoveLight(int loc)
        {
            lights.RemoveAt(loc);
        }

        public static void SetLight(int loc, Light light)
        {
            lights[loc] = light;
        }

        #endregion

        #region Update

        public static void Update(GameTime gameTime)
        {
            if (!GlobalVar.paused)
            {
                for (int i = lights.Count - 1; i >= 0; i--)
                {
                    if (lights[i].Alive)
                    {
                        lights[i].Update(gameTime);
                    }
                    else
                    {
                        //The light is dead and it is not the player's lights so remove it from the list
                        RemoveLight(i);
                    }
                }
            }
        }

        #endregion

        #region Draw

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Light light in lights)
            {
                if (light != null)
                {
                    light.Draw(spriteBatch);
                }
            }
        }

        public static void DrawAura(SpriteBatch spriteBatch)
        {
            foreach (Light light in lights)
            {
                if (light != null)
                {
                    light.DrawAura(spriteBatch);
                }
            }
        }

        #endregion
    }
}
