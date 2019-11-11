using System;
using System.Linq;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Reflection;

namespace GeoReVi
{
    /// <summary>
    /// The bootstrapper handling initial configuration of the application
    /// </summary>
    public class AppBootstrapper : BootstrapperBase
    {
        private CompositionContainer container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            var batch = new CompositionBatch();

            container = new CompositionContainer(
new AggregateCatalog(
AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
));
            //Registering the interfaces in the Composition Batch
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue<IFileManager>(new FileManager());
            batch.AddExportedValue<ILogFactory>(
                new BaseLogFactory(new ILogger[] 
                {
                    new DebugLogger(),
                    new ConsoleLogger(),
                    new FileLogger(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\GeoReVi\Data\Log.txt")
                }));
            batch.AddExportedValue<IShell>(new ShellViewModel(true));
            batch.AddExportedValue(container);

            //Composing the container with the added interfaces
            container.Compose(batch);

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
        }

        /// <summary>
        /// Getting the instance of a service
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = container.GetExportedValues<object>(contract);

            if (exports.Any())
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
            BindingHelper.EnableNestedViewModelActionBinding();
        }

        /// <summary>
        /// Selecting all assemblies
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
            {
                Assembly.GetExecutingAssembly()
            };
        }

        /// <summary>
        /// Displaying the ShellViewModel on startup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //Let the base application do what it needs
            base.OnStartup(sender, e);

            ((BaseLogFactory)IoC.Get<ILogFactory>()).Log("Application starting up...", LogLevel.Informative);

            ((FileManager)IoC.Get<IFileManager>()).WriteTextToFileAsync("Application starting up...", "test.txt");

            DisplayRootViewFor<IShell>();

        }
    }
}
