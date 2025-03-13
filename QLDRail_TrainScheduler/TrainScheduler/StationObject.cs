using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLDRail_TrainScheduler.TrainScheduler
{
    class Station
    {
        public string Name { get; }
        public bool IsStopping { get; }

        public Station(string name, bool isStopping)
        {
            Name = name;
            IsStopping = isStopping;
        }
    }
}
