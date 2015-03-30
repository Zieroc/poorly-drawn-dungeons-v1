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
    public class Goblin : Enemy
    {
        #region Constructor

        public Goblin(Sprite[] image, Vector2 position, bool alive, int direction, int sight, Weapon weapon, Armour armour, GlobalVar.WeaponType preferredWeapon, GlobalVar.DamageType immune, GlobalVar.DamageType vunerable, GlobalVar.DamageType resistant, String name, GlobalVar.CharacterMode mode)
            : base(image, position, alive, direction, sight, weapon, armour, preferredWeapon, immune, vunerable, resistant, mode)
        {
            Random die = new Random();

            //Generate stats
            strength = 6 + (die.Next(6) + 1) - 2;
            dexterity = 6 + (die.Next(6) + 1) + 2;
            intelligence = 6 + (die.Next(6) + 1);
            stamina = die.Next(4) + 1;

            maxHealth = 4 + die.Next(4) + 1 + stamina;
            currentHealth = maxHealth;

            maxMana = die.Next(3) + 1 + intelligence;
            currentMana = maxMana;

            if (armour.Type == GlobalVar.ArmourType.Light)
            {
                defence += StatBonus(dexterity);
            }
            defence += this.armour.DefenceBonus;
            attack = StatBonus(strength);
            attack += this.weapon.AttackBonus;

            this.name = name;
            this.race = "Goblin";
            speed = 60;

            xpValue = 10;

            gold = die.Next(3) + 1;
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            //Do different things based on mode
            if (alive)
            {
                if (mode == GlobalVar.CharacterMode.Attack && GlobalVar.turn == 2)
                {
                    //Goblins just attack
                    mode = GlobalVar.CharacterMode.Attacking;
                    int attackRoll;
                    Random dieRoll = new Random();

                    attackRoll = weaponSkill + attack + (dieRoll.Next(20) + 1);
                    if (attackRoll >= GlobalVar.hero.Defence)
                    {
                        int damageRoll = weapon.CalcDamage();

                        if (GlobalVar.defending)
                        {
                            damageRoll -= GlobalVar.hero.Endurance;

                            if (damageRoll < 0)
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
                            handLoc = new Vector2(position.X, position.Y + 8);
                            break;
                        case 1:
                            handLoc = new Vector2(position.X + Sprite.Width, position.Y + 12);
                            break;
                        case 2:
                            handLoc = new Vector2(position.X + 8, position.Y + Sprite.Height);
                            break;
                        default:
                            handLoc = new Vector2(position.X + 12, position.Y);
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
                                handLoc = new Vector2(position.X, position.Y + 10);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X, position.Y + 8);
                            }
                            break;
                        case 1:
                            if (Sprite.CurrentState == 1)
                            {
                                handLoc = new Vector2(position.X + Sprite.Width, position.Y + 10);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X + Sprite.Width, position.Y + 12);
                            }
                            break;
                        case 2:
                            if (Sprite.CurrentState == 1)
                            {
                                handLoc = new Vector2(position.X + 10, position.Y + Sprite.Height);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X + 8, position.Y + Sprite.Height);
                            }
                            break;
                        default:
                            if (Sprite.CurrentState == 1)
                            {
                                handLoc = new Vector2(position.X + 10, position.Y);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X + 12, position.Y);
                            }
                            break;
                    }
                    break;
                #endregion
                #region Attacking
                case GlobalVar.CharacterMode.Attacking:
                    switch (direction)
                    {
                        case 0:
                            if (Sprite.CurrentState == 1 || Sprite.CurrentState == 3)
                            {
                                handLoc = new Vector2(position.X, position.Y + 5);
                            }
                            else if (Sprite.CurrentState == 2)
                            {
                                handLoc = new Vector2(position.X, position.Y + 2);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X, position.Y + 8);
                            }
                            break;
                        case 1:
                            if (Sprite.CurrentState == 1 || Sprite.CurrentState == 3)
                            {
                                handLoc = new Vector2(position.X + Sprite.Width, position.Y + 15);
                            }
                            else if (Sprite.CurrentState == 2)
                            {
                                handLoc = new Vector2(position.X + Sprite.Width, position.Y + 18);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X + Sprite.Width, position.Y + 12);
                            }
                            break;
                        case 2:
                            if (Sprite.CurrentState == 1 || Sprite.CurrentState == 3)
                            {
                                handLoc = new Vector2(position.X + 5, position.Y + Sprite.Height);
                            }
                            else if (Sprite.CurrentState == 2)
                            {
                                handLoc = new Vector2(position.X + 2, position.Y + Sprite.Height);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X + 8, position.Y + Sprite.Height);
                            }
                            break;
                        default:
                            if (Sprite.CurrentState == 1 || Sprite.CurrentState == 3)
                            {
                                handLoc = new Vector2(position.X + 15, position.Y);
                            }
                            else if (Sprite.CurrentState == 2)
                            {
                                handLoc = new Vector2(position.X + 18, position.Y);
                            }
                            else
                            {
                                handLoc = new Vector2(position.X + 12, position.Y);
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
                            handLoc = new Vector2(position.X, position.Y + 8);
                            break;
                        case 1:
                            handLoc = new Vector2(position.X + Sprite.Width, position.Y + 12);
                            break;
                        case 2:
                            handLoc = new Vector2(position.X + 8, position.Y + Sprite.Height);
                            break;
                        default:
                            handLoc = new Vector2(position.X + 12, position.Y);
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
