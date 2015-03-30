using System;
using Microsoft.Xna.Framework;

namespace Dungeon_Tile_Engine
{
    //The camera will draw an area of the level, this will allow us to draw an area around the player 
    //and have the level bigger than that area
    //Camera is a static class, this means that objects of it can't be made but it can be refrenced by using Camera in the code 
    //as if the class was the object
    public static class Camera
    {
        #region Variables

        //All variables must be made static because the class is static
        private static Vector2 position = Vector2.Zero;     //The position of the camera, set to (0,0) by default
        private static Vector2 viewPortSize = Vector2.Zero; //The height and width of the camera, will be got from the game's viewport size
        private static Rectangle levelRectangle = new Rectangle(0,0,0,0); //The size of the level width and height measured from given x and y

        #endregion

        #region Properties

        public static Vector2 Position
        {
            get { return position; }
            set
            {
                //Set the position to be equal to the passed value but restrict it to within the levelRectangle if it is to big or small
                position = new Vector2(MathHelper.Clamp(value.X, levelRectangle.X, levelRectangle.Width - ViewPortWidth),
                    MathHelper.Clamp(value.Y, levelRectangle.Y, levelRectangle.Height - ViewPortHeight));
            }
        }

        public static Rectangle LevelRectangle
        {
            get { return levelRectangle; }
            set { levelRectangle = value; }
        }

        public static int ViewPortWidth
        {
            get { return (int)viewPortSize.X; }
            set { viewPortSize.X = value; }
        }

        public static int ViewPortHeight
        {
            get { return (int)viewPortSize.Y; }
            set { viewPortSize.Y = value; }
        }

        //Return the view port as a rectangle with its x, y, width and height
        public static Rectangle ViewPort
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, ViewPortWidth, ViewPortHeight);
            }
        }

        #endregion

        #region Methods

        //Move the camera by the given amount, a negative value move back on the x and up on the y
        public static void Move(Vector2 amount)
        {
            Position += amount;
        }

        //Check if the object is visible by seeing if its bounds are within the viewport
        public static bool ObjectIsVisible(Rectangle bounds)
        {
            return ViewPort.Intersects(bounds);
        }

        public static Vector2 LocationToScreen(Vector2 location)
        {
            return location - position;
        }

        public static Rectangle LocationToScreen(Rectangle locationRectangle)
        {
            return new Rectangle(
                locationRectangle.Left - (int)position.X,
                locationRectangle.Top - (int)position.Y,
                locationRectangle.Width,
                locationRectangle.Height);
        }

        public static Vector2 ScreenToLocation(Vector2 screen)
        {
            return screen + position;
        }

        public static Rectangle ScreenToLocation(Rectangle screenRectangle)
        {
            return new Rectangle(
                screenRectangle.Left + (int)position.X,
                screenRectangle.Top + (int)position.Y,
                screenRectangle.Width,
                screenRectangle.Height);
        }

        #endregion
    }
}
