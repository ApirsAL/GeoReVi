using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace GeoReVi
{
    /// <summary>
    /// Interface for an application context
    /// </summary>
    public interface IContext
    {
        bool IsSynchronized { get; }
        void Invoke(Action action);
        void BeginInvoke(Action action);
    }

    /// <summary>
    /// A class to set up a dispatcher
    /// </summary>
    public class DispatchService : IContext
    {
        #region Private member

        private readonly Dispatcher _dispatcher;

        #endregion

        #region  Public properties

        /// <summary>
        /// Check if dispatcher is synchronized
        /// </summary>
        public bool IsSynchronized
        {
            get => this._dispatcher.Thread == Thread.CurrentThread;
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Default
        /// </summary>
        public DispatchService() : this(Dispatcher.CurrentDispatcher)
        {
        }

        /// <summary>
        /// Constructor with certain dispatcher
        /// </summary>
        /// <param name="dispatcher"></param>
        public DispatchService(Dispatcher dispatcher)
        {
            Debug.Assert(dispatcher != null);

            this._dispatcher = dispatcher;
        }

        #endregion
        /// <summary>
        /// Invokes an action
        /// </summary>
        /// <param name="action"></param>
        public void Invoke(Action action)
        {
            Debug.Assert(action != null);

            this._dispatcher.Invoke(action, DispatcherPriority.ContextIdle);
        }

        /// <summary>
        /// Begins to invoke an action
        /// </summary>
        /// <param name="action"></param>
        public void BeginInvoke(Action action)
        {
            Debug.Assert(action != null);

            this._dispatcher.BeginInvoke(action);
        }

    }
}
