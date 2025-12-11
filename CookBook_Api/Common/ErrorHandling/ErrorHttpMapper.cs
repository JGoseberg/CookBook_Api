using CookBook_Api.Enums;

namespace CookBook_Api.Common.ErrorHandling
{
    public static class ErrorHttpMapper
    {
        public static (int StatusCode, ErrorResponse) Map (Error error)
        {
            int statusCode = error.Code switch
            {
                ErrorCode.RecipeNotFound    => StatusCodes.Status404NotFound,
                ErrorCode.UnexpectedError   => StatusCodes.Status500InternalServerError,
                _                           => StatusCodes.Status500InternalServerError
            };

            var response = new ErrorResponse(statusCode.ToString(), error.Message);

            return (statusCode, response);
        }
    }
}
