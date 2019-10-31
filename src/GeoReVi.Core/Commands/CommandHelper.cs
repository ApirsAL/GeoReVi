
using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class CommandHelper
    {
        /// <summary>
        /// Cancellation token source to abort tasks
        /// </summary>
        CancellationTokenSource cts;

        /// <summary>
        /// Runs a command if the updating flag is not set
        /// If the flag is true (indicating the functions is already running) then the action is not run
        /// If the flas is false (indication no running function) then the action is run.
        /// Once the action is finished if it was run, then the flag is reset to false
        /// </summary>
        /// <param name="updatingFlag">The boolean property flag defining if the command is already running</param>
        /// <param name="action">The action to run if the command is not already running</param>
        /// <returns></returns>
        public async Task RunCommand(Expression<Func<bool>> updatingFlag, Func<Task> action)
        {
            // Check if the flag property is true (meaning the function is already running)
            if (updatingFlag.GetPropertyValue())
            {
                return;
            }
            //Set the property flag to true to indicate we are running
            updatingFlag.SetPropertyValue(true);

            try
            {
                //Run the passed in action
                await action();
            }
            finally
            {
                //Set the property flag back to false now it's finished
                updatingFlag.SetPropertyValue(false);
            }

        }

        /// <summary>
        /// Runs a command if the updating flag is not set
        /// If the flag is true (indicating the functions is already running) then the action is not run
        /// If the flas is false (indication no running function) then the action is run.
        /// Once the action is finished if it was run, then the flag is reset to false
        /// </summary>
        /// <param name="updatingFlag">The boolean property flag defining if the command is already running</param>
        /// <param name="action">The action to run if the command is not already running</param>
        /// <returns></returns>
        public async Task RunBackgroundWorkerWithFlagHelperAsync(Expression<Func<bool>> updatingFlag, Func<Task> action)
        {
            
            // Check if the flag property is true (meaning the function is already running)
            if (updatingFlag.GetPropertyValue())
            {
                return;
            }

            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }

            cts = new CancellationTokenSource();
            ((ShellViewModel)IoC.Get<IShell>()).Cts.Add(cts);

            //Set the property flag to true to indicate we are running
            updatingFlag.SetPropertyValue(true);

            //Run the passed in action
            try
            {
                BackgroundWorker bw = new BackgroundWorker();

                bw.DoWork += ((sender1, args) =>
                {
                    new DispatchService().Invoke(
                    async () =>
                    {

                        await action();

                    });
                });


                bw.RunWorkerCompleted += ((sender1, args) =>
                {
                    if (args.Error != null)  // if an exception occurred during DoWork,
                        ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(args.Error.ToString());

                });

                await bw.RunWorkerTaskAsync().ConfigureAwait(true); // start the background worker
            }
            finally
            {
                //Set the property flag back to false now it's finished
                updatingFlag.SetPropertyValue(false);
            }
        }

        /// <summary>
        /// Runs a command if the updating flag is not set
        /// If the flag is true (indicating the functions is already running) then the action is not run
        /// If the flas is false (indication no running function) then the action is run.
        /// Once the action is finished if it was run, then the flag is reset to false
        /// </summary>
        /// <param name="updatingFlag">The boolean property flag defining if the command is already running</param>
        /// <param name="action">The action to run if the command is not already running</param>
        /// <returns></returns>
        public async Task RunBackgroundWorkerHelperAsync(Func<Task> action)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }

            cts = new CancellationTokenSource();
            ((ShellViewModel)IoC.Get<IShell>()).Cts.Add(cts);

            try
            {
                BackgroundWorker bw = new BackgroundWorker();

                bw.DoWork += ((sender1, args) =>
                {

                    new DispatchService().Invoke(
                    async () =>
                    {

                        //Run the passed in action
                        await action();

                    });
                });


                bw.RunWorkerCompleted += ((sender1, args) =>
                {
                    if (args.Error != null)  // if an exception occurred during DoWork,
                        ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(args.Error.ToString());

                });

                await bw.RunWorkerTaskAsync(); // start the background worker

            }
            catch
            {
                cts.Cancel();
                cts=null;
            }
        }
    }
}
