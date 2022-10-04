

using Domain.Enumerables;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Services;
using Infra.Repositories;
using Microsoft.AspNetCore.Identity;
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

            services.AddScoped<AuthenticationService>();
            services.AddScoped<ContactService>();
            services.AddScoped<ProductService>();
        }

        public static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserStore<AppUser>, UserRepository>();
            services.AddScoped<IRoleStore<ERoles>, RoleRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }
    }
}
