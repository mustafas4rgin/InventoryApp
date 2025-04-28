using InventoryApp.Application.Interfaces;
using InventoryApp.Application.Services;
using InventoryApp.Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryApp.Application.Providers.Service;

public class ServiceRegistrationProvider
{
    public static void RegisterServices(IServiceCollection services)
    {
        var servicesToRegister = new (Type Interface, Type Implementation)[]
        {
            (typeof(IGenericService<>),typeof(GenericService<>)),
            (typeof(IUserService),typeof(UserService)),
            (typeof(INotificationService),typeof(NotificationService)),
            (typeof(IProductService),typeof(ProductService)),
            (typeof(ITokenService),typeof(TokenService)),
            (typeof(IAuthService),typeof(AuthService)),
            (typeof(ISupplierService),typeof(SupplierService))
        };
        foreach (var service in servicesToRegister)
        {
            services.AddTransient(service.Interface, service.Implementation);
        }
    }
}