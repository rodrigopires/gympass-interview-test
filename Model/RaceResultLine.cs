using System;
using System.Collections.Generic;
using System.Text;

namespace GympassRace.Model
{
    public class RaceResultLine
    {
        public int Position { get; set; }
        public int PilotCode { get; set; }
        public string PilotName { get; set; }
        public int TotalCompletedLaps { get; set; }
        public TimeSpan RaceDuration { get; set; }
        public TimeSpan DiffRaceDurationWinner { get; set; }
        public int BestLap { get; set; }
        public TimeSpan BestLapTime { get; set; }
        public decimal LapsAvgSpeed { get; set; }
    }
}
