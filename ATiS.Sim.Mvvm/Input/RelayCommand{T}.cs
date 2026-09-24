using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ATiS.Sim.Mvvm
{
    public class RelayCommand<T> : IRelayCommand<T>
    {
        public RelayCommand(Action<T?> execute)
        {
            ArgumentNullException.ThrowIfNull(execute);

            _execute = execute;
        }
        public RelayCommand(Action<T?>? execute, Predicate<T?> canExecute)
        {
            ArgumentNullException.ThrowIfNull(canExecute);

            _execute = execute;
            _canExecute = canExecute;
        }

        private Action<T?>? _execute;
        private Predicate<T?>? _canExecute;
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(T? parameter)
        {
            return _canExecute?.Invoke(parameter) != false;
        }
        public bool CanExecute(object? parameter)
        {
            if (parameter is null && default(T) is not null)
            {
                return false;
            }

            if (!TryGetCommandArgument(parameter, out T? result))
            {
                ThrowArgumentExceptionForInvalidCommandArgument(parameter);
            }

            return CanExecute(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute(T? parameter)
        {
            _execute?.Invoke(parameter);
        }
        public void Execute(object? parameter)
        {
            if (!TryGetCommandArgument(parameter, out T? result))
            {
                ThrowArgumentExceptionForInvalidCommandArgument(parameter);
            }

            Execute(result);
        }
  
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryGetCommandArgument(object? parameter, out T? result)
        {
            if (parameter is null && default(T) is null)
            {
                result = default;

                return true;
            }

            if (parameter is T argument)
            {
                result = argument;

                return true;
            }

            result = default;

            return false;
        }

        [DoesNotReturn]
        internal static void ThrowArgumentExceptionForInvalidCommandArgument(object? parameter)
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            static Exception GetException(object? parameter)
            {
                if (parameter is null)
                {
                    return new ArgumentException($"Parameter \"{nameof(parameter)}\" (object) must not be null, as the command type requires an argument of type {typeof(T)}.", nameof(parameter));
                }

                return new ArgumentException($"Parameter \"{nameof(parameter)}\" (object) cannot be of type {parameter.GetType()}, as the command type requires an argument of type {typeof(T)}.", nameof(parameter));
            }

            throw GetException(parameter);
        }
    }
}
