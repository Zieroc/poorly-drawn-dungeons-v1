using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Items;
using PoorlyDrawnDungeons.Graphics;
using Dungeon_Tile_Engine;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.Characters.Enemies
{
    public class Enemy : Character
    {
        #region Variables

        protected Inventory inventory;
        protected int sight;       //The tile radius of this monster's sight
        protected Weapon weapon;   //The weapon used by this monster
        protected Armour armour;   //The armour worn by this monster
        protected Vector2 spawnPos;

        protected int xpValue;  //How much xp the player gains if they defeat this enemy

        protected GlobalVar.DamageType vunerable;
        protected GlobalVar.DamageType resistant;
        protected GlobalVar.DamageType immune;

        protected GlobalVar.WeaponType preferredWeapon;

        //Timers for guard duty
        protected float waitTimer;
        protected float timer;

        protected bool heroDetected;
        protected bool perceptionTried;

        protected string race;

        protected int gold;

        #endregion

        #region Constructor

        public Enemy(Sprite[] image, Vector2 position, bool alive, int direction, int sight, Weapon weapon, Armour armour, GlobalVar.WeaponType preferredWeapon, GlobalVar.DamageType immune, GlobalVar.DamageType vunerable, GlobalVar.DamageType resistant, GlobalVar.CharacterMode mode)
            : base(image, position, direction)
        {
            inventory = new Inventory(4);

            spriteNum = 0;
            this.alive = alive;

            this.mode = mode;
            velocity = Vector2.Zero;

            CalcBounds();

            blockCode = true;

            inventory.AddItem(weapon);
            this.weapon = (Weapon)inventory.Items[0];

            inventory.AddItem(armour);
            this.armour = (Armour)inventory.Items[1];

            spawnPos = position;

            xpValue = 1;
            
            this.weapon.DrawSprite = true;

            this.preferredWeapon = preferredWeapon;
            this.immune = immune;
            this.vunerable = vunerable;
            this.resistant = resistant;

            if (this.weapon.WeaponType == this.preferredWeapon)
            {
                attack += 2;
            }

            waitTimer = 10000f;
            timer = 0f;

            heroDetected = false;
            perceptionTried = false;

            this.sight = sight;
        }

        #endregion

        #region Properties

        public Inventory Inventory
        {
            get { return inventory; }
        }

        public int Sight
        {
            get { return sight; }
        }

        public Weapon Weapon
        {
            get { return weapon; }
        }

        public Armour Armour
        {
            get { return armour; }
        }

        public int XPValue
        {
            get { return xpValue; }
        }

        public GlobalVar.DamageType Vunerable
        {
            get { return vunerable; }
        }

        public GlobalVar.DamageType Resistant
        {
            get { return resistant; }
        }

        public GlobalVar.DamageType Immune
        {
            get { return immune; }
        }

        public int Gold
        {
            get { return gold; }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (GlobalVar.battleMode && mode != GlobalVar.CharacterMode.Attacking && mode != GlobalVar.CharacterMode.Dying)
            {
                mode = GlobalVar.CharacterMode.Attack;
            }

            velocity = Vector2.Zero;

            if (alive)
            {
                switch (mode)
                {
                    #region Idle
                    case GlobalVar.CharacterMode.Idle:

                        //Has the hero switch from one move mode to another?
                        if (GlobalVar.moveModeSwitched && perceptionTried)
                        {
                            perceptionTried = false;
                            GlobalVar.moveModeSwitched = false;
                        }

                        if (!heroDetected)
                        {
                            if (timer >= waitTimer)
                            {
                                switch (direction)
                                {
                                    case 0:
                                        direction = 1;
                                        break;
                                    case 1:
                                        direction = 0;
                                        break;
                                    case 2:
                                        direction = 3;
                                        break;
                                    case 3:
                                        direction = 2;
                                        break;
                                }

                                timer = 0f;
                            }
                            else
                            {
                                timer += gameTime.ElapsedGameTime.Milliseconds;
                            }
                        }

                        #region Stealth vs Perception Stuff
                        Rectangle perceptionRange = new Rectangle((int)position.X - sight * DungeonMap.TileWidth, (int)position.Y - sight * DungeonMap.TileHeight, Sprite.Width + ((DungeonMap.TileWidth * sight) * 2), Sprite.Height + ((DungeonMap.TileHeight * sight) * 2));

                        if (perceptionRange.Contains((int)GlobalVar.hero.Position.X, (int)GlobalVar.hero.Position.Y))
                        {
                            if (Facing(GlobalVar.hero))
                            {
                                heroDetected = true;
                            }
                            else
                            {
                                if (!perceptionTried)
                                {
                                    perceptionTried = true;
                                    GlobalVar.enemyPerceptionRoll = SkillRoll(perception);

                                    if (GlobalVar.hero.Sneaking)
                                    {
                                        GlobalVar.heroStealthRoll = GlobalVar.hero.SkillRoll(GlobalVar.hero.Stealth);
                                    }
                                    else if (GlobalVar.hero.Sprinting)
                                    {
                                        GlobalVar.heroStealthRoll = GlobalVar.hero.SkillRoll(GlobalVar.hero.Stealth);
                                        GlobalVar.heroStealthRoll -= GlobalVar.hero.StealthMod + 10;
                                    }
                                    else
                                    {
                                        GlobalVar.heroStealthRoll = GlobalVar.hero.SkillRoll(GlobalVar.hero.Stealth);
                                        GlobalVar.heroStealthRoll -= GlobalVar.hero.StealthMod;
                                    }

                                    if (GlobalVar.enemyPerceptionRoll > GlobalVar.heroStealthRoll)
                                    {
                                        switch (direction)
                                        {
                                            case 0:
                                                direction = 1;
                                                break;
                                            case 1:
                                                direction = 0;
                                                break;
                                            case 2:
                                                direction = 3;
                                                break;
                                            case 3:
                                                direction = 2;
                                                break;
                                        }

                                        heroDetected = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            heroDetected = false;
                            perceptionTried = false;
                        }
                        #endregion

                        break;

                    #endregion
                    #region Patrol
                    case GlobalVar.CharacterMode.Patrol:
                        switch (direction)
                        {
                            case 0:
                                velocity.Y -= speed;
                                break;
                            case 1:
                                velocity.Y += speed;
                                break;
                            case 2:
                                velocity.X -= speed;
                                break;
                            case 3:
                                velocity.X += speed;
                                break;
                        }
                        break;
                    #endregion
                    #region Pursue
                    case GlobalVar.CharacterMode.Pursue:

                        break;
                    #endregion
                    #region Attacking
                    case GlobalVar.CharacterMode.Attacking:
                        if (Sprite.FinishedPlaying)
                        {
                            mode = GlobalVar.CharacterMode.Attack;
                            if (GlobalVar.turn == 2)
                            {
                                GlobalVar.turn = 1;
                                GlobalVar.drawBattleMsgString = false;
                            }
                        }
                        break;
                    #endregion


                }
            }

            base.Update(gameTime);


            weapon.MoveTo(HandLoc(), direction, true);
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (alive)
            {
                weapon.Draw(spriteBatch);
            }

            base.Draw(spriteBatch);
        }

        #endregion

        #region Hand Location
        protected virtual Vector2 HandLoc()
        {
            return Vector2.Zero;    //Different enemies have different locations and sizes
        }
        #endregion

        #region Damage Modifiers

        public virtual int ModDamage(GlobalVar.DamageType type, int damage, string specialEffect)
        {
            if (type != GlobalVar.DamageType.NoDamage)
            {
                if (type == vunerable)
                {
                    damage *= 2;
                }
                else if (type == resistant)
                {
                    damage /= 2;
                }
                else if (type == immune)
                {
                    damage = 0;
                }
            }

            if (specialEffect == race)
            {
                damage *= 2;
            }
            else if (specialEffect == "Draining")
            {
                GlobalVar.hero.IncHealth(damage / 2);
            }

            return damage;
        }

        #endregion

        #region Facing

        public bool Facing(Character character)
        {
            bool facing = false; ;

            switch (direction)
            {
                case 0:
                    if (character.Position.Y > position.Y)
                    {
                        facing = true;
                    }
                    else
                    {
                        facing = false;
                    }
                    break;
                case 1:
                    if (character.Position.Y > position.Y)
                    {
                        facing = false;
                    }
                    else
                    {
                        facing = true;
                    }
                    break;
                case 2:
                    if (character.Position.X < position.X)
                    {
                        facing = true;
                    }
                    else
                    {
                        facing = false;
                    }
                    break;
                case 3:
                    if (character.Position.X < position.X)
                    {
                        facing = false;
                    }
                    else
                    {
                        facing = true;
                    }
                    break;
            }

            return facing;
        }
        #endregion
    }
}
