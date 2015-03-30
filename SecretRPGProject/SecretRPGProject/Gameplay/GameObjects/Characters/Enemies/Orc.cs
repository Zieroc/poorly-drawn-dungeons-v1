using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Items;
using PoorlyDrawnDungeons.Graphics;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.Characters.Enemies
{
    public class Orc : Enemy
    {
        #region Constructor

        public Orc(Sprite[] image, Vector2 position, bool alive, int direction, int sight, Weapon weapon, Armour armour, GlobalVar.WeaponType preferredWeapon, GlobalVar.DamageType immune, GlobalVar.DamageType vunerable, GlobalVar.DamageType resistant, String name, GlobalVar.CharacterMode mode)
            : base(image, position, alive, direction, sight, weapon, armour, preferredWeapon, immune, vunerable, resistant, mode)
        {
            Random die = new Random();

            //Generate stats
            strength = 4 + (die.Next(6) + 1) + (die.Next(6) + 1) + 5;
            dexterity = 4 + (die.Next(6) + 1) + (die.Next(6) + 1) - 2;
            intelligence = 4 + (die.Next(6) + 1) + (die.Next(6) + 1);
            stamina = 4 + (die.Next(6) + 1) + (die.Next(6) + 1);

            maxHealth = 4 + die.Next(4) + 1 + stamina;
            currentHealth = maxHealth;

            maxMana = die.Next(6) + 1 + intelligence;
            currentMana = maxMana;

            defence += this.armour.DefenceBonus;
            attack = StatBonus(strength);
            attack += this.weapon.AttackBonus;

            this.name = name;
            this.race = "Orc";
            speed = 48;

            xpValue = 15;

            gold = die.Next(5) + 1;
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (alive)
            {
                if (mode == GlobalVar.CharacterMode.Attack && GlobalVar.turn == 2)
                {
                    mode = GlobalVar.CharacterMode.Attacking;
                    int attackRoll;
                    Random dieRoll = new Random();

                    attackRoll = attack + (dieRoll.Next(20) + 1);
                    if (attackRoll >= GlobalVar.hero.Defence)
                    {
                        int damageRoll = weapon.CalcDamage() + weaponSkill;

                        if (GlobalVar.defending)
                        {
                            damageRoll -= GlobalVar.hero.Endurance;

                            if(damageRoll < 0)
                            {
                                damageRoll = 0;
                            }
                        }

                        GlobalVar.battleMsg = damageRoll.ToString();

                        GlobalVar.hero.DecHealth(damageRoll);
                    }
                    else
                    {
                        if (GlobalVar.defending)
                        {
                            GlobalVar.battleMsg = "Dodge";
                            GlobalVar.dodgeBonus = true;
                        }
                        else
                        {
                            GlobalVar.battleMsg = "Miss";
                        }
                    }

                    GlobalVar.drawBattleMsgString = true;
                }
            }

            base.Update(gameTime);
        }

        #endregion

        #region Hand Location

        protected override Vector2 HandLoc()
        {
            Vector2 handLoc;

            switch (mode)
            {
                #region Idle/Attack
                case GlobalVar.CharacterMode.Idle:
                case GlobalVar.CharacterMode.Attack:
                    switch (direction)
                    {
                        case 0:
                            handLoc = new Vector2(position.X, position.Y + 15);
                            break;
                        case 1:
                            handLoc = new Vector2(position.X + Sprite.Width, position.Y + 17);
                            break;
                        case 2:
                            handLoc = new Vector2(position.X + 15, position.Y + Sprite.Height);
                            break;
                        default:
                            handLoc = new Vector2(position.X + 17, position.Y);
                            break;
                    }
                    break;
                #endregion
                #region Patrol
                case GlobalVar.CharacterMode.Patrol:
                    switch (direction)
                    {
                        case 0:
                            if (Sprite.CurrentState == 1)
                            {
                                handLoc = new Vector2(position.X, position.Y + 15);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X, position.Y + 11);
                            }
                            break;
                        case 1:
                            if (Sprite.CurrentState == 1)
                            {
                                handLoc = new Vector2(position.X + Sprite.Width, position.Y + 17);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X + Sprite.Width, position.Y + 21);
                            }
                            break;
                        case 2:
                            if (Sprite.CurrentState == 1)
                            {
                                handLoc = new Vector2(position.X + 15, position.Y + Sprite.Height);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X + 11, position.Y + Sprite.Height);
                            }
                            break;
                        default:
                            if (Sprite.CurrentState == 1)
                            {
                                handLoc = new Vector2(position.X + 17, position.Y);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X + 21, position.Y);
                            }
                            break;
                    }
                    break;
                #endregion
                #region Attacking and Casting
                case GlobalVar.CharacterMode.Attacking:
                case GlobalVar.CharacterMode.Casting:
                    switch (direction)
                    {
                        case 0:
                            if (Sprite.CurrentState == 0 || Sprite.CurrentState == 4)
                            {
                                handLoc = new Vector2(position.X, position.Y + 15);
                            }
                            else if (Sprite.CurrentState == 1 || Sprite.CurrentState == 3)
                            {
                                handLoc = new Vector2(position.X, position.Y + 11);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X, position.Y + 7);
                            }
                            break;
                        case 1:
                            if (Sprite.CurrentState == 0 || Sprite.CurrentState == 4)
                            {
                                handLoc = new Vector2(position.X + Sprite.Width, position.Y + 17);
                            }
                            else if (Sprite.CurrentState == 1 || Sprite.CurrentState == 3)
                            {
                                handLoc = new Vector2(position.X + Sprite.Width, position.Y + 21);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X + Sprite.Width, position.Y + 25);
                            }
                            break;
                        case 2:
                            if (Sprite.CurrentState == 0 || Sprite.CurrentState == 4)
                            {
                                handLoc = new Vector2(position.X + 15, position.Y + Sprite.Height);
                            }
                            else if (Sprite.CurrentState == 1 || Sprite.CurrentState == 3)
                            {
                                handLoc = new Vector2(position.X + 11, position.Y + Sprite.Height);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X + 7, position.Y + Sprite.Height);
                            }
                            break;
                        default:
                            if (Sprite.CurrentState == 0 || Sprite.CurrentState == 4)
                            {
                                handLoc = new Vector2(position.X + 17, position.Y);
                            }
                            else if (Sprite.CurrentState == 1 || Sprite.CurrentState == 3)
                            {
                                handLoc = new Vector2(position.X + 21, position.Y);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X + 25, position.Y);
                            }
                            break;
                    }
                    break;
                #endregion
                #region Default
                default:
                    switch (direction)
                    {
                        case 0:
                            handLoc = new Vector2(position.X, position.Y + 15);
                            break;
                        case 1:
                            handLoc = new Vector2(position.X + Sprite.Width, position.Y + 17);
                            break;
                        case 2:
                            handLoc = new Vector2(position.X + 15, position.Y + Sprite.Height);
                            break;
                        default:
                            handLoc = new Vector2(position.X + 17, position.Y);
                            break;
                    }
                    break;
                #endregion
            }

            return handLoc;
        }

        #endregion
    }
}
