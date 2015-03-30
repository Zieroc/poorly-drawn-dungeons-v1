using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PoorlyDrawnDungeons.Gameplay.Features;
using PoorlyDrawnDungeons.Graphics;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.Characters
{
    //Default NPC, can't be hurt just talks to player
    public class NPC : Character
    {
        #region Variables

        protected Dialogue[] speech;
        protected bool speaking;        //Is this npc speaking?
        protected int currentSpeech;    //What speech option are we currently on?
        protected int facing;           //The direction the player needs to face to speak to the NPC
        protected int defaultSpeech;    //After the first time talking to this NPC what is the default speech option for later conversations

        #endregion

        #region Constructor

        public NPC(Sprite[] sprite, Vector2 position, int direction, int facing)
            :base(sprite, position, direction)
        {
            alive = true;
            mode = GlobalVar.CharacterMode.Idle;
            name = "NPC";

            maxHealth = 1;
            currentHealth = 1;
            maxMana = 0;
            currentMana = 0;

            attack = 0;
            defence = 10;

            strength = 10;
            dexterity = 10;
            intelligence = 10;
            stamina = 10;

            this.facing = facing;
        }

        public NPC(Dialogue[] speech, Sprite[] sprite, Vector2 position, int direction, int facing, int defaultSpeech)
            :this(sprite, position, direction, facing)
        {
            this.speech = speech;
            this.defaultSpeech = defaultSpeech;
        }

        #endregion

        #region Properties

        public Dialogue Speech
        {
            get { return speech[currentSpeech]; }
        }

        public int Facing
        {
            get { return facing; }
        }

        #endregion

        #region SwitchSpeech

        public void Speak()
        {
            GlobalVar.displayDialogue = true;
            GlobalVar.paused = true;
            GlobalVar.speech = speech[currentSpeech];
            speaking = true;
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (alive)
            {
                if (mode == GlobalVar.CharacterMode.Patrol)
                {
                    switch (direction)
                    {
                        case 0:
                            velocity.Y -= speed;
                            break;
                        case 1:
                            velocity.Y += speed;
                            break;
                        case 2:
                            velocity.X -= speed;
                            break;
                        case 3:
                            velocity.X += speed;
                            break;
                    }
                }

            }

            //If we are speaking make sure we move through dialogue options
            if (speaking)
            {
                switch (GlobalVar.speech.SelectedResponse)
                {
                    case 1:
                        currentSpeech = GlobalVar.speech.R1ID;
                        break;
                    case 2:
                        currentSpeech = GlobalVar.speech.R2ID;
                        break;
                    default:
                        break;
                }

                if (currentSpeech >= 0)
                {
                    speech[currentSpeech].SelectedResponse = 0; //Reset response in case we have returned to a previous speech
                    GlobalVar.speech = speech[currentSpeech];   //Switch to new speech option
                }
                else
                {
                    speaking = false;
                    GlobalVar.displayDialogue = false;
                    GlobalVar.paused = false;
                    currentSpeech = defaultSpeech;
                    speech[currentSpeech].SelectedResponse = 0;
                }
            }

            base.Update(gameTime);
        }

        #endregion
    }
}
