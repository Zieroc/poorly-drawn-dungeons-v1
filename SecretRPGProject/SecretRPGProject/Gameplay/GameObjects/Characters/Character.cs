using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters.Enemies;
using PoorlyDrawnDungeons.Graphics;
using Dungeon_Tile_Engine;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.Characters
{
    [Serializable]
    public class Character : GameObject
    {
        #region Variables

        protected GlobalVar.CharacterMode mode;
        protected String name;
        protected float speed;      //How fast the characters can move
        protected Vector2 velocity; //How fast the character is moving along the x and y
        protected bool blockCode;

        //Statistics
        protected int maxHealth;
        protected int currentHealth;
        protected int maxMana;
        protected int currentMana;
        protected int attack;
        protected int defence;

        protected int strength;
        protected int dexterity;
        protected int intelligence;
        protected int stamina;

        //Skills
        protected int dodge;
        protected int magicSkill;
        protected int perception;
        protected int stealth;
        protected int weaponSkill;
        protected int endurance;
        

        #endregion

        #region Constructor

        public Character(Sprite[] sprite, Vector2 position, int direction)
            :base(sprite, position, direction)
        {
            defence = 10;

            CalcBaseSkills();
        }

        #endregion

        #region Properties

        public GlobalVar.CharacterMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        public String Name
        {
            get { return name; }
        }

        public int MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }

        public int CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }

        public int MaxMana
        {
            get { return maxMana; }
            set { maxMana = value; }
        }

        public int CurrentMana
        {
            get { return currentMana; }
            set { currentMana = value; }
        }

        public int Attack
        {
            get { return attack; }
            set { attack = value; }
        }

        public int Defence
        {
            get { return defence; }
            set { defence = value; }
        }

        public int Strength
        {
            get { return strength; }
            set { strength = value; }
        }

        public int Dexterity
        {
            get { return dexterity; }
            set { dexterity = value; }
        }

        public int Intelligence
        {
            get { return intelligence; }
            set { intelligence = value; }
        }

        public int Stamina
        {
            get { return stamina; }
            set { stamina = value; }
        }

        public int Dodge
        {
            get { return dodge; }
            set { dodge = value; }
        }

        public int MagicSkill
        {
            get { return magicSkill; }
            set { magicSkill = value; }
        }

        public int Perception
        {
            get { return perception; }
            set { perception = value; }
        }

        public int Stealth
        {
            get { return stealth; }
            set { stealth = value; }
        }

        public int WeaponSkill
        {
            get { return weaponSkill; }
            set { weaponSkill = value; }
        }

        public int Endurance
        {
            get { return endurance; }
            set { endurance = value; }
        }

        #endregion

        #region Stat Methods

        public void IncAttack(int amount)
        {
            attack += amount;
        }

        public void DecAttack(int amount)
        {
            attack -= amount;
        }

        public void IncDefence(int amount)
        {
            defence += amount;
        }

        public void DecDefence(int amount)
        {
            defence -= amount;
        }

        public void IncHealth(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        public void DecHealth(int amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                mode = GlobalVar.CharacterMode.Dying;
            }
        }

        public void FillHealth()
        {
            currentHealth = maxHealth;
        }

        public void UpgradeHealth(int amount)
        {
            maxHealth += amount;
            currentHealth = maxHealth;
        }

        public void IncMana(int amount)
        {
            currentMana += amount;
            if (currentMana > maxMana)
            {
                currentMana = maxMana;
            }
        }

        public void DecMana(int amount)
        {
            currentMana -= amount;
        }

        public void FillMana()
        {
            currentMana = maxMana;
        }

        public void UpgradeMana(int amount)
        {
            maxMana += amount;
            currentMana = maxMana;
        }

        public void IncStrength(int amount)
        {
            strength += amount;
        }

        public void DecStrength(int amount)
        {
            strength -= amount;
        }

        public void IncDexterity(int amount)
        {
            dexterity += amount;
        }

        public void DecDexterity(int amount)
        {
            dexterity -= amount;
        }

        public void IncIntelligence(int amount)
        {
            intelligence += amount;
        }

        public void DecIntelligence(int amount)
        {
            intelligence -= amount;
        }

        public void IncStamina(int amount)
        {
            stamina += amount;
        }

        public void DecStamina(int amount)
        {
            stamina -= amount;
        }

        #endregion

        #region Kill, Revive

        public override void Kill()
        {
            currentHealth = 0;

            base.Kill();
        }

        public override void Revive()
        {
            currentHealth = MaxHealth;
            currentMana = MaxMana;

            base.Revive();
        }
        #endregion

        #region Collision

        //Check horizontal collison, check them seperatly because of gridded movement
        private Vector2 HorizontalCollisionTest(Vector2 moveAmount)
        {
            if (moveAmount.X == 0)
                return moveAmount;

            //Find out where the object will be if it moves
            Rectangle afterMoveRect = bounds;
            afterMoveRect.Offset((int)moveAmount.X, 0);
            Vector2 corner1, corner2;

            //Find the corners of our object after it moves
            if (moveAmount.X < 0)
            {
                corner1 = new Vector2(afterMoveRect.Left, afterMoveRect.Top + 1);
                corner2 = new Vector2(afterMoveRect.Left, afterMoveRect.Bottom - 1);
            }
            else
            {
                corner1 = new Vector2(afterMoveRect.Right, afterMoveRect.Top + 1);
                corner2 = new Vector2(afterMoveRect.Right, afterMoveRect.Bottom - 1);
            }

            //What cells of our map would the object be in if it moves
            Vector2 dungeonTile1 = DungeonMap.GetCellByPoint(corner1);
            Vector2 dungeonTile2 = DungeonMap.GetCellByPoint(corner2);

            //If it will be an impassable cell then don't let the object move
            if (!DungeonMap.CellIsPassable(dungeonTile1) || !DungeonMap.CellIsPassable(dungeonTile2))
            {
                moveAmount.X = 0;
                velocity.X = 0;
            }

            if (blockCode)
            {
                if (DungeonMap.CellCode(dungeonTile1) == "BLOCK" || DungeonMap.CellCode(dungeonTile2) == "BLOCK")
                {
                    moveAmount.X = 0;
                    velocity.X = 0;
                }
            }

            //Check for chests
            if (DungeonMap.CellCode(dungeonTile1) == "CHEST0" || DungeonMap.CellCode(dungeonTile2) == "CHEST0")
            {
                moveAmount.X = 0;
                velocity.X = 0;
            }
            if (DungeonMap.CellCode(dungeonTile1) == "CHEST1" || DungeonMap.CellCode(dungeonTile2) == "CHEST1")
            {
                moveAmount.X = 0;
                velocity.X = 0;
            }
            if (DungeonMap.CellCode(dungeonTile1) == "CHEST2" || DungeonMap.CellCode(dungeonTile2) == "CHEST2")
            {
                moveAmount.X = 0;
                velocity.X = 0;
            }
            if (DungeonMap.CellCode(dungeonTile1) == "CHEST3" || DungeonMap.CellCode(dungeonTile2) == "CHEST3")
            {
                moveAmount.X = 0;
                velocity.X = 0;
            }
            if (DungeonMap.CellCode(dungeonTile1).StartsWith("UIT") || DungeonMap.CellCode(dungeonTile2).StartsWith("UIT"))
            {
                moveAmount.X = 0;
                velocity.X = 0;
            }

            //Check for NPCs
            if (DungeonMap.CellCode(dungeonTile1).StartsWith("NPC") || DungeonMap.CellCode(dungeonTile2).StartsWith("NPC"))
            {
                moveAmount.X = 0;
                velocity.X = 0;
            }

            return moveAmount;
        }

        //Check vertical collison, check them seperatly because of gridded movement
        private Vector2 VerticalCollisionTest(Vector2 moveAmount)
        {
            if (moveAmount.Y == 0)
                return moveAmount;

            Rectangle afterMoveRect = bounds;
            afterMoveRect.Offset((int)moveAmount.X, (int)moveAmount.Y);
            Vector2 corner1, corner2;

            if (moveAmount.Y < 0)
            {
                corner1 = new Vector2(afterMoveRect.Left + 1, afterMoveRect.Top);
                corner2 = new Vector2(afterMoveRect.Right - 1, afterMoveRect.Top);
            }
            else
            {
                corner1 = new Vector2(afterMoveRect.Left + 1, afterMoveRect.Bottom);
                corner2 = new Vector2(afterMoveRect.Right - 1, afterMoveRect.Bottom);
            }

            Vector2 dungeonTile1 = DungeonMap.GetCellByPoint(corner1);
            Vector2 dungeonTile2 = DungeonMap.GetCellByPoint(corner2);

            //If it will be an impassable cell then don't let the object move
            if (!DungeonMap.CellIsPassable(dungeonTile1) || !DungeonMap.CellIsPassable(dungeonTile2))
            {
                moveAmount.Y = 0;
                velocity.Y = 0;
            }
            if (blockCode)
            {
                if (DungeonMap.CellCode(dungeonTile1) == "BLOCK" || DungeonMap.CellCode(dungeonTile2) == "BLOCK")
                {
                    moveAmount.Y = 0;
                    velocity.Y = 0;
                }
            }

            //Check for chests
            if (DungeonMap.CellCode(dungeonTile1) == "CHEST0" || DungeonMap.CellCode(dungeonTile2) == "CHEST0")
            {
                moveAmount.Y = 0;
                velocity.Y = 0;
            }
            if (DungeonMap.CellCode(dungeonTile1) == "CHEST1" || DungeonMap.CellCode(dungeonTile2) == "CHEST1")
            {
                moveAmount.Y = 0;
                velocity.Y = 0;
            }
            if (DungeonMap.CellCode(dungeonTile1) == "CHEST2" || DungeonMap.CellCode(dungeonTile2) == "CHEST2")
            {
                moveAmount.Y = 0;
                velocity.Y = 0;
            }
            if (DungeonMap.CellCode(dungeonTile1) == "CHEST3" || DungeonMap.CellCode(dungeonTile2) == "CHEST3")
            {
                moveAmount.Y = 0;
                velocity.Y = 0;
            }
            if (DungeonMap.CellCode(dungeonTile1).StartsWith("UIT") || DungeonMap.CellCode(dungeonTile2).StartsWith("UIT"))
            {
                moveAmount.Y = 0;
                velocity.Y = 0;
            }

            //Check for NPCs
            if (DungeonMap.CellCode(dungeonTile1).StartsWith("NPC") || DungeonMap.CellCode(dungeonTile2).StartsWith("NPC"))
            {
                moveAmount.Y = 0;
                velocity.Y = 0;
            }

            return moveAmount;
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            //Do nothing if object is dead
            if (!alive)
                return;

            if (mode == GlobalVar.CharacterMode.Dying && Sprite.FinishedPlaying)
            {
                Kill();
            }

            if (!GlobalVar.paused || GlobalVar.battleMode)
            {
                int newSpriteNum;

                switch (mode)
                {
                    #region Idle
                    case GlobalVar.CharacterMode.Idle:
                    case GlobalVar.CharacterMode.Attack:
                        newSpriteNum = 0;
                        break;
                    #endregion
                    #region Patrol & Pursue
                    case GlobalVar.CharacterMode.Patrol:
                    case GlobalVar.CharacterMode.Pursue:
                        newSpriteNum = 1;
                        break;
                    #endregion
                    #region Attacking
                    case GlobalVar.CharacterMode.Attacking:
                        newSpriteNum = 2;
                        break;
                    #endregion
                    #region Dying
                    case GlobalVar.CharacterMode.Dying:
                        newSpriteNum = 3;
                        break;
                    #endregion
                    #region Casting
                    case GlobalVar.CharacterMode.Casting:
                        newSpriteNum = 4;
                        break;
                    #endregion


                    default:
                        newSpriteNum = 0;
                        break;
                }

                //How much time has elapsed since last call - used for smoother movement
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                //How far we want to move
                Vector2 moveAmount = velocity * elapsed;

                //Restrict or movement if we will collided with an impassable tile
                moveAmount = HorizontalCollisionTest(moveAmount);
                moveAmount = VerticalCollisionTest(moveAmount);

                //This is what or new position will be with our allowed movement
                Vector2 newPosition = position + moveAmount;

                if (position == newPosition && !GlobalVar.battleMode)
                {
                    if(this is Player)
                    {
                        mode = GlobalVar.CharacterMode.Idle;
                        newSpriteNum = 0;
                    }
                    else if (this is Enemy || this is NPC)
                    {
                        if (mode == GlobalVar.CharacterMode.Patrol)
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
                        }
                    }
                }

                //Move the object to the new position
                position = newPosition;

                if (newSpriteNum != spriteNum)
                {
                    spriteNum = newSpriteNum;
                    Sprite.CurrentState = 0;
                }

                if (Sprite.Direction != direction)
                {
                    Sprite.Direction = direction;
                    Sprite.CurrentState = 0;
                }

                //Update the sprite
                sprite[spriteNum].Update(gameTime);

                //Object has moved so move the collision box to ensure collisions work correctly
                CalcBounds();
            }
        }
        #endregion

        #region Serializable Methods

        public Character(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {
            this.mode = (GlobalVar.CharacterMode)info.GetValue("Mode", typeof(GlobalVar.CharacterMode));
            this.name = (String)info.GetValue("Name", typeof(String));
            this.speed = (float)info.GetValue("Speed", typeof(float));
            this.velocity = (Vector2)info.GetValue("Velocity", typeof(Vector2));
            this.blockCode = (bool)info.GetValue("BlockCode", typeof(bool));
            this.maxHealth = (int)info.GetValue("MaxHealth", typeof(int));
            this.currentHealth = (int)info.GetValue("CurrentHealth", typeof(int));
            this.attack = (int)info.GetValue("Attack", typeof(int));
            this.defence = (int)info.GetValue("Defence", typeof(int));
            this.strength = (int)info.GetValue("Strength", typeof(int));
            this.dexterity = (int)info.GetValue("Dex", typeof(int));
            this.intelligence = (int)info.GetValue("Int", typeof(int));
            this.stamina = (int)info.GetValue("Stamina", typeof(int));
            this.maxMana = (int)info.GetValue("MaxMana", typeof(int));
            this.currentMana = (int)info.GetValue("CurrentMana", typeof(int));
            this.dodge = (int)info.GetValue("Dodge", typeof(int));
            this.magicSkill = (int)info.GetValue("MagicSkill", typeof(int));
            this.perception = (int)info.GetValue("Perception", typeof(int));
            this.stealth = (int)info.GetValue("Stealth", typeof(int));
            this.weaponSkill = (int)info.GetValue("WeaponSkill", typeof(int));
            this.endurance = (int)info.GetValue("Endurance", typeof(int));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Mode", this.mode);
            info.AddValue("Name", this.name);
            info.AddValue("Speed", this.speed);
            info.AddValue("Velocity", this.velocity);
            info.AddValue("BlockCode", this.blockCode);
            info.AddValue("MaxHealth", this.maxHealth);
            info.AddValue("CurrentHealth", this.currentHealth);
            info.AddValue("Attack", this.attack);
            info.AddValue("Defence", this.defence);
            info.AddValue("Strength", this.strength);
            info.AddValue("Dex", this.dexterity);
            info.AddValue("Int", this.intelligence);
            info.AddValue("Stamina", this.stamina);
            info.AddValue("MaxMana", this.maxMana);
            info.AddValue("CurrentMana", this.currentMana);
            info.AddValue("Dodge", this.dodge);
            info.AddValue("MagicSkill", this.magicSkill);
            info.AddValue("Perception", this.perception);
            info.AddValue("Stealth", this.stealth);
            info.AddValue("WeaponSkill", this.weaponSkill);
            info.AddValue("Endurance", this.endurance);

            base.GetObjectData(info, ctxt);
        }

        #endregion

        #region LOS

        protected bool HasLOS(GameObject target)
        {

            //get character’s tile position
            Point p0 = new Point((int)PositionCenter.X, (int)PositionCenter.Y);
            Point line0 = p0;
            //get target tile position
            Point p1 = new Point((int)target.PositionCenter.X, (int)target.PositionCenter.Y);
            Point line1 = p1;

            //begin calculating line
            bool steep = Math.Abs(p1.Y - p0.Y) > Math.Abs(p1.X - p0.X);

            if (steep)
            {
                //swap points due to steep slope
                Point tmpPoint = new Point(p0.X, p0.Y);
                p0 = new Point(tmpPoint.Y, tmpPoint.X);
                tmpPoint = p1;
                p1 = new Point(tmpPoint.Y, tmpPoint.X);
            }

            int deltaX = (int)Math.Abs(p1.X - p0.X);
            int deltaY = (int)Math.Abs(p1.Y - p0.Y);
            int error = 0;
            int deltaError = deltaY;
            int yStep = 0, xStep = 0;
            int x = p0.X, y = p0.Y;
            if (p0.Y < p1.Y) yStep = 4;
            else yStep = -4;
            if (p0.X < p1.X) xStep = 4;
            else xStep = -4;
            int tmpX = 0, tmpY = 0;
            while (x != p1.X)
            {
                x += xStep;
                error += deltaError;
                //move one along on the Y axis
                if ((2 * error) > deltaX)
                {
                    y += yStep;
                    error -= deltaX;
                }
                //flip the coords if steep
                if (steep)
                {
                    tmpX = y;
                    tmpY = x;
                }
                else
                {
                    tmpX = x;
                    tmpY = y;
                }
                //make sure coords are legal
                if (Camera.ObjectIsVisible(bounds))
                {
                    //is this a collidable tile?
                    Tile ts = DungeonMap.GetTileAtPoint(tmpX, tmpY);
                    if (!ts.Passable)
                        return false;
                }
                else
                    //not legal coords
                    return false;
            }

            return true;
        }

        #endregion

        #region Stat Bonus

        public int StatBonus(int stat)
        {
            int bonus = (stat -= 10) / 2;
            if (bonus < 0)
            {
                bonus = 0;
            }

            return bonus;
        }
        #endregion

        #region Skills

        public int SkillRoll(int skill)
        {
            Random dieRoll = new Random();

            int roll = dieRoll.Next(20)  + 1 + skill;

            return roll;
        }

        public void CalcBaseSkills()
        {
            dodge = StatBonus(dexterity);
            magicSkill = StatBonus(intelligence);
            perception = StatBonus(intelligence);
            stealth = StatBonus(dexterity);
            weaponSkill = StatBonus(strength);
            endurance = StatBonus(stamina);

        }

        #endregion
    }
}
