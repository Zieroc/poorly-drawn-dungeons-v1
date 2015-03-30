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
    public class Potion : Item
    {
        #region Variables

        private GlobalVar.Stat type;    //What does this potion affect
        private int amount;                     //How much the potion modifies the stat by
        private GlobalVar.BuffType buffType;
        private double duration;                
        private double timer;

        private bool drank;                     //Have we drank the potion
        private Character user;                 //Who drank this potion

        #endregion

        #region Constructor

        public Potion(GlobalVar.Stat type, int amount, GlobalVar.BuffType buffType, double duration, String name, int price, Texture2D image, Sprite[] sprite, bool drawSprite, Vector2 position, int maxStack, int stack, String filename)
            : base(name, price, image, sprite, drawSprite, position, maxStack, stack, filename)
        {
            this.type = type;
            this.buffType = buffType;
            this.amount = amount;
            this.duration = duration;

            timer = 0;
            drank = false;
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

        public GlobalVar.BuffType BuffType
        {
            get { return buffType; }
        }

        public double Duration
        {
            get {return duration;}
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (drank)
            {
                timer += gameTime.ElapsedGameTime.TotalSeconds;

                if (timer > duration)
                {
                    timer = 0;

                    drank = false;
                    if (type == GlobalVar.Stat.Health)
                    {
                        if (user != null)
                        {
                            user.DecHealth(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Attack)
                    {
                        if (user != null)
                        {
                            user.DecAttack(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Defence)
                    {
                        if (user != null)
                        {
                            user.DecDefence(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Strength)
                    {
                        if(user != null)
                        {
                            user.DecStrength(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Dexterity)
                    {
                        if(user != null)
                        {
                            user.DecDexterity(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Intelligence)
                    {
                        if (user != null)
                        {
                            user.DecIntelligence(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Stamina)
                    {
                        if (user != null)
                        {
                            user.DecStamina(amount);
                        }
                    }
                    else if (type == GlobalVar.Stat.Mana)
                    {
                        if (user != null)
                        {
                            user.DecStamina(amount);
                        }
                    }


                    if (stack < 1)
                    {
                        Kill();
                    }
                    else
                    {
                        active = true;
                    }
                }
            }

            base.Update(gameTime);
        }

        #endregion

        #region OnClick & Use

        public override void OnClick(Player player)
        {
            Use(player);

            base.OnClick(player);
        }

        public void Use(Character user)
        {
            if (active)
            {
                this.user = user;
                stack--;
                active = false;

                switch (type)
                {
                    case GlobalVar.Stat.Attack:
                        user.IncAttack(amount);
                        break;
                    case GlobalVar.Stat.Defence:
                        user.IncDefence(amount);
                        break;
                    case GlobalVar.Stat.Health:
                        user.IncHealth(amount);
                        break;
                    case GlobalVar.Stat.Strength:
                        user.IncStrength(amount);
                        break;
                    case GlobalVar.Stat.Dexterity:
                        user.IncDexterity(amount);
                        break;
                    case GlobalVar.Stat.Stamina:
                        user.IncStamina(amount);
                        break;
                    case GlobalVar.Stat.Intelligence:
                        user.IncIntelligence(amount);
                        break;
                    case GlobalVar.Stat.Mana:
                        user.IncMana(amount);
                        break;
                }

                if (buffType == GlobalVar.BuffType.Temporary)
                {
                    drank = true;
                }
                else
                {
                    if (stack > 0)
                    {
                        active = true;
                    }
                    else
                    {
                        Kill();
                    }
                }
            }
        }

        #endregion

        #region Serializable Methods

        public Potion(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {
            this.type = (GlobalVar.Stat)info.GetValue("Type", typeof(GlobalVar.Stat));
            this.amount = (int)info.GetValue("Amount", typeof(int));
            this.duration = (double)info.GetValue("Duration", typeof(double));
            this.timer = (double)info.GetValue("Timer", typeof(double));
            this.drank = (bool)info.GetValue("Drank", typeof(bool));
            this.user = (Character)info.GetValue("User", typeof(Character));
            this.buffType = (GlobalVar.BuffType)info.GetValue("BuffType", typeof(GlobalVar.BuffType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Type", this.type);
            info.AddValue("Amount", this.amount);
            info.AddValue("Duration", this.duration);
            info.AddValue("Timer", this.timer);
            info.AddValue("Drank", this.drank);
            info.AddValue("User", this.user);
            info.AddValue("BuffType", this.buffType);

            base.GetObjectData(info, ctxt);
        }

        #endregion
    }
}
