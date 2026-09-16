using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ATiS.Sim.Mvvm
{
    public class AsyncRelayCommand<T> : IRelayCommand<T>
    {
        public AsyncRelayCommand(Func<T?, Task> execute)
        {
            ArgumentNullException.ThrowIfNull(execute);

            this.execute = execute;
        }
        public AsyncRelayCommand(Func<T?, Task> execute, Predicate<T?> canExecute)
        {
            ArgumentNullException.ThrowIfNull(execute);
            ArgumentNullException.ThrowIfNull(canExecute);

            this.execute = execute;
            this.canExecute = canExecute;
        }

        private Task? _executionTask;
        private readonly Func<T?, Task> execute;
        private readonly Predicate<T?>? canExecute;
        public event EventHandler? CanExecuteChanged;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(T? parameter)
        {
            bool canExecute = this.canExecute?.Invoke(parameter) != false;
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

            return CanExecute(parameter);
        }
        public void Execute(T? parameter)
        {
            _executionTask = execute(parameter);
            AsyncRelayCommand.AwaitAndThrowIfFailed(_executionTask);
        }
        public void Execute(object? parameter)
        {
            if (!RelayCommand<T>.TryGetCommandArgument(parameter, out T? result))
            {
                RelayCommand<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);
            }

            Execute(result);
        }
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
