using System;
using System.Windows.Input;

namespace GeoReVi.Commands
{
    public class RelayCommand : ICommand
    {
        #region Private members

        /// <summary>
        /// The action to run
        /// </summary>
        private Action _action { get; set; }

        #endregion

        #region Public events
        /// <summary>
        /// The event thats fired wehn the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RelayCommand(Action action)
        {
            this._action = action;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }


        public void Execute(object parameter)
        {
            _action();
        }

        #endregion
    }
}
