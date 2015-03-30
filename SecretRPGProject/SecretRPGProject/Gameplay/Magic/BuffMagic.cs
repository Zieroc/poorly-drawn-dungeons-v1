using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters;
using PoorlyDrawnDungeons.Graphics;
using PoorlyDrawnDungeons.Utility;

namespace PoorlyDrawnDungeons.Gameplay.Magic
{
    [Serializable]
    public class BuffMagic : Magic
    {
        #region Variables

        private GlobalVar.Stat type;
        private GlobalVar.BuffType buffType;
        private int amount;
        private int numDice;
        private int mod;
        private double duration;
        private double timer;

        private bool cast;
        private Character caster;

        #endregion

        #region Constructor

        public BuffMagic(int manaCost, string name, GlobalVar.Stat type, GlobalVar.BuffType buffType, int amount, int numDice, int mod, double duration, Texture2D image, string filename, Sprite effect)
            :base(manaCost, name, image, filename, effect)
        {
            this.type = type;
            this.buffType = buffType;
            this.amount = amount;
            this.numDice = numDice;
            this.mod = mod;
            this.duration = duration;

            timer = 0;
            cast = false;
        }

        #endregion

        #region Properties

        public GlobalVar.Stat Type
        {
            get { return type; }
        }

        public int Amount
        {
            get { return amount; }
        }

        public int NumDice
        {
            get { return numDice; }
        }

        public int Mod
        {
            get { return mod; }
        }

        public GlobalVar.BuffType BuffType
        {
            get { return buffType; }
        }

        public double Duration
        {
            get { return duration; }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (cast)
            {
                timer += gameTime.ElapsedGameTime.TotalSeconds;

                if (timer > duration)
                {
                    cast = false;
                    if (type == GlobalVar.Stat.Health)
                    {
                        if (caster != null)
                        {
                            caster.DecHealth(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Attack)
                    {
                        if (caster != null)
                        {
                            caster.DecAttack(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Defence)
                    {
                        if (caster != null)
                        {
                            caster.DecDefence(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Strength)
                    {
                        if (caster != null)
                        {
                            caster.DecStrength(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Dexterity)
                    {
                        if (caster != null)
                        {
                            caster.DecDexterity(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Intelligence)
                    {
                        if (caster != null)
                        {
                            caster.DecIntelligence(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Stamina)
                    {
                        if (caster != null)
                        {
                            caster.DecStamina(amount);
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        #endregion

        #region Cast

        public override void Cast()
        {
            if (!cast)
            {
                caster = GlobalVar.hero;
                int buff = calcBuff();

                Vector2 position = new Vector2(caster.Position.X + caster.Sprite.Width / 2 - Dungeon_Tile_Engine.DungeonMap.TileWidth / 2, caster.Position.Y + caster.Sprite.Height / 2 - Dungeon_Tile_Engine.DungeonMap.TileHeight / 2);
                MagicEffectManager.AddEffect(new Sprite(effect.Texture, effect.Height, effect.Width, effect.States, effect.Animate, effect.Looping, 100f, effect.Texture.Name), position);

                switch (type)
                {
                    case GlobalVar.Stat.Attack:
                        caster.IncAttack(buff);
                        break;
                    case GlobalVar.Stat.Defence:
                        caster.IncDefence(buff);
                        break;
                    case GlobalVar.Stat.Health:
                        caster.IncHealth(buff);
                        break;
                    case GlobalVar.Stat.Strength:
                        caster.IncStrength(buff);
                        break;
                    case GlobalVar.Stat.Dexterity:
                        caster.IncDexterity(buff);
                        break;
                    case GlobalVar.Stat.Stamina:
                        caster.IncStamina(buff);
                        break;
                    case GlobalVar.Stat.Intelligence:
                        caster.IncIntelligence(buff);
                        break;
                }

                GlobalVar.magicMsg = "+" + buff;
                GlobalVar.drawMagicMsg = true;

                if (buffType == GlobalVar.BuffType.Temporary)
                {
                    cast = true;
                }
            }
            else
            {
                caster.IncMana(manaCost);
                GlobalVar.magicMsg += "Fizzles";
                GlobalVar.drawMagicMsg = true;
            }

            base.Cast();
        }

        private int calcBuff()
        {
            Random die = new Random();

            int buff = mod;
            for (int i = 0; i < numDice; i++)
            {
                buff += die.Next(amount) + 1;
            }

            buff += caster.MagicSkill;

            return buff;
        }

        #endregion

        #region Serializable Methods

        public BuffMagic(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {
            this.type = (GlobalVar.Stat)info.GetValue("Type", typeof(GlobalVar.Stat));
            this.buffType = (GlobalVar.BuffType)info.GetValue("BuffType", typeof(GlobalVar.BuffType));
            this.amount = (int)info.GetValue("Amount", typeof(int));
            this.numDice = (int)info.GetValue("NumDice", typeof(int));
            this.mod = (int)info.GetValue("Mod", typeof(int));
            this.duration = (double)info.GetValue("Duration", typeof(double));
            this.timer = (double)info.GetValue("Timer", typeof(double));
            this.cast = (bool)info.GetValue("Cast", typeof(bool));
            this.caster = (Character)info.GetValue("Caster", typeof(Character));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Type", this.type);
            info.AddValue("BuffType", this.buffType);
            info.AddValue("Amount", this.amount);
            info.AddValue("NumDice", this.numDice);
            info.AddValue("Mod", this.mod);
            info.AddValue("Duration", this.duration);
            info.AddValue("Timer", this.timer);
            info.AddValue("Cast", this.cast);
            info.AddValue("Caster", this.caster);

            base.GetObjectData(info, ctxt);
        }

        #endregion
    }
}
