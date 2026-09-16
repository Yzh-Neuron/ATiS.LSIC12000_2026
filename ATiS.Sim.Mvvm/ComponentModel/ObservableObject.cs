using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ATiS.Sim.Mvvm
{
    public class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);

            if (!FeatureSwitches.EnableINotifyPropertyChangingSupport)
            {
                return;
            }

            PropertyChanging?.Invoke(this, e);
        }
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);

            PropertyChanged?.Invoke(this, e);
        }
        protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            if (!FeatureSwitches.EnableINotifyPropertyChangingSupport)
            {
                return;
            }

            OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
        }
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);

            oldValue = newValue;

            OnPropertyChanged(propertyName);

            return true;
        }
    }
}
