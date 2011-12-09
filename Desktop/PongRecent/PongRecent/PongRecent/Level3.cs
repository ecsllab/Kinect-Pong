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

//using Microsoft.Xna.Samples.GameStateManagement;

namespace PongRecent
{
    class Level3 : Microsoft.Xna.Framework.Game
    {
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

        double xVelocity = 0;
        double yVelocity = 0;

        double scalingValue = 2.0;

        KeyboardState keyboardState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D dotTexture = null, ballTexture = null;

        SpriteFont myfont;
        SpriteFont bigfont;
        Microsoft.Xna.Framework.Rectangle leftHandTick = new Microsoft.Xna.Framework.Rectangle(0, kGameHeight/2-kLRMargin/2, kLRMargin / 5, kLRMargin);
        Microsoft.Xna.Framework.Rectangle RightHandTick = new Microsoft.Xna.Framework.Rectangle(kGameWidth / 2-kLRMargin/2, 0, kLRMargin, kLRMargin/5);
        Microsoft.Xna.Framework.Rectangle ourPaddleRect = new Microsoft.Xna.Framework.Rectangle(kGameWidth - kLRMargin - kPaddleWidth, 0, kPaddleWidth, kPaddleHeight);
        Microsoft.Xna.Framework.Rectangle ourSecPaddleRect = new Microsoft.Xna.Framework.Rectangle(kGameWidth - kLRMargin - kPaddleWidth, kGameHeight-kPaddleHeight, kPaddleWidth, kPaddleHeight);
        Microsoft.Xna.Framework.Rectangle aiPaddleRect = new Microsoft.Xna.Framework.Rectangle(kLRMargin, 0, kPaddleWidth, kPaddleHeight);
        Microsoft.Xna.Framework.Rectangle numberLine = new Microsoft.Xna.Framework.Rectangle(kGameWidth - kLRMargin / 2, 0, kLRMargin / 5 - 2, kGameHeight);
        Microsoft.Xna.Framework.Rectangle xNumberLine = new Microsoft.Xna.Framework.Rectangle(0, kGameHeight - kLRMargin / 2, kGameWidth, kLRMargin / 5 - 2);
        Microsoft.Xna.Framework.Rectangle yAxis = new Microsoft.Xna.Framework.Rectangle(kGameWidth / 2, 0, kLRMargin / 5 - 2, kGameHeight);
        Microsoft.Xna.Framework.Rectangle xAxis = new Microsoft.Xna.Framework.Rectangle(0, kGameHeight / 2, kGameWidth, kLRMargin / 5 - 2);
        Microsoft.Xna.Framework.Rectangle lineTick = new Microsoft.Xna.Framework.Rectangle(kGameWidth - kLRMargin + 2, 0, kLRMargin, kLRMargin / 6);
        Microsoft.Xna.Framework.Rectangle xLineTick = new Microsoft.Xna.Framework.Rectangle(0, kGameHeight - kLRMargin, kLRMargin / 6, kLRMargin);

        const int NumberOfTicksAlongX = 40;
        const int NumberOfTicksAlongY = 20;
        //const int NumberOfGridLinesAlongX = 40;
        // const int NumberOfGridLinesAlongY = 20;
        Microsoft.Xna.Framework.Rectangle[] XTicks = new Microsoft.Xna.Framework.Rectangle[NumberOfTicksAlongX];
        //Microsoft.Xna.Framework.Rectangle[] XLines = new Microsoft.Xna.Framework.Rectangle[NumberOfGridLinesAlongX];
        Microsoft.Xna.Framework.Rectangle[] YTicks = new Microsoft.Xna.Framework.Rectangle[NumberOfTicksAlongY];
        //Microsoft.Xna.Framework.Rectangle[] YLines = new Microsoft.Xna.Framework.Rectangle[NumberOfGridLinesAlongY];

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
        int number = 2;
        public bool positionWorked = false;
        public bool quadrantOne = false;
        public bool quadrantTwo = false;
        public bool quadrantThree = false;
        public bool quadrantFour = false;
        public bool userWantsToResume = false;
        public bool userWantsToQuit = false;
        public bool handedness = false;
        public int difficulty = 0;
        public float playerPos = 0;

        public float rightElbowAngle;
        public float leftElbowAngle;
        public float rightKneeAngle;
        public float leftKneeAngle;

        Runtime runtime = new Runtime();

