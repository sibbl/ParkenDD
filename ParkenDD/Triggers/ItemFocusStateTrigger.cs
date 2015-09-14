using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsStateTriggers;

namespace ParkenDD.Triggers
{
    public class ItemFocusStateTrigger : StateTriggerBase, ITriggerValue
    {
        /// <summary>
        /// Gets or sets the ItemsControl to check the focus state of
        /// </summary>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Value"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof (object), typeof (ItemFocusStateTrigger),
                new PropertyMetadata(true, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ItemFocusStateTrigger) d;
            var val = e.NewValue;
            var uiElement = val as ItemsControl;
            if (uiElement != null)
            {
                uiElement.GotFocus += (sender, args) =>
                {
                    obj.IsActive = true;
                };
                uiElement.LostFocus += (sender, args) =>
                {
                    obj.IsActive = false;
                };
            }
        }

        #region ITriggerValue

        private bool _isActive;

        /// <summary>
        /// Gets a value indicating whether this trigger is active.
        /// </summary>
        /// <value><c>true</c> if this trigger is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return _isActive; }
            private set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    SetActive(value);
                    IsActiveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="IsActive" /> property has changed.
        /// </summary>
        public event EventHandler IsActiveChanged;

        #endregion ITriggerValue
    }
}
