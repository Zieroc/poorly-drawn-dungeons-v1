using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoorlyDrawnDungeons.Gameplay.GameObjects.Items
{
    [Serializable]
    public class Inventory : ISerializable
    {
        #region Variables

        private List<Item> items;
        private List<Item> nonActive;
        private int maxItems;

        #endregion

        #region Constructor

        public Inventory(int maxItems)
        {
            items = new List<Item>();
            nonActive = new List<Item>();
            this.maxItems = maxItems;
        }

        #endregion

        #region Properties

        public List<Item> Items
        {
            get { return items; }
        }

        public int MaxItems
        {
            get { return maxItems; }
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] is Potion)
                {
                    items[i].Update(gameTime);
                }
                if (!items[i].Active && items[i].Stack < 1)
                {
                    nonActive.Add(items[i]);
                }
                if (!items[i].Alive || (!items[i].Active && items[i].Stack < 1))
                {
                    items.RemoveAt(i);
                }
            }

            for (int i = nonActive.Count - 1; i >= 0; i--)
            {
                if (nonActive[i] is Potion)
                {
                    nonActive[i].Update(gameTime);
                }
                if (!nonActive[i].Alive)
                {
                    nonActive.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Add & Remove

        public int AddItem(Item item)
        {
            int leftOver = 0;
            bool alreadyHave = false;

            for (int i = 0; i < items.Count; i++)
            {
                #region AlreadyHaveThisItem
                if (items[i].Name == item.Name)
                {
                    if (items[i].Stack + item.Stack <= items[i].MaxStack)
                    {
                        alreadyHave = true;
                        items[i].IncStack(item.Stack);
                        i = items.Count;
                    }
                    else
                    {
                        int addableStack = items[i].MaxStack - items[i].Stack;

                        items[i].IncStack(addableStack);
                        item.DecStack(addableStack);

                        leftOver = item.Stack;
                    }
                }
                #endregion
            }

            if (items.Count < maxItems)
            {
                if (!alreadyHave)
                {
                    #region IsALight
                    if (item is Light)
                    {
                        Light light = (Light)item;
                        Light newLight = new Light(light.Aura, light.Lifespan, light.Lit, light.Name, light.Price, light.Image, light.Sprites, light.DrawSprite, light.Position, light.MaxStack, light.Stack, light.Image.Name);
                        items.Add(newLight);
                    }
                    #endregion
                    #region IsAWeapon
                    else if (item is Weapon)
                    {
                        Weapon weapon = (Weapon)item;
                        Weapon newWeapon = new Weapon(weapon.AttackBonus, weapon.DamageRoll, weapon.NumDice, weapon.Name, weapon.Price, weapon.Image, weapon.Sprites, weapon.DrawSprite, weapon.Position, weapon.MaxStack, weapon.Stack, weapon.Image.Name, weapon.DamageType, weapon.WeaponType, weapon.SpecialEffect);
                        items.Add(newWeapon);
                    }
                    #endregion
                    #region IsArmour
                    else if (item is Armour)
                    {
                        Armour armour = (Armour)item;
                        Armour newArmour = new Armour(armour.DefenceBonus, armour.Name, armour.Price, armour.Image, armour.Sprites, armour.DrawSprite, armour.Position, armour.MaxStack, armour.Stack, armour.Image.Name, armour.Type);
                        items.Add(newArmour);
                    }
                    #endregion
                    #region IsAPotion
                    else if (item is Potion)
                    {
                        Potion potion = (Potion)item;
                        Potion newPotion = new Potion(potion.Type, potion.Amount, potion.BuffType, potion.Duration, potion.Name, potion.Price, potion.Image, potion.Sprites, potion.DrawSprite, potion.Position, potion.MaxStack, potion.Stack, potion.Image.Name);
                        items.Add(newPotion);
                    }
                    #endregion
                    #region IsAnItem
                    else if (item is Item)
                    {
                        Item newItem = new Item(item.Name, item.Price, item.Image, item.Sprites, item.DrawSprite, item.Position, item.MaxStack, item.Stack, item.Image.Name);
                        items.Add(newItem);
                    }
                    #endregion

                    leftOver = 0;
                }
            }

            return leftOver;
        }

        public void RemoveItem(int loc)
        {
            items[loc].Kill();
            items.RemoveAt(loc);
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Active || items[i].Stack > 0)
                {
                    items[i].DrawImage(spriteBatch, GlobalVar.inventoryPos[i]);
                    if (!GlobalVar.displayShop)
                    {
                        spriteBatch.DrawString(GlobalVar.newsGothicBold8, "x" + items[i].Stack, new Vector2(GlobalVar.inventoryPos[i].X + 1, GlobalVar.inventoryPos[i].Y + 19), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(GlobalVar.newsGothicBold8, (Items[i].Price * 75 / 100) * Items[i].Stack + "G", new Vector2(GlobalVar.inventoryPos[i].X + 1, GlobalVar.inventoryPos[i].Y + 19), Color.White);
                    }
                }
            }
        }

        #endregion

        #region Serializable Methods

        public Inventory(SerializationInfo info, StreamingContext ctxt)
        {
            this.items = (List<Item>)info.GetValue("Items", typeof(List<Item>));
            this.nonActive = (List<Item>)info.GetValue("NonActive", typeof(List<Item>));
            this.maxItems = (int)info.GetValue("MaxItems", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Items", this.items);
            info.AddValue("NonActive", this.nonActive);
            info.AddValue("MaxItems", this.maxItems);
        }

        #endregion
    }
}
