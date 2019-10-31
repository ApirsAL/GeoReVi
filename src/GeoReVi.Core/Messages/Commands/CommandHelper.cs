﻿
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class CommandHelper
    {
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
    }
}
