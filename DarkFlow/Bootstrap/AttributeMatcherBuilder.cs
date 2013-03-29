using System;
using System.Linq;
using System.Reflection;
using Codestellation.DarkFlow.Matchers;

namespace Codestellation.DarkFlow.Bootstrap
{
    public class AttributeMatcherBuilder : StaticExecutorNameBuilder
    {
        private string[] _attributeNames;
        private Assembly _assembly;

        public override IMatcher Build()
        {
            return new AttributeMatcher(ExecutorName, AttributeTypes);
        }

        public Type[] AttributeTypes { get; set; }

        public AttributeMatcherBuilder Match(string attributes)
        {
            _attributeNames = attributes.Split(',').Select(x => x.Trim()).ToArray();
            TryParse();
            return this;
        }

        public AttributeMatcherBuilder FromAssembly(string assemblyName)
        {
            _assembly = Assembly.Load(assemblyName);
            TryParse();
            return this;
        }

        private void TryParse()
        {
            if (_attributeNames == null) return;
            if (_assembly == null) return;

            AttributeTypes = _assembly
                .GetTypes()
                .Where(x => _attributeNames.Any( attrib => attrib.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase)))
                .ToArray();
        }
    }
}