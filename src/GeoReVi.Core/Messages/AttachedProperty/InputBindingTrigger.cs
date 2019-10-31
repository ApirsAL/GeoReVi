using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace GeoReVi
{
    /// <summary>
    /// Class for key input bindings like shortcuts
    /// FROM http://www.felicepollano.com/2011/05/02/InputBindingKeyBindingWithCaliburnMicro.aspx
    /// </summary>
    public class InputBindingTrigger : TriggerBase<FrameworkElement>, ICommand
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public InputBindingTrigger()
        {

        }

        /// <summary>
        /// Property which returns an input binding dependency property of ownertype inputbindingtrigger
        /// </summary>
        public InputBinding InputBinding
        {
            get { return (InputBinding)GetValue(InputBindingProperty); }
            set { SetValue(InputBindingProperty, value); }
        }

        /// <summary>
        /// Dependency property of type input binding
        /// </summary>
        public static readonly DependencyProperty InputBindingProperty =
            DependencyProperty.Register("InputBinding", typeof(InputBinding)
            , typeof(InputBindingTrigger)
            , new UIPropertyMetadata(null));

        /// <summary>
        /// Binds a command to the input binding
        /// </summary>
        protected override void OnAttached()
        {
            if (InputBinding != null)
            {
                InputBinding.Command = this;
                AssociatedObject.InputBindings.Add(InputBinding);
            }
            base.OnAttached();
        }

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            // action is anyway blocked by Caliburn at the invoke level
            return true;
        }
        public event EventHandler CanExecuteChanged = delegate { };

        public void Execute(object parameter)
        {
            InvokeActions(parameter);
        }

        #endregion
    }
}
