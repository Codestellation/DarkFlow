using System;
using System.Linq;
using Castle.MicroKernel;
using NLog;

namespace Codestellation.DarkFlow.CastleWindsor.Impl
{
    public class TaskHandlerSelector : IHandlerSelector
    {
        private readonly IKernel _kernel;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public TaskHandlerSelector(IKernel kernel)
        {
            _kernel = kernel;
        }

        public bool HasOpinionAbout(string key, Type service)
        {
            return typeof(ITask).IsAssignableFrom(service) && typeof(ITask) != service;
        }

        public IHandler SelectHandler(string key, Type service, IHandler[] handlers)
        {
            Logger.Debug("Resolving task of type '{0}'", service);


            var result = handlers.SingleOrDefault(x => x.ComponentModel.Implementation == service);

            if (result == null)
            {
                result = _kernel.GetHandlers(typeof (ITask)).Single(x => x.ComponentModel.Implementation == service);
            }

            return result;
        }
    }
}