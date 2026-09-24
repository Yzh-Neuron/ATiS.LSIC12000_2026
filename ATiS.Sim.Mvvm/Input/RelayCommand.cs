using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ATiS.Sim.Mvvm
{
    public class RelayCommand : ICommand
    {
        public RelayCommand(Action execute)
        {
            ArgumentNullException.ThrowIfNull(execute);
            _execute = execute;
        }
        public RelayCommand(Action? execute, Func<bool> canExecute)
        {
            ArgumentNullException.ThrowIfNull(canExecute);

            _execute = execute;
            _canExecute = canExecute;
        }

        private Action? _execute;
        private Func<bool>? _canExecute;
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() != false;
        }
        public void Execute(object? parameter)
        {
            _execute?.Invoke();
        }
    }
}
