using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters;
using PoorlyDrawnDungeons.Graphics;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.Items
{
    [Serializable]
    public class Armour : Item
    {
        #region Variables

        private int defenceBonus;    //How much does this armour increase the player's defence by
        private GlobalVar.ArmourType type;

        #endregion

        #region Constructor

        public Armour(int defenceBonus, String name, int price, Texture2D image, Sprite[] sprite, bool drawSprite, Vector2 position, int maxStack, int stack, String filename, GlobalVar.ArmourType type)
            : base(name, price, image, sprite, drawSprite, position, maxStack, stack, filename)
        {
            this.defenceBonus = defenceBonus;
            this.type = type;
        }

        #endregion

        #region Properties

        public int DefenceBonus
        {
            get { return defenceBonus; }
        }

        public GlobalVar.ArmourType Type
        {
            get { return type; }
        }

        #endregion

        #region OnClick

        public override void OnClick(Player player)
        {
            player.EquipArmour(this);

            base.OnClick(player);
        }

        #endregion

        #region Serializable Methods

        public Armour(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {
            this.defenceBonus = (int)info.GetValue("DefenceBonus", typeof(int));
            this.type = (GlobalVar.ArmourType)info.GetValue("Type", typeof(GlobalVar.ArmourType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("DefenceBonus", this.defenceBonus);
            info.AddValue("Type", this.type);

            base.GetObjectData(info, ctxt);
        }

        #endregion
    }
}
