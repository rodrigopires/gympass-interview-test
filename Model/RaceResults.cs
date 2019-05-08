using System;
using System.Collections.Generic;
using System.Text;

namespace GympassRace.Model
{
    public class RaceResults
    {
        public RaceResults() {
            ResultLines = new List<RaceResultLine>();
        }

        public RaceResultLine BestLap { get; set; }
        public List<RaceResultLine> ResultLines { get; set; }
    }
}
