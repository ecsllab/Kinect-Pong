using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net; //not too sure what this is for
using Microsoft.Xna.Framework.Storage; //not too sure what this is for

using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.WinForm;


namespace PongRecent
{
    class Level : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Current player.
        /// </summary>
        //Player player;
        int number;
        int numEsc = 0;
        Runtime runtime = new Runtime();
        Viewport viewportGameplay;
        const int kLRMargin = 30, kPaddleWidth = 26, kPaddleHeight = 120;
        const int kBallWidth = 24, kBallHeight = 24;
        const int kGameWidth = 1280, kGameHeight = 1024;
        Texture2D kinectDepthVideo;
        bool passedCenter = false;
        Texture2D SpriteTexture;
        float RotationAngle;
        const int PaddleVelocity = 5;
        const int PaddleTwoVelocity = 3;
        FormWindowState WindowState;
        KeyboardState keyboardState;
        bool paused = false;
        bool pauseKeyDown = false;
        bool pausedForGuide = false;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ScreenManager screenManager;
        bool turnOnPause = false;
        int pauseCheck = 0;
        bool escapeWasOn = false;
        bool escapeOn = false;

        public float playerPos;
        
        public bool positionWorked = false;
        public bool handedness = false;
        public bool userWantsToQuit = false;
        public bool userWantsToResume = false;
        public int difficulty = 0;
        public int gamePlay = 0;

        Texture2D dotTexture = null, ballTexture = null;

        SpriteFont myfont;

        Microsoft.Xna.Framework.Rectangle ourPaddleRect = new Microsoft.Xna.Framework.Rectangle(kLRMargin, 0, kPaddleWidth, kPaddleHeight);
        Microsoft.Xna.Framework.Rectangle aiPaddleRect = new Microsoft.Xna.Framework.Rectangle(kGameWidth - kLRMargin - kPaddleWidth, 0, kPaddleWidth, kPaddleHeight);


        Vector2 ballVelocity;
        Microsoft.Xna.Framework.Rectangle ballRect;

        //Something for the Depth Map
        const int RED_IDX = 2;
        const int GREEN_IDX = 1;
        const int BLUE_IDX = 0;
        byte[] depthFrame32 = new byte[320 * 240 * 4];
        int score1 = 0;
        int score2 = 0;
        int dir = 1;
        int dirSec = 1;

        public Level(bool userPositionWorked, bool hand, int userDifficulty, int levelOfGamePlay)
        {
            this.positionWorked = userPositionWorked;
            this.handedness = hand;
            this.difficulty = userDifficulty;
            this.gamePlay = levelOfGamePlay;
            Initialize();
        }

        protected override void Initialize()
        {

            RestartGame();

            //this.WindowState = FormWindowState.Maximized;
            runtime.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseDepth);
            runtime.SkeletonEngine.TransformSmooth = true;

            runtime.SkeletonEngine.SmoothParameters = new TransformSmoothParameters()
            {
                Smoothing = 0.4f,
                Correction = 0.3f,
                Prediction = 0.6f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
            };

            runtime.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(runtime_SkeletonFrameReady);

            viewportGameplay = new Viewport();

            viewportGameplay.X = 0;
            viewportGameplay.Y = 0;
            viewportGameplay.Width = kGameWidth;//this.graphics.PreferredBackBufferWidth;
            viewportGameplay.Height = kGameHeight;//(int)(this.graphics.PreferredBackBufferHeight / 1.3f);
            //this.graphics.IsFullScreen = true;
            //this.graphics.ApplyChanges();