        public Level3(bool positionWorked, bool hand, int difficulty)
        {
            this.positionWorked = positionWorked;
            this.handedness = hand;
            this.difficulty = difficulty;
            Initialize();
        }

        protected override void Initialize()
        {

            RestartGame();

            this.WindowState = FormWindowState.Maximized;
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
            //runtime.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution640x480, ImageType.Depth);
            //runtime.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(kinectSensor_DepthFrameReady);
            base.Initialize();

            InitializeTicks();
            //InitializeGridLines();

        }
        void InitializeTicks()
        {
            //y axis
            int yPos = 0;
            int tickThickness = kLRMargin / 10;
            for (int i = 0; i < YTicks.Length; i++)
            {
                //Center the Ticks
                YTicks[i].X = kGameWidth / 2 - kLRMargin / 2;//kGameWidth - kLRMargin + 2;
                YTicks[i].Y = yPos;
                YTicks[i].Width = kLRMargin;
                YTicks[i].Height = tickThickness;
                yPos += kGameHeight / YTicks.Length;
            }

            int xPos = 0;

            for (int i = 0; i < XTicks.Length; i++)
            {
                XTicks[i].X = xPos;
                //Center the Ticks
                XTicks[i].Y = kGameHeight / 2 - kLRMargin / 2;//kGameHeight - kLRMargin + 2;
                XTicks[i].Width = tickThickness;
                XTicks[i].Height = kLRMargin;
                xPos += kGameWidth / YTicks.Length;
            }
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

                    //left side
                    Joint jointLeft = data.Joints[JointID.HandLeft];
                    Joint elbowLeft = data.Joints[JointID.ElbowLeft];
                    Joint shoulderLeft = data.Joints[JointID.ShoulderLeft];
                    Joint kneeLeft = data.Joints[JointID.KneeLeft];
                    Joint hipLeft = data.Joints[JointID.HipLeft];
                    Joint footLeft = data.Joints[JointID.FootLeft];

                    //neither side
                    Joint head = data.Joints[JointID.Head];
                    Joint shoulderMid = data.Joints[JointID.ShoulderCenter];

                    if (this.handedness == true)
                    {
                        int handY = (int)joint.ScaleTo(kGameWidth, kGameHeight - kPaddleHeight, 0.6f, 0.4f).Position.Y;
                        handY = Math.Max(handY, 0);
                        handY = Math.Min(handY, kGameHeight - kPaddleHeight);
                        aiPaddleRect.Y = handY;
                    }
                    else
                    {
                        int leftHandY = (int)jointLeft.ScaleTo(kGameWidth, kGameHeight - kPaddleHeight, 0.6f, 0.4f).Position.Y;
                        leftHandY = Math.Max(leftHandY, 0);
                        leftHandY = Math.Min(leftHandY, kGameHeight - kPaddleHeight);
                        aiPaddleRect.Y = leftHandY;
                    }

                    leftHandTick.X = (int)joint.ScaleTo(kGameWidth, kGameHeight - kPaddleHeight, 0.6f, 0.4f).Position.X;
                    RightHandTick.Y = (int)jointLeft.ScaleTo(kGameWidth, kGameHeight - kPaddleHeight, 0.6f, 0.4f).Position.Y;

                    double xPos = (leftHandTick.X - kGameWidth / 2) / YTicks.Length / 3;
                    double yPos = (RightHandTick.Y - kGameHeight / 2) / YTicks.Length / 3;

                    xVelocity = xPos;
                    yVelocity = yPos;

                    //Player Position
                    this.playerPos = hipRight.Position.Z;

                    //Check to see what the users angle is between their shoulder and their elbow
                    Vector3 rightElbowVector = new Vector3(elbowRight.Position.X, elbowRight.Position.Y, elbowRight.Position.Z);
                    Vector3 rightShoulderVector = new Vector3(shoulderRight.Position.X, shoulderRight.Position.Y, shoulderRight.Position.Z);
                    Vector3 rightHandVector = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
                    Vector3 leftElbowVector = new Vector3(elbowLeft.Position.X, elbowLeft.Position.Y, elbowLeft.Position.Z);
                    Vector3 leftShoulderVector = new Vector3(shoulderLeft.Position.X, shoulderLeft.Position.Y, shoulderLeft.Position.Z);
                    Vector3 leftHandVector = new Vector3(jointLeft.Position.X, jointLeft.Position.Y, jointLeft.Position.Z);
                    Vector3 rightKneeVector = new Vector3(kneeRight.Position.X, kneeRight.Position.Y, kneeRight.Position.Z);
                    Vector3 rightHipVector = new Vector3(hipRight.Position.X, hipRight.Position.Y, hipRight.Position.Z);
                    Vector3 rightFootVector = new Vector3(footRight.Position.X, footRight.Position.Y, footRight.Position.Z);
                    Vector3 leftKneeVector = new Vector3(kneeLeft.Position.X, kneeLeft.Position.Y, kneeLeft.Position.Z);
                    Vector3 leftHipVector = new Vector3(hipLeft.Position.X, hipLeft.Position.Y, hipLeft.Position.Z);
                    Vector3 leftFootVector = new Vector3(footLeft.Position.X, footLeft.Position.Y, footLeft.Position.Z);

                    Vector3 rightElbowHandVector = rightElbowVector - rightHandVector;
                    Vector3 rightElbowShoulderVector = rightElbowVector - rightShoulderVector;
                    Vector3 leftElbowHandVector = leftElbowVector - leftHandVector;
                    Vector3 leftElbowShoulderVector = leftElbowVector - leftShoulderVector;
                    Vector3 rightKneeFootVector = rightKneeVector - rightFootVector;
                    Vector3 rightKneeHipVector = rightKneeVector - rightHipVector;
                    Vector3 leftKneeFootVector = leftKneeVector - leftFootVector;
                    Vector3 leftKneeHipVector = leftKneeVector - leftHipVector;

                    rightElbowAngle = AngleBetweenTwoVectors(Vector3.Normalize(rightElbowHandVector), Vector3.Normalize(rightElbowShoulderVector));
                    leftElbowAngle = AngleBetweenTwoVectors(Vector3.Normalize(leftElbowHandVector), Vector3.Normalize(leftElbowShoulderVector));
                    rightKneeAngle = AngleBetweenTwoVectors(Vector3.Normalize(rightKneeFootVector), Vector3.Normalize(rightKneeHipVector));
                    leftKneeAngle = AngleBetweenTwoVectors(Vector3.Normalize(leftKneeFootVector), Vector3.Normalize(leftKneeHipVector));

                    //check to see if the user is making a position to pause the game
                    if ((rightElbowAngle > 85 && rightElbowAngle < 95) && (leftElbowAngle > 85 && leftElbowAngle < 95))
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

        //added, checking to see what the angle is between two vectors using the dot product
        public float AngleBetweenTwoVectors(Vector3 vectorA, Vector3 vectorB)
        {
            float dotProduct = 0.0f;
            double vectorOneMag = 0.0f;
            double vectorTwoMag = 0.0f;
            float multVectorMag = 0.0f;
            double arcAngleBetweenVector = 0.0f;
            double actualAngle = 0.0f;
            float vectorOneX = vectorA.X;
            float vectorOneY = vectorA.Y;
            float vectorOneZ = vectorA.Z;
            float vectorTwoX = vectorB.X;
            float vectorTwoY = vectorB.Y;
            float vectorTwoZ = vectorB.Z;

            dotProduct = Vector3.Dot(vectorA, vectorB);
            vectorOneMag = Math.Abs(Math.Sqrt(Math.Pow((double)vectorOneX, 2.0) + Math.Pow((double)vectorOneY, 2.0) + Math.Pow((double)vectorOneZ, 2.0)));
            vectorTwoMag = Math.Abs(Math.Sqrt(Math.Pow((double)vectorTwoX, 2.0) + Math.Pow((double)vectorTwoY, 2.0) + Math.Pow((double)vectorTwoZ, 2.0)));
            multVectorMag = (float)vectorOneMag * (float)vectorTwoMag;
            arcAngleBetweenVector = (dotProduct / multVectorMag);
            actualAngle = Math.Acos((float)arcAngleBetweenVector);

            return ((float)actualAngle * (float)(180 / Math.PI));
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            myfont = content.Load<SpriteFont>("MyFont");
            SpriteTexture = content.Load<Texture2D>("Arrow");
            bigfont = content.Load<SpriteFont>("BigFont");
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
            int newX = 0;
            int newY = 0;

            //Move the two paddles up and down, hitting the edges of the screen and changing direction
            if (ourPaddleRect.Y > kGameHeight - ourPaddleRect.Height && dir == 1)
                dir = -1;
            if (ourPaddleRect.Y < 0 && dir == -1)
                dir = 1;
            if (dir == 1)
                ourPaddleRect.Y += PaddleVelocity;
            else if (dir == -1)
            {
                ourPaddleRect.Y -= PaddleVelocity;
            }

            if (ourSecPaddleRect.Y > kGameHeight - ourSecPaddleRect.Height && dirSec == 1)
                dirSec = -1;
            if (ourSecPaddleRect.Y < 0 && dirSec == -1)
                dirSec = 1;
            if (dirSec == 1)
                ourSecPaddleRect.Y += PaddleTwoVelocity;
            else if (dirSec == -1)
            {
                ourSecPaddleRect.Y -= PaddleTwoVelocity;
            }

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
                collision = BallCollision.LeftPaddle;
            }
            else if (ourPaddleRect.Intersects(enclosingRect))
            {
                velocity.X *= -1;
                collision = BallCollision.RightPaddle;
            }
            else if (ourSecPaddleRect.Intersects(enclosingRect))
            {
                velocity.X *= -1;
                collision = BallCollision.RightPaddle;
            }
            else if (enclosingRect.X >= (kGameWidth + 10))
            {
                collision = BallCollision.RightMiss;
                score2++;
            }
            else if (enclosingRect.X <= 0)
            {
                collision = BallCollision.LeftMiss;
                score1++;
            }

            if (collision == BallCollision.None && velocity.X > 0)
            {

                /*if (xVelocity < 0)
                    xVelocity = 0;
                */
                velocity.X = (float)(xVelocity);//scalingValue);
                velocity.Y = (float)(yVelocity);//scalingValue);
                                                
                newX = enclosingRect.X+(int)velocity.X;
                newY = enclosingRect.Y + (int)velocity.Y;
                
                if (newY >= kGameHeight - kBallHeight)
                {
                    velocity.Y *= -1;
                }
                else if (newY <= 0)
                {
                    velocity.Y *= -1;
                }
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
            /*int randBallStartXPos = kGameWidth/2;
            Random randomY = new Random();
            int randBallStartYPos = kGameHeight/2;
            */Random randomXVel = new Random();
            int randBallStartXVel = randomXVel.Next(0, 100);
            if (randBallStartXVel % 2 == 0)
                randBallStartXVel = 1;
            else
                randBallStartXVel = -1;
            /*if (this.difficulty == 0)
                ballVelocity = new Vector2((1.0f * randBallStartXVel), 1.0f);
            else if (this.difficulty == 1)
                ballVelocity = new Vector2((6.0f * randBallStartXVel), 6.0f);
            else if (this.difficulty == 2)
                ballVelocity = new Vector2((10.0f * randBallStartXVel), 10.0f);*/
            //ballVelocity = new Vector2((1.0f * randBallStartXVel), 1.0f);
            ballVelocity = new Vector2((1.0f * randBallStartXVel), 1.0f);
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
            double angle = Math.Sin(((float)ballVelocity.Y / (float)ballVelocity.X));
            RotationAngle = (float)angle;
            if (ballVelocity.X < 0)
                RotationAngle -= MathHelper.Pi;
            base.Update(gameTime);
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
            //screenpos.X = ballRect.X + ballRect.Width / 2;
            //screenpos.Y = ballRect.Y + ballRect.Height / 2;
            screenpos.X = kGameWidth / 2;
            screenpos.Y = kGameHeight / 2;
            origin.X = SpriteTexture.Width / 2;
            origin.Y = SpriteTexture.Height / 2;

            spriteBatch.Draw(dotTexture, ourPaddleRect, Microsoft.Xna.Framework.Color.White);
            spriteBatch.Draw(dotTexture, ourSecPaddleRect, Microsoft.Xna.Framework.Color.Black);
            spriteBatch.Draw(dotTexture, aiPaddleRect, Microsoft.Xna.Framework.Color.Navy);
            //Create Text :)
            int score_1 = score1;
            int score_2 = score2;
            int xPos = (ballRect.X - kGameWidth / 2) / YTicks.Length / 3;

            int yPos = -1 * (ballRect.Y - kGameHeight / 2) / YTicks.Length / 3;

            drawXTicks(Microsoft.Xna.Framework.Color.GreenYellow, spriteBatch);
            drawYTicks(Microsoft.Xna.Framework.Color.GreenYellow, spriteBatch);

            //Redrawing Ai Paddle Rect
            spriteBatch.Draw(dotTexture, aiPaddleRect, Microsoft.Xna.Framework.Color.Navy);
            spriteBatch.Draw(dotTexture, ourPaddleRect, Microsoft.Xna.Framework.Color.White);

            height = 0;
            for (int i = 0; i <= 9; i++)
            {
                x = kGameWidth / 2 - kLRMargin / 2 - 2;
                spriteBatch.DrawString(myfont, (10 - i).ToString(), new Vector2(x, height + 2), Microsoft.Xna.Framework.Color.Blue);
                height = height + skip;
            }
            height = height + skip;
            for (int i = -1; i > -10; i--)
            {
                x = kGameWidth / 2 - kLRMargin / 2 - 5;
                spriteBatch.DrawString(myfont, (i).ToString(), new Vector2(x, height + 2), Microsoft.Xna.Framework.Color.Blue);
                height = height + skip;
            }

            height = 0;

            skip = kGameWidth / 20;

            //This should be part of the DrawGridLines function
            height = 0;
            for (int i = 0; i <= 11; i++)
            {
                x = kGameHeight / 2 - kLRMargin / 2 - 5;
                spriteBatch.DrawString(myfont, (i).ToString(), new Vector2((kGameWidth / 2 + height) + 5, x), Microsoft.Xna.Framework.Color.Blue);
                height = height + skip;
            }
            height = skip;
            for (int i = -1; i > -10; i--)
            {
                x = kGameHeight / 2 - kLRMargin / 2 - 5;
                spriteBatch.DrawString(myfont, (i).ToString(), new Vector2(kGameWidth / 2 - height + 2, x), Microsoft.Xna.Framework.Color.Blue);
                height = height + skip;
            }


            spriteBatch.Draw(dotTexture, yAxis, Microsoft.Xna.Framework.Color.GreenYellow);
            spriteBatch.Draw(dotTexture, xAxis, Microsoft.Xna.Framework.Color.GreenYellow);
            spriteBatch.Draw(ballTexture, ballRect, Microsoft.Xna.Framework.Color.Brown);

            if (this.playerPos > 3.4)
                spriteBatch.DrawString(myfont, "Come Closer to the Kinect", new Vector2(kGameWidth / 2 - 150, kGameHeight / 3), Microsoft.Xna.Framework.Color.Red);

            if (this.playerPos < 2.0)
                spriteBatch.DrawString(myfont, "Back Away from the Kinect", new Vector2(kGameWidth / 2 - 150, kGameHeight / 3), Microsoft.Xna.Framework.Color.Red);

            spriteBatch.DrawString(myfont, "X-AXIS", new Vector2(kLRMargin, 400), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, "Y-AXIS", new Vector2(kGameWidth / 2 + 10, kGameHeight - 60), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, "Velocity Vector:", new Vector2(kGameWidth / 2 - 100, 0), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, ballVelocity.ToString(), new Vector2(kGameWidth / 2 + 100, 0), Microsoft.Xna.Framework.Color.Yellow);
            //spriteBatch.DrawString(bigfont, "X:     Y:", new Vector2(kGameWidth / 2 - 120, kGameHeight / 2 - 100), Microsoft.Xna.Framework.Color.Navy);
            //spriteBatch.DrawString(bigfont, xPos.ToString(), new Vector2(kGameWidth / 2 - 80, kGameHeight / 2 - 100), Microsoft.Xna.Framework.Color.Navy);
            //spriteBatch.DrawString(bigfont, yPos.ToString(), new Vector2(kGameWidth / 2 + 50, kGameHeight / 2 - 100), Microsoft.Xna.Framework.Color.Navy);
            spriteBatch.DrawString(myfont, "Computer Score:", new Vector2(kGameWidth - 300, 0), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, score_1.ToString(), new Vector2(kGameWidth - 100, 0), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, "Your Score:", new Vector2(100, 0), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, score2.ToString(), new Vector2(300, 0), Microsoft.Xna.Framework.Color.Yellow);

            //Draw X tick

            spriteBatch.Draw(dotTexture, leftHandTick, Microsoft.Xna.Framework.Color.Teal);
            spriteBatch.Draw(dotTexture, RightHandTick, Microsoft.Xna.Framework.Color.Teal);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawXTicks(Microsoft.Xna.Framework.Color color, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < YTicks.Length; i++)
            {
                spriteBatch.Draw(dotTexture, XTicks[i], color);
            }
        }
        private void drawYTicks(Microsoft.Xna.Framework.Color color, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < YTicks.Length; i++)
            {
                spriteBatch.Draw(dotTexture, YTicks[i], color);
            }
        }

    }
}
