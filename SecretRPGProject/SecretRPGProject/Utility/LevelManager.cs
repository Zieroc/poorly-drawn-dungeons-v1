using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using PoorlyDrawnDungeons.Gameplay;
using PoorlyDrawnDungeons.Gameplay.GameObjects;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters.Enemies;
using PoorlyDrawnDungeons.Gameplay.GameObjects.DungeonItems;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Items;
using PoorlyDrawnDungeons.Graphics;
using Dungeon_Tile_Engine;

namespace PoorlyDrawnDungeons.Utility
{
    public static class LevelManager
    {
        #region Variables

        private static ContentManager content;
        private static int currentLevel;        //What level are we on, can be used for map loading and saving if we add these features
        private static Vector2 spawnLocation;   //Where the player will spawn and respawn on each map
        private static Vector2 restLocation;

        private static Player hero;             //A reference to the hero

        #region Sprites

        private static Sprite[] chest0;
        private static Sprite[] chest1;
        private static Sprite[] chest2;
        private static Sprite[] chest3;
        private static Sprite[] portalSprite;
        private static Sprite[] restSprite;

        #endregion

        #region Lists

        private static List<Chest> chests;
        private static List<Enemy> enemies;
        private static List<Portal> portals;
        private static List<NPC> npcs;

        #endregion

        #endregion

        #region Properties

