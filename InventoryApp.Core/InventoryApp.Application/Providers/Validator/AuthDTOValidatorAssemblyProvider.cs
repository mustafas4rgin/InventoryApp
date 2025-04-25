using InventoryApp.Application.DTOValidators;

namespace InventoryApp.Application.Providers.Validator;

public class AuthDTOValidatorAssemblyProvider
{
    public static Type[] GetValidatorAssemblies()
    {
        return new[]
        {
            typeof(LoginDTOValidator),
            typeof(CreateTokenDTOValidator),
            typeof(UpdateTokenDTOValidator)
        };
    }
}