﻿using System.Runtime.CompilerServices;

namespace TvProgViewer.Core.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the TvProg engine.
    /// </summary>
    public partial class EngineContext
    {
        #region Methods

        /// <summary>
        /// Create a static instance of the TvProg engine.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Create()
        {
            //create TvProgEngine as engine
            return Singleton<IEngine>.Instance ?? (Singleton<IEngine>.Instance = new TvProgEngine());
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton TvProg engine used to access TvProg services.
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Create();
                }

                return Singleton<IEngine>.Instance;
            }
        }

        #endregion
    }
}
