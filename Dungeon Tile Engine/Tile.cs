using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Tile_Engine
{
    public class Tile
    {
        #region Variables

        private int imageIndex;                  //Information on the images index on the tilesheet
        private string code;                     //Used to add speciality to a tile e.g. spawn, start, end, trap etc.
                                                //Can also be used to set areas where enemies and pickups should appear in game
        private bool passable;                   //Is the tile passable or does it block movement

        #endregion

        #region Properties

        public int ImageIndex
        {
            get { return imageIndex; }
            set { imageIndex = value; }
        }

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        public bool Passable
        {
            get { return passable; }
            set { passable = value; }
        }

        #endregion

        #region Constructor

        public Tile()
        {
            imageIndex = 0;

            code = "";
            passable = true;
        }

        public Tile(int imageIndex, string code, bool passable)
        {
            this.imageIndex = imageIndex;
            this.code = code;
            this.passable = passable;
        }

        #endregion

        #region Other

        //Switch passable between true and false, for locking and unlocking areas etc.
        public void TogglePassable()
        {
            passable = !passable;
        }

        #endregion
    }
}
