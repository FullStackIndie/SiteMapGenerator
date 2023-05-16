using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace SiteMapGenerator.DependencyInjection
{
    public class ServiceBuilder : ITypeRegistrar
    {
        private readonly IServiceCollection _services;

        public ServiceBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public ITypeResolver Build()
        {
            return new TypeResolver(_services.BuildServiceProvider());
        }

        public void Register(Type service, Type implementation)
        {
            _services.AddSingleton(service, implementation);
        }

        public void RegisterInstance(Type service, object implementation)
        {
            _services.AddSingleton(service, implementation);
        }

        public void RegisterLazy(Type service, Func<object> func)
        {
            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            _services.AddSingleton(service, (provider) => func());
        }
    }
}
