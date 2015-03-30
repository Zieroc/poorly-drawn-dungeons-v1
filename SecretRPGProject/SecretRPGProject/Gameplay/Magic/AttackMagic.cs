using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters.Enemies;
using PoorlyDrawnDungeons.Graphics;
using PoorlyDrawnDungeons.Utility;

namespace PoorlyDrawnDungeons.Gameplay.Magic
{
    [Serializable]
    public class AttackMagic : Magic
    {
        #region Variables

        protected int damageRoll;
        protected int numDice;

        protected GlobalVar.DamageType damageType;

        protected int attackBonus;

        protected string specialEffect;

        #endregion

        #region Constructor

        public AttackMagic(int manaCost, string name, int damageRoll, int numDice, int attackBonus, GlobalVar.DamageType damageType, Texture2D image, string filename, Sprite effect, string specialEffect)
            :base(manaCost, name, image, filename, effect)
        {
            this.damageRoll = damageRoll;
            this.numDice = numDice;
            this.damageType = damageType;
            this.attackBonus = attackBonus;
            this.specialEffect = specialEffect;
        }

        #endregion

        #region Properties

        public int DamageRoll
        {
            get { return damageRoll; }
        }

        public int NumDice
        {
            get { return numDice; }
        }

        public GlobalVar.DamageType DamageType
        {
            get { return damageType; }
        }

        public int AttackBonus
        {
            get { return attackBonus; }
        }

        public string SpecialEffect
        {
            get { return specialEffect; }
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

            damage += GlobalVar.hero.MagicSkill;

            return damage;
        }

        #endregion

        #region Cast

        public override void Cast()
        {
            int attackRoll;
            Random dieRoll = new Random();

            attackRoll = GlobalVar.hero.StatBonus(GlobalVar.hero.Intelligence) + (dieRoll.Next(20) + 1) + attackBonus;

            if (GlobalVar.dodgeBonus)
            {
                GlobalVar.dodgeBonus = false;
                attackRoll += 2;
            }

            if (attackRoll >= GlobalVar.enemy.Defence)
            {
                int damage = GlobalVar.enemy.ModDamage(damageType,CalcDamage(), specialEffect);
                GlobalVar.battleMsg = damage.ToString();
                GlobalVar.enemy.DecHealth(damage);
            }
            else
            {
                GlobalVar.battleMsg = "Miss";
            }

            GlobalVar.drawBattleMsgString = true;

            Vector2 position = new Vector2(GlobalVar.enemy.Position.X + GlobalVar.enemy.Sprite.Width/2 - Dungeon_Tile_Engine.DungeonMap.TileWidth/2, GlobalVar.enemy.Position.Y + GlobalVar.enemy.Sprite.Height/2 - Dungeon_Tile_Engine.DungeonMap.TileHeight/2);
            MagicEffectManager.AddEffect(new Sprite(effect.Texture, effect.Height, effect.Width, effect.States, effect.Animate, effect.Looping, 100f, effect.Texture.Name), position);

            base.Cast();
        }

        #endregion

        #region Serializable Methods

        public AttackMagic(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {
            this.damageRoll = (int)info.GetValue("DamageRoll", typeof(int));
            this.numDice = (int)info.GetValue("NumDice", typeof(int));
            this.damageType = (GlobalVar.DamageType)info.GetValue("DamageType", typeof(GlobalVar.DamageType));
            this.attackBonus = (int)info.GetValue("AttackBonus", typeof(int));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("DamageRoll", this.damageRoll);
            info.AddValue("NumDice", this.numDice);
            info.AddValue("DamageType", this.damageType);
            info.AddValue("AttackBonus", this.attackBonus);

            base.GetObjectData(info, ctxt);
        }

        #endregion
    }
}
