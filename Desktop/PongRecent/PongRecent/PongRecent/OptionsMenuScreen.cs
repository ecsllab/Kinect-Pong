#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
#if IPHONE
using Microsoft.Xna.Framework;
#else
using Microsoft.Xna.Framework;
#endif
#endregion

namespace PongRecent
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry handednessMenuEntry;
        MenuEntry difficultyMenuEntry;
        MenuEntry playersMenuEntry;

        public bool hand = true; //right is true, left is false
        public int difficulty = 0; //0 beginner, 1 standard, 2 Expert
        public int gameplay = 0;


        enum Handedness
        {
            Left,
            Right,
        }

        enum Difficulty
        {
            Beginner,
            Standard,
            Expert,
        }

        enum Players
        {
            OnePlayerRandom, //one person can use both paddles
            OnePlayerAI, //one person plays against the ai
            TwoPlayer, //two people play against each other
        }

        static Handedness currentUngulate = Handedness.Right;

        static Difficulty currentDifficulty = Difficulty.Beginner;

        static Players currentPlayability = Players.OnePlayerRandom;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            handednessMenuEntry = new MenuEntry(string.Empty);
            difficultyMenuEntry = new MenuEntry(string.Empty);
            playersMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry backMenuEntry = new MenuEntry("Back");

            // Hook up menu event handlers.
            handednessMenuEntry.Selected += HandednessMenuEntrySelected;
            difficultyMenuEntry.Selected += DifficultyMenuEntrySelected;
            playersMenuEntry.Selected += PlayersMenuEntrySelected;
            backMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(handednessMenuEntry);
            MenuEntries.Add(difficultyMenuEntry);
            MenuEntries.Add(playersMenuEntry);
            MenuEntries.Add(backMenuEntry);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            handednessMenuEntry.Text = "Preferred Handedness: " + currentUngulate;
            difficultyMenuEntry.Text = "Level of Difficulty: " + currentDifficulty;
            playersMenuEntry.Text = "Players: " + currentPlayability;
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void HandednessMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentUngulate++;
            if (hand == true)
                hand = false;
            else
                hand = true;

            if (currentUngulate > Handedness.Right)
                currentUngulate = 0;

            SetMenuEntryText();
        }


        void DifficultyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentDifficulty++;
            difficulty++;

            if (currentDifficulty > Difficulty.Expert)
            {
                currentDifficulty = 0;
                difficulty = 0;
            }

            SetMenuEntryText();
        }

        void PlayersMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentPlayability++;
            gameplay++;

            if (currentPlayability > Players.TwoPlayer)
            {
                currentPlayability = 0;
                gameplay = 0;
            }
            SetMenuEntryText();
        }


        #endregion
    }
}
