using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using PoorlyDrawnDungeons.Gameplay.GameObjects.Characters;
using PoorlyDrawnDungeons.Utility;

namespace PoorlyDrawnDungeons.Utility.Storage
{
    [Serializable]
    public class SaveItem : ISerializable
    {
        private Player player;
        private int levelNum;

        public SaveItem(Player player, int levelNum)
        {
            this.player = player;
            this.levelNum = levelNum;
        }

        public Player Player
        {
            get { return player; }
            set { player = value; }
        }

        public int LevelNum
        {
            get { return levelNum; }
            set { levelNum = value; }
        }

        public SaveItem(SerializationInfo info, StreamingContext ctxt)
        {
            this.player = (Player)info.GetValue("Player", typeof(Player));
            this.levelNum = (int)info.GetValue("LevelNum", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Player", this.player);
            info.AddValue("LevelNum", this.levelNum);
        }
    }
}
