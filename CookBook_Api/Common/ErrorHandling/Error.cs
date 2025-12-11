using CookBook_Api.Enums;

namespace CookBook_Api.Common.ErrorHandling
{
    public class Error
    {
        public ErrorCode Code { get; }
        public string Message { get; }

        public Error(ErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
