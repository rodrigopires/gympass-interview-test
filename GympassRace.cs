using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GympassRace.Model;

namespace GympassRace
{
    public class GympassRace
    {
        public RaceLog _raceLog { get; set; }
        public RaceResults _raceResults { get; set; }

        public GympassRace() {
            _raceLog = new RaceLog();
            _raceResults = new RaceResults();
        }

        public void Start(string logFilePath) {

            Console.WriteLine("BEM AMIIIIIIIGOS da Rede Globo, falamos diiiiiireto do autódromo de Interlagos para o Grande Prêmio GYMPASS de F1...");
            Console.WriteLine();
            Console.WriteLine("Vai começar a corrida, foi dada a largada... Haaaaaaja coração!");
            Console.WriteLine();

            ImportRaceLogFile(logFilePath);

            ProcessRaceResults();

            Console.WriteLine("E já acabooou amigo... que rápido Rrrrrrrreginaldo Leme. Confira o resultado da prova:");
            Console.WriteLine();

            PrintRaceResults();

            Console.WriteLine();
            Console.WriteLine("Obrigado pela audiência. Rodrigo e Gympass, TUUUUUDO A VER! :)");

            Console.Read();
        }

        private void ImportRaceLogFile(string logFilePath) {
            var logContent = File.ReadAllText(logFilePath).Split('\n');
            var logLines = from line in logContent
                            select line.Split(';').ToArray();

            foreach (var row in logLines.Skip(1).TakeWhile(r => r.Length > 1 && r.Last().Trim().Length > 0)) {
                var hour = row[0];
                var pilot = row[1];
                var lap = row[2];
                var lapTime = row[3].Replace(".", ":");
                var lapAvgSpeed = row[4];

                var raceLogLine = new RaceLogLine {
                    Hour = TimeSpan.Parse(hour),
                    Pilot = new Pilot {
                        Id = Convert.ToInt32(pilot.Substring(0, 3)),
                        Name = pilot.Substring(6, pilot.Length - 6)
                    },
                    Lap = Convert.ToInt32(lap),
                    LapTime = new TimeSpan(1, 1, Convert.ToInt32(lapTime.Split(':')[0]), Convert.ToInt32(lapTime.Split(':')[1]), Convert.ToInt32(lapTime.Split(':')[2])),
                    LapAvgSpeed = Convert.ToDecimal(lapAvgSpeed)
                };
                _raceLog.RaceLogLines.Add(raceLogLine);
            }
        }

        private void ProcessRaceResults() {
            //Get Race Best Lap
            var logBestLap = _raceLog.RaceLogLines.OrderBy(t => t.LapTime).FirstOrDefault();
            _raceResults.BestLap = new RaceResultLine {
                PilotCode = logBestLap.Pilot.Id,
                PilotName = logBestLap.Pilot.Name,
                RaceDuration = logBestLap.LapTime,
            };

            //Group results by pilot
            var pilotsResults = _raceLog.RaceLogLines.GroupBy(t => t.Pilot.Id);
            foreach (var pilotResult in pilotsResults) {
                var raceDurationSum = new TimeSpan(pilotResult.Sum(t => t.LapTime.Ticks));
                var raceDuration = new TimeSpan(1, 1, raceDurationSum.Minutes, raceDurationSum.Seconds, raceDurationSum.Milliseconds);
                var totalLaps = pilotResult.Max(t => t.Lap);
                var bestLap = pilotResult.OrderBy(t => t.LapTime).FirstOrDefault();
                var lapsAvgSpeed = pilotResult.Average(t => t.LapAvgSpeed);

                var raceResultLine = new RaceResultLine {
                    PilotCode = pilotResult.FirstOrDefault().Pilot.Id,
                    PilotName = pilotResult.FirstOrDefault().Pilot.Name,
                    TotalCompletedLaps = totalLaps,
                    RaceDuration = raceDuration,
                    BestLap = bestLap.Lap,
                    BestLapTime = bestLap.LapTime,
                    LapsAvgSpeed = lapsAvgSpeed
                };
                _raceResults.ResultLines.Add(raceResultLine);
            }
        }

        private void PrintRaceResults() {
            Console.WriteLine("-----------------------------------------------------------------------------------------------------");
            Console.WriteLine("Posição |Piloto                 |Voltas |Tempo Prova |Melhor Volta  |Velocidade Média |Diff. Vencedor");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------");
            var position = 1;
            RaceResultLine resultLineWinner = null;
            foreach (var resultLine in _raceResults.ResultLines.OrderByDescending(t => t.TotalCompletedLaps).ThenBy(t => t.RaceDuration)) {
                resultLine.Position = position;
                if (position == 1) {
                    resultLineWinner = resultLine;
                }

                resultLine.DiffRaceDurationWinner = (position == 1 ? new TimeSpan(1, 1, 0, 0, 0) : resultLineWinner.RaceDuration.Subtract(resultLine.RaceDuration));

                Console.WriteLine($"" +
                    $"{resultLine.Position}" +
                    $"{AddColumnSpace(8, resultLine.Position.ToString())}|" +
                    $"{resultLine.PilotCode.ToString().PadLeft(3, '0')} - {resultLine.PilotName}" +
                    $"{AddColumnSpace(23, (resultLine.PilotCode.ToString().PadLeft(3, '0') + " - " + resultLine.PilotName))}|" +
                    $"{resultLine.TotalCompletedLaps}" +
                    $"{AddColumnSpace(7, resultLine.TotalCompletedLaps.ToString())}|" +
                    $"{resultLine.RaceDuration.ToString(@"mm\:ss\:fff")}" +
                    $"{AddColumnSpace(12, resultLine.RaceDuration.ToString(@"mm\:ss\:fff"))}|" +
                    $"{resultLine.BestLap} ({resultLine.BestLapTime.ToString(@"mm\:ss\:fff")})" +
                    $"{AddColumnSpace(10, resultLine.BestLap.ToString(@"mm\:ss\:fff"))}|" +
                    $"{resultLine.LapsAvgSpeed.ToString("#.###")}" +
                    $"{AddColumnSpace(17, resultLine.LapsAvgSpeed.ToString("#.###"))}|" +
                    $"{resultLine.DiffRaceDurationWinner.ToString(@"mm\:ss\:fff")}" +
                    $"{AddColumnSpace(14, resultLine.DiffRaceDurationWinner.ToString(@"mm\:ss\:fff"))}"
                    );
                position++;
            }
            Console.WriteLine("-----------------------------------------------------------------------------------------------------");

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Melhor Volta da Corrida:");
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Piloto                 |Tempo     ");
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"" +
                    $"{_raceResults.BestLap.PilotCode.ToString().PadLeft(3, '0')} - {_raceResults.BestLap.PilotName}" +
                    $"{AddColumnSpace(23, (_raceResults.BestLap.PilotCode.ToString().PadLeft(3, '0') + " - " + _raceResults.BestLap.PilotName))}|" +
                    $"{_raceResults.BestLap.RaceDuration.ToString(@"mm\:ss\:fff")}" +
                    $"{AddColumnSpace(12, _raceResults.BestLap.RaceDuration.ToString(@"mm\:ss\:fff"))}"
                    );
            Console.WriteLine("----------------------------------");
        }

        private string AddColumnSpace(int columnQtdSpaces, string columnText) {
            var spacesAddQty = columnQtdSpaces - columnText.Length;
            if (spacesAddQty < 0)
                spacesAddQty = 0;

            return new String(' ', spacesAddQty);
        }
    }
}
