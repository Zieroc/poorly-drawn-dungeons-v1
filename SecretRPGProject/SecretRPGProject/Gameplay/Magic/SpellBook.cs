using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters;

namespace PoorlyDrawnDungeons.Gameplay.Magic
{
    [Serializable]
    public class SpellBook : ISerializable
    {
        #region Variables

        private List<Magic> spells;

        #endregion

        #region Constructor

        public SpellBook()
        {
            spells = new List<Magic>();
        }

        #endregion

        #region Properties

        public List<Magic> Spells
        {
            get { return spells; }
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            for (int i = spells.Count - 1; i >= 0; i--)
            {
                spells[i].Update(gameTime);
            }
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < spells.Count; i++)
            {
                spells[i].DrawImage(spriteBatch, GlobalVar.inventoryPos[i]);
            }
        }

        #endregion

        #region Gain Spells

        public void GainSpell(Magic spell)
        {
            #region Is Attack Spell
            if (spell is AttackMagic)
            {
                AttackMagic magic = (AttackMagic)spell;
                AttackMagic newMagic = new AttackMagic(magic.ManaCost, magic.Name, magic.DamageRoll, magic.NumDice, magic.AttackBonus, magic.DamageType, magic.Image, magic.Image.Name, magic.Effect, magic.SpecialEffect);
                spells.Add(newMagic);
            }
            #endregion
            #region Is Buff Spell
            else if (spell is BuffMagic)
            {
                BuffMagic magic = (BuffMagic)spell;
                BuffMagic newMagic = new BuffMagic(magic.ManaCost, magic.Name, magic.Type, magic.BuffType, magic.Amount, magic.NumDice, magic.Mod, magic.Duration, magic.Image, magic.Image.Name, magic.Effect);
                spells.Add(newMagic);
            }
            #endregion
        }

        #endregion

        #region Serializable Methods

        public SpellBook(SerializationInfo info, StreamingContext ctxt)
        {
            this.spells = (List<Magic>)info.GetValue("Spells", typeof(List<Magic>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Spells", this.spells);
        }

        #endregion
    }
}
