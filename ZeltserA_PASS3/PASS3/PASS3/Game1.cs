// Author: Alice Zeltser
// File Name: Game1.cs
// Project Name: PASS3
// Creation Date: Dec. 01, 2023
// Modified Date: Jan. 21, 2024
// Description: The purpose of this program to to collect coins to progress to the next levels, levels diffcuility increase
//              as the left increases

//Course Concept: In the DrawHearts method it uses recursion to draw the hearts (not part of my main course concepts
//                just an extra)
//                In the ReadArraysFromFile(string fileName) I used file IO to  read the maps of the txt file
//                In the AddNewScore() I wrote on the file to put the new best score

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

using GameUtility;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections;
using System.Linq;

namespace PASS3
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        //store spritebact and graphics
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //store infile and outfile
        private static StreamReader inFile;
        private static StreamWriter outFile;

        //store file names
        const string FILE1 = "Map.txt";
        const string FILE2 = "Score.txt";

        //store maps and tileMaps
        private List<string[,]> maps = new List<string[,]>();
        private List<TileMap> tileMaps = new List<TileMap>();

        //store game states
        const int MENU_STATE = 0;
        const int INSTRUCTIONS = 1;
        const int GAME = 2;
        const int END_GAME_STATE = 3;
        const int NEXT_LEVEL_TRANSITION = 4;
        int gameState = MENU_STATE;

        //Store screen dimensions 
        int screenHeight;
        int screenWidth;

        //tile images
        Texture2D[] tileImgs = new Texture2D[8];
        Rectangle[,] tileLocs = new Rectangle[31,28];

        //ghosts images
        Texture2D[] ghostsImgs = new Texture2D[5];

        //PacMan image
        Player player;
        Texture2D [] packmanImg = new Texture2D[5];

        //store heart images and heart rectangle
        Texture2D[] hearts = new Texture2D[2];
        Rectangle[] heartsRec = new Rectangle[3];

        //Store keyboard states
        KeyboardState kb;
        KeyboardState prevKb;

        //Stores mouse state
        MouseState mouse;
        MouseState prevMouse;

        //Store level one
        Level level1;

        //store current level and current tile map 
        Level currentLevel;
        TileMap currentTileMap;

        //store next level
        Level nextLevel;

        //store map images and locations
        Texture2D[] menuButtonImgs = new Texture2D[4];
        Rectangle[] menuButtonLocs = new Rectangle[4];

        //store background image and location
        Texture2D backgroundImg;
        Rectangle backgroundLoc;

        //store game buttons and location
        Texture2D[] gameButtonImgs = new Texture2D[4];
        Rectangle[] gameButtonLocs = new Rectangle[4];

        //store keyboard image and location
        Texture2D keyboardImg;
        Rectangle keyboardLoc;

        //store title and location of title
        string title = "PAC PLUS";
        Vector2 titleLoc;

        //store fonts
        SpriteFont menuFont;
        SpriteFont instructionsFont;
        SpriteFont instructionsFont2;
        SpriteFont scoreFont;
        SpriteFont finalScoreFont;

        //store instructions texts
        string[] instructionsText = new string[3];

        //store initial index of number of white hearts
        int initialIndex = 0;

        //store rectangle
        GameRectangle rectangle;

        //store constants of wall and floor characters
        public const string FLOOR_CHAR = "-";
        public const string WALL_CHAR = "w";
        public const string CHERRY_CHAR = "c";

        //store end game messsage and location
        string endGameMessage = "Thank you for Playing Pac Plus";
        Vector2 endGameMessageLoc;

        //store highscores
        int[] highScores = new int[7];

        //store is transitions 
        string transitionMessage = "LEVEL COMPLETED";
        Vector2 transitionMessageLoc;
        Vector2 transitionMessageSize;

        //store ghosts for each level
        List<Ghost> ghostsLevel1 = new List<Ghost>();
        List<Ghost> ghostsLevel2 = new List<Ghost>();
        List<Ghost> ghostsLevel3 = new List<Ghost>();
        List<Ghost> ghostsLevel4 = new List<Ghost>();
        List<Ghost> ghostsLevel5 = new List<Ghost>();
        List<Ghost> ghostsLevel6 = new List<Ghost>();
        List<Ghost> ghostsLevel7 = new List<Ghost>();

        //store sound effects
        public static SoundEffect playerDiedSound;
        public static SoundEffect playerEatingCoin;
        public static SoundEffect playerEatingCherry;
        public static SoundEffect buttonClickSound;
        public static SoundEffect playerWonSound;

        //store music
        Song backgroundMusic;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 896;
            graphics.PreferredBackBufferHeight = 992;

            //Make mouse visble
            IsMouseVisible = true;

            //Apply changes 
            graphics.ApplyChanges();

            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
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

            //Load images
            tileImgs[TileMap.FLOOR] = Content.Load<Texture2D>("Images/Tiles/Floor");
            tileImgs[TileMap.WALL] = Content.Load<Texture2D>("Images/Tiles/Wall");
            tileImgs[TileMap.TOP_LEFT] = Content.Load<Texture2D>("Images/Tiles/TopLeft");
            tileImgs[TileMap.TOP_RIGHT] = Content.Load<Texture2D>("Images/Tiles/TopRight");
            tileImgs[TileMap.BOTTOM_LEFT] = Content.Load<Texture2D>("Images/Tiles/BottomLeft");
            tileImgs[TileMap.BOTTOM_RIGHT] = Content.Load<Texture2D>("Images/Tiles/BottomRight");
            tileImgs[TileMap.COIN] = Content.Load<Texture2D>("Images/Tiles/CoinImg");
            tileImgs[TileMap.CHERRY] = Content.Load<Texture2D>("Images/Items/Cherry");
     
            //Load ghosts
            ghostsImgs[0] = Content.Load<Texture2D>("Images/Characters/blueGhost");
            ghostsImgs[1] = Content.Load<Texture2D>("Images/Characters/greenGhost");
            ghostsImgs[2] = Content.Load<Texture2D>("Images/Characters/redGhost");
            ghostsImgs[3] = Content.Load<Texture2D>("Images/Characters/yellowGhost");
            ghostsImgs[4] = Content.Load<Texture2D>("Images/Characters/orangeGhost");

            //Load pacman img
            packmanImg[Player.UP] = Content.Load<Texture2D>("Images/Characters/PacManUp");
            packmanImg[Player.RIGHT] = Content.Load<Texture2D>("Images/Characters/PacManRight");
            packmanImg[Player.LEFT] = Content.Load<Texture2D>("Images/Characters/PacManLeft");
            packmanImg[Player.DOWN] = Content.Load<Texture2D>("Images/Characters/PacManDown");

            //load hearts images
            hearts[0] = Content.Load<Texture2D>("Images/Items/FullHeart");
            hearts[1] = Content.Load<Texture2D>("Images/Items/EmptyHeart");

            //load fonts
            scoreFont = Content.Load<SpriteFont>("Fonts/ScoreFont");
            menuFont = Content.Load<SpriteFont>("Fonts/MenuFont");
            instructionsFont = Content.Load<SpriteFont>("Fonts/InstructionsFont");
            instructionsFont2 = Content.Load<SpriteFont>("Fonts/InstructionsFont2");
            finalScoreFont = Content.Load<SpriteFont>("Fonts/FinalScoreFont");

            //load background and set position
            backgroundImg = Content.Load<Texture2D>("Images/Buttons/BackgroundMenu");
            backgroundLoc = new Rectangle(0,0, screenWidth, screenHeight);

            //load sound effects
            playerDiedSound = Content.Load<SoundEffect>("Audio/Sounds/pacman_death");
            playerEatingCoin = Content.Load<SoundEffect>("Audio/Sounds/pacman_chomp");
            playerEatingCherry = Content.Load<SoundEffect>("Audio/Sounds/pacman_eatfruit");
            buttonClickSound = Content.Load<SoundEffect>("Audio/Sounds/Click");
            playerWonSound = Content.Load<SoundEffect>("Audio/Sounds/Won");

            //load music
            backgroundMusic = Content.Load<Song>("Audio/Music/Music");

            //set volume
            SoundEffect.MasterVolume = 0.7f;

            //load menu butttons images and set rectangles of buttons
            menuButtonImgs[0] = Content.Load<Texture2D>("Images/Buttons/MenuButton");
            menuButtonImgs[1] = Content.Load<Texture2D>("Images/Buttons/InstructionsButton");
            menuButtonImgs[2] = Content.Load<Texture2D>("Images/Buttons/ExitButton");
            menuButtonImgs[3] = Content.Load<Texture2D>("Images/Buttons/StartButton");
            menuButtonLocs[0] = new Rectangle((screenWidth / 2) - (menuButtonImgs[0].Width / 2), 700, menuButtonImgs[0].Width, menuButtonImgs[0].Height);
            menuButtonLocs[1] = new Rectangle((screenWidth / 2) - (menuButtonImgs[1].Width / 2), 450, menuButtonImgs[1].Width, menuButtonImgs[1].Height);
            menuButtonLocs[2] = new Rectangle((screenWidth / 2) - (menuButtonImgs[2].Width / 2), 600, menuButtonImgs[2].Width, menuButtonImgs[2].Height);
            menuButtonLocs[3] = new Rectangle((screenWidth / 2) - (menuButtonImgs[3].Width / 2), 300, menuButtonImgs[3].Width, menuButtonImgs[3].Height);

            //load game button imgs and set positions
            gameButtonImgs[0] = Content.Load<Texture2D>("Images/Buttons/GameMenuButton");
            gameButtonImgs[1] = Content.Load<Texture2D>("Images/Buttons/SoundOn");
            gameButtonImgs[2] = Content.Load<Texture2D>("Images/Buttons/SoundOff");
            gameButtonImgs[3] = Content.Load<Texture2D>("Images/Buttons/NextLevelButton");
            gameButtonLocs[0] = new Rectangle(screenWidth - 2 * (TileMap.tileWidth), 0, TileMap.tileWidth, TileMap.tileHeight);
            gameButtonLocs[1] = new Rectangle(screenWidth - TileMap.tileWidth, 0, TileMap.tileWidth, TileMap.tileHeight);
            gameButtonLocs[2] = new Rectangle(screenWidth - TileMap.tileWidth, 0, TileMap.tileWidth, TileMap.tileHeight);
            gameButtonLocs[3] = new Rectangle((screenWidth /2) - (menuButtonImgs[3].Width / 2), 700, menuButtonImgs[3].Width, menuButtonImgs[3].Height);

            //load keyboard imgs
            keyboardImg = Content.Load<Texture2D>("Images/Buttons/keys");
            keyboardLoc = new Rectangle((screenWidth / 2) - (keyboardImg.Width / 2), ((screenHeight / 2) - (keyboardImg.Height / 2)) + 100, keyboardImg.Width,
                          keyboardImg.Height);

            //set position of title
            titleLoc = new Vector2(100, 100);

            //set instructions text
            instructionsText[0] = "Goal:";
            instructionsText[1] = "The goal of the same is to collect all coins to\nadvance to the next level" +
                                    " Watch out there might\nbe ghosts preventing you from easily completing the\nlevel " +
                                    "In certain level or levels the level you go to\nwill depend on how you complete the level";
            instructionsText[2] = "Movement Keys:";

            //set rectangle
            rectangle = new GameRectangle(GraphicsDevice, (screenWidth/2)  - (TileMap.tileWidth * 4 /2), 0, TileMap.tileWidth* 4, TileMap.tileHeight);

            //set end game message size 
            Vector2 endGameMessageSize = instructionsFont.MeasureString(endGameMessage);

            // Calculate the position to center the text
            endGameMessageLoc = new Vector2((screenWidth - endGameMessageSize.X) / 2, 250);

            //Get transition message size and centre text
            transitionMessageSize = instructionsFont.MeasureString(transitionMessage);
            transitionMessageLoc = new Vector2((screenWidth - transitionMessageSize.X) / 2, 300);

            //read files
            ReadFile(FILE1);
            ReadFile(FILE2);
            
            //set tile maps
            tileMaps.Add(new TileMap(maps[0], tileImgs, tileLocs));
            tileMaps.Add(new TileMap(maps[1], tileImgs, tileLocs));
            tileMaps.Add(new TileMap(maps[2], tileImgs, tileLocs));
            tileMaps.Add(new TileMap(maps[3], tileImgs, tileLocs));
            tileMaps.Add(new TileMap(maps[4], tileImgs, tileLocs));
            tileMaps.Add(new TileMap(maps[5], tileImgs, tileLocs));
            tileMaps.Add(new TileMap(maps[6], tileImgs, tileLocs));

            //set player
            player = new Player(1, 1, 0, 0, packmanImg);

            //set level one ghosts 
            ghostsLevel1.Add(new GreenGhost(26, 1, 1, 0, ghostsImgs[1], tileMaps[0]));
            ghostsLevel1.Add(new GreenGhost(1, 5, 1, 0, ghostsImgs[1], tileMaps[0]));
            ghostsLevel1.Add(new GreenGhost(26, 9, 1, 0, ghostsImgs[1], tileMaps[0]));
            ghostsLevel1.Add(new GreenGhost(1, 13, 1, 0, ghostsImgs[1], tileMaps[0]));
            ghostsLevel1.Add(new GreenGhost(26, 17, 1, 0, ghostsImgs[1], tileMaps[0]));
            ghostsLevel1.Add(new GreenGhost(1, 21, 1, 0, ghostsImgs[1], tileMaps[0]));
            ghostsLevel1.Add(new GreenGhost(26, 25, 1, 0, ghostsImgs[1], tileMaps[0]));
            ghostsLevel1.Add(new GreenGhost(1, 29, 1, 0, ghostsImgs[1], tileMaps[0]));

            //set level two ghosts 
            ghostsLevel2.Add(new BlueGhost(26, 29, 0, -1, ghostsImgs[0], tileMaps[1]));
            ghostsLevel2.Add(new BlueGhost(21, 1, 0, 1, ghostsImgs[0], tileMaps[1]));
            ghostsLevel2.Add(new BlueGhost(14, 1, 0, 1, ghostsImgs[0], tileMaps[1]));
            ghostsLevel2.Add(new BlueGhost(6, 1, 0, 1, ghostsImgs[0], tileMaps[1]));
            ghostsLevel2.Add(new BlueGhost(1, 29, 0, -1, ghostsImgs[0], tileMaps[1]));

            //set ghosts for level 3
            ghostsLevel3.Add(new YellowGhost(26, 1, -1, 0, ghostsImgs[3], tileMaps[2]));
            ghostsLevel3.Add(new YellowGhost(26, 8, -1, 0, ghostsImgs[3], tileMaps[2]));
            ghostsLevel3.Add(new YellowGhost(1, 8, 1, 0, ghostsImgs[3], tileMaps[2]));
            ghostsLevel3.Add(new YellowGhost(1, 29, 0, -1, ghostsImgs[3], tileMaps[2]));
            ghostsLevel3.Add(new YellowGhost(26, 29, 0, -1, ghostsImgs[3], tileMaps[2]));

            //set ghots for level 4
            ghostsLevel4.Add(new RedGhost(16, 1, 0, 0, ghostsImgs[2], tileMaps[3]));
            ghostsLevel4.Add(new OrangeGhost(1, 29, 0, 0, ghostsImgs[4], tileMaps[3]));

            //set ghosts for level 5
            ghostsLevel5.Add(new RedGhost(16, 1, 0, 0, ghostsImgs[2], tileMaps[4]));
            ghostsLevel5.Add(new RedGhost(1, 29, 0, 0, ghostsImgs[2], tileMaps[4]));

            //set ghosts for level 6
            ghostsLevel6.Add(new YellowGhost(15, 1, 0, 1, ghostsImgs[3], tileMaps[5]));
            ghostsLevel6.Add(new RedGhost(6, 18, 0, 1, ghostsImgs[2], tileMaps[5]));
            ghostsLevel6.Add(new OrangeGhost(1, 29, 0, 1, ghostsImgs[4], tileMaps[5]));

            //set ghosts for level 7
            ghostsLevel7.Add(new RedGhost(16, 1, 0, 0, ghostsImgs[2], tileMaps[6]));
            ghostsLevel7.Add(new RedGhost(1, 29, 0, 0, ghostsImgs[2], tileMaps[6]));
            ghostsLevel7.Add(new RedGhost(6, 18, 0, 0, ghostsImgs[2], tileMaps[6]));
            ghostsLevel7.Add(new RedGhost(26, 29, 0, 0, ghostsImgs[2], tileMaps[6]));

            //set heart locations
            for (int i = 0; i < heartsRec.Length; i++)
            {
                //set hearts rectangle
                heartsRec[i] = new Rectangle(i * 40, 0, 30, 27);
            }

            //set levels
            level1 = new Level(tileMaps[0], ghostsLevel1, 1);
            Level level2 = new Level(tileMaps[1], ghostsLevel2, 2);
            Level level3 = new Level(tileMaps[2], ghostsLevel3, 3);
            Level level4 = new Level(tileMaps[3], ghostsLevel4, 4);
            Level level5 = new Level(tileMaps[4], ghostsLevel5, 5);
            Level level6 = new Level(tileMaps[5], ghostsLevel6, 6);
            Level level7 = new Level(tileMaps[6], ghostsLevel7, 7);

            //set tree of levels
            level1.TryAddChild(level2);
            level2.TryAddChild(level3);
            level2.TryAddChild(level4);
            level3.TryAddChild(level5);
            level5.TryAddChild(level6);
            level4.TryAddChild(level6);
            level6.TryAddChild(level7);

            //set current level node and currenet map
            currentLevel = level1;
            currentTileMap = level1.map;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Store pervious mouse state and current mouse state
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //checks left button is clicked
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //checks if the mouse is on the sound rectangle
                if (gameButtonLocs[1].Contains(mouse.Position))
                {
                    //Checks if music is off
                    if (MediaPlayer.State != MediaState.Playing)
                    {
                        //Turns on music
                        MediaPlayer.Play(backgroundMusic);
                    }
                    else
                    {
                        //Turns off music
                        MediaPlayer.Stop();
                    }
                }
            }

            //go to a diffrent state based on the value of game state
            switch (gameState)
            {
                case MENU_STATE:
                    //go to update menu
                    UpdateMenu();
                    break;
                case INSTRUCTIONS:
                    //go to instructions
                    Instructions();
                    break;
                case GAME:
                    //go to game
                    Game(gameTime);
                    break;
                case END_GAME_STATE:
                    //go to end game
                    UpdateEndGame();
                    break;
                case NEXT_LEVEL_TRANSITION:
                    //go to next level transiion
                    NextLevelTransition();
                    break;
            }

            base.Update(gameTime);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Updates menu screen, handles user interactions
        private void UpdateMenu()
        {
            // Checks if user clicked the left button
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //checks if the mouse is on the play button
                if (menuButtonLocs[1].Contains(mouse.Position))
                {
                    //play click sound effect
                    buttonClickSound.CreateInstance().Play();

                    //goes to instructions state
                    gameState = INSTRUCTIONS;
                }
                else if (menuButtonLocs[2].Contains(mouse.Position))
                {
                    //play click sound effect
                    buttonClickSound.CreateInstance().Play();

                    //exit game
                    Exit();
                }
                else if (menuButtonLocs[3].Contains(mouse.Position))
                {
                    //play click sound effect
                    buttonClickSound.CreateInstance().Play();

                    //ges to game state
                    gameState = GAME;
                }
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Updates instruction state, handles user interactions
        private void Instructions()
        {
            // Checks if user clicked the left button
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //check if user clicked button
                if (menuButtonLocs[0].Contains(mouse.Position))
                {
                    //play click sound effect
                    buttonClickSound.CreateInstance().Play();

                    //goes to menu state
                    gameState = MENU_STATE;
                }
            }
        }

        //Pre: gameTime
        //Post: N/A
        //Description: Updates game, handling player, ghosts and map interactions
        private void Game(GameTime gameTime)
        {
            //Stores keybored state
            kb = Keyboard.GetState();

            //if user pressed q
            if (kb.IsKeyDown(Keys.Q) && !prevKb.IsKeyDown(Keys.Q))
            {
                //skip level
                player.SetHasWon(true);
            }

            // Checks if user clicked the left button
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //checks if the mouse is on the back to home button
                if (gameButtonLocs[0].Contains(mouse.Position))
                {
                    //play click sound effect
                    buttonClickSound.CreateInstance().Play();

                    //goes to menu state
                    gameState = MENU_STATE;

                    //call the set next level with level parameter
                    SetNextLevel(level1);
                }
            }

            //store previous keyboard state
            prevKb = kb;

            //Call update method and movement
            player.Update(gameTime, currentLevel.map, currentLevel.ghosts);
            PlayerMovement(currentLevel.map.GetMap());

            //for each ghosts in current level
            for (int i = 0; i < currentLevel.ghosts.Count; i++)
            {
                //call update ghost and movement
                currentLevel.ghosts[i].Update(gameTime, currentLevel.map, player);
                currentLevel.ghosts[i].Movement(player);
            }

            //if player died
            if (player.lives == 0)
            {
                //play player died sound effect
                playerDiedSound.CreateInstance().Play();

                //reset the level 
                SetNextLevel(currentLevel);
            }

            //check if player has won
            if (player.hasWon())
            {
                //play win sound effect
                playerWonSound.CreateInstance().Play();

                //Store and get the number of children the level has
                List<Level> currentChildren = currentLevel.GetChildren();

                //check the number of children
                if (currentChildren.Count == 0)
                {
                    //set score
                    currentLevel.score = player.score;

                    //Call add new score method
                    AddNewScore();

                    //go to menu   
                    gameState = END_GAME_STATE;
                }
                else if (currentChildren.Count == 1)
                {
                    //set score
                    currentLevel.score = player.score;

                    //Call add new score method
                    AddNewScore();

                    //change to transition game state
                    gameState = NEXT_LEVEL_TRANSITION;

                    //set next level
                    nextLevel = currentChildren[0];
                }
                else
                {
                    ////set score
                    currentLevel.score = player.score;

                    //check player lives
                    if (player.lives >= 3)
                    {
                        //set score
                        currentLevel.score = player.score;

                        //Call add new score method
                        AddNewScore();

                        //change to transition game state
                        gameState = NEXT_LEVEL_TRANSITION;

                        //set next level
                        nextLevel = currentChildren[0];
                    }
                    else
                    {
                        //set score
                        currentLevel.score = player.score;

                        //Call add new score method
                        AddNewScore();

                        //change to transition game state
                        gameState = NEXT_LEVEL_TRANSITION;

                        //set next level
                        nextLevel = currentChildren[1];
                    }
                }
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: shows transition screen button opition to go to next level
        private void NextLevelTransition()
        {
            // Checks if user clicked the left button
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //checks if the mouse is on the back to home button
                if (gameButtonLocs[3].Contains(mouse.Position))
                {
                    // Set game state
                    gameState = GAME;

                    //call set next level method with parameter of level
                    SetNextLevel(nextLevel);
                }
            }
        }

        //Pre: next level is not null
        //Post: N/A
        //Description: Resets levels that way game is replayable
        private void SetNextLevel(Level nextLevel)
        {
            //set current level and tile map to the next level and tile map
            currentLevel = nextLevel;
            currentTileMap = currentLevel.map;

            //reset player's score win, lives, hearts index, player position, ghost positions
            player.score = 0;
            player.SetHasWon(false);
            player.lives = 3;
            initialIndex = 0;
            player.hasPowerup = false;
            player.ResetToStartingPosition();
            player.ResetGhostsToStartingPosition(currentLevel.ghosts);

            //for each index in currentlevel map
            for (int i = 0; i < currentLevel.map.GetMap().GetLength(0); i++)
            {
                for (int j = 0; j < currentLevel.map.GetMap().GetLength(1); j++)
                {
                    //if current level coins are false
                    if (currentLevel.map.coins[i, j] != null && currentLevel.map.coins[i, j].hasCoin == false)
                    {
                        //set the coins to true
                        currentLevel.map.coins[i, j].hasCoin = true;
                    }
                    if (currentLevel.map.cherries[i, j] != null && currentLevel.map.cherries[i, j].hasCherry == false)
                    {
                        //set the coins to true
                        currentLevel.map.cherries[i,j].hasCherry = true;
                    }
                }
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: updates end game state, handles user interactions
        private void UpdateEndGame()
        {
            // Checks if user clicked the left button
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //check if mouse is within buttons rectangle
                if (menuButtonLocs[0].Contains(mouse.Position))
                {
                    //play click sound effect
                    buttonClickSound.CreateInstance().Play();

                    //goes to menu state
                    gameState = MENU_STATE;

                    //call set to next level method with level parameter
                    SetNextLevel(level1);
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //begin spriteBatch
            spriteBatch.Begin();

            //based on gamestate while go to diffrent game states
            switch (gameState)
            {
                case MENU_STATE:
                    //draw menu
                    DrawMenu();
                    break;
                case INSTRUCTIONS:
                    //draw instructions
                    DrawInstructions();
                    break;
                case GAME:
                    //draw game
                    DrawGame();
                    break;
                case END_GAME_STATE:
                    //draw end game
                    DrawEndGame();
                    break;
                case NEXT_LEVEL_TRANSITION:
                    //draw next level transition
                    DrawNextLevelTransition();
                    break;
            }

            //Checks if sound is playing
            if (MediaPlayer.State != MediaState.Playing)
            {
                //draw soud on image in red
                spriteBatch.Draw(gameButtonImgs[2], gameButtonLocs[2], Color.White);
            }
            else
            {
                //draw sound on image
                spriteBatch.Draw(gameButtonImgs[1], gameButtonLocs[2], Color.White);
            }

            //end spriteBatc
            spriteBatch.End();

            base.Draw(gameTime);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Draw menu page
        private void DrawMenu()
        {
            //draw background
            spriteBatch.Draw(backgroundImg, backgroundLoc, Color.White);

            //draw game title
            spriteBatch.DrawString(menuFont, title, titleLoc, Color.MediumPurple);
            spriteBatch.DrawString(menuFont, title, new Vector2(titleLoc.X + 4, titleLoc.X + 4), Color.White);

            //draw buttons for menu
            HoverDraw(menuButtonImgs[1], menuButtonLocs[1]);
            HoverDraw(menuButtonImgs[2], menuButtonLocs[2]);
            HoverDraw(menuButtonImgs[3], menuButtonLocs[3]);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Draw instructions page
        private void DrawInstructions()
        {
            //draw background
            spriteBatch.Draw(backgroundImg, backgroundLoc, Color.White);

            //draw game title
            spriteBatch.DrawString(menuFont, title, titleLoc, Color.MediumPurple);
            spriteBatch.DrawString(menuFont, title, new Vector2(titleLoc.X + 4, titleLoc.X + 4), Color.White);

            //draw instructions text
            spriteBatch.DrawString(instructionsFont, instructionsText[0], new Vector2(50, 260), Color.DarkSlateBlue);
            spriteBatch.DrawString(instructionsFont2, instructionsText[1], new Vector2(50, 320), Color.White);
            spriteBatch.DrawString(instructionsFont, instructionsText[2], new Vector2(50, 470), Color.DarkSlateBlue);

            //draw back to menu button
            HoverDraw(menuButtonImgs[0], menuButtonLocs[0]);

            //draw keyboard image
            spriteBatch.Draw(keyboardImg, keyboardLoc, Color.White);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Draw game page
        private void DrawGame()
        {
            //draw current tile map
            currentTileMap.Draw(spriteBatch);

            //draw player
            player.Draw(spriteBatch);

            //for each ghost in current level
            for (int i = 0; i < currentLevel.ghosts.Count; i++)
            {
                //draw ghost
                currentLevel.ghosts[i].Draw(spriteBatch, player);
            }

            //draw hearts 
            DrawHearts(spriteBatch, player.lives, initialIndex);

            //for each coin collected increase score by 10
            rectangle.Draw(spriteBatch, Color.White, true);

            //store and set score text, get size of score
            string scoreText = Convert.ToString(player.score);
            Vector2 scoreSize = scoreFont.MeasureString(scoreText);

            //centre the score text in the middle of the screen
            Vector2 centerPosition = new Vector2((screenWidth - scoreSize.X) / 2, 0);

            //draw score
            spriteBatch.DrawString(scoreFont, scoreText, centerPosition, Color.Black);

            //draw back to home button
            HoverDraw(gameButtonImgs[0], gameButtonLocs[0]);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Draw end page
        private void DrawEndGame()
        {
            //draw background
            spriteBatch.Draw(backgroundImg, backgroundLoc, Color.White);

            //store and set score text, get size of score, centre text
            string scoreText = Convert.ToString(player.score);
            Vector2 scoreSize = finalScoreFont.MeasureString("Score: " + scoreText);
            Vector2 centerPosition = new Vector2((screenWidth - scoreSize.X) / 2, 400);

            //draw score
            spriteBatch.DrawString(finalScoreFont, "Score: " + player.score, centerPosition, Color.MediumPurple);

            //get size of best score text and centre it
            Vector2 highScoreSize = finalScoreFont.MeasureString("Best Score: " + highScores[currentLevel.level - 1]);
            Vector2 centerPositionHighScores = new Vector2((screenWidth - highScoreSize.X) / 2, 500);

            //draw score
            spriteBatch.DrawString(finalScoreFont, "Best Score: " + highScores[currentLevel.level - 1], centerPositionHighScores, Color.MediumPurple);

            //draw end message
            spriteBatch.DrawString(instructionsFont, "Thank you for Playing Pac Plus", endGameMessageLoc, Color.White);

            //draw back to menu button
            HoverDraw(menuButtonImgs[0], menuButtonLocs[0]);
        }

        //Pre: N/A
        //Post: N/A
        //Description: Draw end page
        private void DrawNextLevelTransition()
        {
            //draw background
            spriteBatch.Draw(backgroundImg, backgroundLoc, Color.White);

            //draw next level mesage
            spriteBatch.DrawString(instructionsFont, transitionMessage, transitionMessageLoc, Color.White);

            //store and set score text, get size of score, centre text
            string scoreText = Convert.ToString(player.score);
            Vector2 scoreSize = finalScoreFont.MeasureString("Score: " + scoreText);
            Vector2 centerPosition = new Vector2((screenWidth - scoreSize.X) / 2, 400);

            //draw score
            spriteBatch.DrawString(finalScoreFont, "Score: " + player.score, centerPosition, Color.MediumPurple);

            //get size of best score text and centre it
            Vector2 highScoreSize = finalScoreFont.MeasureString("Best Score: " + highScores[currentLevel.level - 1]);
            Vector2 centerPositionHighScores = new Vector2((screenWidth - highScoreSize.X) / 2, 500);

            //draw score
            spriteBatch.DrawString(finalScoreFont, "Best Score: " + highScores[currentLevel.level - 1], centerPositionHighScores, Color.MediumPurple);

            //call hover draw method
            HoverDraw(gameButtonImgs[3], gameButtonLocs[3]);
        }

        //Pre: image is not null and rectangle
        //Post: N/A
        //Description: Draw Game Buttons
        private void HoverDraw(Texture2D img, Rectangle rec)
        {
            //check if the user if hovering the button image
            if (rec.Contains(mouse.Position))
            {
                //Draw hover image of button
                spriteBatch.Draw(img, rec, Color.Yellow);
            }
            else
            {
                //Draw regular button image
                spriteBatch.Draw(img, rec, Color.White);
            }
        }

        //Pre: spriteBatch, lives and currentindex needs be a non-negative number
        //Post: N/A
        //Description: Draw player hearts
        private void DrawHearts(SpriteBatch spriteBatch, int lives, int currentIndex)
        {
            //Check if the current index is greater than or equal to the hearts array length
            if (currentIndex >= heartsRec.Length)
            {
                //Exit the recursive call if the current index exceeds the array length
                return;
            }

            //Determine the index to use based on the number of remaining lives
            int heartIndex = (lives > 0) ? 0 : 1;

            //Draw the heart
            spriteBatch.Draw(hearts[heartIndex], heartsRec[currentIndex], Color.White);

            //Call rhe DrawHearts method
            DrawHearts(spriteBatch, lives - 1, currentIndex + 1);
        }

        //Pre: string of a file
        //Post: N/A
        //Description: Reads file
        private void ReadFile(string file)
        {
            //try reading the file
            try
            {
                //Perform the appropriate operation based on file
                switch (file)
                {
                    case FILE1:
                        //Call ReadAraysFromFile method
                        ReadArraysFromFile(file);
                        break;
                    case FILE2:
                        //Call ReadScoreFile
                        ReadScoreFile(file);
                        break;
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                //if file is not null
                if (inFile != null)
                {
                    //close file
                    inFile.Close();
                }
            }
        }

        //Pre: string of the filename is the correct file
        //Post: N/A
        //Description: reads the file and store reads the high scores 
        private void ReadScoreFile(string fileName)
        {
            //Open file
            inFile = File.OpenText(fileName);

            for (int i = 0; i < highScores.Length; i++)
            {
                highScores[i] = Convert.ToInt32(inFile.ReadLine());
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: overwrites the high score for a level
        private void AddNewScore()
        {
            //store level index
            int levelIndex = currentLevel.level - 1;

            //check if current score is greater than high score
            if (currentLevel.score > highScores[levelIndex])
            {
                try
                {
                    //read all existing scores from the file
                    string[] allScores = File.ReadAllLines(FILE2);

                    // update the score for the specific level
                    allScores[levelIndex] = currentLevel.score.ToString();

                    //write the updated scores back to the file
                    File.WriteAllLines(FILE2, allScores);

                    //update the highScores array with the new score
                    highScores[levelIndex] = currentLevel.score;
                }
                catch (Exception e)
                {
                    //handle expection
                }
            }
        }

        //Pre: string of the filename is the correct file
        //Post: N/A
        //Description: reads the file and stores the 2d arrays of the maps in it 
        private void ReadArraysFromFile(string fileName)
        { 
            //Open file
            inFile = File.OpenText(fileName);

            // Read all lines from the file
            string[] lines = File.ReadAllLines(fileName);

            //store current arrays and current array lines
            List<string[,]> currentArrays = new List<string[,]>();
            List<string[]> currentArrayLines = new List<string[]>();

            //for each line in the file
            foreach (string line in lines)
            {
                //if there is a % sign
                if (line.Trim() == "%")
                {
                    // Convert lines to a 2D array and add to the list
                    currentArrays.Add(ConvertLinesTo2DArray(currentArrayLines.ToArray()));

                    // Clear the current lines for the next array
                    currentArrayLines.Clear();
                }
                else
                {
                    currentArrayLines.Add(line.ToCharArray().Select(c => c.ToString()).ToArray());
                }
            }

            //if current array lines count is greater than 0
            if (currentArrayLines.Count > 0)
            {
                //Add the last array(if any) after the last '%'
                currentArrays.Add(ConvertLinesTo2DArray(currentArrayLines.ToArray()));
            }

            //add the current array to maps list
            maps.AddRange(currentArrays);
        }

        //Pre: lines' is a valid jagged array
        //Post: Converts a jagged array of strings to a 2D array and returns it
        //Description: Converts a jagged array to a 2D array by copying each element
        private string[,] ConvertLinesTo2DArray(string[][] lines)
        {
            //store the row, col and 2d array
            int rows = lines.Length;
            int cols = lines[0].Length;
            string[,] array = new string[rows, cols];

            //for each index in the 2d array
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    //set the 2d array to the jagged 2d array based on current index
                    array[i, j] = lines[i][j];
                }
            }

            //returns 2d array of the map
            return array;
        }

        //Pre: a 2d array of strings
        //Post: N/A
        //Description: Handles movement based on user input
        private void PlayerMovement(string[,] map)
        {
            //Does appropirate operation based on key pressed
            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up))
            {
                //Calls the player move method and moves player up 
                player.Move(0, -1, Player.UP, map);
            }
            else if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right))
            {
                //Calls the player move method and moves player right 
                player.Move(1, 0, Player.RIGHT, map);
            }
            else if (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down))
            {
                //Calls the player move method and moves player down 
                player.Move(0, 1, Player.DOWN, map);
            }
            else if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))
            {
                //Calls the player move method and moves player left 
                player.Move(-1, 0, Player.LEFT, map);
            }
        }
    }
}
