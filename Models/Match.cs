using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCRSimulatorApp.Models
{
    public class Match
    {
        public int MatchNumber { get; set; }

        public int TurnNumber { get; set; }

        public int CenterChipNumber { get; set; }

        public Player Winner { get; set; }
    }
}
