//using System includes
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
using System.Collections.Generic;
using System.Windows.Navigation;
using System.Windows.Shapes;

//using Microsoft includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net; //not too sure what this is for
using Microsoft.Xna.Framework.Storage; //not too sure what this is for
using Microsoft.Research.Kinect.Nui;

//using Coding4Fun
using Coding4Fun.Kinect.WinForm;


namespace PongRecent
{
    class Level2 : Microsoft.Xna.Framework.Game
    {
        //variables
        //Viewport
        Viewport viewportGameplay;

        //Texture variables
        //Texture for the depth video used by the kinect
        Texture2D kinectDepthVideo;
        Texture2D SpriteTexture;
        //Texture used for many different things
        Texture2D dotTexture = null, ballTexture = null;
        //Skeleton textures
        Texture2D headTexture = null;

        //State variables
        FormWindowState WindowState;
        KeyboardState keyboardState;

        //Graphics variable
        GraphicsDeviceManager graphics;

        //spriteBatch/Font, used to put things to the screen
        SpriteBatch spriteBatch;
        SpriteFont myfont;
        SpriteFont bigfont;

        //Constant variables
        const int kLRMargin = 30, kPaddleWidth = 26, kPaddleHeight = 120;
        const int kBallWidth = 24, kBallHeight = 24;
        const int PaddleVelocity = 5;
        const int PaddleTwoVelocity = 3;
        const int kGameWidth = 1280, kGameHeight = 1024;
        const int NumberOfTicksAlongX = 40;
        const int NumberOfTicksAlongY = 20;
        //Non-constant variables
        int score1 = 0;
        int score2 = 0;
        int dir = 1;
        int dirSec = 1;
        int number = 2;
        public int difficulty = 0;
        public bool positionWorked = false;
        public bool quadrantOne = false;
        public bool quadrantTwo = false;
        public bool quadrantThree = false;
        public bool quadrantFour = false;
        public bool userWantsToResume = false;
        public bool userWantsToQuit = false;
        public bool handedness = false;
        bool passedCenter = false;
        public float playerPos = 0;
        public float rightElbowAngle;
        public float leftElbowAngle;
        public float rightKneeAngle;
        public float leftKneeAngle;
        float RotationAngle;

        /// <summary>
        /// Rectangle variables
        /// These are used to test for collisions and to draw things to the screen. There isn't a way to draw just a line so everything that looks like a line is drawn with a rectangle
        /// </summary>
        //Paddle rectangles
        Microsoft.Xna.Framework.Rectangle ourPaddleRect = new Microsoft.Xna.Framework.Rectangle(kLRMargin, 0, kPaddleWidth, kPaddleHeight);
        Microsoft.Xna.Framework.Rectangle aiPaddleRect = new Microsoft.Xna.Framework.Rectangle(kGameWidth - kLRMargin - kPaddleWidth, 0, kPaddleWidth, kPaddleHeight);
        //Line rectangles, these are used to draw the x and y axis and to draw the number lines on them.
        Microsoft.Xna.Framework.Rectangle numberLine = new Microsoft.Xna.Framework.Rectangle(kGameWidth - kLRMargin / 2, 0, kLRMargin / 5 - 2, kGameHeight);
        Microsoft.Xna.Framework.Rectangle xNumberLine = new Microsoft.Xna.Framework.Rectangle(0, kGameHeight - kLRMargin / 2, kGameWidth, kLRMargin / 5 - 2);
        Microsoft.Xna.Framework.Rectangle yAxis = new Microsoft.Xna.Framework.Rectangle(kGameWidth / 2, 0, kLRMargin / 5 - 2, kGameHeight);
        Microsoft.Xna.Framework.Rectangle xAxis = new Microsoft.Xna.Framework.Rectangle(0, kGameHeight / 2, kGameWidth, kLRMargin / 5 - 2);
        Microsoft.Xna.Framework.Rectangle lineTick = new Microsoft.Xna.Framework.Rectangle(kGameWidth - kLRMargin + 2, 0, kLRMargin, kLRMargin / 6);
        Microsoft.Xna.Framework.Rectangle xLineTick = new Microsoft.Xna.Framework.Rectangle(0, kGameHeight - kLRMargin, kLRMargin / 6, kLRMargin);
        Microsoft.Xna.Framework.Rectangle[] XTicks = new Microsoft.Xna.Framework.Rectangle[NumberOfTicksAlongX];
        Microsoft.Xna.Framework.Rectangle[] YTicks = new Microsoft.Xna.Framework.Rectangle[NumberOfTicksAlongY];
        //Ball rectangle, keeps track of where the ball is in the game.
        Microsoft.Xna.Framework.Rectangle ballRect;
        //Skeleton rectangles, allows the joints to be drawn to the screen in the correct place with respect to the player.
        Microsoft.Xna.Framework.Rectangle headRectangle;
        Microsoft.Xna.Framework.Rectangle rightHandRectangle;
        Microsoft.Xna.Framework.Rectangle leftHandRectangle;
        Microsoft.Xna.Framework.Rectangle rightElbowRectangle;
        Microsoft.Xna.Framework.Rectangle leftElbowRectangle;
        Microsoft.Xna.Framework.Rectangle rightShoulderRectangle;
        Microsoft.Xna.Framework.Rectangle leftShoulderRectangle;
        Microsoft.Xna.Framework.Rectangle centerShoulderRectangle;
        Microsoft.Xna.Framework.Rectangle spineRectangle;
        Microsoft.Xna.Framework.Rectangle rightHipRectangle;
        Microsoft.Xna.Framework.Rectangle leftHipRectangle;
        Microsoft.Xna.Framework.Rectangle rightKneeRectangle;
        Microsoft.Xna.Framework.Rectangle leftKneeRectangle;
        Microsoft.Xna.Framework.Rectangle rightFootRectangle;
        Microsoft.Xna.Framework.Rectangle leftFootRectangle;

