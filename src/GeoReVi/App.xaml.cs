using Caliburn.Micro;
using System.Windows;

namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = "Views",
                DefaultSubNamespaceForViewModels = "Core.ViewModels"
            };

            ViewLocator.ConfigureTypeMappings(config);
            ViewModelLocator.ConfigureTypeMappings(config);

            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ((ShellViewModel)IoC.Get<IShell>()).LogError(e.Exception);
            e.Handled = true;
        }
    }
}
