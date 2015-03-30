using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PoorlyDrawnDungeons.Gameplay.Auras;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters;
using PoorlyDrawnDungeons.Graphics;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.Items
{
    [Serializable]
    public class Light : Item
    {
        #region Variables

        private LightAura aura;
        private double lifespan;     //How long the light lasts - a negative number means permanent lighting
        private double timer;
        private bool lit;           //Is the light lit;

        #endregion

        #region Constructor

        public Light(LightAura aura, double lifespan, bool lit, String name, int price, Texture2D image, Sprite[] sprite, bool drawSprite, Vector2 position, int maxStack, int stack, String filename)
            : base(name, price, image, sprite, drawSprite, position, maxStack, stack, filename)
        {
            this.aura = aura;
            this.lifespan = lifespan;
            this.lit = lit;

            timer = 0;
        }

        #endregion

        #region Properties

        public LightAura Aura
        {
            get { return aura; }
        }

        public double Lifespan
        {
            get { return lifespan; }
        }

        public bool Lit
        {
            get { return lit; }
        }

        #endregion

        #region Light

        public void SwitchLit()
        {
            lit = !lit;
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (lit)
            {
                if (lifespan > 0)
                {
                    timer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer > lifespan)
                    {
                        
                        //The torch has been used up if we have more swap it otherwise kill it
                        if (stack > 1)
                        {
                            stack--;
                            timer = 0f;
                        }
                        else
                        {
                            Kill();
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        #endregion

        #region Draw

        public void DrawAura(SpriteBatch spriteBatch)
        {
            if (lit)
            {
                aura.Draw(spriteBatch);
            }
        }

        #endregion

        #region Movement

        public void MoveTo(Vector2 position, int direction, bool offset)
        {
            if (offset)
            {
                if (direction == 0)
                {
                    this.position = new Vector2(position.X + 1 - 4, position.Y - Sprite.Height);
                }
                else if (direction == 1)
                {
                    this.position = new Vector2(position.X - 1, position.Y);
                }
                else if (direction == 2)
                {
                    this.position = new Vector2(position.X - Sprite.Width, position.Y - 1);
                }
                else
                {
                    this.position = new Vector2(position.X, position.Y + 1 - 4);
                }
            }
            else
            {
                this.position = position;
            }

            this.direction = direction;
            Vector2 auraPos = new Vector2(PositionCenter.X - aura.Mask.Width / 2, PositionCenter.Y - aura.Mask.Height / 2);
            aura.MoveTo(auraPos);
        }

        #endregion

        #region Kill

        public override void Kill()
        {
            lit = false;

            base.Kill();
        }

        #endregion

        #region OnClick

        public override void OnClick(Player player)
        {
            if (!player.LightEquipped || !(player.Torch == this))
            {
                if(player.Torch != this && player.Torch.Lit && player.Torch != player.Vision)
                {
                    player.Torch.SwitchLit();
                }

                player.EquipTorch(this);
            }

            base.OnClick(player);
        }

        #endregion

        #region Serializable Methods

        public Light(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {
            this.aura = (LightAura)info.GetValue("Aura", typeof(LightAura));
            this.lifespan = (double)info.GetValue("Lifespan", typeof(double));
            this.timer = (double)info.GetValue("Timer", typeof(double));
            this.lit = (bool)info.GetValue("Lit", typeof(bool));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Aura", this.aura);
            info.AddValue("Lifespan", this.lifespan);
            info.AddValue("Timer", this.timer);
            info.AddValue("Lit", this.lit);

            base.GetObjectData(info, ctxt);
        }

        #endregion
    }
}
