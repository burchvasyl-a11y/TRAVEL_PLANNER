using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace TRAVEL_PLANNER.Models
{
    public class AttractionCardItem : INotifyPropertyChanged
    {
        private bool _isAdded;

        public string Attraction { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string ImagePath { get; set; }

        public ImageSource PreViewImage { get; set; }

        public bool IsAdded
        {
            get => _isAdded;
            set
            {
                if (_isAdded == value)
                {
                    return;
                }

                _isAdded = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
