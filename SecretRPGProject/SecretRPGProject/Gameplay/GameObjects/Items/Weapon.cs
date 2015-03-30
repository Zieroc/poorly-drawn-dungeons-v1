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
    public class Weapon : Item
    {
        #region Variables

        private int attackBonus;    //How much does this weapon increase the player's attack by
        private int damageRoll;      //What type of dice do we use for damage
        private int numDice;      //How many dice do we roll

        private GlobalVar.DamageType damageType;
        private GlobalVar.WeaponType weaponType;
        private string specialEffect;

        #endregion

        #region Constructor

        public Weapon(int attackBonus, int damageRoll, int numDice, String name, int price, Texture2D image, Sprite[] sprite, bool drawSprite, Vector2 position, int maxStack, int stack, String filename, GlobalVar.DamageType damageType, GlobalVar.WeaponType weaponType, string specialEffect)
            : base(name, price, image, sprite, drawSprite, position, maxStack, stack, filename)
        {
            this.attackBonus = attackBonus;
            this.damageRoll = damageRoll;
            this.numDice = numDice;
            this.damageType = damageType;
            this.weaponType = weaponType;
            this.specialEffect = specialEffect;
        }

        #endregion

        #region Properties

        public int AttackBonus
        {
            get { return attackBonus; }
        }

        public int NumDice
        {
            get { return numDice; }
        }

        public int DamageRoll
        {
            get { return damageRoll; }
        }

        public GlobalVar.DamageType DamageType
        {
            get { return damageType; }
        }

        public GlobalVar.WeaponType WeaponType
        {
            get { return weaponType; }
        }

        public string SpecialEffect
        {
            get { return specialEffect; }
        }

        #endregion

        #region Movement

        public void MoveTo(Vector2 position, int direction, bool offset)
        {
            if (offset)
            {
                if (direction == 0)
                {
                    this.position = new Vector2(position.X - 1, position.Y - Sprite.Height);
                }
                else if (direction == 1)
                {
                    this.position = new Vector2(position.X - 4, position.Y);
                }
                else if (direction == 2)
                {
                    this.position = new Vector2(position.X - Sprite.Width, position.Y - 4);
                }
                else
                {
                    this.position = new Vector2(position.X, position.Y - 1);
                }
            }
            else
            {
                this.position = position;
            }

            this.direction = direction;
        }

        #endregion

        #region Damage

        public int CalcDamage()
        {
            Random die = new Random();
            int damage = 0;

            for (int i = 0; i < numDice; i++)
            {
                damage += die.Next(damageRoll) + 1;
            }

            return damage;
        }

        #endregion

        #region OnClick

        public override void OnClick(Player player)
        {
            player.EquipWeapon(this);

            base.OnClick(player);
        }

        #endregion

        #region Serializable Methods

        public Weapon(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {
            this.attackBonus = (int)info.GetValue("AttackBonus", typeof(int));
            this.damageRoll = (int)info.GetValue("DamageRoll", typeof(int));
            this.numDice = (int)info.GetValue("NumDice", typeof(int));
            this.damageType = (GlobalVar.DamageType)info.GetValue("Type", typeof(GlobalVar.DamageType));
            this.weaponType = (GlobalVar.WeaponType)info.GetValue("WeaponType", typeof(GlobalVar.WeaponType));
            this.specialEffect = (string)info.GetValue("SpecialEffect", typeof(string));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("AttackBonus", this.attackBonus);
            info.AddValue("DamageRoll", this.damageRoll);
            info.AddValue("NumDice", this.numDice);
            info.AddValue("Type", this.damageType);
            info.AddValue("WeaponType", this.weaponType);
            info.AddValue("SpecialEffect", this.specialEffect);

            base.GetObjectData(info, ctxt);
        }

        #endregion
    }
}
