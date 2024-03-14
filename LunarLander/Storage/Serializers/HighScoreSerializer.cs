using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LunarLander.Storage 
{
    [DataContract(Name = "HighScores")]
    public class HighScores
    {
        public HighScores() {
        }

        public HighScores(uint score)
        {
            this.Score = score;
            this.TimeStamp = DateTime.Now;

            keys.Add(1, "one");
            keys.Add(2, "two");
        }

        [DataMember()]
        public string Name { get; set; }
        [DataMember()]
        public uint Score { get; set; }
        [DataMember()]
        public DateTime TimeStamp { get; set; }
        [DataMember()]
        public Dictionary<int, String> keys = new Dictionary<int, string>();
    }
}

