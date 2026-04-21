using System.Collections.Generic;

namespace TRAVEL_PLANNER.Models
{
    public class Country
    {
        public string name { get; set; }

        public string info { get; set; }

        public string map { get; set; }

        public string View { get; set; }

        public List<Place> places { get; set; } = new List<Place>();
    }

    public class Place
    {
        public string place { get; set; }

        public string description { get; set; }

        public string image { get; set; }

        public string category { get; set; }
    }
}
