using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ATiS.Sim.Mvvm
{
    public class AsyncRelayCommand : IRelayCommand
    {
        public AsyncRelayCommand(Func<Task> execute)
        {
            ArgumentNullException.ThrowIfNull(execute);

            this.execute = execute;
        }
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
        {
            ArgumentNullException.ThrowIfNull(execute);
            ArgumentNullException.ThrowIfNull(canExecute);

            this.execute = execute;
            this.canExecute = canExecute;
        }

        private Task? _executionTask;
        private readonly Func<Task> execute;
        private readonly Func<bool>? canExecute;
        public event EventHandler? CanExecuteChanged;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object? parameter)
        {
            return (canExecute?.Invoke() != false) && (_executionTask is not { IsCompleted: false });
        }
        public void Execute(object? parameter)
        {
            _executionTask = execute();
            AwaitAndThrowIfFailed(_executionTask);
        }
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        internal static async void AwaitAndThrowIfFailed(Task executionTask)
        {
            await executionTask;
        }
    }
}
