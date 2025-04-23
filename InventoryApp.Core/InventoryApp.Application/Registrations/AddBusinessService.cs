using FluentValidation;
using InventoryApp.Application.Providers.Service;
using InventoryApp.Application.Providers.Validator;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryApp.Application.Registrations;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusinessService(this IServiceCollection services)
    {
        ServiceRegistrationProvider.RegisterServices(services);

        var validatorAssemblies = ValidatorAssemblyProvider.GetValidatorAssemblies();

        var createDtoValidatorAssemblies = CreateDTOValidatorAssemblyProvider.GetValidatorAssemblies();

        var updateDtoValidatorAssemblies = UpdateDTOAssemblyProvider.GetValidatorAssemblies();

        foreach (var assemblyType in createDtoValidatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        foreach (var assemblyType in updateDtoValidatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        foreach (var assemblyType in validatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        
        
        return services;
    }
}