using System;
using System.Collections.Generic;
using System.Text;

namespace GympassRace.Model
{
    public class RaceLog
    {
        public RaceLog() {
            RaceLogLines = new List<RaceLogLine>();
        }

        public List<RaceLogLine> RaceLogLines { get; set; }
    }
}
