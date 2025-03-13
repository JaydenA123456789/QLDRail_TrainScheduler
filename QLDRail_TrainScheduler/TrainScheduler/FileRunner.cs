using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace QLDRail_TrainScheduler.TrainScheduler
{
    class FileRunner
    {
        public FileRunner(string arg)
        {
            ProcessTextFile(arg);
        }

        private void ProcessTextFile(string arg)
        {
            List<string> rawStationList = new List<string>();

            string filePath = Path.Combine("TrainSequences", arg);
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        rawStationList.Add(line);
                    }
                }
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }
            //Create Sequence object, sequence object will contain all common statistics and store the stations as a list, for operations
            SequenceObject sequence = new SequenceObject(rawStationList);

            //print out the scenario output
            Console.Write("This train ");
            DescribeTrainRoute(sequence, rawStationList);
            Console.Write("\n");
        }

        private void DescribeTrainRoute(SequenceObject sequence, List<string> rawStationList)
        {
            if (sequence.StationsCount <= 1)
            {
                Console.WriteLine("Assumption failed, there should be 2 or more stations");
                return;
            }

            //Scenario 1: “This train stops at Station1 and Station2 only”
            if (sequence.StationsCount == 2)
            {
                Console.Write($"stops at {sequence.StationList[0].Name} and {sequence.StationList[1].Name} only");
                return;
            }

            //If 3 or more stations:
            //Scenario 2: “This train stops at all stations”
            if (sequence.StoppingStationsCount == sequence.StationsCount)
            {
                Console.Write("stops at all stations");
                return;
            }

            //Scenario 3: “This train stops at all stations ExpressStation”
            if (sequence.ExpressStationsCount == 1)
            {
                //find the one express station by looping through the station list until we find it
                string expressStationName = "";
                for (int i = 0; i < sequence.StationList.Count; i++)
                {
                    if (!sequence.StationList[i].IsStopping)
                    {
                        expressStationName = sequence.StationList[i].Name;
                        break;
                    }
                }
                Console.Write($"stops at all stations except {expressStationName}");
                return;
            }

            //Scenarios if there is an express section, not just an express station:
            //Scenario 4: “This train runs express from Station1 to Station2“,
            if (sequence.ExpressSectionCount == 1 && sequence.StoppingStationsCount == 2)
            {
                Console.Write($"runs express from {sequence.StationList[0].Name} to {sequence.StationList[sequence.StationList.Count - 1].Name}");
                return;
            }

            //Scenario 5: “This train runs express from Station1 to Station2, stopping only at Station3”
            if (sequence.StoppingStationsCount == 3)
            {
                //Get a list of all the stopping stations to print out
                List<string> stoppingStationList = new List<string>();
                
                for (int i = 0; i < sequence.StationList.Count; i++)
                {
                    if (sequence.StationList[i].IsStopping)
                    {
                        stoppingStationList.Add(sequence.StationList[i].Name);
                    }
                }
                Console.Write($"runs express from {stoppingStationList[0]} to {stoppingStationList[2]}, stopping only at {stoppingStationList[1]}");
                return;
            }

            //Scenario 6, there needs to be a more complex combination, this code will split it into sections of stopping stations, and recursivly call the PrintScenario function
            SplitAndPrintComplexScenario(sequence, rawStationList); 
        }

        private void SplitAndPrintComplexScenario(SequenceObject sequence, List<string> rawStationList)
        {
            int stationIndex = 0;
            for (int stoppingStationIndex = 0; stoppingStationIndex < sequence.StoppingStationsCount;)
            {
                List<Station> stationSublist = new List<Station>();

                int stoppingSectionCount = stoppingStationIndex + 3; //This variable holds the number of stopping stations in the current section
                // Handle edge case where the number of stopping stations is not perfectly divisible by 3
                if (sequence.StoppingStationsCount % 3 == 1 && stoppingStationIndex >= sequence.StoppingStationsCount - 4)
                {
                    stoppingSectionCount = stoppingStationIndex + 2;
                }

                //Loop through StationList and add the next 2 or 3 stopping stations to the sublist
                for (int j = stationIndex; stoppingStationIndex < stoppingSectionCount; j++)
                {
                    if (j >= sequence.StationsCount) break;

                    stationIndex++;
                    stationSublist.Add(sequence.StationList[j]);

                    if (sequence.StationList[j].IsStopping)
                    {
                        stoppingStationIndex++;
                    }
                }

                SequenceObject newSequence = new SequenceObject(stationSublist);
                DescribeTrainRoute(newSequence, rawStationList);

                if (stoppingStationIndex < sequence.StoppingStationsCount)
                {
                    Console.Write(" then ");
                }
            }
        }
    }
}
