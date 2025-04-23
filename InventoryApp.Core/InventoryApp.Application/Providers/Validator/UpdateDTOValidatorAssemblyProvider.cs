using InventoryApp.Application.DTOValidators.Update;

namespace InventoryApp.Application.Providers.Validator;

public class UpdateDTOAssemblyProvider
{
    public static Type[] GetValidatorAssemblies()
    {
        return new[]
        {
            typeof(UpdateCategoryDTOValidator),
            typeof(UpdateNotificationDTOValidator),
            typeof(UpdateProductDTOValidator),
            typeof(UpdateRoleDTOValidator),
            typeof(UpdateSupplierDTOValidator),
            typeof(UpdateUserDTOValidator)
        };
    }
}