        public static int CurrentLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; }
        }

        public static Vector2 SpawnLocation
        {
            get { return spawnLocation; }
            set { spawnLocation = value; }  //Will change between levels but may needed to be changed dynamically such as at a checkpoint
        }

        public static List<Chest> Chests
        {
            get { return chests; }
        }

        public static List<Enemy> Enemies
        {
            get { return enemies; }
        }

        public static List<NPC> NPCs
        {
            get { return npcs; }
        }

        public static Player Hero
        {
            set { hero = value; }
        }

        #endregion

        #region Initalisation

        //Initalise player and content - will be passed through from the main game code
        public static void Intialise(ContentManager contentMan, Player player)
        {
            content = contentMan;
            hero = player;

            #region Sprites

            #region Chests
            chest0 = new Sprite[2];
            chest0[0] = new Sprite(content.Load<Texture2D>(@"Textures\DungeonItems\Chests\ChestClosed"), @"Textures\DungeonItems\Chests\ChestClosed");
            chest0[1] = new Sprite(content.Load<Texture2D>(@"Textures\DungeonItems\Chests\ChestOpened"), @"Textures\DungeonItems\Chests\ChestOpened");
            chest1 = new Sprite[2];
            chest1[0] = new Sprite(content.Load<Texture2D>(@"Textures\DungeonItems\Chests\Chest1Closed"), @"Textures\DungeonItems\Chests\Chest1Closed");
            chest1[1] = new Sprite(content.Load<Texture2D>(@"Textures\DungeonItems\Chests\Chest1Opened"), @"Textures\DungeonItems\Chests\Chest1Opened");
            chest2 = new Sprite[2];
            chest2[0] = new Sprite(content.Load<Texture2D>(@"Textures\DungeonItems\Chests\Chest2Closed"), @"Textures\DungeonItems\Chests\Chest2Closed");
            chest2[1] = new Sprite(content.Load<Texture2D>(@"Textures\DungeonItems\Chests\Chest2Opened"), @"Textures\DungeonItems\Chests\Chest2Opened");
            chest3 = new Sprite[2];
            chest3[0] = new Sprite(content.Load<Texture2D>(@"Textures\DungeonItems\Chests\Chest3Closed"), @"Textures\DungeonItems\Chests\Chest3Closed");
            chest3[1] = new Sprite(content.Load<Texture2D>(@"Textures\DungeonItems\Chests\Chest3Opened"), @"Textures\DungeonItems\Chests\Chest3Opened");
            #endregion

            #region Portal
            portalSprite = new Sprite[1];
            portalSprite[0] = new Sprite(content.Load<Texture2D>(@"Textures\DungeonItems\Portal"), 32, 32, 3, true, true, 500f, @"Textures\DungeonItems\Portal");
            #endregion

            #region Rest Area
            restSprite = new Sprite[1];
            restSprite[0] = new Sprite(content.Load<Texture2D>(@"Textures\DungeonItems\RestArea"), 80, 80, 1, true, true, 500f, @"Textures\DungeonItems\RestArea");
            #endregion

            #endregion

            #region Lists

            chests = new List<Chest>();
            enemies = new List<Enemy>();
            portals = new List<Portal>();
            npcs = new List<NPC>();

            #endregion
        }

        #endregion

        #region Level Loading

        //Load a level based on the passed through level number, map file must be in the Levels folder
        public static void LoadLevel(int levelNumber)
        {
            currentLevel = levelNumber;

            Random numGen = new Random();

            DungeonMap.LoadMap(TitleContainer.OpenStream(@"Content\Maps\MAP" + levelNumber.ToString().PadLeft(3, '0') + ".MAP")); //Level numbers must be padded with 0s just like they are when we save maps

            //Clear the lists as this is a fresh level
            chests.Clear();
            enemies.Clear();
            portals.Clear();

            //Loop through each tile on the map and look for the start area and put the player there
            for (int x = 0; x < DungeonMap.MapWidth; x++)
            {
                for (int y = 0; y < DungeonMap.MapHeight; y++)
                {
                    //Set start location
                    if (DungeonMap.CellCode(x, y) == "START")
                    {
                        hero.Position = new Vector2(x * DungeonMap.TileWidth, y * DungeonMap.TileHeight);
                    }
                    #region Chests
                    else if (DungeonMap.CellCode(x, y) == "CHEST0" || DungeonMap.CellCode(x, y) == "UIT0")
                    {
                        Item[] items;

                        if (DungeonMap.CellCode(x, y) == "UIT0")
                        {
                            items = new Item[1];
                            items[0] = GlobalVar.uniqueItems[currentLevel];
                        }
                        else
                        {
                            items = new Item[numGen.Next(4) + 1];

                            for (int i = 0; i < items.Length; i++)
                            {
                                items[i] = GlobalVar.itemChart[numGen.Next(GlobalVar.itemChart.Length)];
                            }
                        }

                        chests.Add(new Chest(chest0, new Vector2(x * DungeonMap.TileWidth, y * DungeonMap.TileHeight), 0, items, 0));
                    }
                    else if (DungeonMap.CellCode(x, y) == "CHEST1" || DungeonMap.CellCode(x, y) == "UIT1")
                    {
                        Item[] items;

                        if (DungeonMap.CellCode(x, y) == "UIT1")
                        {
                            items = new Item[1];
                            items[0] = GlobalVar.uniqueItems[currentLevel];
                        }
                        else
                        {
                            items = new Item[numGen.Next(4) + 1];

                            for (int i = 0; i < items.Length; i++)
                            {
                                items[i] = GlobalVar.itemChart[numGen.Next(GlobalVar.itemChart.Length)];
                            }
                        }

                        chests.Add(new Chest(chest1, new Vector2(x * DungeonMap.TileWidth, y* DungeonMap.TileHeight), 0, items, 1));
                    }
                    else if (DungeonMap.CellCode(x, y) == "CHEST2" || DungeonMap.CellCode(x, y) == "UIT2")
                    {
                        Item[] items;

                        if (DungeonMap.CellCode(x, y) == "UIT2")
                        {
                            items = new Item[1];
                            items[0] = GlobalVar.uniqueItems[currentLevel];
                        }
                        else
                        {
                            items = new Item[numGen.Next(4) + 1];

                            for (int i = 0; i < items.Length; i++)
                            {
                                items[i] = GlobalVar.itemChart[numGen.Next(GlobalVar.itemChart.Length)];
                            }
                        }

                        chests.Add(new Chest(chest2, new Vector2(x * DungeonMap.TileWidth, y * DungeonMap.TileHeight), 0, items, 2));
                    }
                    else if (DungeonMap.CellCode(x, y) == "CHEST3" || DungeonMap.CellCode(x, y) == "UIT3")
                    {
                        Item[] items;

                        if (DungeonMap.CellCode(x, y) == "UIT3")
                        {
                            items = new Item[1];
                            items[0] = GlobalVar.uniqueItems[currentLevel];
                        }
                        else
                        {
                            items = new Item[numGen.Next(4) + 1];

                            for (int i = 0; i < items.Length; i++)
                            {
                                items[i] = GlobalVar.itemChart[numGen.Next(GlobalVar.itemChart.Length)];
                            }
                        }

                        chests.Add(new Chest(chest3, new Vector2(x * DungeonMap.TileWidth, y * DungeonMap.TileHeight), 0, items, 3));
                    }
                    #endregion
                    #region Enemies

                    #region Goblin
                    else if (DungeonMap.CellCode(x, y).StartsWith("GOB"))
                    {
                        Sprite[] goblin;
                        goblin = new Sprite[4];
                        goblin[0] = new Sprite(content.Load<Texture2D>(@"Textures\Characters\Enemies\Goblins\GoblinIdle"), 20, 20, 1, true, true, @"Textures\Characters\Enemies\Goblins\GoblinIdle");
                        goblin[1] = new Sprite(content.Load<Texture2D>(@"Textures\Characters\Enemies\Goblins\GoblinWalking"), 20, 20, 2, true, true, @"Textures\Characters\Enemies\Goblins\GoblinWalking");
                        goblin[2] = new Sprite(content.Load<Texture2D>(@"Textures\Characters\Enemies\Goblins\GoblinAttacking"), 20, 20, 5, true, false, 100f, @"Textures\Characters\Enemies\Goblins\GoblinAttacking");
                        goblin[3] = new Sprite(content.Load<Texture2D>(@"Textures\Characters\Enemies\Goblins\GoblinDying"), 20, 20, 5, true, false, 100f, @"Textures\Characters\Enemies\Goblins\GoblinDying");


                        #region Rogue
                        if (DungeonMap.CellCode(x, y) == "GOBRV")
                        {
                            enemies.Add(new Goblin(goblin, new Vector2(x * DungeonMap.TileWidth + DungeonMap.TileWidth / 2 - goblin[0].Width, y * DungeonMap.TileHeight + DungeonMap.TileHeight / 2 - goblin[0].Height), true, 0, 2, GlobalVar.dagger, GlobalVar.leatherChestplate, GlobalVar.WeaponType.Dagger, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, "Goblin Rogue", GlobalVar.CharacterMode.Patrol));
                        }
                        else if (DungeonMap.CellCode(x, y) == "GOBRH")
                        {
                            enemies.Add(new Goblin(goblin, new Vector2(x * DungeonMap.TileWidth + DungeonMap.TileWidth / 2 - goblin[0].Width, y * DungeonMap.TileHeight + DungeonMap.TileHeight / 2 - goblin[0].Height), true, 2, 2, GlobalVar.dagger, GlobalVar.leatherChestplate, GlobalVar.WeaponType.Dagger, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, "Goblin Rogue", GlobalVar.CharacterMode.Patrol));
                        }
                        #endregion
                        #region Guard
                        else if(DungeonMap.CellCode(x, y) == "GOBGV")
                        {
                            enemies.Add(new Goblin(goblin, new Vector2(x * DungeonMap.TileWidth + DungeonMap.TileWidth / 2 - goblin[0].Width, y * DungeonMap.TileHeight + DungeonMap.TileHeight / 2 - goblin[0].Height), true, 0, 2, GlobalVar.dagger, GlobalVar.leatherChestplate, GlobalVar.WeaponType.Dagger, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, "Goblin Guard", GlobalVar.CharacterMode.Idle));
                        }
                        else if (DungeonMap.CellCode(x, y) == "GOBGH")
                        {
                            enemies.Add(new Goblin(goblin, new Vector2(x * DungeonMap.TileWidth + DungeonMap.TileWidth / 2 - goblin[0].Width, y * DungeonMap.TileHeight + DungeonMap.TileHeight / 2 - goblin[0].Height), true, 3, 2, GlobalVar.dagger, GlobalVar.leatherChestplate, GlobalVar.WeaponType.Dagger, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, "Goblin Guard", GlobalVar.CharacterMode.Idle));
                        }
                        #endregion
                    }
                    #endregion
                    #region Orc
                    else if (DungeonMap.CellCode(x, y).StartsWith("ORC"))
                    {
                        Sprite[] orc;
                        orc = new Sprite[4];
                        orc[0] = new Sprite(content.Load<Texture2D>(@"Textures\Characters\Enemies\Orc\OrcIdle"), 32, 32, 1, true, true, @"Textures\Characters\Enemies\Orc\OrcIdle");
                        orc[1] = new Sprite(content.Load<Texture2D>(@"Textures\Characters\Enemies\Orc\OrcWalking"), 32, 32, 2, true, true, @"Textures\Characters\Enemies\Orc\OrcWalking");
                        orc[2] = new Sprite(content.Load<Texture2D>(@"Textures\Characters\Enemies\Orc\OrcAttack"), 32, 32, 5, true, false, 100f, @"Textures\Characters\Enemies\Orc\OrcAttack");
                        orc[3] = new Sprite(content.Load<Texture2D>(@"Textures\Characters\Enemies\Orc\OrcDying"), 32, 32, 5, true, false, 100f, @"Textures\Characters\Enemies\Orc\OrcDying");

                        #region Warrior
                        if (DungeonMap.CellCode(x, y) == "ORCWV")
                        {
                            enemies.Add(new Orc(orc, new Vector2(x * DungeonMap.TileWidth, y * DungeonMap.TileHeight), true, 0, 2, GlobalVar.sword, GlobalVar.leatherChestplate, GlobalVar.WeaponType.Axe, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, "Orc Warrior", GlobalVar.CharacterMode.Patrol));
                        }
                        else if (DungeonMap.CellCode(x, y) == "ORCWH")
                        {
                            enemies.Add(new Orc(orc, new Vector2(x * DungeonMap.TileWidth, y * DungeonMap.TileHeight), true, 2, 2, GlobalVar.sword, GlobalVar.leatherChestplate, GlobalVar.WeaponType.Axe, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, "Orc Warrior", GlobalVar.CharacterMode.Patrol));
                        }
                        #endregion
                        #region Guard
                        else if (DungeonMap.CellCode(x, y) == "ORCGV")
                        {
                            enemies.Add(new Orc(orc, new Vector2(x * DungeonMap.TileWidth + DungeonMap.TileWidth / 2 - orc[0].Width, y * DungeonMap.TileHeight + DungeonMap.TileHeight / 2 - orc[0].Height), true, 0, 2, GlobalVar.sword, GlobalVar.leatherChestplate, GlobalVar.WeaponType.Axe, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, "Orc Guard", GlobalVar.CharacterMode.Idle));
                        }
                        else if (DungeonMap.CellCode(x, y) == "ORCGH")
                        {
                            enemies.Add(new Orc(orc, new Vector2(x * DungeonMap.TileWidth + DungeonMap.TileWidth / 2 - orc[0].Width, y * DungeonMap.TileHeight + DungeonMap.TileHeight / 2 - orc[0].Height), true, 2, 2, GlobalVar.sword, GlobalVar.leatherChestplate, GlobalVar.WeaponType.Axe, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, GlobalVar.DamageType.NoDamage, "Orc Guard", GlobalVar.CharacterMode.Idle));
                        }
                        #endregion
                    }
                    #endregion

                    #endregion
                    #region Portals & Rest Areas
                    if (DungeonMap.CellCode(x, y).StartsWith("P_"))
                    {
                        string[] code = DungeonMap.CellCode(x, y).Split('_');

                        portals.Add(new Portal(portalSprite, new Vector2(x * DungeonMap.TileWidth, y * DungeonMap.TileHeight), 0, int.Parse(code[1])));
                    }
                    if (DungeonMap.CellCode(x, y) == ("REST"))
                    {
                        restLocation = new Vector2(x * DungeonMap.TileWidth - restSprite[0].Width/2, y * DungeonMap.TileHeight - restSprite[0].Height/2);
                        GlobalVar.restPoint = new RestArea(restSprite,  restLocation, 0);
                    }
                    #endregion
                    #region NPCs

                    #region Gorc
                    else if (DungeonMap.CellCode(x, y).StartsWith("NPC"))
                    {
                        if (DungeonMap.CellCode(x, y) == ("NPCGORC"))
                        {
                            Sprite[] gorc;
                            gorc = new Sprite[4];
                            gorc[0] = new Sprite(content.Load<Texture2D>(@"Textures\Characters\NPC\GorcIdle"), 32, 32, 1, true, true, @"Textures\Characters\NPC\GorcIdle");
                            gorc[1] = new Sprite(content.Load<Texture2D>(@"Textures\Characters\NPC\GorcWalking"), 32, 32, 2, true, true, @"Textures\Characters\NPC\GorcWalking");
                            gorc[2] = new Sprite(content.Load<Texture2D>(@"Textures\Characters\NPC\GorcAttack"), 32, 32, 5, true, false, 100f, @"Textures\Characters\NPC\GorcAttack");
                            gorc[3] = new Sprite(content.Load<Texture2D>(@"Textures\Characters\NPC\GorcDying"), 32, 32, 5, true, false, 100f, @"Textures\Characters\NPC\GorcDying");

                            npcs.Add(new NPC(GlobalVar.gorcSpeech, gorc, new Vector2(x * DungeonMap.TileWidth, y * DungeonMap.TileHeight), 0, 1, 0));
                        }
                    }
                    #endregion

                    #endregion
                }
            }

            //Set spawn point
            spawnLocation = hero.Position;

            GlobalVar.saveCalled = true;
        }

        //Reload a level if player dies, call loadlevel but player goes at currently stored spawn point incase he has reached checkpoint
        public static void ReloadLevel()
        {
            Vector2 currentSpawn = spawnLocation;   //Store the spawn
            LoadLevel(currentLevel);                //Load the level again
            spawnLocation = currentSpawn;           //Reset spawn to what it should be
        }

        public static void Respawn()
        {
            hero.Position = restLocation;
            hero.Revive();
        }

        #endregion

        #region Update

        //Update and Draw needed here because most objects are created through the level manager
        public static void Update(GameTime gameTime)
        {
            if (!GlobalVar.paused)
            {
                #region Chests
                for (int i = chests.Count - 1; i >= 0; i--)
                {
                    if (chests[i].Alive)
                    {
                        chests[i].Update(gameTime);

                        Rectangle openableArea = new Rectangle(chests[i].Bounds.X - 10, chests[i].Bounds.Y - 10, chests[i].Bounds.Width + 20, chests[i].Bounds.Height + 20);

                        if (hero.Collided(openableArea) && Keyboard.GetState().IsKeyDown(Keys.Space) && hero.Direction == chests[i].Facing)
                        {
                            chests[i].Open();
                        }
                    }
                    else
                    {
                        chests.RemoveAt(i);
                    }
                }
                #endregion
                #region Enemies
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    if (enemies[i].Alive)
                    {
                        enemies[i].Update(gameTime);

                        if(enemies[i].Collided(hero.Bounds) && enemies[i].CurrentHealth > 0)
                        {
                            //Store enemies original location and mode
                            GlobalVar.enemyOldDirection = enemies[i].Direction;
                            GlobalVar.enemyOldMode = enemies[i].Mode;
                            GlobalVar.enemyOldPosition = enemies[i].Position;

                            GlobalVar.paused = true;
                            GlobalVar.battleTransition = true;
                            GlobalVar.enemy = enemies[i];
                            GlobalVar.turn = 1;
                            GlobalVar.defending = false;
                            enemies[i].Mode = GlobalVar.CharacterMode.Attack;
                        }

                        Rectangle backstapArea = new Rectangle(enemies[i].Bounds.X - 10, enemies[i].Bounds.Y - 10, enemies[i].Bounds.Width + 20, enemies[i].Bounds.Height + 20);

                        if (!enemies[i].Collided(hero.Bounds) && hero.Collided(backstapArea))
                        {
                            switch (enemies[i].Direction)
                            {
                                case 0:
                                    if (hero.Position.Y > enemies[i].Position.Y && Keyboard.GetState().IsKeyDown(Keys.Space))
                                    {
                                        hero.Mode = GlobalVar.CharacterMode.Attacking;
                                        if (hero.CharacterClass == GlobalVar.CharacterClass.Thief)
                                        {
                                            enemies[i].DecHealth(enemies[i].MaxHealth);
                                        }
                                        else
                                        {
                                            enemies[i].DecHealth(GlobalVar.hero.Weapon.DamageRoll * GlobalVar.hero.Weapon.NumDice);

                                            #region Battle
                                            if (enemies[i].CurrentHealth > 0)
                                            {
                                                //Store enemies original location and mode
                                                GlobalVar.enemyOldDirection = enemies[i].Direction;
                                                GlobalVar.enemyOldMode = enemies[i].Mode;
                                                GlobalVar.enemyOldPosition = enemies[i].Position;

                                                GlobalVar.paused = true;
                                                GlobalVar.battleTransition = true;
                                                GlobalVar.enemy = enemies[i];
                                                GlobalVar.turn = 1;
                                                GlobalVar.defending = false;
                                                enemies[i].Mode = GlobalVar.CharacterMode.Attack;
                                            }
                                            #endregion
                                        }
                                    }
                                    break;
                                case 1:
                                    if (hero.Position.Y < enemies[i].Position.Y && Keyboard.GetState().IsKeyDown(Keys.Space))
                                    {
                                        hero.Mode = GlobalVar.CharacterMode.Attacking;
                                        if (hero.CharacterClass == GlobalVar.CharacterClass.Thief)
                                        {
                                            enemies[i].DecHealth(enemies[i].MaxHealth);
                                        }
                                        else
                                        {
                                            enemies[i].DecHealth(GlobalVar.hero.Weapon.DamageRoll * GlobalVar.hero.Weapon.NumDice);

                                            #region Battle
                                            if (enemies[i].CurrentHealth > 0)
                                            {
                                                //Store enemies original location and mode
                                                GlobalVar.enemyOldDirection = enemies[i].Direction;
                                                GlobalVar.enemyOldMode = enemies[i].Mode;
                                                GlobalVar.enemyOldPosition = enemies[i].Position;

                                                GlobalVar.paused = true;
                                                GlobalVar.battleTransition = true;
                                                GlobalVar.enemy = enemies[i];
                                                GlobalVar.turn = 1;
                                                GlobalVar.defending = false;
                                                enemies[i].Mode = GlobalVar.CharacterMode.Attack;
                                            }
                                            #endregion
                                        }
                                    }
                                    break;
                                case 2:
                                    if (hero.Position.X > enemies[i].Position.X && Keyboard.GetState().IsKeyDown(Keys.Space))
                                    {
                                        hero.Mode = GlobalVar.CharacterMode.Attacking;
                                        if (hero.CharacterClass == GlobalVar.CharacterClass.Thief)
                                        {
                                            enemies[i].DecHealth(enemies[i].MaxHealth);
                                        }
                                        else
                                        {
                                            enemies[i].DecHealth(GlobalVar.hero.Weapon.DamageRoll * GlobalVar.hero.Weapon.NumDice);

                                            #region Battle
                                            if (enemies[i].CurrentHealth > 0)
                                            {
                                                //Store enemies original location and mode
                                                GlobalVar.enemyOldDirection = enemies[i].Direction;
                                                GlobalVar.enemyOldMode = enemies[i].Mode;
                                                GlobalVar.enemyOldPosition = enemies[i].Position;

                                                GlobalVar.paused = true;
                                                GlobalVar.battleTransition = true;
                                                GlobalVar.enemy = enemies[i];
                                                GlobalVar.turn = 1;
                                                GlobalVar.defending = false;
                                                enemies[i].Mode = GlobalVar.CharacterMode.Attack;
                                            }
                                            #endregion
                                        }
                                    }
                                    break;
                                case 3:
                                    if (hero.Position.X < enemies[i].Position.X && Keyboard.GetState().IsKeyDown(Keys.Space))
                                    {
                                        hero.Mode = GlobalVar.CharacterMode.Attacking;
                                        if (hero.CharacterClass == GlobalVar.CharacterClass.Thief)
                                        {
                                            enemies[i].DecHealth(enemies[i].MaxHealth);
                                        }
                                        else
                                        {
                                            enemies[i].DecHealth(GlobalVar.hero.Weapon.DamageRoll * GlobalVar.hero.Weapon.NumDice);

                                            #region Battle
                                            if (enemies[i].CurrentHealth > 0)
                                            {
                                                //Store enemies original location and mode
                                                GlobalVar.enemyOldDirection = enemies[i].Direction;
                                                GlobalVar.enemyOldMode = enemies[i].Mode;
                                                GlobalVar.enemyOldPosition = enemies[i].Position;

                                                GlobalVar.paused = true;
                                                GlobalVar.battleTransition = true;
                                                GlobalVar.enemy = enemies[i];
                                                GlobalVar.turn = 1;
                                                GlobalVar.defending = false;
                                                enemies[i].Mode = GlobalVar.CharacterMode.Attack;
                                            }
                                            #endregion
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        enemies.RemoveAt(i);
                    }
                }
                #endregion
                #region Portals
                for (int i = portals.Count - 1; i >= 0; i--)
                {
                    if (portals[i].Alive)
                    {
                        portals[i].Update(gameTime);

                        if (hero.Collided(portals[i].Bounds) && Keyboard.GetState().IsKeyDown(Keys.Space))
                        {
                            LoadLevel(portals[i].Map);
                        }
                    }
                    else
                    {
                        portals.RemoveAt(i);
                    }
                }
                #endregion
                #region RestArea
                GlobalVar.restPoint.Update(gameTime);
                if (hero.Collided(GlobalVar.restPoint.Bounds) && Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    GlobalVar.restPoint.Rested = true;
                    hero.FillHealth();
                    hero.FillMana();
                }
                #endregion
                #region NPCs
                for (int i = npcs.Count - 1; i >= 0; i--)
                {
                    if (npcs[i].Alive)
                    {
                        npcs[i].Update(gameTime);

                        Rectangle speakableArea = new Rectangle(npcs[i].Bounds.X - 10, npcs[i].Bounds.Y - 10, npcs[i].Bounds.Width + 20, npcs[i].Bounds.Height + 20);

                        if (hero.Collided(speakableArea) && Keyboard.GetState().IsKeyDown(Keys.Space) && hero.Direction == npcs[i].Facing)
                        {
                            GlobalVar.speaker = npcs[i];
                            npcs[i].Speak();
                        }
                    }
                    else
                    {
                        npcs.RemoveAt(i);
                    }
                }
                #endregion

                if (hero.Alive)
                {
                    //Check if we hit a killer tile
                    CheckCurrentCellCode();
                }
            }
            if (GlobalVar.battleMode)
            {
                #region Enemies
                GlobalVar.enemy.Update(gameTime);
                #endregion
            }
            if (GlobalVar.displayDialogue)
            {
                #region NPCs
                GlobalVar.speaker.Update(gameTime);
                #endregion
            }
        }

        #endregion

        #region Draw

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Chest chest in chests)
            {
                chest.Draw(spriteBatch);
            }
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
            foreach (Portal portal in portals)
            {
                portal.Draw(spriteBatch);
            }
            foreach (NPC npc in npcs)
            {
                npc.Draw(spriteBatch);
            }

            GlobalVar.restPoint.Draw(spriteBatch);
        }

        #endregion

        #region Private Methods

        //Check code in a cell, for codes that only take effect once player is in their cell
        private static void CheckCurrentCellCode()
        {
            /*
            //Check cells from characters center;
            string code = DungeonMap.CellCode(DungeonMap.GetCellByPoint(hero.PositionCenter));
            if (code == "DEAD")
            {
            }
            */
        }

        #endregion
    }
}
