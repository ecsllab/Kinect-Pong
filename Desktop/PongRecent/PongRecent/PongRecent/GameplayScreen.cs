﻿#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


using Microsoft.Research.Kinect.Nui;

#endregion

namespace PongRecent
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        private Texture2D segment;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        private bool spaceKeyDown = false;
        Random random = new Random();

        Level level;
        bool positionWorkedIs;

        #endregion

       

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(bool hand, int difficulty, int gamePlay)
        {
            positionWorkedIs = false;
            level = new Level(positionWorkedIs, hand, difficulty, gamePlay);
           //positionWorked2 = new Level2(positionWorked2Is);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("MyFont");
            segment = content.Load<Texture2D>("Dot");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            //Game
            level.LoadContent(content, ScreenManager.GraphicsDevice);
            //level2.LoadContent(content, ScreenManager.GraphicsDevice);
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                level.Update(gameTime);
                //level2.Update(gameTime);
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        /// 



        public override void HandleInput(InputState input)
        {
           if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            if (level.positionWorked)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                //level.UnloadContent();
            }
            else if (level.userWantsToQuit == true)
            {
                ScreenManager.AddScreen(new QuitMenuScreen(), ControllingPlayer);
                level.UnloadContent();
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            level.Draw(gameTime, spriteBatch);
            //level2.Draw(gameTime, spriteBatch);
            //spriteBatch.Begin();

        }


        #endregion
    }
}


