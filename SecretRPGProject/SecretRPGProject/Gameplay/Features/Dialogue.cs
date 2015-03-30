using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PoorlyDrawnDungeons.Gameplay.Features
{
    public class Dialogue
    {
        #region Variables

        private string text;            //What are we saying in this dialogue
        private string responseOne;     //What is the first response to this dialogue
        private string responseTwo;     //What is the second response to this dialogue

        private int numResponse;        //How many responses do we have to this dialogue 0, 1 or 2
        private int selectedResponse;   //What responses has been selected 0, 1 or 2
        private int id;                 //The integer id of this dialogue - used for jumping to new dialogue options
        private int r1Id;               //ID to jump to for response 1
        private int r2Id;               //ID to jump to for respone 2

        #endregion

        #region Constructor

        public Dialogue(string dialogue, string responseOne, string responseTwo, int numResponse, int id, int r1Id, int r2Id)
        {
            this.text = dialogue;
            this.responseOne = responseOne;
            this.responseTwo = responseTwo;
            this.numResponse = numResponse;
            this.id = id;
            this.r1Id = r1Id;
            this.r2Id = r2Id;

            selectedResponse = 0;

            this.text = WrapText(GlobalVar.newsGothicBold12, 580f);
        }

        #endregion

        #region Properties

        public string Text
        {
            get { return text; }
        }

        public string ResponseOne
        {
            get { return responseOne; }
        }

        public string ResponseTwo
        {
            get { return responseTwo; }
        }

        public int NumResponse
        {
            get { return numResponse; }
        }

        public int SelectedResponse
        {
            get { return selectedResponse; }
            set { selectedResponse = value; }
        }

        public int ID
        {
            get { return id; }
        }

        public int R1ID
        {
            get { return r1Id; }
        }

        public int R2ID
        {
            get { return r2Id; }
        }

        #endregion

        #region Update

        public void Update()
        {
            //Have we chosen a response?
            MouseState ms = Mouse.GetState();

            //We don't need to see if a response is selected if there are no responses
            if (numResponse > 0)
            {
                if (ms.LeftButton == ButtonState.Pressed && GlobalVar.mouseReleased)
                {
                    GlobalVar.mouseReleased = false;

                    //Check if the mouse is pressed in a response location
                    Rectangle responseOneRec = new Rectangle(320 - (int)GlobalVar.newsGothicBold12.MeasureString(responseOne).X /2, 410 + (int)GlobalVar.newsGothicBold12.MeasureString(text).Y + 20, (int)GlobalVar.newsGothicBold12.MeasureString(responseOne).X + 4, (int)GlobalVar.newsGothicBold12.MeasureString(responseOne).Y + 4);
                    Rectangle responseTwoRec = new Rectangle(320 - (int)GlobalVar.newsGothicBold12.MeasureString(responseTwo).X /2, 410 + (int)GlobalVar.newsGothicBold12.MeasureString(text).Y + 50, (int)GlobalVar.newsGothicBold12.MeasureString(responseTwo).X + 4, (int)GlobalVar.newsGothicBold12.MeasureString(responseTwo).Y + 4);

                    if (responseOneRec.Contains(ms.X, ms.Y))
                    {
                        selectedResponse = 1;
                    }
                    else if (responseTwoRec.Contains(ms.X, ms.Y) && numResponse == 2)
                    {
                        selectedResponse = 2;
                    }
                }
                else if (ms.LeftButton == ButtonState.Released)
                {
                    GlobalVar.mouseReleased = true;
                }
            }

        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle responseOneRec = new Rectangle(320 - (int)GlobalVar.newsGothicBold12.MeasureString(responseOne).X / 2, 410 + (int)GlobalVar.newsGothicBold12.MeasureString(text).Y + 20, (int)GlobalVar.newsGothicBold12.MeasureString(responseOne).X + 4, (int)GlobalVar.newsGothicBold12.MeasureString(responseOne).Y + 4);
            Rectangle responseTwoRec = new Rectangle(320 - (int)GlobalVar.newsGothicBold12.MeasureString(responseTwo).X / 2, 410 + (int)GlobalVar.newsGothicBold12.MeasureString(text).Y + 50, (int)GlobalVar.newsGothicBold12.MeasureString(responseTwo).X + 4, (int)GlobalVar.newsGothicBold12.MeasureString(responseTwo).Y + 4);

            spriteBatch.DrawString(GlobalVar.newsGothicBold12, text, new Vector2(320 - (int)GlobalVar.newsGothicBold12.MeasureString(text).X / 2, 410), Color.White);

            Color colour = Color.White;

            MouseState ms = Mouse.GetState();

            if (responseOneRec.Contains(ms.X, ms.Y))
            {
                colour = Color.Yellow;
            }
            else
            {
                colour = Color.White;
            }
            spriteBatch.DrawString(GlobalVar.newsGothicBold12, responseOne, new Vector2(responseOneRec.X, responseOneRec.Y), colour);
            
            if (responseTwoRec.Contains(ms.X, ms.Y))
            {
                colour = Color.Yellow;
            }
            else
            {
                colour = Color.White;
            }
            spriteBatch.DrawString(GlobalVar.newsGothicBold12, responseTwo, new Vector2(responseTwoRec.X, responseTwoRec.Y), colour);
            
        }

        #endregion

        #region WordWrap

        public string WrapText(SpriteFont spriteFont, float maxLineWidth)
        {
            string[] words = text.Split(' ');

            StringBuilder sb = new StringBuilder();

            float lineWidth = 0f;

            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}
