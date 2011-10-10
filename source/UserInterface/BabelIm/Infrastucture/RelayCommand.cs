using System;
using System.Diagnostics;
using System.Windows.Input;

namespace BabelIm.Infrastructure {
    /// <summary>
    ///   A command whose sole purpose is to 
    ///   relay its functionality to other
    ///   objects by invoking delegates. The
    ///   default return value for the CanExecute
    ///   method is 'true'.
    /// </summary>
    public class RelayCommand<T> : ICommand {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public RelayCommand(Action<T> execute)
            : this(execute, null) {
        }

        /// <summary>
        ///   Creates a new command.
        /// </summary>
        /// <param name = "execute">The execution logic.</param>
        /// <param name = "canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute) {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) {
            return _canExecute == null ? true : _canExecute((T) parameter);
        }

        public event EventHandler CanExecuteChanged {
            add {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter) {
            _execute((T) parameter);
        }

        #endregion
    }

    /// <summary>
    ///   A command whose sole purpose is to 
    ///   relay its functionality to other
    ///   objects by invoking delegates. The
    ///   default return value for the CanExecute
    ///   method is 'true'.
    /// </summary>
    public class RelayCommand : ICommand {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        /// <summary>
        ///   Creates a new command that can always execute.
        /// </summary>
        /// <param name = "execute">The execution logic.</param>
        public RelayCommand(Action execute)
            : this(execute, null) {
        }

        /// <summary>
        ///   Creates a new command.
        /// </summary>
        /// <param name = "execute">The execution logic.</param>
        /// <param name = "canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<bool> canExecute) {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) {
            return _canExecute == null ? true : _canExecute();
        }

        public event EventHandler CanExecuteChanged {
            add {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter) {
            _execute();
        }

        #endregion
    }
}