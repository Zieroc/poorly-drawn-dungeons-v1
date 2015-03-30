using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorlyDrawnDungeons.Graphics;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.DungeonItems
{
    public class RestArea : GameObject
    {
        #region Variables

        private bool rested;        //Have we rested at this point
        private bool resurrected;   //Have we been resurrected at this point?

        #endregion

        #region Constructor

        public RestArea(Sprite[] sprite, Vector2 position, int direction)
            :base(sprite, position, direction)
        {
            alive = true;

            rested = false;
            resurrected = false;
        }

        #endregion

        #region Properties

        public bool Rested
        {
            get { return rested; }
            set { rested = value; }
        }

        public bool Resurrected
        {
            get { return resurrected; }
            set { resurrected = value; }
        }

        #endregion
    }
}
