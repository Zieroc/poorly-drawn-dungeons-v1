using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoorlyDrawnDungeons.Graphics
{
    [Serializable]
    public class Sprite : ISerializable
    {
        #region Variables

        private Texture2D texture;      //The image or sprite sheet being used
    
        private int height;             //The height of a single image
        private int width;              //The width of a single image
    
        private int columns;            //The number of columns in a sprite sheet
        private int rows;               //The number of rows in a sprite sheet
    
        private int states;              //The number of animations to loop through
        private int currentState;       //The current animation state
    
        private bool animate;           //Is the sprite animating through states
        private bool looping;           //Is the animation looping back to start when final state is reached

        private float timer;            //Current time
        private float interval;         //Interval between state changes

        private int direction;          //If we have multiple directions this code will handle it

        #endregion

        #region Constructors

        public Sprite(Texture2D texture, String filename)
            : this(texture, texture.Height, texture.Width, 1, false, false, 250f, filename)
        {
        }

        public Sprite(Texture2D texture, int height, int width, int state, String filename)
            : this(texture, height, width, state, false, false, 250f, filename)
        {
        }

        public Sprite(Texture2D texture, int height, int width, int state, bool animate, bool looping, String filename)
            : this(texture, height, width, state, animate, looping, 250f, filename)
        {
        }

        public Sprite(Texture2D texture, int height, int width, int state, bool animate, bool looping, float interval, String filename)
        {
            this.texture = texture;
            this.texture.Name = filename;
            this.height = height;
            this.width = width;
            this.states = state;
            this.animate = animate;
            this.looping = looping;

            //Calculate the number of rows and columns
            columns = texture.Width / width;
            rows = texture.Height / height;

            direction = 0;
            currentState = 0;
            timer = 0;
            this.interval = interval;
        }

        #endregion

        #region Properties

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        public int States
        {
            get { return states; }
        }

        public bool Animate
        {
            get { return animate; }
            set { animate = value; }
        }

        public bool Looping
        {
            get { return looping; }
            set { looping = value; }
        }

        public bool FinishedPlaying
        {
            get
            {
                if (looping == false)
                {
                    if (currentState == states - 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public int Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            if (animate && currentState < states)
            {
                //Increase the timer by the number of milliseconds since update was last called
                timer += (float)gameTime.ElapsedGameTime.Milliseconds;

                //Check the timer is more than the chosen interval
                if (timer > interval)
                {
                    //Show the next frame
                    currentState++;
                    //Reset the timer
                    timer = 0f;
                }
            }

            if (currentState == states)
            {
                if (looping)
                {
                    currentState = 0;   //Animation should loop so set currentState back to 0
                }
                else
                {
                    currentState--;     //Animation has reached end so drop back one so the image can be drawn
                }
            }
        }

        #endregion

        #region Draw

        //Draw with no effects at given position and depth
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float depth)
        {
            int imgX = width * ((currentState + (states * direction)) % columns);
            int imgY = height * ((currentState + (states * direction)) / columns);

            Rectangle sourceRect = new Rectangle(imgX, imgY, width, height);

            spriteBatch.Draw(texture, position, sourceRect, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, depth);
        }

        //Draw at given position and depth and a given colour - will be used to change sprite transperancy
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colour, float depth)
        {
            int imgX = width * ((currentState + (states * direction)) % columns);
            int imgY = height * ((currentState + (states * direction)) / columns);

            Rectangle sourceRect = new Rectangle(imgX, imgY, width, height);

            spriteBatch.Draw(texture, position, sourceRect, colour, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, depth);
        }

        #endregion

        #region Serializable Methods

        public Sprite(SerializationInfo info, StreamingContext ctxt)
        {
            this.texture = GlobalVar.content.Load<Texture2D>((String)info.GetValue("Texture", typeof(String)));
            this.height = (int)info.GetValue("Height", typeof(int));
            this.width = (int)info.GetValue("Width", typeof(int));
            this.columns = (int)info.GetValue("Columns", typeof(int));
            this.rows = (int)info.GetValue("Rows", typeof(int));
            this.states = (int)info.GetValue("States", typeof(int));
            this.currentState = (int)info.GetValue("CurrentState", typeof(int));
            this.animate = (bool)info.GetValue("Animate", typeof(bool));
            this.looping = (bool)info.GetValue("Looping", typeof(bool));
            this.timer = (float)info.GetValue("Timer", typeof(float));
            this.interval = (float)info.GetValue("Interval", typeof(float));
            this.direction = (int)info.GetValue("Direction", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Texture", this.texture.Name);
            info.AddValue("Height", this.height);
            info.AddValue("Width", this.width);
            info.AddValue("Columns", this.columns);
            info.AddValue("Rows", this.rows);
            info.AddValue("States", this.states);
            info.AddValue("CurrentState", this.currentState);
            info.AddValue("Animate", this.animate);
            info.AddValue("Looping", this.looping);
            info.AddValue("Timer", this.timer);
            info.AddValue("Interval", this.interval);
            info.AddValue("Direction", this.direction);
        }

        #endregion
    }
}
