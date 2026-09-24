using System.Windows.Input;

namespace ATiS.Sim.Mvvm
{
    public interface IRelayCommand<in T> : ICommand
    {
        bool CanExecute(T? parameter);

        void Execute(T? parameter);
    }
}
