using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Tile_Engine
{
    //Only one level will be displayed at a time so only a single map object is needed so the class can be made static
    public static class DungeonMap
    {
        #region Variables

        //Constants
        public const int TileWidth = 32;
        public const int TileHeight = 32;
        public const int MapWidth = 100;    //This can be changed in the code to match the size of we agree upon for levels
        public const int MapHeight = 100;    //This can be changed in the code to match the size of we agree upon for levels
        private const int defaultTile = 0;  //This is the tile that will be used by default

        //Variables
        //static private Tile[,] mapCells = new Tile[MapWidth, MapHeight];
        private static List<Tile[]> mapCells = new List<Tile[]>();
        public static bool editMode = false; //Set to true for testing purposes while designing levels will allow a tiles speciality code to be visible

        public static SpriteFont spriteFont;
        private static Texture2D tileSheet; //The tiles used for drawing the map, set to private as it should not be manipulated outside of the map class

        #endregion

        #region Initialisation

        //initalise the variables and set tile sheet
        public static void Initialise(Texture2D tileTexture)
        {
            tileSheet = tileTexture;

            //By default set the entire map to the default tile, all passable and not codes
            for (int i = 0; i < MapWidth; i++)
            {
                mapCells.Add(new Tile[MapHeight]);
                for (int j = 0; j < MapHeight; j++)
                {
                        mapCells[i][j] = new Tile(defaultTile, "", true);
                }
            }
        }

        #endregion

        #region Tile and Tile Sheet

        //Tile and Tile Sheet Methods
        public static int TilesPerRow
        {
            //Divide the width of the tile sheet by the width of a tile to calculate how many are in each row
            get { return tileSheet.Width / TileWidth; }
        }

        //Work out where a tile is based on the number, numbers work across the sheet before dropping to the next line
        public static Rectangle TileSourceRectangle(int tileIndex)
        {
            //This is the same method used to find the correct image on a sprite sheet from the sprite class
            return new Rectangle(
                (tileIndex % TilesPerRow) * TileWidth,
                (tileIndex / TilesPerRow) * TileHeight,
                TileWidth, TileHeight);
        }

        #endregion

        #region Map Cells

        public static int GetCellByPointX(int x)
        {
            return x / TileWidth;
        }

        public static int GetCellByPointY(int y)
        {
            return y / TileHeight;
        }

        public static Vector2 GetCellByPoint(Vector2 point)
        {
            return new Vector2(
                GetCellByPointX((int)point.X),
                GetCellByPointY((int)point.Y));
        }

        public static Vector2 GetCellCenter(int x, int y)
        {
            return new Vector2(
                (x * TileWidth) + (TileWidth / 2),
                (y * TileHeight) + (TileHeight / 2));
        }

        public static Vector2 GetCellCenter(Vector2 cell)
        {
            return GetCellCenter((int)cell.X, (int)cell.Y);
        }

        public static Rectangle CellWorldRectangle(int cellX, int cellY)
        {
            return new Rectangle(
            cellX * TileWidth, cellY * TileHeight,
            TileWidth, TileHeight);
        }
        public static Rectangle CellWorldRectangle(Vector2 cell)
        {
            return CellWorldRectangle((int)cell.X, (int)cell.Y);
        }

        public static Rectangle CellScreenRectangle(int x, int y)
        {
            return Camera.LocationToScreen(CellWorldRectangle(x, y));
        }

        public static Rectangle CellScreenRectangle(Vector2 cell)
        {
            return CellScreenRectangle((int)cell.X, (int)cell.Y);
        }

        public static bool CellIsPassable(int x, int y)
        {
            Tile tile = GetTileAtCell(x, y);

            if (tile == null)
                return false;
            else
                return tile.Passable;
        }

        public static bool CellIsPassable(Vector2 cell)
        {
            return CellIsPassable((int)cell.X, (int)cell.Y);
        }

        public static bool CellIsPassableByPoint(Vector2 point)
        {
            return CellIsPassable(
            GetCellByPointX((int)point.X),
            GetCellByPointY((int)point.Y));
        }

        public static string CellCode(int x, int y)
        {
            Tile tile = GetTileAtCell(x, y);

            if (tile == null)
                return "";
            else
                return tile.Code;
        }

        public static string CellCode(Vector2 cell)
        {
            return CellCode((int)cell.X, (int)cell.Y);
        }

        #endregion

        #region Get/Set Tiles

        public static Tile GetTileAtCell(int x, int y)
        {
            if ((x >= 0) && (x < MapWidth) &&
            (y >= 0) && (y < MapHeight))
            {
                return mapCells[x][y];
            }
            else
            {
                return null;
            }
        }

        //Set entire tile
        public static void SetTileAtCell(int x, int y, Tile tile)
        {
            if ((x >= 0) && (x < MapWidth) &&
            (y >= 0) && (y < MapHeight))
            {
                mapCells[x][y] = tile;
            }
        }

        //Set just the image of the tile
        public static void SetTileAtCell(int x, int y, int tileIndex)
        {
            if ((x >= 0) && (x < MapWidth) && (y >= 0) && (y < MapHeight))
            {
                mapCells[x][y].ImageIndex = tileIndex;
            }
        }

        static public Tile GetTileAtPoint(int x, int y)
        {
            return GetTileAtCell( GetCellByPointX(x), GetCellByPointY(y));
        }

        static public Tile GetTileAtPoint(Vector2 point)
        {
            return GetTileAtPoint( (int)point.X, (int)point.Y);
        }

        #endregion

        #region Draw

        //Draw the map based on what can be seen by the camera
        public static void Draw(SpriteBatch spriteBatch)
        {
            int startX = GetCellByPointX((int)Camera.Position.X);
            int endX = GetCellByPointX((int)Camera.Position.X + Camera.ViewPortWidth);

            int startY = GetCellByPointY((int)Camera.Position.Y);
            int endY = GetCellByPointY((int)Camera.Position.Y + Camera.ViewPortHeight);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                        if ((x >= 0) && (y >= 0) && (x < MapWidth) && (y < MapHeight))
                        {
                            spriteBatch.Draw(tileSheet, CellScreenRectangle(x, y), TileSourceRectangle(mapCells[x][y].ImageIndex),
                                       Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);
                        }

                        if (editMode)
                        {
                            DrawEditModeItems(spriteBatch, x, y);
                        }
                }
            }
        }

        public static void DrawEditModeItems(SpriteBatch spriteBatch, int x, int y)
        {
            if ((x < 0) || (x >= MapWidth) || (y < 0) || (y >= MapHeight))
            {
                return;
            }
            if (!CellIsPassable(x, y))
            {
                spriteBatch.Draw(tileSheet, CellScreenRectangle(x, y), TileSourceRectangle(1), new Color(255, 0, 0, 80),
                    0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            }
            if (mapCells[x][y].Code != "")
            {
                Rectangle screenRect = CellScreenRectangle(x, y);
                spriteBatch.DrawString(spriteFont, mapCells[x][y].Code, new Vector2(screenRect.X, screenRect.Y), Color.White,
                    0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            }
        }

        #endregion

        #region Saving & Loading

        public static void SaveMap(Stream fileStream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Tile[]>));
            serializer.Serialize(fileStream, mapCells);
            fileStream.Close();
        }

        public static void LoadMap(Stream fileStream)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Tile[]>));
                mapCells = (List<Tile[]>)serializer.Deserialize(fileStream);
                fileStream.Close();
            }
            catch
            {

                ClearMap();
            }
        }

        #endregion

        #region Other

        public static void ClearMap()
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    mapCells[x][y] = new Tile(0, "", true);
                }
            }
        }

        #endregion
    }
}