        //Vector2 variables
        //skeleton vectors
        Vector2 handRightVector;
        Vector2 handLeftVector;
        Vector2 elbowRightVector;
        Vector2 elbowLeftVector;
        Vector2 shoulderRightVector;
        Vector2 shoulderLeftVector;
        Vector2 vectorBetweenRHandElbow; 
        //ballVelocity vector
        Vector2 ballVelocity;

        //Runtime variable
        Runtime runtime = new Runtime();
        

        /// <summary>
        /// Class constructor. This allows what the player chose from the options menu to be implemented into the game.
        /// </summary>
        public Level2(bool positionWorked, bool hand, int difficulty)
        {
            this.positionWorked = positionWorked;
            this.handedness = hand;
            this.difficulty = difficulty;
            Initialize();
        }

        /// <summary>
        /// Class initialize function. This initializes the window and allows for skeletal tracking.
        /// </summary>
        protected override void Initialize()
        {

            RestartGame();

            this.WindowState = FormWindowState.Maximized;
            runtime.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseDepth);
            runtime.SkeletonEngine.TransformSmooth = true;

            //Used to make skeletal tracking better.
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
            viewportGameplay.Width = kGameWidth;
            viewportGameplay.Height = kGameHeight;

            //Make the game full screen
            //this.graphics.IsFullScreen = true;
            //this.graphics.ApplyChanges();
            base.Initialize();

            InitializeTicks();

        }

        /// <summary>
        /// Creates the tick lines on the x and y axis.
        /// </summary>
        void InitializeTicks()
        {
            //y axis
            int yPos = 0;
            int tickThickness = kLRMargin / 10;
            for (int i = 0; i < YTicks.Length; i++)
            {
                //Center the Ticks
                YTicks[i].X = kGameWidth / 2 - kLRMargin / 2;
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
                XTicks[i].Y = kGameHeight / 2 - kLRMargin / 2;
                XTicks[i].Width = tickThickness;
                XTicks[i].Height = kLRMargin;
                xPos += kGameWidth / YTicks.Length;
            }
        }


