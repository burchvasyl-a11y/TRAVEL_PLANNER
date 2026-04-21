using System;
using System.Collections.Generic;
using System.Linq;

namespace TRAVEL_PLANNER.Models
{
    public class SavedTrip
    {
        public string CountryName { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public List<string> Categories { get; set; } = new List<string>();

        public List<string> SelectedPlaces { get; set; } = new List<string>();

        public string DisplayDateRange => $"{DateFrom:dd.MM.yyyy}-{DateTo:dd.MM.yyyy}";

        public string PrimaryCategory => Categories.FirstOrDefault() ?? "Без категорії";

        public string SecondaryCategory => Categories.Skip(1).FirstOrDefault();
    }
}
