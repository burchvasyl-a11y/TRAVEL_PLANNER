using System.Collections.Generic;

namespace TRAVEL_PLANNER.Models
{
    public class City
    {
        public string name { get; set; }

        public string info { get; set; }

        public string view { get; set; }

        public string country { get; set; }

        public List<Attraction> attractions { get; set; } = new List<Attraction>();
    }

    public class Attraction
    {
        public string attraction { get; set; }

        public string description { get; set; }

        public string image { get; set; }

        public string category { get; set; }
    }
}
