using GalaSoft.MvvmLight;
using ParkenDD.Api.Models;

namespace ParkenDD.Models
{
    public class SelectableParkingLot : ViewModelBase
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(() => IsSelected, ref _isSelected, value); }
        }

        public ParkingLot ParkingLot { get; set; }

        public SelectableParkingLot(ParkingLot lot)
        {
            ParkingLot = lot;
        }
    }
}
