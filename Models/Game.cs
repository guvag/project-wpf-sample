using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCRSimulatorApp.Models
{
    public class Game
    {
        public int PlayerNumber { get; set; }

        public int GameNumber { get; set; }

        public int ShortestTurnNumber { get; set; }

        public int LargestTurnNumber { get; set; }

        public int Average { get; set; }

        public Dictionary<int, string> Dice {get;set;}

        public List<Match> MatchList { get; set; }
        public List<Player> PlayerList { get; set; }
    }


}
