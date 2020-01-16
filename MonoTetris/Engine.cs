using System;
using System.IO;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoTetris
{
    public class Engine : Game
    {
        // Graphics
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D tetrisBackground;
        private Texture2D tetrisTextures;
        private SpriteFont gameFont;
        readonly Rectangle[] blockRectangles = new Rectangle[7];
        
        // Game
        private Board board;
        private Score score;
        private bool pause = false;
        
        // Input
        private KeyboardState oldKeyboardState = Keyboard.GetState();

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            // Create sprite rectangles for each figure in texture file
            // O figure
            blockRectangles[0] = new Rectangle(312, 0, 24, 24);
            // I figure
            blockRectangles[1] = new Rectangle(0, 24, 24, 24);
            // J figure
            blockRectangles [2] = new Rectangle (120, 0, 24, 24);
            // L figure
            blockRectangles [3] = new Rectangle (216, 24, 24, 24);
            // S figure
            blockRectangles [4] = new Rectangle (48, 96, 24, 24);
            // Z figure
            blockRectangles [5] = new Rectangle (240, 72, 24, 24);
            // T figure
            blockRectangles [6] = new Rectangle (144, 96, 24, 24);
        }
        
        protected override void Initialize()
        {
            Window.Title = "MonoTetris";

            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
            graphics.ApplyChanges();

            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 10.0f);
            
            // try to open file if exists, otherwise create it
            using (FileStream fileStream = File.Open("record.dat", FileMode.OpenOrCreate))
            {
                fileStream.Close();
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Add the SpriteBatch service
            Services.AddService(typeof(SpriteBatch), spriteBatch);
            
            // Load 2D texture
            tetrisBackground = Content.Load<Texture2D>("background");
            tetrisTextures = Content.Load<Texture2D>("tetris");
            
            // Load gamefont
            gameFont = Content.Load<SpriteFont>("Arial");
            
            // Create game field
            board = new Board(this, ref tetrisTextures, blockRectangles);
            board.Initialize();
            Components.Add(board);
            
            // Save player's score and game level
            score = new Score(this, gameFont);
            score.Initialize();
            Components.Add(score);
            
            // Load game record
            using (StreamReader streamReader = File.OpenText("record.dat"))
            {
                string player = null;
                if ((player = streamReader.ReadLine()) != null)
                {
                    score.RecordPlayer = player;
                }
                
                int record = 0;

                if ((record = Convert.ToInt32(streamReader.ReadLine())) != 0)
                {
                    score.RecordScore = record;
                }
            }

        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState ();
            if (keyboardState.IsKeyDown (Keys.Escape))
                this.Exit ();

            // Check pause
            bool pauseKey = (oldKeyboardState.IsKeyDown (Keys.P) && (keyboardState.IsKeyUp (Keys.P)));

            oldKeyboardState = keyboardState;

            if (pauseKey)
            {
                pause = !pause;
            }

            if (!pause)
            {
                // Find dynamic figure position
                board.FindDynamicFigure();
                
                // Increase player score
                int lines = board.DestroyLines();
                if (lines > 0)
                {
                    score.Value += (int) ((5.0f / 2.0f) * lines * (lines + 3));
                    board.Speed += 0.005f;
                }

                score.Level = (int) (10 * board.Speed);
                
                // Create new shape in game
                if (!board.CreateNewFigure())
                {
                    GameOver();
                }
                else
                {
                    // If left key is pressed
                    if (keyboardState.IsKeyDown(Keys.Left))
                    {
                        board.MoveFigureLeft ();
                    }
                    // If right key is pressed
                    if (keyboardState.IsKeyDown(Keys.Right))
                    {
                        board.MoveFigureRight ();
                    }
                    // If down key is pressed
                    if (keyboardState.IsKeyDown(Keys.Down))
                    {
                        board.MoveFigureDown ();
                    }

                    // Rotate figure
                    if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Space))
                    {
                        board.RotateFigure ();
                    }
                        

                    // Moving figure
                    if (board.Movement >= 1) 
                    {
                        board.Movement = 0;
                        board.MoveFigureDown ();
                    }
                    else
                    {
                        board.Movement += board.Speed;
                    }
                        
                }
            }

            base.Update(gameTime);
        }

        private void GameOver()
        {
            if (score.Value > score.RecordScore)
            {
                score.RecordScore = score.Value;
                pause = true;
                
                Record record = new Record();

                score.RecordPlayer = record.Player;

                using (StreamWriter writer = File.CreateText("record.dat"))
                {
                    writer.WriteLine(score.RecordPlayer);
                    writer.WriteLine(score.RecordScore);
                }

                pause = false;
            }
            
            board.Initialize();
            score.Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            
            spriteBatch.Draw(tetrisBackground, Vector2.Zero, Color.White);

            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}