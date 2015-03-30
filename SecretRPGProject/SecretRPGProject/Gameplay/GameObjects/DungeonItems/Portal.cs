using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Items;
using PoorlyDrawnDungeons.Graphics;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.DungeonItems
{
    public class Portal : GameObject
    {
        #region Variables

        private int map;    //What map do we go to

        #endregion

        #region Constructor

        public Portal(Sprite[] sprite, Vector2 position, int direction, int map)
            :base(sprite, position, direction)
        {
            this.map = map;
            alive = true;
        }

        #endregion

        #region Properties

        public int Map
        {
            get { return map; }
        }

        #endregion
    }
}
