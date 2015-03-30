using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Items;
using PoorlyDrawnDungeons.Graphics;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.DungeonItems
{
    public class Chest : GameObject
    {
        #region Variables

        private Inventory inventory;    //The chest's inventory
        private int facing;             //What direction the player needs to be facing to open the chest

        #endregion

        #region Constructor

        public Chest(Sprite[] sprite, Vector2 position, int direction, Item[] items, int facing)
            :base(sprite, position, direction)
        {
            this.facing = facing;
            inventory = new Inventory(4);

            alive = true;

            for (int i = 0; i < items.Length; i++)
            {
                //Item maybe be null if random generator creates stacked items so add in a null check
                if (items[i] != null)
                {
                    inventory.AddItem(items[i]);
                }
            }
        }

        #endregion

        #region Properties

        public Inventory Inventory
        {
            get { return inventory; }
        }

        public int Facing
        {
            get { return facing; }
        }

        #endregion

        #region Open & Close

        public void Open()
        {
            spriteNum = 1;
            GlobalVar.displayChestBar = true;
            GlobalVar.displayChest = this;
            GlobalVar.paused = true;
        }

        public void Close()
        {
            spriteNum = 0;
            GlobalVar.displayChestBar = false;
            GlobalVar.paused = false;
        }

        #endregion
    }
}
