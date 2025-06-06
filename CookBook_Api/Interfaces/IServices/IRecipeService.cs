using CookBook_Api.Common;

namespace CookBook_Api.Interfaces.IServices
{
    public interface IRecipeService
    {
        Result<Uri?> ValidateAndParseUri (string uri);

    }
}
