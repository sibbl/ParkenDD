using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Api.Models;
using ParkenDD.ViewModels;

namespace ParkenDD.Controls
{
    public sealed partial class ParkingLotLoadDonut : UserControl
    {
        private static MainViewModel MainVm => ServiceLocator.Current.GetInstance<MainViewModel>();

        private bool _isVisible;

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
                WatchParkingLotChanges();
            };

            WatchParkingLotChanges();

            MainVm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(MainVm.SelectedParkingLot))
                {
                    UpdateIsSelected();
                }
            };
        }

        private void WatchParkingLotChanges()
        {
            var pl = DataContext as ParkingLot;
            if (pl != null)
            {
                pl.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(pl.FreeLots) ||
                        args.PropertyName == nameof(pl.TotalLots))
                    {
                        Draw();
                    }
                };
            }
        }

        private void UpdateIsSelected()
        {
            VisualStateManager.GoToState(this, ParkingLot == MainVm?.SelectedParkingLot ? "Selected" : "Unselected", true);
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ParkingLotLoadDonut), new PropertyMetadata(false, IsSelectedPropertyChanged));

        private static void IsSelectedPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as ParkingLotLoadDonut;
            control?.UpdateIsSelected();
        }

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty ParkingLotProperty = DependencyProperty.Register("ParkingLot", typeof(ParkingLot), typeof(ParkingLotLoadDonut), new PropertyMetadata(null, ParkingLotPropertyChanged));

        private static void ParkingLotPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as ParkingLotLoadDonut;
            control?.UpdateIsSelected();
            control?.Draw();
        }

        public ParkingLot ParkingLot
        {
            get { return (ParkingLot) GetValue(ParkingLotProperty); }
            set { SetValue(ParkingLotProperty, value); }
        }

        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(AnimateProperty, value); }
        }

        public static readonly DependencyProperty AnimateProperty =
            DependencyProperty.Register("Animate",
                typeof(bool),
                typeof(ParkingLotForecastChart),
                new PropertyMetadata(false, OnAnimatePropertyChanged));

        private static void OnAnimatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as ParkingLotLoadDonut;
            control?.UpdateAnimation();
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
                if (lot != null)
                {
                    PlayAnimation();
                }
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
            PlayAnimation();
        }

        private void PlayAnimation()
        {
            if (!_isVisible)
            {
                _isVisible = true;
                if (Animate)
                {
                    PopoutAnimationStoryboard.Begin();
                }
            }
        }

        public void UpdateAnimation()
        {
            if (Animate)
            {
                ScaleTransform.ScaleX = ScaleTransform.ScaleY = 0;
            }
            else
            {
                ScaleTransform.ScaleX = ScaleTransform.ScaleY = 1;
            }
        }
    }
}
