using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace QLDRail_TrainScheduler.TrainScheduler
{
    class SequenceObject
    {
        public int StationsCount { get; private set; }
        public int StoppingStationsCount { get; private set; }
        public int ExpressStationsCount { get; private set; }
        public int ExpressSectionCount { get; private set; }

        public List<Station> StationList { get; private set; }


        // Constructor for reading from file
        public SequenceObject(List<string> lineArray)
        {
            StationList = new List<Station>();

            foreach (string line in lineArray)
            {
                string[] parts = line.Split(", ");

                if (parts.Length == 2)
                {
                    if (bool.TryParse(parts[1], out bool isStoppingStation))
                    {
                        StationList.Add(new Station(parts[0], isStoppingStation));
                    }
                    else
                    {
                        Console.WriteLine($"Invalid station format: {line}");
                    }
                }
                else
                {
                    Console.WriteLine($"Skipping malformed line: {line}");
                }
            }

            InitializeStationData();
        }

        // Constructor for direct list input
        public SequenceObject(List<Station> newStationList)
        {
            StationList = new List<Station>(newStationList);
            InitializeStationData();
        }

        private void InitializeStationData()
        {
            RemoveLeadingAndTrailingExpressStations();
            CalculateStationStatistics();
        }

        private void RemoveLeadingAndTrailingExpressStations()
        {
            // Remove express stations at the start
            while (StationList.Count > 0 && !StationList.First().IsStopping)
            {
                StationList.RemoveAt(0);
            }

            // Remove express stations at the end
            while (StationList.Count > 0 && !StationList.Last().IsStopping)
            {
                StationList.RemoveAt(StationList.Count - 1);
            }
        }

        private void CalculateStationStatistics()
        {
            StationsCount = StationList.Count;
            StoppingStationsCount = StationList.Count(station => station.IsStopping);
            ExpressStationsCount = StationsCount - StoppingStationsCount;

            ExpressSectionCount = 0;
            bool currentlyInExpressSection = false;

            for (int i = 0; i < StationsCount; i++)
            {
                if (!StationList[i].IsStopping)
                {
                    if (!currentlyInExpressSection)
                    {
                        ExpressSectionCount++;
                        currentlyInExpressSection = true;
                    }
                }
                else
                {
                    currentlyInExpressSection = false;
                }
            }
        }
    }
}
