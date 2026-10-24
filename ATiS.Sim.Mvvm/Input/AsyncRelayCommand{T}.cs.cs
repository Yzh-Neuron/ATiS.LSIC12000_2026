using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ATiS.Sim.Mvvm
{
    public class AsyncRelayCommand<T> : IRelayCommand<T>
    {
        public AsyncRelayCommand(Func<T?, Task> execute)
        {
            ArgumentNullException.ThrowIfNull(execute);

            _execute = execute;
        }
        public AsyncRelayCommand(Func<T?, Task>? execute, Predicate<T?> canExecute)
        {
            ArgumentNullException.ThrowIfNull(canExecute);

            _execute = execute;
            _canExecute = canExecute;
        }

        private Task? _executionTask;
        private readonly Func<T?, Task>? _execute;
        private readonly Predicate<T?>? _canExecute;
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(T? parameter)
        {
            bool canExecute = this._canExecute?.Invoke(parameter) != false;
            return canExecute && (_executionTask is not { IsCompleted: false });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object? parameter)
        {
            if (parameter is null && default(T) is not null)
            {
                return false;
            }

            if (!RelayCommand<T>.TryGetCommandArgument(parameter, out T? result))
            {
                RelayCommand<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);
            }

            return CanExecute(result);
        }
        public void Execute(T? parameter)
        {
            if (_execute != null)
            {
                _executionTask = _execute.Invoke(parameter);
                AsyncRelayCommand.AwaitAndThrowIfFailed(_executionTask);
            }
        }
        public void Execute(object? parameter)
        {
            if (!RelayCommand<T>.TryGetCommandArgument(parameter, out T? result))
            {
                RelayCommand<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);
            }

            Execute(result);
        }
    }
}
