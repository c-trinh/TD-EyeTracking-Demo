using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

/************************* NOTE:
 App.xaml is the entry point of every windows store application
 Game1.cs contains the logic of the game (updating data, refreshing display)
 GamePage.xaml is the default page initialized with the Game1 class

    Lifecycle of a game application:
    Initialize() ----> LoadContent() ----> Update() ----> UnloadContent()
                                             | ^
                                          Game Loop
                                             V |
                                            Draw()

    Data Binding will be the primary way that xaml elements interact with the underlying entities.
    More info at: docs.microsoft.com/en-us/dotnet/framework/wpf/data/data-binding-overview

    Need to bind the xaml elements to the appropriate entities
    EX:
        Binding Target                                     Binding Source
    xaml Fire UC current Position        ------------>   Position component
 **************************/

namespace GodsOfCalamityBeta
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static float screenWidth;    // must be accessible by systems when creating new entities
        public static float screenHeight;
        public static int VillageID;
        public enum EntityType { Village, Meteor, Lightning, Fire };
        Texture2D village;
        Texture2D map;
        Texture2D lightning_1;
        Texture2D lightning_2;
        Texture2D lightning_3;
        Texture2D meteorNW_1;
        Texture2D meteorNW_2;
        Texture2D meteorNW_3;
        Texture2D meteorNE_1;
        Texture2D meteorNE_2;
        Texture2D meteorNE_3;
        Texture2D meteorSW_1;
        Texture2D meteorSW_2;
        Texture2D meteorSW_3;
        Texture2D meteorSE_1;
        Texture2D meteorSE_2;
        Texture2D meteorSE_3;
        Texture2D fire_1;
        Texture2D fire_2;
        Texture2D fire_3;
        Texture2D ashes;
        Texture2D explosion;
        Texture2D[] dwellTextures = new Texture2D[6];
        public static World _world;         // Must be accessible by systems to create new entities
        public static Dictionary<string, SpriteClass> SpriteDict = new Dictionary<string, SpriteClass>();   // will be passed to spawn system to attach sprite components
        public static bool isPaused;
        public static int[,] fireGrid; // 2d array represents the grid fire can be spawned on. The array will store entityIDs
        public static int fireGridMaxX;
        public static int fireGridMaxY;
        public static int cloudNum = 0;
        public static int fireNum = 0;

        public static GamePage myGamePage;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// Allows the game to perform any initialization it needs to before starting to run.
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Game does not start in paused state
            isPaused = false;

            /// Launch game in fullscreen
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            
            /// Get screen Width and Height
            screenHeight = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Height);
            var screenHeight2 = GraphicsDevice.Viewport.Bounds.Height;
            screenWidth = ScaleToHighDPI((float)ApplicationView.GetForCurrentView().VisibleBounds.Width);
            var screenWidth2 = GraphicsDevice.Viewport.Bounds.Width;

            base.Initialize();
        }

        /// LoadContent called once and is the place to load all of your content.
        protected override void LoadContent()
        {
            /// Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadTextures();

            // ***************Default Aspect Builder --Subject to change!--***************
            AspectBuilder defaultAspectBuilder = new AspectBuilder();
            fireGridMaxX = (int)(screenWidth / fire_1.Width);
            fireGridMaxY = (int)(screenHeight / fire_1.Height);
            fireGrid = new int[fireGridMaxX, fireGridMaxY]; // initialize fireGrid to fit the users screen dimensions
            fireGridMaxX -= 1;
            fireGridMaxY -= 1;

            // Load world ***********************************************************************************
            _world = new WorldBuilder()
                .AddSystem(new Systems.MovementSystem(screenWidth, screenHeight))
                .AddSystem(new Systems.RenderSystem(defaultAspectBuilder))
                .AddSystem(new Systems.SpawnSystem())
                .AddSystem(new Systems.EntityDestructSystem())
                .AddSystem(new Systems.FireSpawnSystem(defaultAspectBuilder))
                .AddSystem(new Systems.UpdateFrontEndSystem(defaultAspectBuilder))
                .AddSystem(new Systems.DamageSystem(5))
                .AddSystem(new Systems.AnimationSystem(defaultAspectBuilder))
                .AddSystem(new Systems.FireBurnoutSystem(defaultAspectBuilder))
                /// Add systems here via ".AddSystem(new [SystemName](Parameters))"
                
                .Build();

            //load and build village
            Entity VillageEntity = _world.CreateEntity();
            ECSComponents.PositionComponent villageposition = new ECSComponents.PositionComponent((float) 1/2 * screenWidth, (float) 1/2 * screenHeight, 0);
            ECSComponents.StatusComponent villagestatus = new ECSComponents.StatusComponent(false, EntityType.Village);
            ECSComponents.SpriteComponent villagesprite = new ECSComponents.SpriteComponent(SpriteDict["village"], 1);
            ECSComponents.CollisionBoxComponet villageHitBox = new ECSComponents.CollisionBoxComponet(villagesprite.Sprite, villageposition);
            ECSComponents.HealthComponent villageHealth = new ECSComponents.HealthComponent(100);

            VillageEntity.Attach(villageposition);
            VillageEntity.Attach(villagestatus);
            VillageEntity.Attach(villagesprite);
            VillageEntity.Attach(villageHitBox);
            VillageEntity.Attach(villageHealth);

            VillageID = VillageEntity.Id;
            // ***********************************************************************************************
        }

        /// UnloadContent will be called once per game and is the place to unload game-specific content.
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing audio.
        protected override void Update(GameTime gameTime)
        {
            if (!isPaused)
            {
                _world.Update(gameTime);
                base.Update(gameTime);
            }
        }

        /// This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            spriteBatch.Draw(map, new Rectangle(0, 0, (int)screenWidth, (int)screenHeight), Color.White);   // draws map to the screen

            spriteBatch.End();
            _world.Draw(gameTime);
            base.Draw(gameTime);
        }

        /// Used to scale to high DPI displays
        public static float ScaleToHighDPI(float f)
        {
            Windows.Graphics.Display.DisplayInformation d = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
            f *= (float)d.RawPixelsPerViewPixel;
            return f;
        }

        public void RestartGame()
        {
            //Initialize();
            LoadContent();
        }

        private void LoadTextures()
        {
            map = Content.Load<Texture2D>("map");
            village = Content.Load<Texture2D>("Village");
            lightning_1 = Content.Load<Texture2D>("cloud_idle_1");
            lightning_2 = Content.Load<Texture2D>("cloud_idle_2");
            lightning_3 = Content.Load<Texture2D>("cloud_idle_3");
            meteorNW_1 = Content.Load<Texture2D>("MeteorNW_1");
            meteorNW_2 = Content.Load<Texture2D>("MeteorNW_2");
            meteorNW_3 = Content.Load<Texture2D>("MeteorNW_3");
            meteorNE_1 = Content.Load<Texture2D>("MeteorNE_1");
            meteorNE_2 = Content.Load<Texture2D>("MeteorNE_2");
            meteorNE_3 = Content.Load<Texture2D>("MeteorNE_3");
            meteorSW_1 = Content.Load<Texture2D>("MeteorSW_1");
            meteorSW_2 = Content.Load<Texture2D>("MeteorSW_2");
            meteorSW_3 = Content.Load<Texture2D>("MeteorSW_3");
            meteorSE_1 = Content.Load<Texture2D>("MeteorSE_1");
            meteorSE_2 = Content.Load<Texture2D>("MeteorSE_2");
            meteorSE_3 = Content.Load<Texture2D>("MeteorSE_3");
            fire_1 = Content.Load<Texture2D>("fire_idle_1");
            fire_2 = Content.Load<Texture2D>("fire_idle_2");
            fire_3 = Content.Load<Texture2D>("fire_idle_3");
            ashes = Content.Load<Texture2D>("ashes");
            explosion = Content.Load<Texture2D>("explosion");
            dwellTextures[0] = Content.Load<Texture2D>("cloud_dwell_1");
            dwellTextures[1] = Content.Load<Texture2D>("cloud_dwell_2");
            dwellTextures[2] = Content.Load<Texture2D>("cloud_dwell_3");
            dwellTextures[3] = Content.Load<Texture2D>("fire_dwell_1");
            dwellTextures[4] = Content.Load<Texture2D>("fire_dwell_2");
            dwellTextures[5] = Content.Load<Texture2D>("fire_dwell_3");

            /// Creates the SpriteTextureComponents so they can be attached to entities
            SpriteClass lightningSprite_1 = new SpriteClass(GraphicsDevice, lightning_1, 1); // scale of sprites may be adjusted here
            SpriteClass lightningSprite_2 = new SpriteClass(GraphicsDevice, lightning_2, 1); // scale of sprites may be adjusted here
            SpriteClass lightningSprite_3 = new SpriteClass(GraphicsDevice, lightning_3, 1); // scale of sprites may be adjusted here
            SpriteClass meteorSpriteNE_1 = new SpriteClass(GraphicsDevice, meteorNE_1, 1); // scale of sprites may be adjusted here
            SpriteClass meteorSpriteNE_2 = new SpriteClass(GraphicsDevice, meteorNE_2, 1); // scale of sprites may be adjusted here
            SpriteClass meteorSpriteNE_3 = new SpriteClass(GraphicsDevice, meteorNE_3, 1); // scale of sprites may be adjusted here
            SpriteClass meteorSpriteNW_1 = new SpriteClass(GraphicsDevice, meteorNW_1, 1); // scale of sprites may be adjusted here
            SpriteClass meteorSpriteNW_2 = new SpriteClass(GraphicsDevice, meteorNW_2, 1); // scale of sprites may be adjusted here
            SpriteClass meteorSpriteNW_3 = new SpriteClass(GraphicsDevice, meteorNW_3, 1); // scale of sprites may be adjusted here
            SpriteClass meteorSpriteSW_1 = new SpriteClass(GraphicsDevice, meteorSW_1, 1); // scale of sprites may be adjusted here
            SpriteClass meteorSpriteSW_2 = new SpriteClass(GraphicsDevice, meteorSW_2, 1); // scale of sprites may be adjusted here
            SpriteClass meteorSpriteSW_3 = new SpriteClass(GraphicsDevice, meteorSW_3, 1); // scale of sprites may be adjusted here
            SpriteClass meteorSpriteSE_1 = new SpriteClass(GraphicsDevice, meteorSE_1, 1); // scale of sprites may be adjusted here
            SpriteClass meteorSpriteSE_2 = new SpriteClass(GraphicsDevice, meteorSE_2, 1); // scale of sprites may be adjusted here
            SpriteClass meteorSpriteSE_3 = new SpriteClass(GraphicsDevice, meteorSE_3, 1); // scale of sprites may be adjusted here
            SpriteClass fireSprite_1 = new SpriteClass(GraphicsDevice, fire_1, 1); // scale of sprites may be adjusted here
            SpriteClass fireSprite_2 = new SpriteClass(GraphicsDevice, fire_2, 1); // scale of sprites may be adjusted here
            SpriteClass fireSprite_3 = new SpriteClass(GraphicsDevice, fire_3, 1); // scale of sprites may be adjusted here
            SpriteClass villageSprite = new SpriteClass(GraphicsDevice, village, 1); // scale of sprites may be adjusted here
            SpriteClass lightningDwell_1 = new SpriteClass(GraphicsDevice, dwellTextures[0], 1);
            SpriteClass lightningDwell_2 = new SpriteClass(GraphicsDevice, dwellTextures[1], 1);
            SpriteClass lightningDwell_3 = new SpriteClass(GraphicsDevice, dwellTextures[2], 1);
            SpriteClass fireDwell_1 = new SpriteClass(GraphicsDevice, dwellTextures[3], 1);
            SpriteClass fireDwell_2 = new SpriteClass(GraphicsDevice, dwellTextures[4], 1);
            SpriteClass fireDwell_3 = new SpriteClass(GraphicsDevice, dwellTextures[5], 1);
            SpriteClass explosion_sprite = new SpriteClass(GraphicsDevice, explosion, 1);
            SpriteClass ashes_sprite = new SpriteClass(GraphicsDevice, ashes, 1);

            SpriteDict.Clear();

            SpriteDict.Add("lightning_1", lightningSprite_1);
            SpriteDict.Add("lightning_2", lightningSprite_2);
            SpriteDict.Add("lightning_3", lightningSprite_3);
            SpriteDict.Add("meteorNW_1", meteorSpriteNW_1);
            SpriteDict.Add("meteorNW_2", meteorSpriteNW_2);
            SpriteDict.Add("meteorNW_3", meteorSpriteNW_3);
            SpriteDict.Add("meteorNE_1", meteorSpriteNE_1);
            SpriteDict.Add("meteorNE_2", meteorSpriteNE_2);
            SpriteDict.Add("meteorNE_3", meteorSpriteNE_3);
            SpriteDict.Add("meteorSW_1", meteorSpriteSW_1);
            SpriteDict.Add("meteorSW_2", meteorSpriteSW_2);
            SpriteDict.Add("meteorSW_3", meteorSpriteSW_3);
            SpriteDict.Add("meteorSE_1", meteorSpriteSE_1);
            SpriteDict.Add("meteorSE_2", meteorSpriteSE_2);
            SpriteDict.Add("meteorSE_3", meteorSpriteSE_3);
            SpriteDict.Add("fire_1", fireSprite_1);
            SpriteDict.Add("fire_2", fireSprite_2);
            SpriteDict.Add("fire_3", fireSprite_3);
            SpriteDict.Add("village", villageSprite);
            SpriteDict.Add("lightning_dwell_1", lightningDwell_1);
            SpriteDict.Add("lightning_dwell_2", lightningDwell_2);
            SpriteDict.Add("lightning_dwell_3", lightningDwell_3);
            SpriteDict.Add("fire_dwell_1", fireDwell_1);
            SpriteDict.Add("fire_dwell_2", fireDwell_2);
            SpriteDict.Add("fire_dwell_3", fireDwell_3);
            SpriteDict.Add("ashes", ashes_sprite);
            SpriteDict.Add("explosion", explosion_sprite);
        }
    }
}
