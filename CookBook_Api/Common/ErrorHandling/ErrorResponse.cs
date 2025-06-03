namespace CookBook_Api.Common.ErrorHandling
{
    public record ErrorResponse (string Code, string Message)
    {
        public static ErrorResponse CreateFromError(Error error) =>
            new(error.Code, error.Message);
    }
}
