using System;

namespace GympassRace
{
    class Program
    {
        static void Main(string[] args) {
            var logFilePath = "C:\\Users\\Rodrigo\\source\\repos\\GympassRace\\RaceLog.csv";

            var gympassRace = new GympassRace();
            gympassRace.Start(logFilePath);
        }
    }
}
