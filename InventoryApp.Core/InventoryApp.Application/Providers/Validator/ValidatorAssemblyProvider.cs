using InventoryApp.Application.Validators;
using LibraryApp.Application.Validators;

namespace InventoryApp.Application.Providers.Validator;

public class ValidatorAssemblyProvider
{
    public static Type[] GetValidatorAssemblies()
    {
        return new[]
        {
            typeof(CategoryValidator),
            typeof(NotificationValidator),
            typeof(ProductValidator),
            typeof(RoleValidator),
            typeof(SupplierValidator),
            typeof(UserValidator)
        };
    }
}