            base.Initialize();

        }


        void runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            int index = 0;
            //Get Joint Information
            foreach (SkeletonData data in e.SkeletonFrame.Skeletons)
            {
                if (data.TrackingState == SkeletonTrackingState.Tracked)
                {
                    //assign the joints
                    //right side
                   
                    Joint joint = data.Joints[JointID.HandRight];
                    Joint elbowRight = data.Joints[JointID.ElbowRight];
                    Joint shoulderRight = data.Joints[JointID.ShoulderRight];
                    Joint kneeRight = data.Joints[JointID.KneeRight];
                    Joint hipRight = data.Joints[JointID.HipRight];
                    Joint footRight = data.Joints[JointID.FootRight];
                    Joint ankleRight = data.Joints[JointID.AnkleRight];

                    //left side
                    Joint jointLeft = data.Joints[JointID.HandLeft];
                    Joint elbowLeft = data.Joints[JointID.ElbowLeft];
                    Joint shoulderLeft = data.Joints[JointID.ShoulderLeft];
                    Joint kneeLeft = data.Joints[JointID.KneeLeft];
                    Joint hipLeft = data.Joints[JointID.HipLeft];
                    Joint footLeft = data.Joints[JointID.FootLeft];
                    Joint ankleLeft = data.Joints[JointID.AnkleLeft];

                    //neither side
                    Joint head = data.Joints[JointID.Head];
                    Joint shoulderMid = data.Joints[JointID.ShoulderCenter];


                    //Player Position
                    this.playerPos = hipRight.Position.Z;

                    int handY = (int)joint.ScaleTo(kGameWidth, kGameHeight - kPaddleHeight, 0.6f, 0.4f).Position.Y;
                    handY = Math.Max(handY, 0);
                    handY = Math.Min(handY, kGameHeight - kPaddleHeight);
                    aiPaddleRect.Y = handY;

                    int leftHandY = (int)jointLeft.ScaleTo(kGameWidth, kGameHeight - kPaddleHeight, 0.6f, 0.4f).Position.Y;
                    leftHandY = Math.Max(leftHandY, 0);
                    leftHandY = Math.Min(leftHandY, kGameHeight - kPaddleHeight);
                    if (this.gamePlay == 0)
                        ourPaddleRect.Y = leftHandY;
                    else if (this.gamePlay == 1)
                        ourPaddleRect.Y = ballRect.Y;

                    if (this.handedness == false)
                        aiPaddleRect.Y = leftHandY;

                    //check to see if the user is making a position to pause the game
                    if ((footRight.Position.Z < hipRight.Position.Z) && (footLeft.Position.Z > hipLeft.Position.Z) && (joint.Position.Y > head.Position.Y)) 
                    {
                        //pause game
                        this.positionWorked = true;
                    }
                    else 
                        this.positionWorked = false;


                    

                    ////check to see if the user is making a position to quit the game
                    if ((joint.Position.Y > head.Position.Y) && (jointLeft.Position.Y > head.Position.Y) && (footRight.Position.X > hipRight.Position.X) && (footLeft.Position.X < hipLeft.Position.X))
                    {
                        userWantsToQuit = true;
                    }
                    else
                        userWantsToQuit = false;
                }
            }
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            myfont = content.Load<SpriteFont>("MyFont");
            //SpriteTexture = content.Load<Texture2D>("Arrow");

            dotTexture = content.Load<Texture2D>("Dot");
            ballTexture = content.Load<Texture2D>("Ball");
        }

        enum BallCollision
        {
            None,
            RightPaddle,
            LeftPaddle,
            RightMiss,
            LeftMiss
        }


        //different poses change the balls velocity, etc.
        private BallCollision AdjustBallPositionWithScreenBounds(ref Microsoft.Xna.Framework.Rectangle enclosingRect, ref Vector2 velocity, ref Microsoft.Xna.Framework.Rectangle paddleRect)
        {
            BallCollision collision = BallCollision.None;


            enclosingRect.X += (int)velocity.X;
            enclosingRect.Y += (int)velocity.Y;

            if (enclosingRect.Y >= kGameHeight - kBallHeight)
            {
                velocity.Y *= -1;
            }
            else if (enclosingRect.Y <= 0)
            {
                velocity.Y *= -1;
            }

            if (aiPaddleRect.Intersects(enclosingRect))
            {
                velocity.X *= -1;
                collision = BallCollision.RightPaddle;
            }
            else if (ourPaddleRect.Intersects(enclosingRect))
            {
                velocity.X *= -1;
                collision = BallCollision.LeftPaddle;
            }

            else if (enclosingRect.X >= (kGameWidth + 10))
            {
                collision = BallCollision.RightMiss;
                score1++;
            }
            else if (enclosingRect.X <= 0)
            {
                collision = BallCollision.LeftMiss;
                score2++;
            }

            return collision;
        }

        private void RestartGame()
        {
            int ballMinXPos = 350;
            int ballMaxXPos = kGameWidth - 350;
            Random randomX = new Random();
            int randBallStartXPos = randomX.Next(ballMinXPos, ballMaxXPos);
            Random randomY = new Random();
            int randBallStartYPos = randomX.Next(kGameHeight);
            Random randomXVel = new Random();
            int randBallStartXVel = randomXVel.Next(0, 100);
            if (randBallStartXVel % 2 == 0)
                randBallStartXVel = 1;
            else
                randBallStartXVel = -1;
            if (this.difficulty == 0)
                ballVelocity = new Vector2((4.0f * randBallStartXVel), 4.0f);
            else if (this.difficulty == 1)
                ballVelocity = new Vector2((6.0f * randBallStartXVel), 6.0f);
            else if (this.difficulty == 2)
                ballVelocity = new Vector2((10.0f * randBallStartXVel), 10.0f);
            //ballVelocity = new Vector2((1.0f * randBallStartXVel), 1.0f);
            ballRect = new Microsoft.Xna.Framework.Rectangle(randBallStartXPos, randBallStartYPos, kBallWidth, kBallHeight);
        }

        public void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                this.Exit();
            keyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            BallCollision collision = AdjustBallPositionWithScreenBounds(ref ballRect, ref ballVelocity, ref ourPaddleRect);

                    if (collision > 0)
                    {
                        passedCenter = false;

                        float newY = (new Random().Next(80) + 1) / 10.0f;
                        ballVelocity.Y = ballVelocity.Y > 0 ? newY : -newY;
                    }

                    if (collision == BallCollision.RightMiss || collision == BallCollision.LeftMiss)
                    {
                        RestartGame();
                    }

                    //Calculate angle for Arrow Display
                    /*double angle = Math.Sin(((float)ballVelocity.Y / (float)ballVelocity.X));
                    RotationAngle = (float)angle;
                    if (ballVelocity.X < 0)
                        RotationAngle -= MathHelper.Pi;*/
                    base.Update(gameTime);
               // }
           // }

        }

        public void UnloadContent()
        {
            runtime.Uninitialize();
        }
		

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int height = 0;
            int x = 0;
            int skip = kGameHeight / 20;
            //GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            spriteBatch.Begin();

            Vector2 screenpos, origin;
            screenpos.X = ballRect.X + ballRect.Width / 2;
            screenpos.Y = ballRect.Y + ballRect.Height / 2;

            if (this.gamePlay == 1)
                ourPaddleRect.Y = ballRect.Y - ourPaddleRect.Height;
            else if (this.gamePlay == 2)
                ourPaddleRect.Y = 0;
            spriteBatch.Draw(dotTexture, ourPaddleRect, Microsoft.Xna.Framework.Color.White);
            spriteBatch.Draw(dotTexture, aiPaddleRect, Microsoft.Xna.Framework.Color.Navy);

            //Get rid of these constants 100, 400, 60 .... etc below. 
            spriteBatch.Draw(ballTexture, ballRect, Microsoft.Xna.Framework.Color.Brown);
            //spriteBatch.Draw(SpriteTexture, screenpos, null, Microsoft.Xna.Framework.Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
            //Create Text :)
            int score_1 = score1;
            int score_2 = score2;
            int xPos = ballRect.X / 100;
            int yPos = ballRect.Y / 100;
            spriteBatch.DrawString(myfont, this.playerPos.ToString(), new Vector2(0, 0), Microsoft.Xna.Framework.Color.Brown);
            if(this.playerPos > 3.4)
                spriteBatch.DrawString(myfont, "Come Closer to the Kinect", new Vector2(kGameWidth / 2 - 150, kGameHeight / 3), Microsoft.Xna.Framework.Color.Red);

            if (this.playerPos < 2.0)
                spriteBatch.DrawString(myfont, "Back Away from the Kinect", new Vector2(kGameWidth/2-150, kGameHeight/3), Microsoft.Xna.Framework.Color.Red);

            spriteBatch.DrawString(myfont, "Computer Score:", new Vector2(100, 0), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, score_1.ToString(), new Vector2(300, 0), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, "Your Score:", new Vector2(kGameWidth - 300, 0), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, score2.ToString(), new Vector2(kGameWidth - 100, 0), Microsoft.Xna.Framework.Color.Yellow);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}
