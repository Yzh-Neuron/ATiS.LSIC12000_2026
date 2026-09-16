using System.Windows.Input;

namespace ATiS.Sim.Mvvm
{
    public interface IRelayCommand : ICommand
    {
        void NotifyCanExecuteChanged();
    }
}

