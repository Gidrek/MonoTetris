using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoTetris
{
    /// <summary>
    /// This is a game component that implements IUpdateable
    /// </summary>
    public class Score : DrawableGameComponent
    {
        // Graphic
        protected SpriteBatch sBatch;
        protected SpriteFont font;
        
        // Counters
        protected int value;
        protected int level;
        protected int recordScore = 0;
        protected string recordPlayer = "Player 1";

        public Score(Game game, SpriteFont font)
            : base(game)
        {
            sBatch = (SpriteBatch) Game.Services.GetService(typeof(SpriteBatch));
            this.font = font;
        }
        
        // Allows the game component to perform any initialization it needs to before
        // starting to run. This is where it can query for any required services and load content
        public override void Initialize()
        {
            value = 0;
            level = 1;
            base.Initialize();
        }

        public int Value
        {
            get => value;
            set => this.value = value;
        }

        public int Level
        {
            get => level;
            set => level = value;
        }

        public int RecordScore
        {
            get => recordScore;
            set => recordScore = value;
        }

        public string RecordPlayer
        {
            get => recordPlayer;
            set => recordPlayer = value;
        }
        
        /// <summary>
        /// This is called when the game should draw itself
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timming values.</param>
        public override void Draw(GameTime gameTime)
        {
            sBatch.DrawString(font, "Score:\n" + value + "\nLevel: " + level, new Vector2(1.5f * 24, 3 * 24), Color.Green);
            sBatch.DrawString(font, "Record:\n" + recordPlayer + "\n" + recordScore, new Vector2(1.5f * 24, 13 * 24), Color.Orange);
            base.Draw(gameTime);
        }
    }
}