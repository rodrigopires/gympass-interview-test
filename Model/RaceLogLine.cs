using System;

namespace GympassRace.Model
{
    public class RaceLogLine
    {
        public TimeSpan Hour { get; set; }
        public Pilot Pilot { get; set; }
        public int Lap { get; set; }
        public TimeSpan LapTime { get; set; }
        public decimal LapAvgSpeed { get; set; }
    }
}