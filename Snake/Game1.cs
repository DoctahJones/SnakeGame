using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snake.Particles;
using Snake.Screens;
using System;
using System.Collections.Generic;

namespace Snake
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        const float SNAKE_SPEED_EASY = 0.8f;
        const float SNAKE_SPEED_NORMAL = 0.5f;
        const float SNAKE_SPEED_HARD = 0.3f;


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ScreenManager screenManager;
        InputState inputState;


        private FontDisplayInfo fontInfo;

        private Texture2D menuBackground;

        private int mapTilesWide, mapTilesHigh;

        SnakeGameScreen snakeScreen;




        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            mapTilesWide = 16;
            mapTilesHigh = 9;

            screenManager = new ScreenManager(GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height);

            inputState = new InputState(Keyboard.GetState(), Keyboard.GetState(), Mouse.GetState(), Mouse.GetState());

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadFontInfo();

            menuBackground = Content.Load<Texture2D>("Graphics\\mainMenuBackground");

            ParticleManager particleManager = new ParticleManager();

            //create snake map
            TiledGameScreen snakeMap = BuildTiledBackgroundScreen();

            //create snake
            Snake snake = BuildSnake(snakeMap.TileSize);

            //create food
            SnakeFood snakeFood = BuildSnakeFood(snakeMap.TileSize, particleManager);

            //create snakeGameScreen
            snakeScreen = new SnakeGameScreen(screenManager, "snakeScreen");
            snakeScreen.Initialize(snakeMap, snakeFood, snake);
            screenManager.AddScreen(snakeScreen.ScreenName, snakeScreen);

            //create menu screen.
            MainMenuScreen menuScreen = BuildMainMenu();
            screenManager.AddScreen(menuScreen.ScreenName, menuScreen);
            screenManager.EnableScreen(menuScreen.ScreenName);

            //create pause screen
            MainMenuScreen pauseScreen = BuildPauseMenu();
            screenManager.AddScreen(pauseScreen.ScreenName, pauseScreen);

            //create difficulty screen
            MainMenuScreen difficultyScreen = BuildDifficultyMenu();
            screenManager.AddScreen(difficultyScreen.ScreenName, difficultyScreen);

            //create end game screen
            SnakeEndGameScreen endScreen = BuildEndGameScreen();
            screenManager.AddScreen(endScreen.ScreenName, endScreen);

        }

        private void LoadFontInfo()
        {
            SpriteFont font = Content.Load<SpriteFont>("Fonts\\menuFont");
            SpriteFont selectedFont = Content.Load<SpriteFont>("Fonts\\menuSelectedFont");
            Color color = new Color(246, 19, 26);
            Color selectedColor = new Color(0, 153, 26);

            fontInfo = new FontDisplayInfo(font, color);
            fontInfo.ColorSelected = selectedColor;
            fontInfo.FontSelected = selectedFont;
        }



        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            UpdateInputState();

            screenManager.Update(gameTime, inputState);

            base.Update(gameTime);
        }

        private void UpdateInputState()
        {
            inputState.OldKeyboardState = inputState.CurrentKeyboardState;
            inputState.CurrentKeyboardState = Keyboard.GetState();
            inputState.OldMouseState = inputState.CurrentMouseState;
            inputState.CurrentMouseState = Mouse.GetState();
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Beige);

            spriteBatch.Begin();
            screenManager.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }


        private MainMenuScreen BuildMainMenu()
        {
            MainMenuScreen m = new MainMenuScreen(screenManager, "mainMenu");
            MainMenuItem m1 = new MainMenuItem("Start Game");
            m1.Click += new MainMenuItem.ClickHandler(SetScreenNewSnakeGameScreen);
            MainMenuItem m2 = new MainMenuItem("Difficulty");
            m2.Click += new MainMenuItem.ClickHandler(SetScreenDifficultyScreen);
            MainMenuItem m3 = new MainMenuItem("Exit");
            m3.Click += new MainMenuItem.ClickHandler(ExitApplication);
            List<MainMenuItem> menuItems = new List<MainMenuItem>();
            menuItems.Add(m1);
            menuItems.Add(m2);
            menuItems.Add(m3);

            m.Initialize(menuBackground, menuItems, fontInfo.FontNormal, fontInfo.ColorNormal);
            m.SelectedFont = fontInfo.FontSelected;
            m.SelectedColor = fontInfo.ColorSelected;
            return m;
        }

        private MainMenuScreen BuildPauseMenu()
        {
            MainMenuScreen p = new MainMenuScreen(screenManager, "pauseMenu");

            MainMenuItem m1 = new MainMenuItem("Continue");
            m1.Click += new MainMenuItem.ClickHandler(SetScreenSnakeGameScreen);
            MainMenuItem m2 = new MainMenuItem("Main Menu");
            m2.Click += new MainMenuItem.ClickHandler(SetScreenMainMenu);
            MainMenuItem m3 = new MainMenuItem("Exit");
            m3.Click += new MainMenuItem.ClickHandler(ExitApplication);

            List<MainMenuItem> menuItems = new List<MainMenuItem>();
            menuItems.Add(m1);
            menuItems.Add(m2);
            menuItems.Add(m3);

            p.Initialize(menuBackground, menuItems, fontInfo.FontNormal, fontInfo.ColorNormal);
            p.SelectedFont = fontInfo.FontSelected;
            p.SelectedColor = fontInfo.ColorSelected;

            return p;
        }

        public void SetScreenNewSnakeGameScreen(MainMenuItem m, EventArgs e)
        {
            snakeScreen.ResetSnakeScreen();
            SetScreenSnakeGameScreen(m, e);
        }

        /// <summary>
        /// Sets the current screen to the snake game screen in whatever state it currently is.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="e"></param>
        public void SetScreenSnakeGameScreen(MainMenuItem m, EventArgs e)
        {
            screenManager.DisableOtherScreens("snakeScreen");
            screenManager.EnableScreen("snakeScreen");
        }

        public void SetScreenMainMenu(MainMenuItem m, EventArgs e)
        {
            screenManager.DisableOtherScreens("mainMenu");
            screenManager.EnableScreen("mainMenu");
        }

        public void SetScreenDifficultyScreen(MainMenuItem m, EventArgs e)
        {
            screenManager.DisableOtherScreens("difficultyMenu");
            screenManager.EnableScreen("difficultyMenu");
        }

        public void ExitApplication(MainMenuItem m, EventArgs e)
        {
            Exit();
        }

        private MainMenuScreen BuildDifficultyMenu()
        {
            MainMenuScreen menu = new MainMenuScreen(screenManager, "difficultyMenu");

            MainMenuItem m1 = new MainMenuItem("Easy");
            m1.Click += new MainMenuItem.ClickHandler(SetDifficultyEasy);
            MainMenuItem m2 = new MainMenuItem("Normal");
            m2.Click += new MainMenuItem.ClickHandler(SetDifficultyNormal);
            MainMenuItem m3 = new MainMenuItem("Hard");
            m3.Click += new MainMenuItem.ClickHandler(SetDifficultyHard);

            List<MainMenuItem> menuItems = new List<MainMenuItem>();
            menuItems.Add(m1);
            menuItems.Add(m2);
            menuItems.Add(m3);

            menu.Initialize(menuBackground, menuItems, fontInfo.FontNormal, fontInfo.ColorNormal);
            menu.SelectedFont = fontInfo.FontSelected;
            menu.SelectedColor = fontInfo.ColorSelected;

            return menu;
        }

        public void SetDifficultyEasy(MainMenuItem m, EventArgs e)
        {
            this.snakeScreen.SetSnakeSpeed(SNAKE_SPEED_EASY);
            SetScreenMainMenu(m, e);
        }

        public void SetDifficultyNormal(MainMenuItem m, EventArgs e)
        {
            this.snakeScreen.SetSnakeSpeed(SNAKE_SPEED_NORMAL);
            SetScreenMainMenu(m, e);
        }
        public void SetDifficultyHard(MainMenuItem m, EventArgs e)
        {
            this.snakeScreen.SetSnakeSpeed(SNAKE_SPEED_HARD);
            SetScreenMainMenu(m, e);
        }

        private TiledGameScreen BuildTiledBackgroundScreen()
        {
            TiledGameScreen tScreen = new TiledGameScreen(screenManager, "tiledScreen");
            TiledGameScreenCell[,] t = new TiledGameScreenCell[mapTilesWide, mapTilesHigh];
            Texture2D backgroundTile = Content.Load<Texture2D>("Graphics\\floor");
            Random r = new Random();

            for (int i = 0; i < t.GetLength(0); i++)
            {
                for (int j = 0; j < t.GetLength(1); j++)
                {
                    t[i, j] = new TiledGameScreenCell(backgroundTile, true, Color.White, 0f);
                }
            }
            tScreen.Initialize(t);
            return tScreen;
        }

        private Snake BuildSnake(Point tileSize)
        {
            Snake snake = new Snake(tileSize);
            Animation snakeHead = new Animation(GraphicsDevice);
            Texture2D headTexture = Content.Load<Texture2D>("Graphics\\Snake\\snakeHead");
            snakeHead.Initialize(headTexture, Vector2.Zero, 256, 256, 3, 500, Color.White, 1, true);
            snakeHead.Scale = (float)tileSize.X / 256f;


            Texture2D bodyTexture = Content.Load<Texture2D>("Graphics\\Snake\\snakeBodies");

            snake.Initialize(snakeHead, bodyTexture, 256, 5, 0.5f);

            return snake;
        }

        private SnakeFood BuildSnakeFood(Point tileSize, ParticleManager particle)
        {
            SnakeFood snakeFood = new SnakeFood(particle, tileSize, mapTilesWide, mapTilesHigh);
            Texture2D[] foods = new Texture2D[6];
            foods[0] = Content.Load<Texture2D>("Graphics\\Fruits\\Apple");
            foods[1] = Content.Load<Texture2D>("Graphics\\Fruits\\Banana");
            foods[2] = Content.Load<Texture2D>("Graphics\\Fruits\\Cherries");
            foods[3] = Content.Load<Texture2D>("Graphics\\Fruits\\Lemon");
            foods[4] = Content.Load<Texture2D>("Graphics\\Fruits\\Orange");
            foods[5] = Content.Load<Texture2D>("Graphics\\Fruits\\Watermelon");
            snakeFood.Initialize(GraphicsDevice, foods, 128, 128);

            return snakeFood;
        }

        private SnakeEndGameScreen BuildEndGameScreen()
        {
            SnakeEndGameScreen endGameScreen = new SnakeEndGameScreen(screenManager, "endGameScreen");
            endGameScreen.Initialize(snakeScreen, fontInfo.FontNormal, Color.Black);
            return endGameScreen;
        }

    }
}
