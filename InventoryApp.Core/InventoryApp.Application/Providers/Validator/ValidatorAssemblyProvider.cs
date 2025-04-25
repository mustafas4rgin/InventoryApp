using InventoryApp.Application.Validators;
using LibraryApp.Application.Validators;

namespace InventoryApp.Application.Providers.Validator;

public class ValidatorAssemblyProvider
{
    public static Type[] GetValidatorAssemblies()
    {
        return new[]
        {
            typeof(UserValidator),
            typeof(RoleValidator),
            typeof(SupplierValidator),
            typeof(NotificationValidator),
            typeof(CategoryValidator),
            typeof(ProductValidator),
            typeof(AccessTokenValidator)
        };
    }
}