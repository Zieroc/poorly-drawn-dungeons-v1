using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using PoorlyDrawnDungeons.Gameplay;
using PoorlyDrawnDungeons.Gameplay.Auras;
using PoorlyDrawnDungeons.Gameplay.Features;
using PoorlyDrawnDungeons.Gameplay.GameObjects;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Items;
using PoorlyDrawnDungeons.Gameplay.GameObjects.DungeonItems;
using PoorlyDrawnDungeons.Gameplay.Magic;
using PoorlyDrawnDungeons.Graphics;
using PoorlyDrawnDungeons.Utility;
using PoorlyDrawnDungeons.Utility.Storage;
using Dungeon_Tile_Engine;

namespace PoorlyDrawnDungeons
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region Game States
        enum GameState
        {
            Title,
            CharacterCreation,
            Intro,
            Loading,
            Playing,
            Gameover
        }

        GameState state;

        #endregion

        #region RenderTargets & Effects
        RenderTarget2D mainScene;
        RenderTarget2D lightMask;

        Effect lightingEffect;

        Texture2D blackSquare;
        
        #endregion

        #region Light Masks
        Texture2D brightness1;
        Texture2D brightness3;
        #endregion

        #region Screens & Interface

        Texture2D sidebar;
        Texture2D heroFace;

        Texture2D loadScreen;

        Texture2D titleScreen;
        Texture2D newGameOption;
        Texture2D newGameSelected;
        Texture2D loadGameOption;
        Texture2D loadGameSelected;

        Texture2D characterCreationScreen;
        Texture2D clericOption;
        Texture2D clericSelected;
        Texture2D thiefOption;
        Texture2D thiefSelected;
        Texture2D warriorOption;
        Texture2D warriorSelected;
        Texture2D wizardOption;
        Texture2D wizardSelected;

        Texture2D levelMenuScreen;
        Texture2D strOption;
        Texture2D strSelected;
        Texture2D staOption;
        Texture2D staSelected;
        Texture2D dexOption;
        Texture2D dexSelected;
        Texture2D intOption;
        Texture2D intSelected;

        Texture2D redX;
        Texture2D darkRedX;

        Texture2D chestItemBar;
        Texture2D shopItemBar;
        Texture2D pausedTexture;

        Texture2D battleMenuFull;
        Texture2D battleMenuNoMagic;
        Texture2D battleMenuSelected;

        Texture2D battleTransitionScreen;
        Texture2D battleWonScreen;

        Texture2D gameOverScreen;

        #endregion

        #region Player & Sprites

        Player hero;
        Sprite[] playerSprites;
        Sprite[] blankSprites; //Needed to prevent crash

        #endregion

        #region Videos

        Video introVid;
        VideoPlayer vidPlayer;
        Texture2D vidTexture;
        bool hasVideoPlayed;

        #endregion

        #region Saving & Loading

        //Data needed for saving
        IAsyncResult result;
        bool GameSaveRequested;
        bool GameLoadRequested;
        bool loadedSuccessful;
        bool loadTried;
        bool loading;

        #endregion

        #region Timers

        float manaRegenTimer;
        float manaRegenInterval;

        #endregion

        #endregion

        #region Constructor

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            this.Components.Add(new GamerServicesComponent(this));
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Need if compiling with Reach setting
            GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            #region Video
            vidPlayer = new VideoPlayer();
            vidPlayer.IsLooped = false;
            hasVideoPlayed = false;
            #endregion

            #region States & Minor States

            state = GameState.Title;

            GlobalVar.paused = false;
            GlobalVar.displayChestBar = false;

            GlobalVar.saveCalled = false;
            GlobalVar.loadCalled = false;

            GlobalVar.battleMode = false;
            GlobalVar.battleTransition = false;

            GlobalVar.battleMsg = "";
            GlobalVar.drawBattleMsgString = false;

            GlobalVar.magicMsg = "";
            GlobalVar.drawMagicMsg = false;

            GlobalVar.timer = 0;

            GlobalVar.victoryTransition = false;

            GlobalVar.displaySpellBook = false;

            GlobalVar.levelMenu = false;

            GlobalVar.displayDialogue = false;
            GlobalVar.displayShop = false;

            #endregion

            #region Sprite Arrays
            playerSprites = new Sprite[5];
            blankSprites = new Sprite[1]; //Will be used for any item that needs no sprite to be drawn
            #endregion

            #region Item Sprites

            GlobalVar.torchSprite = new Sprite[1];

            GlobalVar.daggerSprite = new Sprite[1];

            GlobalVar.swordSprite = new Sprite[1];

            GlobalVar.maceSprite = new Sprite[1];

            GlobalVar.staffSprite = new Sprite[1];

            GlobalVar.orcsbaneSprite = new Sprite[1];

            #endregion

            #region Mouse Click Control
            GlobalVar.mouseReleased = true;
            #endregion

            #region Saving & Loading

            GameSaveRequested = false;
            GameLoadRequested = false;
            loadedSuccessful = false;
            loadTried = false;
            loading = false;

            #endregion

            #region Timer

            manaRegenTimer = 0f;
            manaRegenInterval = 10000f;

            #endregion

            #region Dialogue

            GlobalVar.gorcSpeech = new Dialogue[2];
            
            #endregion

            MagicEffectManager.Initialise();

            GlobalVar.content = Content;

            base.Initialize();
        }

        #endregion

        #region Load & Unload
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            blankSprites[0] = new Sprite(Content.Load<Texture2D>(@"Textures\Items\Lights\vision"), @"Textures\Items\Lights\vision"); //Loaded first because it is used in several areas

            #region Screens & Interface

            titleScreen = Content.Load<Texture2D>(@"Textures\Interface\Title\TitleScreen");
            newGameOption = Content.Load<Texture2D>(@"Textures\Interface\Title\NewGameOption");
            newGameSelected = Content.Load<Texture2D>(@"Textures\Interface\Title\NewGameSelected");
            loadGameOption = Content.Load<Texture2D>(@"Textures\Interface\Title\LoadGameOption");
            loadGameSelected = Content.Load<Texture2D>(@"Textures\Interface\Title\LoadGameSelected");

            characterCreationScreen = Content.Load<Texture2D>(@"Textures\Interface\CharacterCreation\CharacterCreationScreen");
            clericOption = Content.Load<Texture2D>(@"Textures\Interface\CharacterCreation\ClericOption");
            clericSelected = Content.Load<Texture2D>(@"Textures\Interface\CharacterCreation\ClericSelected");
            thiefOption = Content.Load<Texture2D>(@"Textures\Interface\CharacterCreation\ThiefOption");
            thiefSelected = Content.Load<Texture2D>(@"Textures\Interface\CharacterCreation\ThiefSelected");
            warriorOption = Content.Load<Texture2D>(@"Textures\Interface\CharacterCreation\WarriorOption");
            warriorSelected = Content.Load<Texture2D>(@"Textures\Interface\CharacterCreation\WarriorSelected");
            wizardOption = Content.Load<Texture2D>(@"Textures\Interface\CharacterCreation\WizardOption");
            wizardSelected = Content.Load<Texture2D>(@"Textures\Interface\CharacterCreation\WizardSelected");

            levelMenuScreen = Content.Load<Texture2D>(@"Textures\Interface\LevelMenu\LevelMenu");
            strOption = Content.Load<Texture2D>(@"Textures\Interface\LevelMenu\StrengthOption");
            strSelected = Content.Load<Texture2D>(@"Textures\Interface\LevelMenu\StrengthSelected");
            staOption = Content.Load<Texture2D>(@"Textures\Interface\LevelMenu\StaminaOption");
            staSelected = Content.Load<Texture2D>(@"Textures\Interface\LevelMenu\StaminaSelected");
            dexOption = Content.Load<Texture2D>(@"Textures\Interface\LevelMenu\DexterityOption");
            dexSelected = Content.Load<Texture2D>(@"Textures\Interface\LevelMenu\DexteritySelected");
            intOption = Content.Load<Texture2D>(@"Textures\Interface\LevelMenu\IntelligenceOption");
            intSelected = Content.Load<Texture2D>(@"Textures\Interface\LevelMenu\IntelligenceSelected");

            loadScreen = Content.Load<Texture2D>(@"Textures\Interface\LoadScreen");

            sidebar = Content.Load<Texture2D>(@"Textures\Interface\Sidebar");
            heroFace = Content.Load<Texture2D>(@"Textures\Interface\HeroFace");

            redX = Content.Load<Texture2D>(@"Textures\Interface\RedX");
            darkRedX = Content.Load<Texture2D>(@"Textures\Interface\DarkRedX");

            chestItemBar = Content.Load<Texture2D>(@"Textures\Interface\ChestItemBar");
            shopItemBar = Content.Load<Texture2D>(@"Textures\Interface\ShopItemBar");
            pausedTexture = Content.Load<Texture2D>(@"Textures\Interface\Paused");

            battleMenuFull = Content.Load<Texture2D>(@"Textures\Interface\BattleMenu\AttackMenu1");
            battleMenuNoMagic = Content.Load<Texture2D>(@"Textures\Interface\BattleMenu\AttackMenu2");
            battleMenuSelected = Content.Load<Texture2D>(@"Textures\Interface\BattleMenu\AttackMenuSelected");

            battleTransitionScreen = Content.Load<Texture2D>(@"Textures\Interface\BattleMenu\BattleTransition");
            battleWonScreen = Content.Load<Texture2D>(@"Textures\Interface\BattleMenu\BattleWon");

            gameOverScreen = Content.Load<Texture2D>(@"Textures\Interface\GameOver");

            #endregion

            #region Light Masks

            brightness1 = Content.Load<Texture2D>(@"Textures\LightMasks\basicLightMask");
            brightness3 = Content.Load<Texture2D>(@"Textures\LightMasks\3bright");

            #endregion

            #region Loading Items

            #region Lights

            GlobalVar.torchSprite[0] = new Sprite(Content.Load<Texture2D>(@"Textures\Items\Lights\TorchSprite"), 8, 8, 2, true, true, @"Textures\Items\Lights\TorchSprite");
            GlobalVar.torch = new Light(new LightAura(brightness3, Vector2.Zero, @"Textures\LightMasks\3bright"), 1200, false, "Torch", 2, Content.Load<Texture2D>(@"Textures\Items\Inventory Images\TorchImage"), GlobalVar.torchSprite, false, Vector2.Zero, 99, 1, @"Textures\Items\Inventory Images\TorchImage");

            #endregion

            #region Weapons

            GlobalVar.unarmed = new Weapon(0, 1, 2, "Unarmed", 0, Content.Load<Texture2D>(@"Textures\Items\Lights\vision"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Lights\vision", GlobalVar.DamageType.NoDamage, GlobalVar.WeaponType.Unarmed, "None");

            GlobalVar.daggerSprite[0] = new Sprite(Content.Load<Texture2D>(@"Textures\Items\Weapons\Dagger"), 9, 9, 1, @"Textures\Items\Weapons\Dagger");
            GlobalVar.dagger = new Weapon(1, 4, 1, "Dagger", 2, Content.Load<Texture2D>(@"Textures\Items\Inventory Images\DaggerImage"), GlobalVar.daggerSprite, false, Vector2.Zero, 1, 1, @"Textures\Items\Inventory Images\DaggerImage", GlobalVar.DamageType.Slashing, GlobalVar.WeaponType.Dagger, "None");

            GlobalVar.swordSprite[0] = new Sprite(Content.Load<Texture2D>(@"Textures\Items\Weapons\Sword"), 11, 11, 1, @"Textures\Items\Weapons\Sword");
            GlobalVar.sword = new Weapon(1, 6, 1, "Iron Sword", 5, Content.Load<Texture2D>(@"Textures\Items\Inventory Images\SwordImage"), GlobalVar.swordSprite, false, Vector2.Zero, 1, 1, @"Textures\Items\Inventory Images\SwordImage", GlobalVar.DamageType.Slashing, GlobalVar.WeaponType.Sword, "None");

            GlobalVar.maceSprite[0] = new Sprite(Content.Load<Texture2D>(@"Textures\Items\Weapons\Mace"), 9, 9, 1, @"Textures\Items\Weapons\Mace");
            GlobalVar.mace = new Weapon(1, 6, 1, "Mace", 4, Content.Load<Texture2D>(@"Textures\Items\Inventory Images\MaceImage"), GlobalVar.maceSprite, false, Vector2.Zero, 1, 1, @"Textures\Items\Inventory Images\MaceImage", GlobalVar.DamageType.Piercing, GlobalVar.WeaponType.Mace, "None");

            GlobalVar.staffSprite[0] = new Sprite(Content.Load<Texture2D>(@"Textures\Items\Weapons\Staff"), 9, 9, 1, @"Textures\Items\Weapons\Staff");
            GlobalVar.staff = new Weapon(1, 4, 1, "Staff", 3, Content.Load<Texture2D>(@"Textures\Items\Inventory Images\StaffImage"), GlobalVar.staffSprite, false, Vector2.Zero, 1, 1, @"Textures\Items\Inventory Images\StaffImage", GlobalVar.DamageType.NoDamage, GlobalVar.WeaponType.Staff, "None");
            #endregion

            #region Potions

            GlobalVar.minorAttackPotion = new Potion(GlobalVar.Stat.Attack, 1, GlobalVar.BuffType.Temporary, 30, "Minor Attack Potion", 20, Content.Load<Texture2D>(@"Textures\Items\Potions\AttackPotion"), blankSprites, false, Vector2.Zero, 99, 1, @"Textures\Items\Potions\AttackPotion");
            GlobalVar.minorDefencePotion = new Potion(GlobalVar.Stat.Defence, 2, GlobalVar.BuffType.Temporary, 30, "Minor Defence Potion", 20, Content.Load<Texture2D>(@"Textures\Items\Potions\DefencePotion"), blankSprites, false, Vector2.Zero, 99, 1, @"Textures\Items\Potions\DefencePotion");
            GlobalVar.minorHealthPotion = new Potion(GlobalVar.Stat.Health, 5, GlobalVar.BuffType.Permenant, 0, "Minor Health Potion", 15, Content.Load<Texture2D>(@"Textures\Items\Potions\HealthPotion"), blankSprites, false, Vector2.Zero, 99, 1, @"Textures\Items\Potions\HealthPotion");
            GlobalVar.minorManaPotion = new Potion(GlobalVar.Stat.Mana, 5, GlobalVar.BuffType.Permenant, 0, "Minor Mana Potion", 20, Content.Load<Texture2D>(@"Textures\Items\Potions\ManaPotion"), blankSprites, false, Vector2.Zero, 99, 1, @"Textures\Items\Potions\ManaPotion");

            #endregion

            #region Armour

            GlobalVar.ironChestplate = new Armour(4, "Iron Armour", 12, Content.Load<Texture2D>(@"Textures\Items\Armour\IronChestplate"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Armour\IronChestplate", GlobalVar.ArmourType.Heavy);
            GlobalVar.leatherChestplate = new Armour(1, "Leather Armour", 6, Content.Load<Texture2D>(@"Textures\Items\Armour\LeatherChestplate"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Armour\LeatherChestplate", GlobalVar.ArmourType.Light);
            GlobalVar.unarmoured = new Armour(0, "-", 0, Content.Load<Texture2D>(@"Textures\Items\Lights\vision"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Lights\vision", GlobalVar.ArmourType.Light);

            #endregion

            #region Item Chart

            GlobalVar.itemChart = new Item[20];

            for (int i = 0; i < 6; i++)
            {
                GlobalVar.itemChart[i] = GlobalVar.torch;
            }
            GlobalVar.itemChart[6] = GlobalVar.dagger;
            GlobalVar.itemChart[7] = GlobalVar.minorAttackPotion;
            GlobalVar.itemChart[8] = GlobalVar.minorDefencePotion;
            GlobalVar.itemChart[9] = GlobalVar.minorHealthPotion;
            GlobalVar.itemChart[10] = GlobalVar.minorHealthPotion;
            GlobalVar.itemChart[11] = GlobalVar.minorHealthPotion;
            GlobalVar.itemChart[12] = GlobalVar.ironChestplate;
            GlobalVar.itemChart[13] = GlobalVar.leatherChestplate;
            GlobalVar.itemChart[14] = GlobalVar.sword;
            GlobalVar.itemChart[15] = GlobalVar.minorManaPotion;
            GlobalVar.itemChart[16] = GlobalVar.minorManaPotion;
            GlobalVar.itemChart[17] = GlobalVar.staff;
            GlobalVar.itemChart[18] = GlobalVar.mace;
            GlobalVar.itemChart[19] = GlobalVar.dagger;

            #endregion

            #region Unique Items

            GlobalVar.orcsbaneSprite[0] = new Sprite(Content.Load<Texture2D>(@"Textures\Items\Weapons\Orcsbane"), 11, 11, 1, @"Textures\Items\Weapons\Orcsbane");
            GlobalVar.orcsbane = new Weapon(2, 6, 1, "Orcsbane", 20, Content.Load<Texture2D>(@"Textures\Items\Inventory Images\OrcsbaneImage"), GlobalVar.orcsbaneSprite, false, Vector2.Zero, 1, 1, @"Textures\Items\Inventory Images\OrcsbaneImage", GlobalVar.DamageType.Slashing, GlobalVar.WeaponType.Sword, "Orc");

            GlobalVar.lightSteelArmour = new Armour(6, "Divine Steel Armour", 40, Content.Load<Texture2D>(@"Textures\Items\Armour\IronChestplate"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Armour\IronChestplate", GlobalVar.ArmourType.Light);

            GlobalVar.uniqueItems = new Item[3];
            GlobalVar.uniqueItems[0] = GlobalVar.orcsbane;
            GlobalVar.uniqueItems[1] = GlobalVar.lightSteelArmour;
            GlobalVar.uniqueItems[2] = GlobalVar.lightSteelArmour; //Will add actual unique item soon

            #endregion

            #endregion

            #region Loading Magic

            #region Spells

            GlobalVar.energyBolt = new AttackMagic(2, "Energy Bolt", 6, 1, 5, GlobalVar.DamageType.Energy, Content.Load<Texture2D>(@"Textures\Magic\Energy Magic"), @"Textures\Magic\Energy Magic",
                new Sprite(Content.Load<Texture2D>(@"Textures\Magic\Effects\EnergyBolt"), 32, 32, 5, true, false, @"Textures\Magic\Effects\EnergyBolt"), "None");
            GlobalVar.frostBurst = new AttackMagic(4, "Frost Burst", 4, 2, 5, GlobalVar.DamageType.Cold, Content.Load<Texture2D>(@"Textures\Magic\Cold Magic"), @"Textures\Magic\Cold Magic",
                new Sprite(Content.Load<Texture2D>(@"Textures\Magic\Effects\FrostBurst"), 32, 32, 5, true, false, @"Textures\Magic\Effects\FrostBurst"), "None");

            GlobalVar.spellLevels = new Magic[2];
            GlobalVar.spellLevels[0] = GlobalVar.energyBolt;
            GlobalVar.spellLevels[1] = GlobalVar.frostBurst;
            #endregion

            #region Prayers

            GlobalVar.healingPrayer = new BuffMagic(2, "Healing Prayer", GlobalVar.Stat.Health, GlobalVar.BuffType.Permenant, 4, 1, 2, 0, Content.Load<Texture2D>(@"Textures\Magic\Healing Magic"), @"Textures\Magic\Healing Magic",
                new Sprite(Content.Load<Texture2D>(@"Textures\Magic\Effects\Buff Effect"), 32, 32, 5, true, false, @"Textures\Magic\Effects\Buff Effect"));
            GlobalVar.divineRays = new AttackMagic(4, "Divine Rays", 8, 1, 6, GlobalVar.DamageType.Divine, Content.Load<Texture2D>(@"Textures\Magic\Divine Magic"), @"Textures\Magic\Divine Magic",
                new Sprite(Content.Load<Texture2D>(@"Textures\Magic\Effects\DivineRays"), 32, 32, 5, true, false, @"Textures\Magic\Effects\DivineRays"), "None");

            GlobalVar.prayerLevels = new Magic[2];
            GlobalVar.prayerLevels[0] = GlobalVar.healingPrayer;
            GlobalVar.prayerLevels[1] = GlobalVar.divineRays;


            #endregion

            #endregion

            #region Sprites
            //Player & Sprites
            playerSprites[0] = new Sprite(Content.Load<Texture2D>(@"Textures\Characters\PC\Hero"), 32, 32, 1, true, true, @"Textures\Characters\PC\Hero");
            playerSprites[1] = new Sprite(Content.Load<Texture2D>(@"Textures\Characters\PC\HeroWalking"), 32, 32, 2, true, true, @"Textures\Characters\PC\HeroWalking");
            playerSprites[2] = new Sprite(Content.Load<Texture2D>(@"Textures\Characters\PC\HeroAttack"), 32, 32, 5, true, false, 100f, @"Textures\Characters\PC\HeroAttack");
            playerSprites[3] = new Sprite(Content.Load<Texture2D>(@"Textures\Characters\PC\HeroDying"), 32, 32, 5, true, false, 100f, @"Textures\Characters\PC\HeroDying");
            playerSprites[4] = new Sprite(Content.Load<Texture2D>(@"Textures\Characters\PC\HeroCast"), 32, 32, 5, true, false, 100f, @"Textures\Characters\PC\HeroCast");

            //For loading purposes
            #endregion

            #region Camera Set Up

            Camera.LevelRectangle = new Rectangle(0, 0, DungeonMap.MapWidth * DungeonMap.TileWidth, DungeonMap.MapHeight * DungeonMap.TileHeight);
            Camera.Position = Vector2.Zero;
            Camera.ViewPortWidth = 640;
            Camera.ViewPortHeight = 600;

            #endregion

            #region Lighting Textures and Renders

            lightingEffect = Content.Load<Effect>(@"Effects\lighting");

            mainScene = new RenderTarget2D(GraphicsDevice, Camera.ViewPortWidth, Camera.ViewPortHeight);
            lightMask = new RenderTarget2D(GraphicsDevice, Camera.ViewPortWidth, Camera.ViewPortHeight);

            blackSquare = Content.Load<Texture2D>(@"Textures\Tiles\Black");

            #endregion

            #region Initalising Maps & Levels

            DungeonMap.Initialise(Content.Load<Texture2D>(@"Textures\Tiles\TileSheet"));
            LevelManager.Intialise(Content, hero);

            #endregion

            #region Videos & Audio

            introVid = Content.Load<Video>(@"Videos\Intro");

            #endregion

            #region Fonts

            GlobalVar.blackwoodCastle8 = Content.Load<SpriteFont>(@"Fonts\BlackwoodCastle8");
            GlobalVar.newsGothicBold8 = Content.Load<SpriteFont>(@"Fonts\NewsGothicBold8");
            GlobalVar.newsGothicBold12 = Content.Load<SpriteFont>(@"Fonts\NewsGothicBold12");

            #endregion

            #region Dialogue

            GlobalVar.gorcSpeech[0] = new Dialogue("I am Gorc Orclin, King of the Dungeon!", "It is great to meet you, your grace", "Sure you are", 2, 0, -1, 1);
            GlobalVar.gorcSpeech[1] = new Dialogue("How dare you question my title!", "I am sorry, your grace", "", 1, 1, -1, -1);

            #endregion

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #endregion

        #region Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            #region Title
            if (state == GameState.Title)
            {
                IsMouseVisible = true;

                MouseState ms = Mouse.GetState();

                if (!loading)
                {
                    Rectangle newGameRec = new Rectangle(0, 269, newGameOption.Width, newGameOption.Height);
                    Rectangle loadGameRec = new Rectangle(526, 269, loadGameOption.Width, loadGameOption.Height);

                    #region New Game
                    if (newGameRec.Contains(ms.X, ms.Y))
                    {
                        if (ms.LeftButton == ButtonState.Pressed && GlobalVar.mouseReleased)
                        {
                            GlobalVar.mouseReleased = false;
                            state = GameState.CharacterCreation;
                            IsMouseVisible = false;
                        }
                    }
                    #endregion
                    #region Load Game
                    if (loadGameRec.Contains(ms.X, ms.Y))
                    {
                        if (ms.LeftButton == ButtonState.Pressed && GlobalVar.mouseReleased)
                        {
                            GlobalVar.mouseReleased = false;
                            GlobalVar.loadCalled = true;
                            loading = true;
                        }
                    }
                    #endregion

                    if (ms.LeftButton == ButtonState.Released)
                    {
                        GlobalVar.mouseReleased = true;
                    }
                }
                else
                {
                    #region Was Load Successful
                    if (loadTried)
                    {
                        loadTried = false;
                        loading = false;
                        if (loadedSuccessful)
                        {
                            state = GameState.Playing;
                        }
                        else
                        {
                            state = GameState.CharacterCreation;
                        }
                    }
                    #endregion
                }
            }
            #endregion
            #region Class Selection
            else if (state == GameState.CharacterCreation)
            {
                IsMouseVisible = true;

                MouseState ms = Mouse.GetState();

                Rectangle clericRec = new Rectangle(50, 277, clericOption.Width, clericOption.Height);
                Rectangle thiefRec = new Rectangle(450, 277, thiefOption.Width, thiefOption.Height);
                Rectangle warriorRec = new Rectangle(50, 437, warriorOption.Width, warriorOption.Height);
                Rectangle wizardRec = new Rectangle(450, 437, wizardOption.Width, wizardOption.Height);

                #region Cleric
                if (clericRec.Contains(ms.X, ms.Y))
                {
                    if (ms.LeftButton == ButtonState.Pressed && GlobalVar.mouseReleased)
                    {
                        GlobalVar.mouseReleased = false;

                        hero = new Player(playerSprites, Vector2.Zero, true, new Light(new LightAura(brightness1, Vector2.Zero, @"Textures\LightMasks\basicLightMask"), -1, true, "Vision", 0, Content.Load<Texture2D>(@"Textures\Items\Lights\vision"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Lights\vision"), GlobalVar.dagger, GlobalVar.leatherChestplate, GlobalVar.CharacterClass.Cleric);
                        hero.Inventory.AddItem(GlobalVar.torch);
                        
                        LevelManager.Hero = hero;
                        GlobalVar.hero = hero;

                        state = GameState.Intro;
                        IsMouseVisible = false;
                    }
                }
                #endregion
                #region Thief
                if (thiefRec.Contains(ms.X, ms.Y))
                {
                    if (ms.LeftButton == ButtonState.Pressed && GlobalVar.mouseReleased)
                    {
                        GlobalVar.mouseReleased = false;

                        hero = new Player(playerSprites, Vector2.Zero, true, new Light(new LightAura(brightness1, Vector2.Zero, @"Textures\LightMasks\basicLightMask"), -1, true, "Vision", 0, Content.Load<Texture2D>(@"Textures\Items\Lights\vision"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Lights\vision"), GlobalVar.dagger, GlobalVar.leatherChestplate, GlobalVar.CharacterClass.Thief);
                        hero.Inventory.AddItem(GlobalVar.torch);
                        
                        LevelManager.Hero = hero;
                        GlobalVar.hero = hero;
                        
                        state = GameState.Intro;
                        IsMouseVisible = false;
                    }
                }
                #endregion
                #region Warrior
                if (warriorRec.Contains(ms.X, ms.Y))
                {
                    if (ms.LeftButton == ButtonState.Pressed && GlobalVar.mouseReleased)
                    {
                        GlobalVar.mouseReleased = false;
                        
                        hero = new Player(playerSprites, Vector2.Zero, true, new Light(new LightAura(brightness1, Vector2.Zero, @"Textures\LightMasks\basicLightMask"), -1, true, "Vision", 0, Content.Load<Texture2D>(@"Textures\Items\Lights\vision"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Lights\vision"), GlobalVar.dagger, GlobalVar.leatherChestplate, GlobalVar.CharacterClass.Warrior);
                        hero.Inventory.AddItem(GlobalVar.torch);
                        
                        LevelManager.Hero = hero;
                        GlobalVar.hero = hero;

                        state = GameState.Intro;
                        IsMouseVisible = false;
                    }
                }
                #endregion
                #region Wizard
                if (wizardRec.Contains(ms.X, ms.Y))
                {
                    if (ms.LeftButton == ButtonState.Pressed && GlobalVar.mouseReleased)
                    {
                        GlobalVar.mouseReleased = false;
                        
                        hero = new Player(playerSprites, Vector2.Zero, true, new Light(new LightAura(brightness1, Vector2.Zero, @"Textures\LightMasks\basicLightMask"), -1, true, "Vision", 0, Content.Load<Texture2D>(@"Textures\Items\Lights\vision"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Lights\vision"), GlobalVar.dagger, GlobalVar.leatherChestplate, GlobalVar.CharacterClass.Wizard);
                        hero.Inventory.AddItem(GlobalVar.torch);
                        
                        LevelManager.Hero = hero;
                        GlobalVar.hero = hero;
                        
                        state = GameState.Intro;
                        IsMouseVisible = false;
                    }
                }
                #endregion

                if (ms.LeftButton == ButtonState.Released)
                {
                    GlobalVar.mouseReleased = true;
                }
            }
            #endregion
            #region Loading
            else if (state == GameState.Loading || state == GameState.Intro)
            {
                StartNewGame();
                state = GameState.Playing;
            }
            #endregion
            #region Playing
            else if (state == GameState.Playing)
            {
                hero.Update(gameTime);
                LevelManager.Update(gameTime);
                LightManager.Update(gameTime);
                MagicEffectManager.Update(gameTime);

                #region Interface

                MouseState ms = Mouse.GetState();

                if (!GlobalVar.battleTransition && !GlobalVar.victoryTransition)
                {
                    IsMouseVisible = true;
                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        #region Inventory/Spell Book
                        if (!GlobalVar.displaySpellBook && !GlobalVar.displayShop)
                        {
                            for (int i = 0; i < hero.Inventory.Items.Count; i++)
                            {
                                Rectangle rec = new Rectangle((int)GlobalVar.inventoryPos[i].X, (int)GlobalVar.inventoryPos[i].Y, 28, 30);

                                if (rec.Contains(ms.X, ms.Y) && GlobalVar.mouseReleased)
                                {
                                    GlobalVar.mouseReleased = false;
                                    Rectangle xRec;

                                    if (i % 4 != 3)
                                    {
                                        xRec = new Rectangle((int)GlobalVar.inventoryPos[i].X + 24, (int)GlobalVar.inventoryPos[i].Y, 5, 5);
                                    }
                                    else
                                    {
                                        xRec = new Rectangle((int)GlobalVar.inventoryPos[i].X + 23, (int)GlobalVar.inventoryPos[i].Y, 5, 5);
                                    }

                                    if (xRec.Contains(ms.X, ms.Y))
                                    {
                                        hero.Inventory.RemoveItem(i);
                                    }
                                    else
                                    {
                                        hero.Inventory.Items[i].OnClick(hero);
                                    }
                                }
                            }
                        }
                        else if(GlobalVar.displaySpellBook)
                        {
                            for (int i = 0; i < hero.SpellBook.Spells.Count; i++)
                            {
                                Rectangle rec = new Rectangle((int)GlobalVar.inventoryPos[i].X, (int)GlobalVar.inventoryPos[i].Y, 28, 30);

                                if (rec.Contains(ms.X, ms.Y) && GlobalVar.mouseReleased)
                                {
                                    GlobalVar.mouseReleased = false;

                                    hero.Spell = i;
                                }
                            }
                        }
                        #endregion

                        #region Display Chest Bar
                        if (GlobalVar.displayChestBar)
                        {
                            for (int x = 0; x < GlobalVar.displayChest.Inventory.Items.Count; x++)
                            {
                                Rectangle itemRec = new Rectangle((int)GlobalVar.chestInvPos[x].X, (int)GlobalVar.chestInvPos[x].Y, 28, 30);

                                if (itemRec.Contains(ms.X, ms.Y) && GlobalVar.mouseReleased)
                                {
                                    GlobalVar.mouseReleased = false;
                                    if (hero.Inventory.Items.Count != hero.Inventory.MaxItems)
                                    {
                                        int leftOver = hero.Inventory.AddItem(GlobalVar.displayChest.Inventory.Items[x]);

                                        if (leftOver == 0)
                                        {
                                            GlobalVar.displayChest.Inventory.RemoveItem(x);
                                        }
                                        else
                                        {
                                            int decStack = (leftOver - GlobalVar.displayChest.Inventory.Items[x].Stack) * -1;
                                            GlobalVar.displayChest.Inventory.Items[x].DecStack(decStack);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Shop

                        if (GlobalVar.displayShop)
                        {
                            for (int x = 0; x < GlobalVar.shop.Items.Count; x++)
                            {
                                Rectangle itemRec = new Rectangle((int)GlobalVar.shopInvPos[x].X, (int)GlobalVar.shopInvPos[x].Y, 28, 30);
                                
                                if (itemRec.Contains(ms.X, ms.Y) && GlobalVar.mouseReleased)
                                {
                                    GlobalVar.mouseReleased = false;
                                    if (hero.Inventory.Items.Count != hero.Inventory.MaxItems)
                                    {
                                        if (hero.Gold >= GlobalVar.shop.Items[x].Price)
                                        {
                                            hero.DecGold(GlobalVar.shop.Items[x].Price);
                                            hero.Inventory.AddItem(GlobalVar.shop.Items[x]);
                                        }
                                    }
                                }
                            }
                            for (int i = 0; i < hero.Inventory.Items.Count; i++)
                            {
                                Rectangle rec = new Rectangle((int)GlobalVar.inventoryPos[i].X, (int)GlobalVar.inventoryPos[i].Y, 28, 30);

                                if (rec.Contains(ms.X, ms.Y) && GlobalVar.mouseReleased)
                                {
                                    GlobalVar.mouseReleased = false;
                                    hero.IncGold((hero.Inventory.Items[i].Price * 75 / 100) * hero.Inventory.Items[i].Stack);
                                    hero.Inventory.RemoveItem(i);
                                }
                            }
                        }

                        #endregion

                        #region Dialogue

                        if (GlobalVar.displayDialogue)
                        {
                            GlobalVar.speech.Update();
                        }

                        #endregion
                    }
                    else
                    {
                        GlobalVar.mouseReleased = true;
                    }
                }
                else
                {
                    IsMouseVisible = false;
                }

                #endregion

                #region BattleTransitions

                #region Battle Transition
                if (GlobalVar.battleTransition)
                {
                    GlobalVar.timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (GlobalVar.timer > 1.5)
                    {
                        GlobalVar.battleMode = true;
                        GlobalVar.battleTransition = false;
                        GlobalVar.timer = 0;

                        GlobalVar.oldDirection = hero.Direction;

                        if (GlobalVar.hero.Direction > 1)
                        {
                            if (GlobalVar.enemy.Position.X > hero.Position.X)
                            {
                                //Position the battle properly
                                hero.Direction = 3;
                                GlobalVar.enemy.Direction = 2;
                                
                                hero.Position = new Vector2(DungeonMap.GetCellByPoint(hero.Position).X * DungeonMap.TileWidth, hero.Position.Y);

                                GlobalVar.enemy.Position = new Vector2(hero.Position.X + hero.Sprite.Width + 2, hero.Position.Y + hero.Sprite.Height / 2 - GlobalVar.enemy.Sprite.Height / 2);
                            }
                            else
                            {
                                //Position the battle properly
                                hero.Direction = 2;
                                GlobalVar.enemy.Direction = 3;

                                hero.Position = new Vector2(DungeonMap.GetCellByPoint(hero.Position).X * DungeonMap.TileWidth, hero.Position.Y);

                                GlobalVar.enemy.Position = new Vector2(hero.Position.X - hero.Sprite.Width - 2, hero.Position.Y + hero.Sprite.Height / 2 - GlobalVar.enemy.Sprite.Height / 2);

                            }
                        }
                        else
                        {
                            if (GlobalVar.enemy.Position.Y > hero.Position.Y)
                            {
                                //Position the battle properly
                                hero.Direction = 1;
                                GlobalVar.enemy.Direction = 0;
                                
                                hero.Position = new Vector2(hero.Position.X, DungeonMap.GetCellByPoint(hero.Position).Y * DungeonMap.TileHeight);

                                GlobalVar.enemy.Position = new Vector2(hero.Position.X + hero.Sprite.Width / 2 - GlobalVar.enemy.Sprite.Width / 2, hero.Position.Y + hero.Sprite.Height + 2);
                            }
                            else
                            {
                                //Position the battle properly
                                hero.Direction = 0;
                                GlobalVar.enemy.Direction = 1;

                                hero.Position = new Vector2(hero.Position.X, DungeonMap.GetCellByPoint(hero.Position).Y * DungeonMap.TileHeight);

                                GlobalVar.enemy.Position = new Vector2(hero.Position.X + hero.Sprite.Width / 2 - GlobalVar.enemy.Sprite.Width / 2, hero.Position.Y - hero.Sprite.Height + 2);

                            }
                        }
                    }
                }
                #endregion

                #region Victory Transition
                if (GlobalVar.victoryTransition)
                {
                    GlobalVar.timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (GlobalVar.timer > 1.5)
                    {
                        GlobalVar.victoryTransition = false;
                        GlobalVar.timer = 0;

                        hero.Direction = GlobalVar.oldDirection;
                        hero.IncGold(GlobalVar.enemy.Gold);
                        GlobalVar.displayChest = new Chest(blankSprites, Vector2.Zero, 1, GlobalVar.enemy.Inventory.Items.ToArray(), 1);
                        GlobalVar.displayChestBar = true;
                    }
                }
                #endregion

                if (GlobalVar.battleMode && (!GlobalVar.enemy.Alive || !hero.Alive))
                {
                    if (!GlobalVar.enemy.Alive)
                    {
                        hero.GainXP(GlobalVar.enemy.XPValue);
                        GlobalVar.victoryTransition = true;
                        GlobalVar.battleMode = false;
                        GlobalVar.drawBattleMsgString = false;
                    }
                    if (!hero.Alive)
                    {
                        if (GlobalVar.restPoint.Rested && !GlobalVar.restPoint.Resurrected)
                        {
                            LevelManager.Respawn();
                            hero.Mode = GlobalVar.CharacterMode.Idle;

                            //Restore enemy to original position and mode;
                            GlobalVar.enemy.Direction = GlobalVar.enemyOldDirection;
                            GlobalVar.enemy.Position = GlobalVar.enemyOldPosition;
                            GlobalVar.enemy.Mode = GlobalVar.enemyOldMode;

                            GlobalVar.enemy.FillHealth();
                            GlobalVar.enemy.FillMana();

                            GlobalVar.battleMode = false;
                            GlobalVar.drawBattleMsgString = false;
                            GlobalVar.paused = false;

                            GlobalVar.restPoint.Resurrected = true;
                        }
                        else
                        {
                            state = GameState.Gameover;
                        }
                    
                    }

                }

                #endregion

                #region LevelMenu
                if (GlobalVar.levelMenu && !GlobalVar.battleMode && !GlobalVar.victoryTransition)
                {
                    Rectangle strRec = new Rectangle(210, 230, 220, 60);
                    Rectangle staRec = new Rectangle(210, 295, 220, 60);
                    Rectangle dexRec = new Rectangle(210, 360, 220, 60);
                    Rectangle intRec = new Rectangle(210, 425, 220, 60);

                    if (ms.LeftButton == ButtonState.Pressed && GlobalVar.mouseReleased)
                    {
                        GlobalVar.mouseReleased = false;

                        if (strRec.Contains(ms.X, ms.Y))
                        {
                            hero.LevelUp(GlobalVar.Stat.Strength);
                            GlobalVar.levelMenu = false;
                            GlobalVar.paused = false;
                        }
                        else if (staRec.Contains(ms.X, ms.Y))
                        {
                            hero.LevelUp(GlobalVar.Stat.Stamina);
                            GlobalVar.levelMenu = false;
                            GlobalVar.paused = false;
                        }
                        else if (dexRec.Contains(ms.X, ms.Y))
                        {
                            hero.LevelUp(GlobalVar.Stat.Dexterity);
                            GlobalVar.levelMenu = false;
                            GlobalVar.paused = false;
                        }
                        else if (intRec.Contains(ms.X, ms.Y))
                        {
                            hero.LevelUp(GlobalVar.Stat.Intelligence);
                            GlobalVar.levelMenu = false;
                            GlobalVar.paused = false;
                        }
                            
                    }
                    else if(ms.LeftButton == ButtonState.Released)
                    {
                        GlobalVar.mouseReleased = true;
                    }
                }
                #endregion

                #region ManaRegen
                if (!GlobalVar.paused)
                {
                    if (hero.CurrentMana < hero.MaxMana)
                    {
                        manaRegenTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                        if (manaRegenTimer >= manaRegenInterval)
                        {
                            hero.IncMana(1);
                            manaRegenTimer = 0f;
                        }
                    }
                }
                #endregion
            }
            #endregion
            #region GameOver
            else if (state == GameState.Gameover)
            {
                GlobalVar.timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (GlobalVar.timer > 2)
                {
                    GlobalVar.paused = false;
                    GlobalVar.displayChestBar = false;
                    GlobalVar.battleMode = false;
                    GlobalVar.battleTransition = false;
                    GlobalVar.drawBattleMsgString = false;
                    GlobalVar.drawMagicMsg = false;
                    GlobalVar.timer = 0;
                    GlobalVar.victoryTransition = false;
                    GlobalVar.displaySpellBook = false;
                    GlobalVar.levelMenu = false;

                    for (int i = LightManager.Lights.Count - 1; i >= 0; i--)
                    {
                        LightManager.Lights.RemoveAt(i);
                    }

                    state = GameState.Title;
                }
            }
            #endregion

            #region Saving & Loading

            if (GlobalVar.saveCalled)
            {
                GlobalVar.saveCalled = false;
                // Set the request flag
                if ((!Guide.IsVisible) && (GameSaveRequested == false))
                {
                    GameSaveRequested = true;
                    result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                }
            }
            if ((GameSaveRequested) && (result.IsCompleted))
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    Save(device);
                }
                // Reset the request flag
                GameSaveRequested = false;
            }

            if (GlobalVar.loadCalled)
            {
                GlobalVar.loadCalled = false;
                // Set the request flag
                if ((!Guide.IsVisible) && (GameLoadRequested == false))
                {
                    GameLoadRequested = true;
                    result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                }
            }
            if ((GameLoadRequested) && (result.IsCompleted))
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    loadedSuccessful = Load(device);
                    loadTried = true;
                }
                // Reset the request flag
                GameLoadRequested = false;
            }

            #endregion

            base.Update(gameTime);
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            #region Title

            if (state == GameState.Title)
            {
                GraphicsDevice.Clear(Color.Black);

                Rectangle newGameRec = new Rectangle(0, 269, newGameOption.Width, newGameOption.Height);
                Rectangle loadGameRec = new Rectangle(526, 269, loadGameOption.Width, loadGameOption.Height);

                MouseState ms = Mouse.GetState();

                spriteBatch.Begin();
                spriteBatch.Draw(titleScreen, new Rectangle(0, 0, titleScreen.Width, titleScreen.Height), Color.White);
                if (newGameRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(newGameSelected, newGameRec, Color.White);
                }
                else
                {
                    spriteBatch.Draw(newGameOption, newGameRec, Color.White);
                }
                if(loadGameRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(loadGameSelected, loadGameRec, Color.White);
                }
                else
                {
                    spriteBatch.Draw(loadGameOption, loadGameRec, Color.White);
                }
                spriteBatch.End();
            }

            #endregion
            #region Class Selection
            if (state == GameState.CharacterCreation)
            {
                GraphicsDevice.Clear(Color.Black);

                Rectangle clericRec = new Rectangle(50, 277, clericOption.Width, clericOption.Height);
                Rectangle thiefRec = new Rectangle(450, 277, thiefOption.Width, thiefOption.Height);
                Rectangle warriorRec = new Rectangle(50, 437, warriorOption.Width, warriorOption.Height);
                Rectangle wizardRec = new Rectangle(450, 437, wizardOption.Width, wizardOption.Height);

                MouseState ms = Mouse.GetState();

                spriteBatch.Begin();
                spriteBatch.Draw(characterCreationScreen, new Rectangle(0, 0, characterCreationScreen.Width, characterCreationScreen.Height), Color.White);
                if (clericRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(clericSelected, clericRec, Color.White);
                }
                else
                {
                    spriteBatch.Draw(clericOption, clericRec, Color.White);
                }
                if (thiefRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(thiefSelected, thiefRec, Color.White);
                }
                else
                {
                    spriteBatch.Draw(thiefOption, thiefRec, Color.White);
                }
                if (warriorRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(warriorSelected, warriorRec, Color.White);
                }
                else
                {
                    spriteBatch.Draw(warriorOption, warriorRec, Color.White);
                }
                if (wizardRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(wizardSelected, wizardRec, Color.White);
                }
                else
                {
                    spriteBatch.Draw(wizardOption, wizardRec, Color.White);
                }
                spriteBatch.End();
            }
            #endregion
            #region Loading
            if (state == GameState.Loading)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(loadScreen, new Rectangle(0, 0, loadScreen.Width, loadScreen.Height), Color.White);
                spriteBatch.End();
            }
            #endregion
            #region Playing
            if (state == GameState.Playing)
            {
                DrawMainScene(gameTime);
                DrawLightMask(gameTime);

                GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                lightingEffect.Parameters["lightMask"].SetValue(lightMask);
                lightingEffect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(mainScene, new Vector2(0, 0), Color.White);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                DrawInterface(spriteBatch);

                spriteBatch.End();
            }
            #endregion
            #region GameOver
            if (state == GameState.Gameover)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(gameOverScreen, new Rectangle(0, 0, gameOverScreen.Width, gameOverScreen.Height), Color.White);
                spriteBatch.End();
            }
            #endregion
            base.Draw(gameTime);
        }

        private void DrawMainScene(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(mainScene);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            
            DungeonMap.Draw(spriteBatch);

            hero.Draw(spriteBatch);

            LightManager.Draw(spriteBatch);
            LevelManager.Draw(spriteBatch);
            MagicEffectManager.Draw(spriteBatch);

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

        }

        private void DrawLightMask(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(lightMask);
            GraphicsDevice.Clear(Color.Black);

            // Create a Black Background
            spriteBatch.Begin();
            spriteBatch.Draw(blackSquare, new Vector2(0, 0), new Rectangle(0, 0, 800, 800), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            LightManager.DrawAura(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
        }

        private void DrawInterface(SpriteBatch spriteBatch)
        {
            MouseState ms = Mouse.GetState();

            #region Calc Hero Health
            int healthState;
            //Calculate what the health state of the hero is
            double heroHealth = (double)hero.CurrentHealth / (double)hero.MaxHealth * 100;
            if (heroHealth >= 75.0)
            {
                healthState = 0;
            }
            else if (heroHealth >= 50)
            {
                healthState = 1;
            }
            else if (heroHealth >= 25)
            {
                healthState = 2;
            }
            else
            {
                healthState = 3;
            }
            #endregion

            if (!GlobalVar.battleTransition && !GlobalVar.victoryTransition)
            {
                #region MoveMode

                string mode = "";
                if (hero.Sneaking)
                {
                    mode = "SNEAKING";
                }
                if (hero.Sprinting)
                {
                    mode = "SPRINTING";
                }

                spriteBatch.DrawString(GlobalVar.newsGothicBold8, mode, new Vector2(5, 5), Color.White);

                #endregion

                #region Sidebar

                int manaMod = 0;
                if (hero.CharacterClass == GlobalVar.CharacterClass.Warrior)
                {
                    manaMod = 20;
                }

                spriteBatch.Draw(sidebar, new Rectangle(640, 0, 160, 600), sidebar.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                spriteBatch.Draw(heroFace, new Rectangle(670, 20, 60, 60), new Rectangle(healthState * 60, 0, 60, 60), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(GlobalVar.newsGothicBold8, "Lvl: " + hero.Level, new Vector2(735, 20), Color.White);
                spriteBatch.DrawString(GlobalVar.newsGothicBold8, "XP: " + hero.XP, new Vector2(735, 35), Color.White);
                spriteBatch.DrawString(GlobalVar.newsGothicBold8, "Att: " + hero.Attack, new Vector2(735, 50), Color.White);
                spriteBatch.DrawString(GlobalVar.newsGothicBold8, "Def: " + hero.Defence, new Vector2(735, 65), Color.White);
                spriteBatch.DrawString(GlobalVar.newsGothicBold8, "HP: " + hero.CurrentHealth + "\\" + hero.MaxHealth, new Vector2(670, 90), Color.White);
                if (hero.CharacterClass != GlobalVar.CharacterClass.Warrior)
                {
                    spriteBatch.DrawString(GlobalVar.newsGothicBold8, "MP: " + hero.CurrentMana + "\\" + hero.MaxMana, new Vector2(670, 110), Color.White);
                }
                spriteBatch.DrawString(GlobalVar.newsGothicBold8, "Str: " + hero.Strength, new Vector2(670, 130 - manaMod), Color.White);
                spriteBatch.DrawString(GlobalVar.newsGothicBold8, "Sta: " + hero.Stamina, new Vector2(730, 130 - manaMod), Color.White);
                spriteBatch.DrawString(GlobalVar.newsGothicBold8, "Dex: " + hero.Dexterity, new Vector2(670, 150 - manaMod), Color.White);
                spriteBatch.DrawString(GlobalVar.newsGothicBold8, "Int: " + hero.Intelligence, new Vector2(730, 150 - manaMod), Color.White);
                
                #endregion

                #region Equipped Items
                if (hero.Weapon != null && hero.Weapon.DrawSprite)
                {
                    spriteBatch.DrawString(GlobalVar.newsGothicBold8, "Weapon: \n   " + hero.Weapon.Name, new Vector2(670, 170), Color.White);
                }
                if (hero.Armour != null && hero.Armour.Alive)
                {
                   spriteBatch.DrawString(GlobalVar.newsGothicBold8, "Armour: \n   " + hero.Armour.Name, new Vector2(670, 200), Color.White);
                }

                if (hero.CharacterClass == GlobalVar.CharacterClass.Cleric)
                {
                    spriteBatch.DrawString(GlobalVar.newsGothicBold8, "Prayer: \n   " + hero.SpellBook.Spells[hero.Spell].Name, new Vector2(670, 230), Color.White);
                }
                else if (hero.CharacterClass == GlobalVar.CharacterClass.Wizard)
                {
                    spriteBatch.DrawString(GlobalVar.newsGothicBold8, "Spell: \n   " + hero.SpellBook.Spells[hero.Spell].Name, new Vector2(670, 230), Color.White);
                }
                #endregion

                #region Inv/Spells
                if (!GlobalVar.displaySpellBook)
                {
                    hero.Inventory.Draw(spriteBatch);

                    for (int i = 0; i < hero.Inventory.Items.Count; i++)
                    {
                        #region Deletion X
                        Rectangle xRec;

                        if (i % 4 != 3)
                        {
                            xRec = new Rectangle((int)GlobalVar.inventoryPos[i].X + 24, (int)GlobalVar.inventoryPos[i].Y, 5, 5);
                        }
                        else
                        {
                            xRec = new Rectangle((int)GlobalVar.inventoryPos[i].X + 23, (int)GlobalVar.inventoryPos[i].Y, 5, 5);
                        }

                        if (xRec.Contains(ms.X, ms.Y))
                        {
                            spriteBatch.Draw(darkRedX, xRec, Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(redX, xRec, Color.White);
                        }
                        #endregion

                        #region Display Item Name
                        Rectangle rec = new Rectangle((int)GlobalVar.inventoryPos[i].X, (int)GlobalVar.inventoryPos[i].Y, 28, 30);
                        
                        if (rec.Contains(ms.X, ms.Y))
                        {
                            string itemName = hero.Inventory.Items[i].Name;

                            #region Weapon
                            if (hero.Inventory.Items[i] is Weapon)
                            {
                                Weapon weapon = (Weapon)hero.Inventory.Items[i];
                                if (weapon.WeaponType == hero.PreferredWeapon)
                                {
                                    itemName += " (P)";
                                }

                                itemName += "\n(ATT: +" + weapon.AttackBonus + " DAM: " + weapon.NumDice + "-" + weapon.DamageRoll * weapon.NumDice + ")";
                            }
                            #endregion
                            #region Armour
                            else if (hero.Inventory.Items[i] is Armour)
                            {
                                Armour armour = (Armour)hero.Inventory.Items[i];

                                if (armour.Type == GlobalVar.ArmourType.Light)
                                {
                                    itemName += " (L)";
                                }
                                else
                                {
                                    itemName += " (H)";
                                }

                                itemName += "\n(DEF: +" + armour.DefenceBonus + ")";
                            }
                            #endregion
                            #region Potion
                            else if (hero.Inventory.Items[i] is Potion)
                            {
                                Potion potion = (Potion)hero.Inventory.Items[i];

                                itemName += "\n";

                                switch (potion.Type)
                                {
                                    case GlobalVar.Stat.Attack:
                                        itemName += "ATT: ";
                                        break;
                                    case GlobalVar.Stat.Defence:
                                        itemName += "DEF: ";
                                        break;
                                    case GlobalVar.Stat.Dexterity:
                                        itemName += "DEX: ";
                                        break;
                                    case GlobalVar.Stat.Health:
                                        itemName += "HP: ";
                                        break;
                                    case GlobalVar.Stat.Intelligence:
                                        itemName += "INT: ";
                                        break;
                                    case GlobalVar.Stat.Mana:
                                        itemName += "MP: ";
                                        break;
                                    case GlobalVar.Stat.Stamina:
                                        itemName += "STA: ";
                                        break;
                                    case GlobalVar.Stat.Strength:
                                        itemName += "STR: ";
                                        break;
                                }

                                itemName += "+" + potion.Amount;

                                if (potion.BuffType == GlobalVar.BuffType.Temporary)
                                {
                                    itemName += " (Temp)";
                                }
                            }
                            #endregion

                            if (hero.Inventory.Items[i] is Weapon || hero.Inventory.Items[i] is Armour || hero.Inventory.Items[i] is Potion)
                            {
                                spriteBatch.DrawString(GlobalVar.newsGothicBold8, itemName, new Vector2(662, 270), Color.White);
                            }
                            else
                            {
                                spriteBatch.DrawString(GlobalVar.newsGothicBold8, itemName, new Vector2(662, 285), Color.White);
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    hero.SpellBook.Draw(spriteBatch);

                    #region Display Spell/Prayer Name
                    
                    for (int i = 0; i < hero.SpellBook.Spells.Count; i++)
                    {
                        Rectangle rec = new Rectangle((int)GlobalVar.inventoryPos[i].X, (int)GlobalVar.inventoryPos[i].Y, 28, 30);

                        String spellName = hero.SpellBook.Spells[i].Name;
                        spellName += " (" + hero.SpellBook.Spells[i].ManaCost + " MP)";

                        if (hero.SpellBook.Spells[i] is BuffMagic)
                        {
                            BuffMagic spell = (BuffMagic)hero.SpellBook.Spells[i];

                            spellName += "\n(";

                            switch (spell.Type)
                            {
                                case GlobalVar.Stat.Attack:
                                    spellName += "ATT: ";
                                        break;
                                case GlobalVar.Stat.Defence:
                                        spellName += "DEF: ";
                                        break;
                                case GlobalVar.Stat.Health:
                                        spellName += "HP: ";
                                        break;
                                case GlobalVar.Stat.Mana:
                                        spellName += "MP: ";
                                        break;
                                case GlobalVar.Stat.Dexterity:
                                        spellName += "DEX: ";
                                        break;
                                case GlobalVar.Stat.Intelligence:
                                        spellName += "INT: ";
                                        break;
                                case GlobalVar.Stat.Stamina:
                                        spellName += "STA: ";
                                        break;
                                case GlobalVar.Stat.Strength:
                                        spellName += "STR: ";
                                        break;
                            }
                            spellName += (spell.Mod + spell.NumDice) + "-" + (spell.Amount * spell.NumDice + spell.Mod) + ")";
                        }
                        else
                        {
                            AttackMagic spell = (AttackMagic)hero.SpellBook.Spells[i];
                            spellName += "\n(ATT: " + spell.AttackBonus + " DAM: " + spell.NumDice + "-" + spell.DamageRoll * spell.NumDice + ")";
                        }

                        if (rec.Contains(ms.X, ms.Y))
                        {
                            spriteBatch.DrawString(GlobalVar.newsGothicBold8, spellName, new Vector2(662, 270), Color.White);
                        }
                    }
                    #endregion
                }
                #endregion
            }

            #region LevelMenu
            if (GlobalVar.levelMenu && !GlobalVar.battleMode && !GlobalVar.victoryTransition)
            {
                Rectangle menuRec = new Rectangle(170, 100, 300, 400);
                Rectangle strRec = new Rectangle(210, 230, 220, 60);
                Rectangle staRec = new Rectangle(210, 295, 220, 60);
                Rectangle dexRec = new Rectangle(210, 360, 220, 60);
                Rectangle intRec = new Rectangle(210, 425, 220, 60);

                spriteBatch.Draw(levelMenuScreen, menuRec, levelMenuScreen.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);

                if (strRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(strOption, strRec, Color.White);
                }
                else
                {
                    spriteBatch.Draw(strSelected, strRec, Color.White);
                }
                
                if (staRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(staOption, staRec, Color.White);
                }
                else
                {
                    spriteBatch.Draw(staSelected, staRec, Color.White);
                }
                
                if (dexRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(dexOption, dexRec, Color.White);
                }
                else
                {
                    spriteBatch.Draw(dexSelected, dexRec, Color.White);
                }
                
                if (intRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(intOption, intRec, Color.White);
                }
                else
                {
                    spriteBatch.Draw(intSelected, intRec, Color.White);
                }

            }
            #endregion

            #region Opened Chest

            if (GlobalVar.displayChestBar)
            {
                spriteBatch.Draw(chestItemBar, GlobalVar.chestRec, chestItemBar.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);

                for (int x = 0; x < GlobalVar.displayChest.Inventory.Items.Count; x++)
                {
                    GlobalVar.displayChest.Inventory.Items[x].DrawImage(spriteBatch, GlobalVar.chestInvPos[x]);
                    spriteBatch.DrawString(GlobalVar.newsGothicBold8, "x" + GlobalVar.displayChest.Inventory.Items[x].Stack, new Vector2(GlobalVar.chestInvPos[x].X + 1, GlobalVar.chestInvPos[x].Y + 19), Color.White);

                    Rectangle rect = new Rectangle((int)GlobalVar.chestInvPos[x].X, (int)GlobalVar.chestInvPos[x].Y, 28, 30);

                    if (rect.Contains(ms.X, ms.Y))
                    {
                        string itemName = GlobalVar.displayChest.Inventory.Items[x].Name;

                        #region Weapon
                        if (GlobalVar.displayChest.Inventory.Items[x] is Weapon)
                        {
                            Weapon weapon = (Weapon)GlobalVar.displayChest.Inventory.Items[x];
                            if (weapon.WeaponType == hero.PreferredWeapon)
                            {
                                itemName += " (P)";
                            }

                            itemName += "\n(ATT: +" + weapon.AttackBonus + " DAM: " + weapon.NumDice + "-" + weapon.DamageRoll * weapon.NumDice + ")";
                        }
                        #endregion
                        #region Armour
                        else if (GlobalVar.displayChest.Inventory.Items[x] is Armour)
                        {
                            Armour armour = (Armour)GlobalVar.displayChest.Inventory.Items[x];

                            if (armour.Type == GlobalVar.ArmourType.Light)
                            {
                                itemName += " (L)";
                            }
                            else
                            {
                                itemName += " (H)";
                            }

                            itemName += "\n(DEF: +" + armour.DefenceBonus + ")";
                        }
                        #endregion
                        #region Potion
                        else if (GlobalVar.displayChest.Inventory.Items[x] is Potion)
                        {
                            Potion potion = (Potion)GlobalVar.displayChest.Inventory.Items[x];
                            itemName += "\n";

                            switch (potion.Type)
                            {
                                case GlobalVar.Stat.Attack:
                                    itemName += "ATT: ";
                                    break;
                                case GlobalVar.Stat.Defence:
                                    itemName += "DEF: ";
                                    break;
                                case GlobalVar.Stat.Dexterity:
                                    itemName += "DEX: ";
                                    break;
                                case GlobalVar.Stat.Health:
                                    itemName += "HP: ";
                                    break;
                                case GlobalVar.Stat.Intelligence:
                                    itemName += "INT: ";
                                    break;
                                case GlobalVar.Stat.Mana:
                                    itemName += "MP: ";
                                    break;
                                case GlobalVar.Stat.Stamina:
                                    itemName += "STA: ";
                                    break;
                                case GlobalVar.Stat.Strength:
                                    itemName += "STR: ";
                                    break;
                            }

                            itemName += "+" + potion.Amount;

                            if (potion.BuffType == GlobalVar.BuffType.Temporary)
                            {
                                itemName += " (Temp)";
                            }
                        }
                        #endregion

                        if (GlobalVar.displayChest.Inventory.Items[x] is Weapon || GlobalVar.displayChest.Inventory.Items[x] is Armour || GlobalVar.displayChest.Inventory.Items[x] is Potion)
                        {
                            spriteBatch.DrawString(GlobalVar.newsGothicBold8, itemName, new Vector2(662, 270), Color.White);
                        }
                        else
                        {
                            spriteBatch.DrawString(GlobalVar.newsGothicBold8, itemName, new Vector2(662, 285), Color.White);
                        }
                    }
                }
            }

            #endregion

            #region Shop

            if (GlobalVar.displayShop)
            {
                spriteBatch.Draw(shopItemBar, GlobalVar.shopRec, shopItemBar.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);

                for (int x = 0; x < GlobalVar.shop.Items.Count; x++)
                {
                    GlobalVar.shop.Items[x].DrawImage(spriteBatch, GlobalVar.shopInvPos[x]);
                    spriteBatch.DrawString(GlobalVar.newsGothicBold8, GlobalVar.shop.Items[x].Price + "G", new Vector2(GlobalVar.shopInvPos[x].X + 1, GlobalVar.shopInvPos[x].Y + 19), Color.White);

                    Rectangle rect = new Rectangle((int)GlobalVar.shopInvPos[x].X, (int)GlobalVar.shopInvPos[x].Y, 28, 30);

                    if (rect.Contains(ms.X, ms.Y))
                    {
                        string itemName = GlobalVar.shop.Items[x].Name;

                        #region Weapon
                        if (GlobalVar.shop.Items[x] is Weapon)
                        {
                            Weapon weapon = (Weapon)GlobalVar.shop.Items[x];
                            if (weapon.WeaponType == hero.PreferredWeapon)
                            {
                                itemName += " (P)";
                            }

                            itemName += "\n(ATT: +" + weapon.AttackBonus + " DAM: " + weapon.NumDice + "-" + weapon.DamageRoll * weapon.NumDice + ")";
                        }
                        #endregion
                        #region Armour
                        else if (GlobalVar.shop.Items[x] is Armour)
                        {
                            Armour armour = (Armour)GlobalVar.shop.Items[x];

                            if (armour.Type == GlobalVar.ArmourType.Light)
                            {
                                itemName += " (L)";
                            }
                            else
                            {
                                itemName += " (H)";
                            }

                            itemName += "\n(DEF: +" + armour.DefenceBonus + ")";
                        }
                        #endregion
                        #region Potion
                        else if (GlobalVar.shop.Items[x] is Potion)
                        {
                            Potion potion = (Potion)GlobalVar.shop.Items[x];
                            itemName += "\n";

                            switch (potion.Type)
                            {
                                case GlobalVar.Stat.Attack:
                                    itemName += "ATT: ";
                                    break;
                                case GlobalVar.Stat.Defence:
                                    itemName += "DEF: ";
                                    break;
                                case GlobalVar.Stat.Dexterity:
                                    itemName += "DEX: ";
                                    break;
                                case GlobalVar.Stat.Health:
                                    itemName += "HP: ";
                                    break;
                                case GlobalVar.Stat.Intelligence:
                                    itemName += "INT: ";
                                    break;
                                case GlobalVar.Stat.Mana:
                                    itemName += "MP: ";
                                    break;
                                case GlobalVar.Stat.Stamina:
                                    itemName += "STA: ";
                                    break;
                                case GlobalVar.Stat.Strength:
                                    itemName += "STR: ";
                                    break;
                            }

                            itemName += "+" + potion.Amount;

                            if (potion.BuffType == GlobalVar.BuffType.Temporary)
                            {
                                itemName += " (Temp)";
                            }
                        }
                        #endregion

                        if (GlobalVar.shop.Items[x] is Weapon || GlobalVar.shop.Items[x] is Armour || GlobalVar.shop.Items[x] is Potion)
                        {
                            spriteBatch.DrawString(GlobalVar.newsGothicBold8, itemName, new Vector2(662, 270), Color.White);
                        }
                        else
                        {
                            spriteBatch.DrawString(GlobalVar.newsGothicBold8, itemName, new Vector2(662, 285), Color.White);
                        }
                    }
                }
            }

            #endregion

            #region Paused

            if (GlobalVar.paused && !GlobalVar.displayChestBar && !GlobalVar.battleMode && !GlobalVar.battleTransition && !GlobalVar.levelMenu && !GlobalVar.displayDialogue && !GlobalVar.displayShop)
            {
                spriteBatch.Draw(pausedTexture, new Rectangle(95, 250, 450, 100 ), pausedTexture.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            }

            #endregion

            #region BattleTransition
            if (GlobalVar.battleTransition)
            {
                spriteBatch.Draw(battleTransitionScreen, new Rectangle(0, 0, battleTransitionScreen.Width, battleTransitionScreen.Height), Color.White);
            }

            if (GlobalVar.victoryTransition)
            {
                spriteBatch.Draw(battleWonScreen, new Rectangle(0, 0, battleWonScreen.Width, battleWonScreen.Height), Color.White);
            }
            #endregion

            #region BattleMode
            if (GlobalVar.battleMode)
            {
                if (hero.CharacterClass == GlobalVar.CharacterClass.Warrior || hero.CharacterClass == GlobalVar.CharacterClass.Thief)
                {
                    spriteBatch.Draw(battleMenuNoMagic, new Rectangle(0, 500, battleMenuNoMagic.Width, battleMenuNoMagic.Height), battleMenuNoMagic.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                }
                else
                {
                    spriteBatch.Draw(battleMenuFull, new Rectangle(0, 500, battleMenuFull.Width, battleMenuFull.Height), battleMenuFull.Bounds, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                }

                if(GlobalVar.lightAttackRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(battleMenuSelected, GlobalVar.lightAttackRec, new Rectangle(35, 25, 120, 50), Color.White);
                }
                else if (GlobalVar.heavyAttackRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(battleMenuSelected, GlobalVar.heavyAttackRec, new Rectangle(185, 25, 120, 50), Color.White);
                }
                else if (GlobalVar.defendRec.Contains(ms.X, ms.Y))
                {
                    spriteBatch.Draw(battleMenuSelected, GlobalVar.defendRec, new Rectangle(335, 25, 120, 50), Color.White);
                }
                else if (GlobalVar.magicRec.Contains(ms.X, ms.Y))
                {
                    if (hero.CharacterClass == GlobalVar.CharacterClass.Cleric || hero.CharacterClass == GlobalVar.CharacterClass.Wizard)
                    {
                        spriteBatch.Draw(battleMenuSelected, GlobalVar.magicRec, new Rectangle(485, 25, 120, 50), Color.White);
                    }
                }

                if (GlobalVar.drawBattleMsgString)
                {
                    if (GlobalVar.turn == 1)
                    {
                        spriteBatch.DrawString(GlobalVar.newsGothicBold8, GlobalVar.battleMsg, Camera.LocationToScreen(new Vector2(GlobalVar.enemy.Position.X + GlobalVar.enemy.Sprite.Width / 2 - 4, GlobalVar.enemy.Position.Y - 10)), Color.Red);
                    }
                    else
                    {
                        spriteBatch.DrawString(GlobalVar.newsGothicBold8, GlobalVar.battleMsg, Camera.LocationToScreen(new Vector2(hero.Position.X + hero.Sprite.Width/2 - 4, hero.Position.Y - 10)), Color.Red);
                    }
                }
                if (GlobalVar.drawMagicMsg)
                {
                    if (GlobalVar.turn == 1)
                    {
                        spriteBatch.DrawString(GlobalVar.newsGothicBold8, GlobalVar.magicMsg, Camera.LocationToScreen(new Vector2(hero.Position.X + 4, hero.Position.Y - 10)), Color.LightGreen);
                    }
                    else
                    {
                        spriteBatch.DrawString(GlobalVar.newsGothicBold8, GlobalVar.magicMsg, Camera.LocationToScreen(new Vector2(GlobalVar.enemy.Position.X + 4, GlobalVar.enemy.Position.Y - 10)), Color.LightGreen);
                    }
                }
            }

            #endregion

            #region Dialogue

            if (GlobalVar.displayDialogue)
            {
                GlobalVar.speech.Draw(spriteBatch);
            }

            #endregion
        }

        #endregion

        #region Private Methods

        private void StartNewGame()
        {
            hero.Revive();
            LevelManager.LoadLevel(0);
        }
        #endregion

        #region Saving & Loading

        public void Save(StorageDevice device)
        {
            // Open a storage container.
            IAsyncResult result = device.BeginOpenContainer("Poorly Drawn Dungeons", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "PDD.sav";

            // Check to see whether the save exists.
            if (container.FileExists(filename))
                // Delete it so that we can create one fresh.
                container.DeleteFile(filename);

            // Create the file.
            Stream stream = container.CreateFile(filename);

            //Get The Data to be saved
            SaveItem saveData = new SaveItem(hero, LevelManager.CurrentLevel);

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, saveData);

            //Close the file
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();
        }

        public bool Load(StorageDevice device)
        {
            // Open a storage container.
            IAsyncResult result =
                device.BeginOpenContainer("Poorly Drawn Dungeons", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "PDD.sav";

            // Check to see whether the save exists.
            if (!container.FileExists(filename))
            {
                // If not, dispose of the container and return.
                container.Dispose();
                return false;
            }

            // Open the file.
            Stream stream = container.OpenFile(filename, FileMode.Open);

            BinaryFormatter formatter = new BinaryFormatter();
            SaveItem data = (SaveItem)formatter.Deserialize(stream);

            // Close the file.
            stream.Close();

            // Dispose the container.
            container.Dispose();

            #region Cleric
            if (data.Player.CharacterClass == GlobalVar.CharacterClass.Cleric)
            {
                hero = new Player(playerSprites, Vector2.Zero, true, new Light(new LightAura(brightness1, Vector2.Zero, @"Textures\LightMasks\basicLightMask"), -1, true, "Vision", 0, Content.Load<Texture2D>(@"Textures\Items\Lights\vision"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Lights\vision"), GlobalVar.dagger, GlobalVar.leatherChestplate, GlobalVar.CharacterClass.Cleric);

                LevelManager.Hero = hero;
                GlobalVar.hero = hero;
            }
            #endregion
            #region Thief
            if (data.Player.CharacterClass == GlobalVar.CharacterClass.Thief)
            {
                hero = new Player(playerSprites, Vector2.Zero, true, new Light(new LightAura(brightness1, Vector2.Zero, @"Textures\LightMasks\basicLightMask"), -1, true, "Vision", 0, Content.Load<Texture2D>(@"Textures\Items\Lights\vision"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Lights\vision"), GlobalVar.dagger, GlobalVar.leatherChestplate, GlobalVar.CharacterClass.Thief);

                LevelManager.Hero = hero;
                GlobalVar.hero = hero;
            }
            #endregion
            #region Warrior
            if (data.Player.CharacterClass == GlobalVar.CharacterClass.Warrior)
            {
                hero = new Player(playerSprites, Vector2.Zero, true, new Light(new LightAura(brightness1, Vector2.Zero, @"Textures\LightMasks\basicLightMask"), -1, true, "Vision", 0, Content.Load<Texture2D>(@"Textures\Items\Lights\vision"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Lights\vision"), GlobalVar.dagger, GlobalVar.leatherChestplate, GlobalVar.CharacterClass.Warrior);

                LevelManager.Hero = hero;
                GlobalVar.hero = hero;
            }
            #endregion
            #region Wizard
            if (data.Player.CharacterClass == GlobalVar.CharacterClass.Wizard)
            {
                    hero = new Player(playerSprites, Vector2.Zero, true, new Light(new LightAura(brightness1, Vector2.Zero, @"Textures\LightMasks\basicLightMask"), -1, true, "Vision", 0, Content.Load<Texture2D>(@"Textures\Items\Lights\vision"), blankSprites, false, Vector2.Zero, 1, 1, @"Textures\Items\Lights\vision"), GlobalVar.dagger, GlobalVar.leatherChestplate, GlobalVar.CharacterClass.Wizard);

                    LevelManager.Hero = hero;
                    GlobalVar.hero = hero;
            }
            #endregion

            //Assign data
            hero.Inventory = data.Player.Inventory;
            hero.SpellBook = data.Player.SpellBook;
            hero.Spell = data.Player.Spell;
            hero.Weapon = data.Player.Weapon;
            hero.Armour = data.Player.Armour;
            hero.Attack = data.Player.Attack;
            hero.Defence = data.Player.Defence;
            hero.Strength = data.Player.Strength;
            hero.Stamina = data.Player.Stamina;
            hero.Dexterity = data.Player.Dexterity;
            hero.Intelligence = data.Player.Intelligence;
            hero.CurrentMana = data.Player.CurrentMana;
            hero.MaxMana = data.Player.MaxMana;
            hero.CurrentHealth = data.Player.CurrentHealth;
            hero.MaxHealth = data.Player.MaxHealth;
            hero.Direction = data.Player.Direction;
            hero.Level = data.Player.Level;
            hero.XP = data.Player.XP;
            hero.XPReq = data.Player.XPReq;
            hero.CharacterClass = data.Player.CharacterClass;
            hero.PreferredWeapon = data.Player.PreferredWeapon;
            hero.ProfBonus = data.Player.ProfBonus;
            hero.Gold = data.Player.Gold;
            hero.Dodge = data.Player.Dodge;
            hero.Endurance = data.Player.Endurance;
            hero.MagicSkill = data.Player.MagicSkill;
            hero.Perception = data.Player.Perception;
            hero.Stealth = data.Player.Stealth;
            hero.WeaponSkill = data.Player.WeaponSkill;
            hero.DodgeMod = data.Player.DodgeMod;
            hero.EnduranceMod = data.Player.EnduranceMod;
            hero.MagicSkillMod = data.Player.MagicSkillMod;
            hero.PerceptionMod = data.Player.PerceptionMod;
            hero.StealthMod = data.Player.StealthMod;
            hero.WeaponSkillMod = data.Player.WeaponSkillMod;

            for (int i = 0; i < hero.Inventory.Items.Count; i++ )
            {
                if (hero.Inventory.Items[i] is Light)
                {
                    Light light = (Light)hero.Inventory.Items[i];
                    if (light.Lit)
                    {
                        light.SwitchLit();
                    }
                }
            }

            LevelManager.CurrentLevel = data.LevelNum;

            LevelManager.LoadLevel(LevelManager.CurrentLevel);
            return true;
        }

        #endregion
    }
}
