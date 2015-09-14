using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using ParkenDD.Api.Models;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.ViewModels;

namespace ParkenDD.Controls
{
    public sealed partial class ParkingLotLoadDonut : UserControl
    {
        private static MainViewModel MainVm => ServiceLocator.Current.GetInstance<MainViewModel>();

        public ParkingLotLoadDonut()
        {
            InitializeComponent();
            ParkingLot = null;
            Loaded += (sender, args) =>
            {
                UpdateIsSelected();
                Draw();
            };
            DataContextChanged += (sender, args) =>
            {
                UpdateIsSelected();
                Draw();
            };

            MainVm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(MainVm.SelectedParkingLot))
                {
                    UpdateIsSelected();
                }
            };
        }

        private void UpdateIsSelected()
        {
            VisualStateManager.GoToState(this, ParkingLot == MainVm?.SelectedParkingLot?.ParkingLot ? "Selected" : "Unselected", true);
        }

        public static readonly DependencyProperty ParkingLotProperty = DependencyProperty.Register("ParkingLot", typeof(ParkingLot), typeof(ParkingLotLoadDonut), null);
        public ParkingLot ParkingLot
        {
            get
            {
                return (ParkingLot)GetValue(ParkingLotProperty);
            }

            set
            {
                SetValue(ParkingLotProperty, value);
                UpdateIsSelected();
                Draw();
            }
        }

        private void Draw()
        {
            if (ActualHeight > 0)
            {
                FreeLabel.FontSize = ActualHeight*0.25;
            }
            var value = double.NaN;
            var lot = ParkingLot;
            if (lot != null && lot.TotalLots != 0)
            {
                value = ((double)lot.FreeLots) / ((double)lot.TotalLots);
            }
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                ValueInvertedCircle.Visibility = ValuePath.Visibility = Visibility.Collapsed;
                FreeLabel.Text = "?";
                return;
            }
            if (value < 0)
            {
                value = 0;
            }else if (value > 1)
            {
                value = 1;
            }
            ValueInvertedCircle.Visibility = ValuePath.Visibility = Visibility.Visible;
            FreeLabel.Text = Math.Round(value * 100) + "%";

            ValuePath.SetValue(Path.DataProperty, null);
            var pg = new PathGeometry();
            var fig = new PathFigure();

            var height = ActualHeight;
            var width = ActualWidth;
            var radius = height / 2;
            var theta = (360 * value) - 90;
            var xC = radius;
            var yC = radius;

            if (value == 1) theta += 1;

            var finalPointX = xC + (radius * Math.Cos(theta * 0.0174));
            var finalPointY = yC + (radius * Math.Sin(theta * 0.0174));

            fig.StartPoint = new Point(radius, radius);
            var firstLine = new LineSegment {Point = new Point(radius, 0)};
            fig.Segments.Add(firstLine);

            if (value > 0.25)
            {
                var firstQuart = new ArcSegment
                {
                    Point = new Point(width, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    Size = new Size(radius, radius)
                };
                fig.Segments.Add(firstQuart);
            }

            if (value > 0.5)
            {
                var secondQuart = new ArcSegment
                {
                    Point = new Point(radius, height),
                    SweepDirection = SweepDirection.Clockwise,
                    Size = new Size(radius, radius)
                };
                fig.Segments.Add(secondQuart);
            }

            if (value > 0.75)
            {
                var thirdQuart = new ArcSegment
                {
                    Point = new Point(0, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    Size = new Size(radius, radius)
                };
                fig.Segments.Add(thirdQuart);
            }

            var finalQuart = new ArcSegment
            {
                Point = new Point(finalPointX, finalPointY),
                SweepDirection = SweepDirection.Clockwise,
                Size = new Size(radius, radius)
            };
            fig.Segments.Add(finalQuart);

            var lastLine = new LineSegment {Point = new Point(radius, radius)};
            fig.Segments.Add(lastLine);
            pg.Figures.Add(fig);
            ValuePath.SetValue(Path.DataProperty, pg);
        }
    }
}
