using FluentValidation;
using InventoryApp.Application.Providers.Service;
using InventoryApp.Application.Providers.Validator;
using InventoryApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryApp.Application.Registrations;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusinessService(this IServiceCollection services)
    {
        ServiceRegistrationProvider.RegisterServices(services);

        return services;
    }
    public static IServiceCollection AddValidatorService(this IServiceCollection services)
    {
        var validatorAssemblies = ValidatorAssemblyProvider.GetValidatorAssemblies();

        var createDtoValidatorAssemblies = CreateDTOValidatorAssemblyProvider.GetValidatorAssemblies();

        var updateDtoValidatorAssemblies = UpdateDTOAssemblyProvider.GetValidatorAssemblies();

        var authDtoValidatorAssemblies = AuthDTOValidatorAssemblyProvider.GetValidatorAssemblies();

        foreach (var assemblytype in authDtoValidatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblytype);

        foreach (var assemblyType in createDtoValidatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        foreach (var assemblyType in updateDtoValidatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        foreach (var assemblyType in validatorAssemblies)
            services.AddValidatorsFromAssemblyContaining(assemblyType);

        services.AddHostedService<TokenCleanupService>();


        return services;
    }
}