

using Microsoft.Extensions.DependencyInjection;

namespace Cross.Cutting.IoC
{
    public static class IoCExtension
    {
        public static void AddServicesAndRepositories(this IServiceCollection services)
        {
            AddServices(services);
            AddRepositories(services);
        }

        public static void AddServices(IServiceCollection services)
        {

        }

        public static void AddRepositories(IServiceCollection services)
        {

        }
    }
}
