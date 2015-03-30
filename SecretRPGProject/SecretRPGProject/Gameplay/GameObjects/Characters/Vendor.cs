using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorlyDrawnDungeons.Gameplay.Features;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Items;
using PoorlyDrawnDungeons.Graphics;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.Characters
{
    public class Vendor : NPC
    {
        #region Variables

        private Inventory shop;

        #endregion

        #region Constructor

        public Vendor(string introduction, Inventory shop, Sprite[] sprite, Vector2 position, int direction, int facing)
            :base(sprite, position, direction, facing)
        {
            this.shop = shop;

            speech = new Dialogue[1];
            speech[0] = new Dialogue(introduction, "What are you selling?", "", 1, 0, -1, -1);

            name = "Vendor";
        }

        #endregion

        #region Properties

        public Inventory Shop
        {
            get { return shop; }
        }

        #endregion

        #region Show Shop
        
        public void OpenShop()
        {
            GlobalVar.displayShop = true;
            GlobalVar.paused = true;
            GlobalVar.shop = this.shop;
        }

        #endregion
    }
}
