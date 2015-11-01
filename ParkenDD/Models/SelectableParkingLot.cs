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
            set { Set(ref _isSelected, value); }
        }

        private ParkingLot _parkingLot;
        public ParkingLot ParkingLot
        {
            get { return _parkingLot; }
            set { Set(ref _parkingLot, value); }
        }

        public SelectableParkingLot(ParkingLot lot)
        {
            ParkingLot = lot;
        }

        public void RaiseParkingLotPropertyChanged()
        {
            RaisePropertyChanged(() => ParkingLot);
        }
    }
}
