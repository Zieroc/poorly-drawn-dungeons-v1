using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PoorlyDrawnDungeons.Gameplay.Auras;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Items;
using PoorlyDrawnDungeons.Gameplay.Magic;
using PoorlyDrawnDungeons.Graphics;
using PoorlyDrawnDungeons.Utility;
using Dungeon_Tile_Engine;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.Characters
{
    [Serializable]
    public class Player : Character
    {
        #region Variables

        private Inventory inventory;
        private SpellBook spellBook;    //Will only be available to magical classes

        private int level;
        private int xp;
        private int xpReq;

        private int gold;               //How much Gold do we have

        private Light vision;           //The player's default light source is there own eyes
        private Light torch;            //The light the player is carrying with them
        private bool lightEquipped;

        private Weapon weapon;          //The weapon the player has equipped
        private Armour armour;          //The armour the player has equipeed

        private bool escapedPressed;
        private bool tabPressed;
        private bool shiftPressed;
        private bool ctrlPressed;

        private GlobalVar.CharacterClass characterClass;
        private GlobalVar.WeaponType preferredWeapon;
        private bool profBonus;

        private int spell;              //The index number of the player's selected spell;

        private float sprintSpeed;      //What speed do we move when sprinting
        private bool sprinting;         //Are we sprinting?

        private float sneakSpeed;
        private bool sneaking;

        //Trained modifiers
        private int dodgeMod;
        private int magicSkillMod;
        private int perceptionMod;
        private int stealthMod;
        private int weaponSkillMod;
        private int enduranceMod;

        #endregion

        #region Constructor


        public Player(Sprite[] image, Light vision, Weapon weapon, Armour armour)
            : this(image, Vector2.Zero, false, vision, weapon, armour, GlobalVar.CharacterClass.Warrior)
        {
        }

        public Player(Sprite[] image, Vector2 position, bool alive, Light vision, Weapon weapon, Armour armour, GlobalVar.CharacterClass characterClass)
            :base(image, position, 0)
        {
            direction = 0;
            profBonus = false;
            inventory = new Inventory(32);

            spellBook = new SpellBook();
            spell = 0;

            gold = 0;

            spriteNum = 0;
            this.alive = alive;

            mode = GlobalVar.CharacterMode.Idle;
            name = "Bob";
            speed = 48;
            velocity = Vector2.Zero;
            CalcBounds();

            sprintSpeed = 96;
            sprinting = false;

            sneakSpeed = 32;
            sneaking = false;

            this.vision = vision;
            torch = vision;
            lightEquipped = false;
            LightManager.AddLight(this.vision);

            this.inventory.AddItem(weapon);
            this.inventory.Items[0].OnClick(this);

            this.inventory.AddItem(armour);
            this.inventory.Items[1].OnClick(this);

            escapedPressed = false;
            shiftPressed = false;
            tabPressed = false;
            blockCode = false;

            level = 1;
            xp = 0;
            xpReq = 100;

            this.characterClass = characterClass;

            //Calculate stats

            Random die = new Random();


            strength = 6 + (die.Next(6) + 1);
            dexterity = 6 + (die.Next(6) + 1);
            intelligence = 6 + (die.Next(6) + 1);
            stamina = 6 + (die.Next(6) + 1);

            dodgeMod = 0;
            magicSkillMod = 0;
            perceptionMod = 0;
            stealthMod = 0;
            weaponSkillMod = 0;
            enduranceMod = 0;

            //Modifiers based on class
            switch (characterClass)
            {
                case GlobalVar.CharacterClass.Cleric:
                    intelligence += 5;
                    strength += 2;
                    stamina += 1;

                    preferredWeapon = GlobalVar.WeaponType.Mace;
                    spellBook.GainSpell(GlobalVar.prayerLevels[0]);

                    //enduranceMod += ?;
                    //
                    break;
                case GlobalVar.CharacterClass.Thief:
                    dexterity += 5;
                    strength += 2;
                    stamina += 1;

                    preferredWeapon = GlobalVar.WeaponType.Dagger;

                    //dodgeMod += ?;
                    //stealthMod += ?;
                    break;
                case GlobalVar.CharacterClass.Warrior:
                    strength += 5;
                    stamina += 2;
                    dexterity += 1;

                    preferredWeapon = GlobalVar.WeaponType.Sword;

                    //weaponSkillMod += ?;
                    //enduranceMod += ?;
                    break;
                case GlobalVar.CharacterClass.Wizard:
                    intelligence += 5;
                    stamina += 2;
                    dexterity += 1;

                    preferredWeapon = GlobalVar.WeaponType.Staff;
                    spellBook.GainSpell(GlobalVar.spellLevels[0]);

                    //magicSkillMod += ?;
                    //perceptionMod += ?;
                    break;
                default:
                    break;
            }

            maxHealth = 8 + die.Next(8) + 1 + stamina;
            currentHealth = maxHealth;
            attack += StatBonus(strength);
            if (characterClass == GlobalVar.CharacterClass.Wizard)
            {
                maxMana = die.Next(6) + 1 + intelligence;
            }
            else
            {
                maxMana = die.Next(6) + 1 + StatBonus(intelligence);
            }
            currentMana = maxMana;

            CalcAttack();
            CalcDefence();
        }

        #endregion

        #region Properties

        public Inventory Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }

        public SpellBook SpellBook
        {
            get { return spellBook; }
            set { spellBook = value; }
        }

        public int Spell
        {
            get { return spell; }
            set { spell = value; }
        }

        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public int XP
        {
            get { return xp; }
            set { xp = value; }
        }

        public int XPReq
        {
            get { return xpReq; }
            set { xpReq = value; }
        }

        public Light Vision
        {
            get { return vision; }
        }

        public Light Torch
        {
            get { return torch; }
            set { torch = value; }
        }

        public bool LightEquipped
        {
            get { return lightEquipped; }
            set { lightEquipped = value; }
        }

        public Weapon Weapon
        {
            get { return weapon; }
            set { weapon = value; }
        }

        public Armour Armour
        {
            get { return armour; }
            set { armour = value; }
        }

        public GlobalVar.CharacterClass CharacterClass
        {
            get { return characterClass; }
            set { characterClass = value; }
        }

        public GlobalVar.WeaponType PreferredWeapon
        {
            get { return preferredWeapon; }
            set { preferredWeapon = value; }
        }

        public bool ProfBonus
        {
            get { return profBonus; }
            set { profBonus = value; }
        }

        public int Gold
        {
            get { return gold; }
            set { gold = value; }
        }

        public bool Sneaking
        {
            get { return sneaking; }
            set { sneaking = value; }
        }

        public bool Sprinting
        {
            get { return sprinting; }
            set { sprinting = value; }
        }

        public int DodgeMod
        {
            get { return dodgeMod; }
            set { dodgeMod = value; }
        }

        public int MagicSkillMod
        {
            get { return magicSkillMod; }
            set { magicSkillMod = value; }
        }

        public int PerceptionMod
        {
            get { return perceptionMod; }
            set { perceptionMod = value; }
        }

        public int StealthMod
        {
            get { return stealthMod; }
            set { stealthMod = value; }
        }

        public int WeaponSkillMod
        {
            get { return weaponSkillMod; }
            set { weaponSkillMod = value; }
        }

        public int EnduranceMod
        {
            get { return enduranceMod; }
            set { enduranceMod = value; }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (GlobalVar.battleMode && (mode != GlobalVar.CharacterMode.Attacking && mode != GlobalVar.CharacterMode.Casting && mode != GlobalVar.CharacterMode.Dying))
            {
                mode = GlobalVar.CharacterMode.Attack;
            }

            velocity = Vector2.Zero; //Reset velocity so that player moves smoothly

            KeyboardState keyState = Keyboard.GetState();

            float actualSpeed = speed;

            if (sprinting)
            {
                actualSpeed = sprintSpeed;

                if (armour.Type == GlobalVar.ArmourType.Heavy && characterClass != GlobalVar.CharacterClass.Warrior)
                {
                    actualSpeed -= 24;
                }
            }
            else if (sneaking)
            {
                if (characterClass != GlobalVar.CharacterClass.Thief)
                {
                    actualSpeed = sneakSpeed;
                }
            }

            switch (mode)
            {
                #region Idle
                case GlobalVar.CharacterMode.Idle:
                    if (!GlobalVar.paused)
                    {
                        if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
                        {
                            mode = GlobalVar.CharacterMode.Patrol;
                            velocity.Y -= actualSpeed;
                            direction = 0;
                        }
                        else if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                        {
                            mode = GlobalVar.CharacterMode.Patrol;
                            velocity.Y += actualSpeed;
                            direction = 1;
                        }
                        else if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                        {
                            mode = GlobalVar.CharacterMode.Patrol;
                            velocity.X -= actualSpeed;
                            direction = 2;
                        }
                        else if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                        {
                            mode = GlobalVar.CharacterMode.Patrol;
                            velocity.X += actualSpeed;
                            direction = 3;
                        }
                    }

                    break;
                #endregion
                #region Patrol
                case GlobalVar.CharacterMode.Patrol:

                    if (!GlobalVar.paused)
                    {
                        if (keyState.IsKeyDown(Keys.Up)  || keyState.IsKeyDown(Keys.W))
                        {
                            velocity.Y -= actualSpeed;
                            direction = 0;
                        }
                        else if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                        {
                            velocity.Y += actualSpeed;
                            direction = 1;
                        }
                        else if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                        {
                            velocity.X -= actualSpeed;
                            direction = 2;
                        }
                        else if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                        {
                            velocity.X += actualSpeed;
                            direction = 3;
                        }

                        if (keyState.IsKeyUp(Keys.Down) && keyState.IsKeyUp(Keys.S) && keyState.IsKeyUp(Keys.Up) && keyState.IsKeyUp(Keys.W) && keyState.IsKeyUp(Keys.Left) && keyState.IsKeyUp(Keys.A) && keyState.IsKeyUp(Keys.Right) && keyState.IsKeyUp(Keys.D))
                        {
                            mode = GlobalVar.CharacterMode.Idle;
                        }
                    }
                    break;
                #endregion
                #region Attack
                case GlobalVar.CharacterMode.Attack:
                    //Get Mouse State
                    MouseState ms = Mouse.GetState();

                    if (GlobalVar.turn == 1)
                    {
                        if (GlobalVar.defending)
                        {
                            GlobalVar.defending = false;
                            defence -= dodge;
                        }

                        if (ms.LeftButton == ButtonState.Pressed && GlobalVar.mouseReleased)
                        {
                            GlobalVar.mouseReleased = false;
                            if (GlobalVar.lightAttackRec.Contains(ms.X, ms.Y))
                            {
                                mode = GlobalVar.CharacterMode.Attacking;
                                int attackRoll;
                                Random dieRoll = new Random();

                                attackRoll = weaponSkill + attack + (dieRoll.Next(20) + 1);

                                if (GlobalVar.dodgeBonus)
                                {
                                    GlobalVar.dodgeBonus = false;
                                    attackRoll += 2;
                                }

                                if (attackRoll >= GlobalVar.enemy.Defence)
                                {
                                    int damageRoll = GlobalVar.enemy.ModDamage(weapon.DamageType, weapon.CalcDamage(), weapon.SpecialEffect);
                                    GlobalVar.battleMsg = damageRoll.ToString();

                                    GlobalVar.enemy.DecHealth(damageRoll);
                                }
                                else
                                {
                                    GlobalVar.battleMsg = "Miss";
                                }

                                GlobalVar.drawBattleMsgString = true;
                                GlobalVar.drawMagicMsg = false;
                            }
                            else if (GlobalVar.heavyAttackRec.Contains(ms.X, ms.Y))
                            {
                                mode = GlobalVar.CharacterMode.Attacking;
                                int attackRoll;
                                Random dieRoll = new Random();

                                attackRoll = attack + (dieRoll.Next(20) + 1);

                                if (GlobalVar.dodgeBonus)
                                {
                                    GlobalVar.dodgeBonus = false;
                                    attackRoll += 2;
                                }
                                
                                if (attackRoll >= GlobalVar.enemy.Defence)
                                {
                                    int damageRoll = weapon.CalcDamage() + weaponSkill;

                                    damageRoll = GlobalVar.enemy.ModDamage(weapon.DamageType, damageRoll, weapon.SpecialEffect);
                                    
                                    GlobalVar.battleMsg = damageRoll.ToString();

                                    GlobalVar.enemy.DecHealth(damageRoll);
                                }
                                else
                                {
                                    GlobalVar.battleMsg = "Miss";
                                }

                                GlobalVar.drawBattleMsgString = true;
                                GlobalVar.drawMagicMsg = false;
                            }
                            else if (GlobalVar.defendRec.Contains(ms.X, ms.Y))
                            {
                                if (GlobalVar.dodgeBonus)
                                {
                                    GlobalVar.dodgeBonus = false;
                                }

                                GlobalVar.defending = true;
                                defence += dodge;
                                GlobalVar.turn++;
                            }
                            else if (GlobalVar.magicRec.Contains(ms.X, ms.Y))
                            {
                                if (spellBook.Spells[spell].CanCast())
                                {
                                    mode = GlobalVar.CharacterMode.Casting;
                                    spellBook.Spells[spell].Cast();
                                }

                            }
                            else
                            {
                                GlobalVar.mouseReleased = true;
                            }
                        }
                    }
                    break;
                #endregion
                #region Attacking
                case GlobalVar.CharacterMode.Attacking:
                    if (Sprite.FinishedPlaying)
                    {
                        mode = GlobalVar.CharacterMode.Attack;
                        GlobalVar.turn++;
                        GlobalVar.drawBattleMsgString = false;
                        GlobalVar.drawMagicMsg = false;
                    }
                    break;
                #endregion
                #region Casting
                case GlobalVar.CharacterMode.Casting:
                    if (Sprite.FinishedPlaying)
                    {
                        mode = GlobalVar.CharacterMode.Attack;
                        GlobalVar.turn++;
                        GlobalVar.drawBattleMsgString = false;
                        GlobalVar.drawMagicMsg = false;
                    }
                    break;
                #endregion
            }

            #region EscapePressed
            if (keyState.IsKeyDown(Keys.Escape) && !escapedPressed)
            {
                escapedPressed = true;

                if (!GlobalVar.battleMode && !GlobalVar.battleTransition && !GlobalVar.levelMenu)
                {
                    if (GlobalVar.displayChestBar)
                    {
                        GlobalVar.displayChest.Close();
                    }
                    else if (GlobalVar.displayShop)
                    {
                        GlobalVar.displayShop = false;
                        GlobalVar.paused = false;
                    }
                    else if (GlobalVar.displayDialogue)
                    {
                        GlobalVar.displayDialogue = false;
                        GlobalVar.paused = false;
                    }
                    else
                    {
                        GlobalVar.paused = !GlobalVar.paused;
                    }
                }
            }
            else if(keyState.IsKeyUp(Keys.Escape))
            {
                escapedPressed = false;
            }
            #endregion

            #region TabPressed
            if (keyState.IsKeyDown(Keys.Tab) && !tabPressed)
            {
                tabPressed = true;

                if (characterClass == GlobalVar.CharacterClass.Cleric || characterClass == GlobalVar.CharacterClass.Wizard)
                {
                    GlobalVar.displaySpellBook = !GlobalVar.displaySpellBook;
                }
            }
            else if (keyState.IsKeyUp(Keys.Tab))
            {
                tabPressed = false;
            }
            #endregion

            #region Shift or Control Pressed

            if((keyState.IsKeyDown(Keys.LeftShift) || keyState.IsKeyDown(Keys.RightShift)) && !shiftPressed)
            {
                shiftPressed = true;
                sprinting = !sprinting;
                sneaking = false;
                GlobalVar.moveModeSwitched = true;
            }
            else if (keyState.IsKeyUp(Keys.LeftShift) && keyState.IsKeyUp(Keys.RightShift))
            {
                shiftPressed = false;
            }

            if ((keyState.IsKeyDown(Keys.LeftControl) || keyState.IsKeyDown(Keys.RightControl)) && !ctrlPressed)
            {
                ctrlPressed = true;
                sneaking = !sneaking;
                sprinting = false;
                GlobalVar.moveModeSwitched = true;
            }
            else if (keyState.IsKeyUp(Keys.LeftControl) && keyState.IsKeyUp(Keys.RightControl))
            {
                ctrlPressed = false;
            }

            #endregion

            base.Update(gameTime);

            #region Move Carried Items
            vision.MoveTo(PositionCenter, direction, false);
            if (torch != vision)
            {
                torch.MoveTo(OffHandPosition(), direction, true);
            }

            if (lightEquipped && !torch.Alive)
            {
                lightEquipped = false;
                vision.SwitchLit();
            }

            if (weapon != null)
            {
                weapon.MoveTo(MainHandPosition(), direction, true);
            }
            #endregion

            #region Equipped Items Lost
            //Switch an dead item to be inactive and lose the bonus
            if (!weapon.Alive && weapon.Active)
            {
                EquipWeapon(GlobalVar.unarmed);
            }

            if (armour != null && !armour.Alive && armour.Active)
            {
                EquipArmour(GlobalVar.unarmoured);
            }
            #endregion

            //Update the inventory
            inventory.Update(gameTime);
            spellBook.Update(gameTime);

            //Call reposition camera in case the player has moved near the camera edge
            RepositionCamera();
        }
        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            weapon.Draw(spriteBatch);

            base.Draw(spriteBatch);
        }

        #endregion

        #region Private Methods

        //Method only used by the player class so made private
        private void RepositionCamera()
        {
            Vector2 cameraPosition = new Vector2(PositionCenter.X - Camera.ViewPortWidth / 2, PositionCenter.Y - Camera.ViewPortHeight / 2);

            Camera.Position = cameraPosition;
        }

        private Vector2 MainHandPosition()
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

        private Vector2 OffHandPosition()
        {
            Vector2 handLoc;

            if (mode == GlobalVar.CharacterMode.Patrol)
            {
                switch (direction)
                {
                    case 0:
                        if (Sprite.CurrentState == 0)
                        {
                            handLoc = new Vector2(position.X + Sprite.Width, position.Y + 15);
                        }
                        else
                        {
                            handLoc = new Vector2(position.X + Sprite.Width, position.Y + 11);
                        }
                        break;
                    case 1:
                        if (Sprite.CurrentState == 0)
                        {
                            handLoc = new Vector2(position.X, position.Y + 17);
                        }
                        else
                        {
                            handLoc = new Vector2(position.X, position.Y + 21);
                        }
                        break;
                    case 2:
                        if (Sprite.CurrentState == 0)
                        {
                            handLoc = new Vector2(position.X + 15, position.Y);
                        }
                        else
                        {
                            handLoc = new Vector2(position.X + 11, position.Y);
                        }
                        break;
                    default:
                        if (Sprite.CurrentState == 0)
                        {
                            handLoc = new Vector2(position.X + 17, position.Y + Sprite.Height);
                        }
                        else
                        {
                            handLoc = new Vector2(position.X + 21, position.Y + Sprite.Height);
                        }
                        break;
                }
            }
            else if (mode == GlobalVar.CharacterMode.Casting)
            {
                switch (direction)
                {
                    case 0:
                        if (Sprite.CurrentState == 0 || Sprite.CurrentState == 4)
                        {
                            handLoc = new Vector2(position.X + Sprite.Width, position.Y + 15);
                        }
                        else if (Sprite.CurrentState == 1 || Sprite.CurrentState == 3)
                        {
                            handLoc = new Vector2(position.X + Sprite.Width, position.Y + 11);
                        }
                        else
                        {
                            handLoc = new Vector2(position.X + Sprite.Width, position.Y + 7);
                        }
                        break;
                    case 1:
                        if (Sprite.CurrentState == 0 || Sprite.CurrentState == 4)
                        {
                            handLoc = new Vector2(position.X, position.Y + 17);
                        }
                        else if (Sprite.CurrentState == 1 || Sprite.CurrentState == 3)
                        {
                            handLoc = new Vector2(position.X, position.Y + 21);
                        }
                        else
                        {
                            handLoc = new Vector2(position.X, position.Y + 25);
                        }
                        break;
                    case 2:
                        if (Sprite.CurrentState == 0 || Sprite.CurrentState == 4)
                        {
                            handLoc = new Vector2(position.X + 15, position.Y);
                        }
                        else if (Sprite.CurrentState == 1 || Sprite.CurrentState == 3)
                        {
                            handLoc = new Vector2(position.X + 11, position.Y);
                        }
                        else
                        {
                            handLoc = new Vector2(position.X + 7, position.Y);
                        }
                        break;
                    default:
                        if (Sprite.CurrentState == 0 || Sprite.CurrentState == 4)
                        {
                            handLoc = new Vector2(position.X + 17, position.Y + Sprite.Height);
                        }
                        else if (Sprite.CurrentState == 1 || Sprite.CurrentState == 3)
                        {
                            handLoc = new Vector2(position.X + 21, position.Y + Sprite.Height);
                        }
                        else
                        {
                            handLoc = new Vector2(position.X + 25, position.Y + Sprite.Height);
                        }
                        break;
                }
            }
            else
            {
                switch (direction)
                {
                    case 0:
                        handLoc = new Vector2(position.X + Sprite.Width, position.Y + 15);
                        break;
                    case 1:
                        handLoc = new Vector2(position.X, position.Y + 17);
                        break;
                    case 2:
                        handLoc = new Vector2(position.X + 15, position.Y);
                        break;
                    default:
                        handLoc = new Vector2(position.X + 17, position.Y + Sprite.Height);
                        break;
                }
            }

            return handLoc;
        }

        #endregion

        #region Equip Methods

        public void EquipTorch(Light torch)
        {
            this.torch = torch;
            lightEquipped = true;
            torch.SwitchLit();
            torch.DrawSprite = true;

            vision.SwitchLit();
            LightManager.AddLight(this.torch);
            LightManager.AddLight(new Light(new LightAura(torch.Aura.Mask, new Vector2(position.X + 25, position.Y - 25), "AURA"), torch.Lifespan, true, "Torch", 50, torch.Image, torch.Sprites, false, new Vector2(position.X + 25, position.Y - 25), 1, 1, "TORCH"));
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (profBonus)
            {
                attack -= 2;
                profBonus = false;
            }

            if (this.weapon == null)
            {
                this.weapon = weapon;
                weapon.DrawSprite = true;
                attack += weapon.AttackBonus;
            }
            else if (this.weapon.Name != weapon.Name)
            {
                //We no longer draw the old weapon
                this.weapon.DrawSprite = false;
                
                //We lose our old attack bonus
                attack -= this.weapon.AttackBonus;

                //Swap weapons
                this.weapon = weapon;
                weapon.DrawSprite = true;

                //We gain the new weapon's attack bonus
                attack += weapon.AttackBonus;
            }
            else if (this.weapon.Name == weapon.Name && !this.weapon.Alive)
            {
                this.weapon = weapon;
                weapon.DrawSprite = true;

                //We gain the new weapon's attack bonus
                attack += weapon.AttackBonus;

                if (!this.weapon.Active)
                {
                    this.weapon.Active = true;
                }
            }

            if (this.weapon.WeaponType == preferredWeapon)
            {
                attack += 2;
                profBonus = true;
            }
        }

        public void EquipArmour(Armour armour)
        {
            if (this.armour == null)
            {
                this.armour = armour;
                defence += armour.DefenceBonus;
            }
            else if (this.armour.Name != armour.Name && armour.Alive)
            {
                if (this.armour.Type == GlobalVar.ArmourType.Light || this.characterClass == GlobalVar.CharacterClass.Warrior)
                {
                    defence -= StatBonus(dexterity);
                }

                //We lose our old attack bonus
                defence -= this.armour.DefenceBonus;

                //Swap weapons
                this.armour = armour;

                //We gain the new weapon's attack bonus
                defence += armour.DefenceBonus;
            }
            else if (this.armour.Name == armour.Name && !armour.Alive)
            {
                if (this.armour.Type == GlobalVar.ArmourType.Light || this.characterClass == GlobalVar.CharacterClass.Warrior)
                {
                    defence -= StatBonus(dexterity);
                }

                this.armour = armour;

                //We gain the new armour's attack bonus
                defence += armour.DefenceBonus;

                if (!this.armour.Active)
                {
                    this.armour.Active = true;
                }
            }

            if (this.armour.Type == GlobalVar.ArmourType.Light || this.characterClass == GlobalVar.CharacterClass.Warrior)
            {
                defence += StatBonus(dexterity);
            }
        }

        #endregion

        #region Serializable Methods

        public Player(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {
            this.inventory = (Inventory)info.GetValue("Inventory", typeof(Inventory));
            this.spellBook = (SpellBook)info.GetValue("SpellBook", typeof(SpellBook));
            this.vision = (Light)info.GetValue("Vision", typeof(Light));
            this.torch = (Light)info.GetValue("Torch", typeof(Light));
            this.lightEquipped = (bool)info.GetValue("LightEquipped", typeof(bool));
            this.weapon = (Weapon)info.GetValue("Weapon", typeof(Weapon));
            this.armour = (Armour)info.GetValue("Armour", typeof(Armour));
            this.escapedPressed = (bool)info.GetValue("EscapePressed", typeof(bool));
            this.tabPressed = (bool)info.GetValue("ShiftPressed", typeof(bool));
            this.level = (int)info.GetValue("Level", typeof(int));
            this.xp = (int)info.GetValue("XP", typeof(int));
            this.characterClass = (GlobalVar.CharacterClass)info.GetValue("Class", typeof(GlobalVar.CharacterClass));
            this.preferredWeapon = (GlobalVar.WeaponType)info.GetValue("PreferredWeapon", typeof(GlobalVar.WeaponType));
            this.profBonus = (bool)info.GetValue("ProfBonus", typeof(bool));
            this.xpReq = (int)info.GetValue("XPReq", typeof(int));
            this.spell = (int)info.GetValue("Spell", typeof(int));
            this.gold = (int)info.GetValue("Gold", typeof(int));
            this.dodgeMod = (int)info.GetValue("DodgeMod", typeof(int));
            this.magicSkillMod = (int)info.GetValue("MagicSkillMod", typeof(int));
            this.perceptionMod = (int)info.GetValue("PerceptionMod", typeof(int));
            this.stealthMod = (int)info.GetValue("StealthMod", typeof(int));
            this.weaponSkillMod = (int)info.GetValue("WeaponSkillMod", typeof(int));
            this.enduranceMod = (int)info.GetValue("EnduranceMod", typeof(int));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Inventory", this.inventory);
            info.AddValue("SpellBook", this.spellBook);
            info.AddValue("Vision", this.vision);
            info.AddValue("Torch", this.torch);
            info.AddValue("LightEquipped", this.lightEquipped);
            info.AddValue("Weapon", this.weapon);
            info.AddValue("Armour", this.armour);
            info.AddValue("EscapePressed", this.escapedPressed);
            info.AddValue("ShiftPressed", this.tabPressed);
            info.AddValue("Level", this.level);
            info.AddValue("XP", this.xp);
            info.AddValue("Class", this.characterClass);
            info.AddValue("PreferredWeapon", this.preferredWeapon);
            info.AddValue("ProfBonus", this.profBonus);
            info.AddValue("XPReq", this.xpReq);
            info.AddValue("Spell", this.spell);
            info.AddValue("Gold", this.gold);
            info.AddValue("DodgeMod", this.dodgeMod);
            info.AddValue("MagicSkillMod", this.magicSkillMod);
            info.AddValue("PerceptionMod", this.perceptionMod);
            info.AddValue("StealthMod", this.stealthMod);
            info.AddValue("WeaponSkillMod", this.weaponSkillMod);
            info.AddValue("EnduranceMod", this.enduranceMod);

            base.GetObjectData(info, ctxt);
        }

        #endregion

        #region Levelling and XP

        public void GainXP(int amount)
        {
            xp += amount;

            if (xp >= xpReq)
            {
                xpReq += 100 + ((25 * ((level + 1) / 5 + 1)) * ((level + 1) - 1));
                GlobalVar.levelMenu = true;
                GlobalVar.paused = true;
            }
        }

        public void LevelUp(GlobalVar.Stat chosenStat)
        {
            Random die = new Random();
            level++;

            switch (chosenStat)
            {
                case GlobalVar.Stat.Strength:
                    strength++;
                    break;
                case GlobalVar.Stat.Dexterity:
                    dexterity++;
                    break;
                case GlobalVar.Stat.Intelligence:
                    intelligence++;
                    break;
                case GlobalVar.Stat.Stamina:
                    stamina++;
                    break;
            }

            switch (characterClass)
            {
                case GlobalVar.CharacterClass.Cleric:
                    UpgradeHealth(die.Next(6) + 1 + StatBonus(stamina));
                    UpgradeMana(die.Next(6) + 1 + StatBonus(intelligence));

                    if (level <= GlobalVar.prayerLevels.Length)
                    {
                        spellBook.GainSpell(GlobalVar.prayerLevels[level - 1]);
                    }
                    break;
                case GlobalVar.CharacterClass.Thief:
                    UpgradeHealth(die.Next(8) + 1 + StatBonus(stamina));
                    UpgradeMana(die.Next(4) + 1 + StatBonus(intelligence));
                    break;
                case GlobalVar.CharacterClass.Warrior:
                    UpgradeHealth(die.Next(6) + 1 + die.Next(6) + 1 + StatBonus(stamina));
                    break;
                case GlobalVar.CharacterClass.Wizard:
                    UpgradeHealth(die.Next(4) + 1 + StatBonus(stamina));
                    UpgradeMana(die.Next(8) + 1 + StatBonus(intelligence));

                    if (level <= GlobalVar.spellLevels.Length)
                    {
                        spellBook.GainSpell(GlobalVar.spellLevels[level - 1]);
                    }
                    break;
            }

            CalcAttack();
            CalcDefence();
            CalcSkills();
        }

        public int LevelBonus()
        {
            return level / 3;
        }

        public void CalcAttack()
        {
            attack = 0;
            if (weapon.WeaponType == PreferredWeapon)
            {
                attack += 2;
                switch (characterClass)
                {
                    case GlobalVar.CharacterClass.Cleric:
                        attack += StatBonus(intelligence);
                        break;
                    case GlobalVar.CharacterClass.Thief:
                        attack += StatBonus(dexterity);
                        break;
                    case GlobalVar.CharacterClass.Warrior:
                        attack += StatBonus(strength);
                        break;
                    case GlobalVar.CharacterClass.Wizard:
                        attack += StatBonus(intelligence);
                        break;
                }
            }
            else
            {
                attack += StatBonus(strength);
            }

            attack += LevelBonus();
            attack += weapon.AttackBonus;
            
        }

        public void CalcDefence()
        {
            defence = 10;
            defence += armour.DefenceBonus;
            defence += LevelBonus();
            if (armour.Type == GlobalVar.ArmourType.Light || characterClass == GlobalVar.CharacterClass.Warrior)
            {
                defence += StatBonus(dexterity);
            }
        }
        #endregion

        #region Gold

        public void IncGold(int amount)
        {
            gold += amount;
        }

        public void DecGold(int amount)
        {
            gold -= amount;
        }
        #endregion

        #region Skills

        public void CalcSkills()
        {
            CalcBaseSkills();

            dodge += dodgeMod;
            magicSkill += magicSkillMod;
            perception += perceptionMod;
            stealth += stealthMod;
            weaponSkill += weaponSkillMod;
            endurance += enduranceMod;
        }

        #endregion
    }
}