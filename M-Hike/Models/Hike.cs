using SQLite;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace M_Hike.Models
{
    [Table("hikes")]
    public class Hike : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool ParkingAvailability { get; set; }
        public double Length { get; set; }
        public int Difficulty { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Members { get; set; } = string.Empty;
        public string Gear { get; set; } = string.Empty;

        private bool _isSelected;
        [Ignore]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}