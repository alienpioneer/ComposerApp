using System.Windows.Input;

namespace AppComposer.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action callable;
        private readonly Func<bool>? canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute=null)
        {
            this.callable = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute?.Invoke() ?? false;
        }

        public void Execute(object? parameter)
        {
            if (canExecute != null)
            {
                callable();
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}
