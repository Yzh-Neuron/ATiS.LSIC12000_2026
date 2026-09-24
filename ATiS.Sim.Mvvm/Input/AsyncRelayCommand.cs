using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;

namespace ATiS.Sim.Mvvm
{
    public class AsyncRelayCommand : ICommand
    {
        public AsyncRelayCommand(Func<Task> execute)
        {
            ArgumentNullException.ThrowIfNull(execute);

            _execute = execute;
        }
        public AsyncRelayCommand(Func<Task>? execute, Func<bool> canExecute)
        {
            ArgumentNullException.ThrowIfNull(canExecute);

            _execute = execute;
            _canExecute = canExecute;
        }

        private Task? _executionTask;
        private readonly Func<Task>? _execute;
        private readonly Func<bool>? _canExecute;
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object? parameter)
        {
            return (_canExecute?.Invoke() != false) && (_executionTask is not { IsCompleted: false });
        }
        public void Execute(object? parameter)
        {
            if (_execute != null)
            {
                _executionTask = _execute.Invoke();
                AwaitAndThrowIfFailed(_executionTask);
            }
        }

        internal static async void AwaitAndThrowIfFailed(Task executionTask)
        {
            await executionTask;
        }
    }
}
