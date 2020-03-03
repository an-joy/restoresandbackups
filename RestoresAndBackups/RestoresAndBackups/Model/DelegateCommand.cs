using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestoresAndBackups.Model
{
    public class DelegateCommand : ICommand
    {
        private Action<object> _executeDelegate;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> executeDelegate)
        {
            _executeDelegate = executeDelegate;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _executeDelegate(parameter);
        }
    }
}
