using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using PoorlyDrawnDungeons.Gameplay;
using PoorlyDrawnDungeons.Gameplay.Features;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters.Enemies;
using PoorlyDrawnDungeons.Gameplay.GameObjects.DungeonItems;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Items;
using PoorlyDrawnDungeons.Gameplay.Magic;
using PoorlyDrawnDungeons.Graphics;

namespace PoorlyDrawnDungeons
{
    public static class GlobalVar
    {
        #region Enums
        //For global variables and Enums
        public enum CharacterMode
        {
            Idle,
            Patrol,
            Pursue,
            Attack,
            Attacking,
            Casting,
            Dying
        }

        public enum Stat
        {
            Health,
            Mana,
            Attack,
            Defence,
            Strength,
            Dexterity,
            Intelligence,
            Stamina
        }

        public enum CharacterClass
        {
            Cleric,
            Thief,
            Warrior,
            Wizard
        }

        public enum DamageType
        {
            Cold,
            Energy,
            Fire,
            Lightning,
            Water,
            Divine,
            Necrotic,
            Slashing,
            Piercing,
            NoDamage
        }

        public enum BuffType
        {
            Temporary,
            Permenant
        }

        public enum WeaponType
        {
            Axe,
            Dagger,
            Mace,
            Spear,
            Staff,
            Sword,
            Unarmed
        }

        public enum ArmourType
        {
            Light,
            Heavy
        }

        #endregion

        #region Inventory Positions
        public static Vector2[] inventoryPos =
        {
            new Vector2(660, 310), new Vector2(694, 310), new Vector2(728, 310), new Vector2(762, 310),
            new Vector2(660, 346), new Vector2(694, 346), new Vector2(728, 346), new Vector2(762, 346),
            new Vector2(660, 382), new Vector2(694, 382), new Vector2(728, 382), new Vector2(762, 382),
            new Vector2(660, 418), new Vector2(694, 418), new Vector2(728, 418), new Vector2(762, 418),
            new Vector2(660, 454), new Vector2(694, 454), new Vector2(728, 454), new Vector2(762, 454),
            new Vector2(660, 490), new Vector2(694, 490), new Vector2(728, 490), new Vector2(762, 490),
            new Vector2(660, 525), new Vector2(694, 525), new Vector2(728, 525), new Vector2(762, 525),
            new Vector2(660, 560), new Vector2(694, 560), new Vector2(728, 560), new Vector2(762, 560)
        };

        public static Rectangle chestRec = new Rectangle(250, 230, 140, 41);

        public static Vector2[] chestInvPos =
        {
            new Vector2(255, 235), new Vector2(289, 235), new Vector2(323, 235), new Vector2(357, 235)
        };

        public static Rectangle shopRec = new Rectangle(250, 230, 140, 149);

        public static Vector2[] shopInvPos =
        {
            new Vector2(255, 235), new Vector2(289, 235), new Vector2(323, 235), new Vector2(357, 235),
            new Vector2(255, 271), new Vector2(289, 271), new Vector2(323, 271), new Vector2(357, 271),
            new Vector2(255, 306), new Vector2(289, 306), new Vector2(323, 306), new Vector2(357, 306),
            new Vector2(255, 342), new Vector2(289, 342), new Vector2(323, 342), new Vector2(357, 342)
        };

        #endregion

        #region Items

        //Items will be declared here so they can be used when needed

        #region Lights

        public static Light torch;
        public static Sprite[] torchSprite;

        #endregion

        #region Weapons

        public static Weapon dagger;
        public static Sprite[] daggerSprite;

        public static Weapon sword;
        public static Sprite[] swordSprite;

        public static Weapon mace;
        public static Sprite[] maceSprite;

        public static Weapon staff;
        public static Sprite[] staffSprite;

        public static Weapon unarmed;

        #endregion

        #region Potions

        public static Potion minorHealthPotion;
        public static Potion minorAttackPotion;
        public static Potion minorDefencePotion;
        public static Potion minorManaPotion;

        #endregion

        #region Armour

        public static Armour ironChestplate;
        public static Armour leatherChestplate;

        public static Armour unarmoured;

        #endregion

        #region ItemChart

        public static Item[] itemChart; //Used to randomly add items to chests.

        #endregion

        #endregion

        #region Unique Items

        public static Sprite[] orcsbaneSprite;
        public static Weapon orcsbane;

        public static Armour lightSteelArmour;

        #region Unique Item Chart

        public static Item[] uniqueItems;

        #endregion

        #endregion

        #region Magic

        #region Spells

        public static AttackMagic energyBolt;
        public static AttackMagic frostBurst;

        public static Magic[] spellLevels;
        #endregion

        #region Prayers

        public static BuffMagic healingPrayer;
        public static AttackMagic divineRays;

        public static Magic[] prayerLevels;
        #endregion

        #endregion

        #region Sprite Fonts

        public static SpriteFont blackwoodCastle8;
        public static SpriteFont newsGothicBold8;
        public static SpriteFont newsGothicBold12;

        #endregion

        #region Minor States

        public static bool paused;
        public static bool displayChestBar;
        public static Chest displayChest;

        public static bool saveCalled;
        public static bool loadCalled;

        public static bool battleMode;
        public static bool battleTransition;
        public static bool victoryTransition;
        public static Enemy enemy;
        public static Player hero;

        public static bool levelMenu;

        public static float timer;

        #endregion

        #region Battle Mode

        public static Rectangle lightAttackRec = new Rectangle(35, 525, 120, 50);
        public static Rectangle heavyAttackRec = new Rectangle(185, 525, 120, 50);
        public static Rectangle defendRec = new Rectangle(335, 525, 120, 50);
        public static Rectangle magicRec = new Rectangle(485, 525, 120, 50);

        public static int turn;             //0 - Player's Turn, 1 - Enemies Turn
        public static float battleTimer;    //A timer just for the battle mode
        public static bool defending;       //Is the player defending?
        public static bool dodgeBonus;      //Is the player getting a +2 dodge bonus?

        public static int oldDirection;
        public static int enemyOldDirection;
        public static Vector2 enemyOldPosition;
        public static CharacterMode enemyOldMode;

        #endregion

        #region Interface And Graphics

        public static ContentManager content;
        public static bool mouseReleased;

        public static bool drawBattleMsgString;
        public static string battleMsg;

        public static string magicMsg;
        public static bool drawMagicMsg;

        public static bool displaySpellBook;

        public static bool displayShop;
        public static Inventory shop;

        public static bool displayDialogue;
        public static Dialogue speech;
        public static NPC speaker;

        #endregion

        #region DungeonItems

        public static RestArea restPoint;

        #endregion

        #region Skill Rolls and Things

        public static int heroStealthRoll;
        public static int enemyPerceptionRoll;

        public static bool moveModeSwitched;

        #endregion

        #region NPCDialogue

        public static Dialogue[] gorcSpeech;

        #endregion
    }
}
