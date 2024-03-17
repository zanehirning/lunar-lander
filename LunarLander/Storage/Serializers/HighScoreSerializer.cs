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

        public HighScores(List<int> scores)
        {
            this.Scores = scores;
        }

        [DataMember()]
        public List<int> Scores { get; set; }
    }
}