        void runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //Get Joint Information
            foreach (SkeletonData data in e.SkeletonFrame.Skeletons)
            {
                if (data.TrackingState == SkeletonTrackingState.Tracked)
                {
                    //variables
                    //Assign the joints

                    //Right side
                    Joint joint = data.Joints[JointID.HandRight];
                    Joint elbowRight = data.Joints[JointID.ElbowRight];
                    Joint shoulderRight = data.Joints[JointID.ShoulderRight];
                    Joint kneeRight = data.Joints[JointID.KneeRight];
                    Joint hipRight = data.Joints[JointID.HipRight];
                    Joint footRight = data.Joints[JointID.FootRight];
                    Joint ankleRight = data.Joints[JointID.AnkleRight];

                    //Left side
                    Joint jointLeft = data.Joints[JointID.HandLeft];
                    Joint elbowLeft = data.Joints[JointID.ElbowLeft];
                    Joint shoulderLeft = data.Joints[JointID.ShoulderLeft];
                    Joint kneeLeft = data.Joints[JointID.KneeLeft];
                    Joint hipLeft = data.Joints[JointID.HipLeft];
                    Joint footLeft = data.Joints[JointID.FootLeft];
                    Joint ankleLeft = data.Joints[JointID.AnkleLeft];

                    //Neither side
                    Joint head = data.Joints[JointID.Head];
                    Joint shoulderMid = data.Joints[JointID.ShoulderCenter];
                    Joint spine = data.Joints[JointID.Spine];

                    //Vector3 variables
                    //Getting the position of each joint and putting them into a vector3
                    Vector3 rightElbowVector = new Vector3(elbowRight.Position.X, elbowRight.Position.Y, elbowRight.Position.Z);
                    Vector3 rightShoulderVector = new Vector3(shoulderRight.Position.X, shoulderRight.Position.Y, shoulderRight.Position.Z);
                    Vector3 rightHandVector = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
                    Vector3 leftElbowVector = new Vector3(elbowLeft.Position.X, elbowLeft.Position.Y, elbowLeft.Position.Z);
                    Vector3 leftShoulderVector = new Vector3(shoulderLeft.Position.X, shoulderLeft.Position.Y, shoulderLeft.Position.Z);
                    Vector3 leftHandVector = new Vector3(jointLeft.Position.X, jointLeft.Position.Y, jointLeft.Position.Z);
                    Vector3 rightKneeVector = new Vector3(kneeRight.Position.X, kneeRight.Position.Y, kneeRight.Position.Z);
                    Vector3 rightHipVector = new Vector3(hipRight.Position.X, hipRight.Position.Y, hipRight.Position.Z);
                    Vector3 rightAnkleVector = new Vector3(ankleRight.Position.X, ankleRight.Position.Y, ankleRight.Position.Z);
                    Vector3 leftKneeVector = new Vector3(kneeLeft.Position.X, kneeLeft.Position.Y, kneeLeft.Position.Z);
                    Vector3 leftHipVector = new Vector3(hipLeft.Position.X, hipLeft.Position.Y, hipLeft.Position.Z);
                    Vector3 leftAnkleVector = new Vector3(ankleLeft.Position.X, ankleLeft.Position.Y, ankleLeft.Position.Z);

                    //Getting the vector between two joints 
                    Vector3 rightElbowHandVector = rightElbowVector - rightHandVector;
                    Vector3 rightElbowShoulderVector = rightElbowVector - rightShoulderVector;
                    Vector3 leftElbowHandVector = leftElbowVector - leftHandVector;
                    Vector3 leftElbowShoulderVector = leftElbowVector - leftShoulderVector;
                    Vector3 rightKneeAnkleVector = rightKneeVector - rightAnkleVector;
                    Vector3 rightKneeHipVector = rightKneeVector - rightHipVector;
                    Vector3 leftKneeAnkleVector = leftKneeVector - leftAnkleVector;
                    Vector3 leftKneeHipVector = leftKneeVector - leftHipVector;

                    //Skeleton rectangles
                    //Scaling each position so it can be put back to the screen accordingly.
                    int headX = (int)head.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int headY = (int)head.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int rightHandX = (int)joint.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int rightHandY = (int)joint.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int myLeftHandX = (int)jointLeft.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int myLeftHandY = (int)jointLeft.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int rightElbowX = (int)elbowRight.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int rightElbowY = (int)elbowRight.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int myLeftElbowX = (int)elbowLeft.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int myLeftElbowY = (int)elbowLeft.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int rightShoulderX = (int)shoulderRight.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int rightShoulderY = (int)shoulderRight.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int myLeftShoulderX = (int)shoulderLeft.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int myLeftShoulderY = (int)shoulderLeft.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int centerShoulderX = (int)shoulderMid.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int centerShoulderY = (int)shoulderMid.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int spineX = (int)spine.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int spineY = (int)spine.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int rightHipX = (int)hipRight.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int rightHipY = (int)hipRight.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int myLeftHipX = (int)hipLeft.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int myLeftHipY = (int)hipLeft.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int rightKneeX = (int)kneeRight.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int rightKneeY = (int)kneeRight.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int myLeftKneeX = (int)kneeLeft.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int myLeftKneeY = (int)kneeLeft.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int rightFootX = (int)footRight.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int rightFootY = (int)footRight.ScaleTo(kGameWidth, kGameHeight).Position.Y;
                    int myLeftFootX = (int)footLeft.ScaleTo(kGameWidth, kGameHeight).Position.X;
                    int myLeftFootY = (int)footLeft.ScaleTo(kGameWidth, kGameHeight).Position.Y;

                    //Setting the actual rectangle of the joint using the scaled variables above. This will be used to print the joints to the screen.
                    headRectangle = new Microsoft.Xna.Framework.Rectangle(headX, headY, 75, 75);
                    rightHandRectangle = new Microsoft.Xna.Framework.Rectangle(rightHandX, rightHandY, 30, 30);
                    leftHandRectangle = new Microsoft.Xna.Framework.Rectangle(myLeftHandX, myLeftHandY, 30, 30);
                    rightElbowRectangle = new Microsoft.Xna.Framework.Rectangle(rightElbowX, rightElbowY, 30, 30);
                    leftElbowRectangle = new Microsoft.Xna.Framework.Rectangle(myLeftElbowX, myLeftElbowY, 30, 30);
                    rightShoulderRectangle = new Microsoft.Xna.Framework.Rectangle(rightShoulderX, rightShoulderY, 30, 30);
                    leftShoulderRectangle = new Microsoft.Xna.Framework.Rectangle(myLeftShoulderX, myLeftShoulderY, 30, 30);
                    centerShoulderRectangle = new Microsoft.Xna.Framework.Rectangle(centerShoulderX, centerShoulderY, 30, 30);
                    rightHipRectangle = new Microsoft.Xna.Framework.Rectangle(rightHipX, rightHipY, 30, 30);
                    leftHipRectangle = new Microsoft.Xna.Framework.Rectangle(myLeftHipX, myLeftHipY, 30, 30);
                    rightKneeRectangle = new Microsoft.Xna.Framework.Rectangle(rightKneeX, rightKneeY, 30, 30);
                    leftKneeRectangle = new Microsoft.Xna.Framework.Rectangle(myLeftKneeX, myLeftKneeY, 30, 30);
                    rightFootRectangle = new Microsoft.Xna.Framework.Rectangle(rightFootX, rightFootY, 30, 30);
                    leftFootRectangle = new Microsoft.Xna.Framework.Rectangle(myLeftFootX, myLeftFootY, 30, 30);

                    //Skeleton vectors
                    //handRightVector = new Vector2(joint.Position.X, joint.Position.Y);
                    //handLeftVector = new Vector2(jointLeft.Position.X, jointLeft.Position.Y);
                    //elbowRightVector = new Vector2(elbowRight.Position.X, elbowRight.Position.Y);
                    //elbowLeftVector = new Vector2(elbowLeft.Position.X, elbowLeft.Position.Y);
                    //shoulderRightVector = new Vector2(shoulderRight.Position.X, shoulderRight.Position.Y);
                    //shoulderLeftVector = new Vector2(shoulderLeft.Position.X, shoulderLeft.Position.Y);

                    //Check to see what the users angle is at their elbow
                    rightElbowAngle = AngleBetweenTwoVectors(Vector3.Normalize(rightElbowHandVector), Vector3.Normalize(rightElbowShoulderVector));
                    leftElbowAngle = AngleBetweenTwoVectors(Vector3.Normalize(leftElbowHandVector), Vector3.Normalize(leftElbowShoulderVector));
                    //Check to see what the users angle is at their knee
                    rightKneeAngle = AngleBetweenTwoVectors(Vector3.Normalize(rightKneeAnkleVector), Vector3.Normalize(rightKneeHipVector));
                    leftKneeAngle = AngleBetweenTwoVectors(Vector3.Normalize(leftKneeAnkleVector), Vector3.Normalize(leftKneeHipVector));

                    //Checking which hand the user wants to use.
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

                    //Assigning the players position
                    this.playerPos = hipRight.Position.Z;

                    //Check the players position
                    //Check if they want to quit
                    QuitGame(joint, elbowRight, jointLeft, elbowLeft);

                    ////check to see if the user is making a position to quit the game
                    if ((leftKneeAngle > 95 && leftKneeAngle < 115) && (rightKneeAngle > 95 && rightKneeAngle < 115) && (joint.Position.Y > head.Position.Y) && (jointLeft.Position.Y > head.Position.Y))
                    {
                        userWantsToQuit = true;
                    }
                    else
                        userWantsToQuit = false;


                    //quadrantOne
                    if ((joint.Position.Y > shoulderRight.Position.Y) && (jointLeft.Position.Y > head.Position.Y))
                    {
                        this.quadrantOne = true;
                    }
                    else
                        this.quadrantOne = false;
                    //quadrantTwo
                    if ((jointLeft.Position.Y > shoulderLeft.Position.Y) && (joint.Position.Y > head.Position.Y))
                    {
                        this.quadrantTwo = true;
                    }
                    else
                        this.quadrantTwo = false;
                    //quadrantThree
                    if ((jointLeft.Position.Y > shoulderLeft.Position.Y) && (jointLeft.Position.Y < head.Position.Y) && (joint.Position.Y < hipRight.Position.Y))
                    {
                        this.quadrantThree = true;
                    }
                    else
                        this.quadrantThree = false;
                    //quadrantFour
                    if ((joint.Position.Y > shoulderRight.Position.Y) && (jointLeft.Position.Y < hipRight.Position.Y))
                    {
                        this.quadrantFour = true;
                    }
                    else
                        this.quadrantFour = false;
                }
            }
        }

        public void QuitGame(Joint joint, Joint elbowRight, Joint jointLeft, Joint elbowLeft)
        {
            if ((rightElbowAngle > 85 && rightElbowAngle < 95) && (joint.Position.Y < elbowRight.Position.Y) && (leftElbowAngle > 85 && leftElbowAngle < 95) && (jointLeft.Position.Y < elbowLeft.Position.Y))
            {
                //User wants to quit the game.
                this.positionWorked = true;
            }
            else
                this.positionWorked = false;
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
            headTexture = content.Load<Texture2D>("SushilFace");
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
                ballVelocity = new Vector2((1.0f * randBallStartXVel), 1.0f);
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

            //spriteBatch.Draw(dotTexture, ourPaddleRect, Microsoft.Xna.Framework.Color.White);
            //spriteBatch.Draw(dotTexture, ourSecPaddleRect, Microsoft.Xna.Framework.Color.Black);
            //spriteBatch.Draw(dotTexture, aiPaddleRect, Microsoft.Xna.Framework.Color.Navy);
            //Create Text :)
            int score_1 = score1;
            int score_2 = score2;
            int xPos = (ballRect.X - kGameWidth / 2) / YTicks.Length / 3;

            int yPos = -1 * (ballRect.Y - kGameHeight / 2) / YTicks.Length / 3;

            ourPaddleRect.Y = ballRect.Y - kPaddleHeight / 2;

            //draw the joints
            spriteBatch.Draw(headTexture, headRectangle, Microsoft.Xna.Framework.Color.Beige);
            spriteBatch.Draw(ballTexture, rightHandRectangle, Microsoft.Xna.Framework.Color.DarkOrange);
            spriteBatch.Draw(ballTexture, leftHandRectangle, Microsoft.Xna.Framework.Color.DarkOrange);
            spriteBatch.Draw(ballTexture, rightElbowRectangle, Microsoft.Xna.Framework.Color.DarkOrange);
            spriteBatch.Draw(ballTexture, leftElbowRectangle, Microsoft.Xna.Framework.Color.DarkOrange);
            spriteBatch.Draw(ballTexture, rightShoulderRectangle, Microsoft.Xna.Framework.Color.DarkOrange);
            spriteBatch.Draw(ballTexture, leftShoulderRectangle, Microsoft.Xna.Framework.Color.DarkOrange);
            spriteBatch.Draw(ballTexture, centerShoulderRectangle, Microsoft.Xna.Framework.Color.DarkOrange);
            spriteBatch.Draw(ballTexture, rightHipRectangle, Microsoft.Xna.Framework.Color.DarkOrange);
            spriteBatch.Draw(ballTexture, leftHipRectangle, Microsoft.Xna.Framework.Color.DarkOrange);
            spriteBatch.Draw(ballTexture, rightKneeRectangle, Microsoft.Xna.Framework.Color.DarkOrange);
            spriteBatch.Draw(ballTexture, leftKneeRectangle, Microsoft.Xna.Framework.Color.DarkOrange);
            spriteBatch.Draw(ballTexture, rightFootRectangle, Microsoft.Xna.Framework.Color.DarkOrange);
            spriteBatch.Draw(ballTexture, leftFootRectangle, Microsoft.Xna.Framework.Color.DarkOrange);

            if (quadrantOne)
            {
                spriteBatch.DrawString(myfont, "Quadrant 1!!! ", new Vector2(kGameWidth - kGameWidth/4, kGameHeight - 3*(kGameHeight/4)), Microsoft.Xna.Framework.Color.Gainsboro);
                if (xPos >= 0 && yPos >= 0)
                {
                    spriteBatch.Draw(ballTexture, ballRect, Microsoft.Xna.Framework.Color.Brown);
                    if (ballVelocity.X > 0)
                        aiPaddleRect.Y = ballRect.Y;
                }
            }
            else if (quadrantTwo)
            {
                spriteBatch.DrawString(myfont, "Quadrant 2!!! ", new Vector2(kGameWidth - 3*(kGameWidth / 4), kGameHeight - 3 * (kGameHeight / 4)), Microsoft.Xna.Framework.Color.Gainsboro);
                if (xPos <= 0 && yPos >= 0)
                {
                    spriteBatch.Draw(ballTexture, ballRect, Microsoft.Xna.Framework.Color.Brown);
                    if (ballVelocity.X < 0)
                        ourPaddleRect.Y = -kPaddleHeight;
                }
            }
            else if (quadrantThree)
            {
                spriteBatch.DrawString(myfont, "Quadrant 3!!! ", new Vector2(kGameWidth - 3 * (kGameWidth / 4), kGameHeight - (kGameHeight / 4)), Microsoft.Xna.Framework.Color.Gainsboro);
                if (xPos <= 0 && yPos <= 0)
                {
                    spriteBatch.Draw(ballTexture, ballRect, Microsoft.Xna.Framework.Color.Brown);
                    if (ballVelocity.X < 0)
                        ourPaddleRect.Y = -kPaddleHeight;
                }
            }
            else if (quadrantFour)
            {
                spriteBatch.DrawString(myfont, "Quadrant 4!!! ", new Vector2(kGameWidth - (kGameWidth / 4), kGameHeight - (kGameHeight / 4)), Microsoft.Xna.Framework.Color.Gainsboro);
                if(xPos>=0 && yPos<=0)
                {
                    spriteBatch.Draw(ballTexture, ballRect, Microsoft.Xna.Framework.Color.Brown);
                    if (ballVelocity.X > 0)
                        aiPaddleRect.Y = ballRect.Y;
                }
            }
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


            if (this.playerPos > 3.4)
                spriteBatch.DrawString(myfont, "Come Closer to the Kinect", new Vector2(kGameWidth / 2 - 150, kGameHeight / 3), Microsoft.Xna.Framework.Color.Red);

            if (this.playerPos < 2.0)
                spriteBatch.DrawString(myfont, "Back Away from the Kinect", new Vector2(kGameWidth / 2 - 150, kGameHeight / 3), Microsoft.Xna.Framework.Color.Red);

            spriteBatch.DrawString(myfont, "X-AXIS", new Vector2(kLRMargin, 400), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, "Y-AXIS", new Vector2(kGameWidth / 2 + 10, kGameHeight - 60), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, "Velocity Vector:", new Vector2(kGameWidth / 2 - 100, 0), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, ballVelocity.ToString(), new Vector2(kGameWidth / 2 + 100, 0), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(bigfont, "X:     Y:", new Vector2(kGameWidth / 2-120, kGameHeight/2-100), Microsoft.Xna.Framework.Color.Navy);
            spriteBatch.DrawString(bigfont, xPos.ToString(), new Vector2(kGameWidth / 2-80, kGameHeight/2-100), Microsoft.Xna.Framework.Color.Navy);
            spriteBatch.DrawString(bigfont, yPos.ToString(), new Vector2(kGameWidth / 2+50, kGameHeight/2-100), Microsoft.Xna.Framework.Color.Navy);
            spriteBatch.DrawString(myfont, "Computer Score:", new Vector2(100, 0), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, score_1.ToString(), new Vector2(300, 0), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, "Your Score:", new Vector2(kGameWidth - 300, 0), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, score2.ToString(), new Vector2(kGameWidth - 100, 0), Microsoft.Xna.Framework.Color.Yellow);

            //draw the angle between the elbow and the shoulder
            spriteBatch.DrawString(myfont, rightElbowAngle.ToString(), new Vector2(kGameWidth - 100, 200), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, leftElbowAngle.ToString(), new Vector2(kGameWidth - 100, 300), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, rightKneeAngle.ToString(), new Vector2(kGameWidth - 100, 400), Microsoft.Xna.Framework.Color.Yellow);
            spriteBatch.DrawString(myfont, leftKneeAngle.ToString(), new Vector2(kGameWidth - 100, 500), Microsoft.Xna.Framework.Color.Yellow);

            //end of new stuff

            //Draw Arrow
            spriteBatch.Draw(SpriteTexture, screenpos, null, Microsoft.Xna.Framework.Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
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
