using InventoryApp.Application.DTOValidators.Create;
using InventoryApp.Application.Providers.DTOValidators.Create;

namespace InventoryApp.Application.Providers.Validator;

public class CreateDTOValidatorAssemblyProvider
{
    public static Type[] GetValidatorAssemblies()
    {
        return new[]
        {
            typeof(CreateUserDTOValidator),
            typeof(CreateCategoryDTOValidator),
            typeof(CreateNotificationDTOValidator),
            typeof(CreateProductDTOValidator),
            typeof(CreateSupplierDTOValidator),
            typeof(CreateRoleDTOValidator)
        };
    }
}