#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
#if IPHONE
using Microsoft.Xna.Framework;
#else
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.WinForm;
#endif
#endregion

namespace PongRecent
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization
        OptionsMenuScreen optionsMenu = new OptionsMenuScreen();
        HowToMenuScreen howToScreen = new HowToMenuScreen("\n Kinect Pong is just like the original pong game, \n instead of using a joystick, you use your hands to control the paddle. \n Use the positions above to perform actions \n such as pause and quit.", false);
        static int kGameWidth = 1280;
        static int kGameHeight = 1024;
        public static Microsoft.Xna.Framework.Rectangle originalPong = new Microsoft.Xna.Framework.Rectangle((kGameWidth/2), (kGameHeight/2), 50, 50);
        public static Microsoft.Xna.Framework.Rectangle rightHandRectangle;
        Vector2 handRightVector;
        public static bool pressButton = false;
        int selectedEntry = 0;

        //Runtime runtime = new Runtime();

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            // Create our menu entries.
            MenuEntry pongMenuEntry = new MenuEntry("Pong");
            MenuEntry level2MenuEntry = new MenuEntry("Level 2");
            MenuEntry level3MenuEntry = new MenuEntry("Level 3");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry howToMenuEntry = new MenuEntry("How to play");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            pongMenuEntry.Selected += pongMenuEntrySelected;
            level2MenuEntry.Selected += Level2MenuEntrySelected;
            level3MenuEntry.Selected += Level3MenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            howToMenuEntry.Selected += HowToMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(pongMenuEntry);
            MenuEntries.Add(level2MenuEntry);
            MenuEntries.Add(level3MenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(howToMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            //Initialize();
        }


        #endregion


        //protected void Initialize()
        //{
        //    runtime.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseDepth);
        //    runtime.SkeletonEngine.TransformSmooth = true;

        //    runtime.SkeletonEngine.SmoothParameters = new TransformSmoothParameters()
        //    {
        //        Smoothing = 0.4f,
        //        Correction = 0.3f,
        //        Prediction = 0.6f,
        //        JitterRadius = 1.0f,
        //        MaxDeviationRadius = 0.5f
        //    };

        //    runtime.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(runtime_SkeletonFrameReady);
        //}


        //void runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        //{
        //    int index = 0;
        //    //Get Joint Information
        //    foreach (SkeletonData data in e.SkeletonFrame.Skeletons)
        //    {
        //        if (data.TrackingState == SkeletonTrackingState.Tracked)
        //        {
        //            //assign the joints
        //            //right side
        //            Joint joint = data.Joints[JointID.HandRight];
        //            Joint elbowRight = data.Joints[JointID.ElbowRight];
        //            Joint shoulderRight = data.Joints[JointID.ShoulderRight];
        //            Joint kneeRight = data.Joints[JointID.KneeRight];
        //            Joint hipRight = data.Joints[JointID.HipRight];
        //            Joint footRight = data.Joints[JointID.FootRight];

        //            //left side
        //            Joint jointLeft = data.Joints[JointID.HandLeft];
        //            Joint elbowLeft = data.Joints[JointID.ElbowLeft];
        //            Joint shoulderLeft = data.Joints[JointID.ShoulderLeft];
        //            Joint kneeLeft = data.Joints[JointID.KneeLeft];
        //            Joint hipLeft = data.Joints[JointID.HipLeft];
        //            Joint footLeft = data.Joints[JointID.FootLeft];

        //            //neither side
        //            Joint head = data.Joints[JointID.Head];
        //            Joint shoulderMid = data.Joints[JointID.ShoulderCenter];

        //            //check for rightside tracking
        //            if (joint.TrackingState != JointTrackingState.Tracked)
        //                break;
        //            if (elbowRight.TrackingState != JointTrackingState.Tracked)
        //                break;
        //            if (shoulderRight.TrackingState != JointTrackingState.Tracked)
        //                break;

        //            //check for leftside tracking
        //            if (jointLeft.TrackingState != JointTrackingState.Tracked)
        //                break;
        //            if (elbowLeft.TrackingState != JointTrackingState.Tracked)
        //                break;
        //            if (shoulderLeft.TrackingState != JointTrackingState.Tracked)
        //                break;

        //            //check for head tracking
        //            if (head.TrackingState != JointTrackingState.Tracked)
        //                break;
        //            if (shoulderMid.TrackingState != JointTrackingState.Tracked)
        //                break;

        //            handRightVector = new Vector2(joint.Position.X, joint.Position.Y);
        //            int handRightX = (int)joint.ScaleTo(kGameWidth, kGameHeight).Position.X;
        //            int handRightY = (int)joint.ScaleTo(kGameWidth, kGameHeight).Position.Y;
        //            rightHandRectangle = new Microsoft.Xna.Framework.Rectangle(handRightX, handRightY, 30, 30);

        //            //check to see where the user is putting their hand and move on the menu accordingly
        //            //top selection
        //            if ((rightHandRectangle.Left > originalPong.Left) && (rightHandRectangle.Right < originalPong.Right) && (rightHandRectangle.Bottom < originalPong.Bottom) && (rightHandRectangle.Top > originalPong.Top))
        //            {
        //                selectedEntry = 0;
        //                pressButton = true;
        //            }
        //            //if (jointLeft.Position.Z < elbowLeft.Position.Z)
        //            //    pressButton = true;
        //            else
        //                pressButton = false;

        //            //second selection
        //        //    else if (joint.Position.Y > head.Position.Y)
        //        //    {
        //        //        selectedEntry = 1;

        //        //        if (jointLeft.Position.Z < elbowLeft.Position.Z)
        //        //            pressButton = true;
        //        //        else
        //        //            pressButton = false;
        //        //    }

        //        //    //third selection
        //        //    else if ((joint.Position.Y > shoulderRight.Position.Y) && (joint.Position.Y < head.Position.Y))
        //        //    {
        //        //        selectedEntry = 2;

        //        //        if (jointLeft.Position.Z < elbowLeft.Position.Z)
        //        //            pressButton = true;
        //        //        else
        //        //            pressButton = false;
        //        //    }

        //        //    //fourth selection
        //        //    else if ((joint.Position.Y < shoulderRight.Position.Y) && (joint.Position.Y > hipRight.Position.Y))
        //        //    {
        //        //        selectedEntry = 3;

        //        //        if (jointLeft.Position.Z < elbowLeft.Position.Z)
        //        //            pressButton = true;
        //        //        else
        //        //            pressButton = false;
        //        //    }

        //        //    //fifth selection
        //        //    else if (joint.Position.X < hipRight.Position.X)
        //        //    {
        //        //        selectedEntry = 4;

        //        //        if (jointLeft.Position.Z < elbowLeft.Position.Z)
        //        //            pressButton = true;
        //        //        else
        //        //            pressButton = false;
        //        //    }
        //       }
        //    }
        //}


        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void pongMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(optionsMenu.hand, optionsMenu.difficulty, optionsMenu.gameplay));
        }

        void Level2MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new Level2Screen(optionsMenu.hand, optionsMenu.difficulty));
        }

        void Level3MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new Level3Screen(optionsMenu.hand, optionsMenu.difficulty));
        }
        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(optionsMenu, e.PlayerIndex);
        }

        void HowToMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(howToScreen, e.PlayerIndex);
        }
        
        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
