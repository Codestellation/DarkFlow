using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Codestellation.DarkFlow.CastleWindsor
{
    public class TaskInstaller : IWindsorInstaller
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public TaskInstaller(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes
                    .From(_assemblies.SelectMany(x => x.GetTypes()))
                    .BasedOn<IPersistentTask>()
                    .WithServiceSelf()
                    .LifestyleTransient());
        }
    }
}