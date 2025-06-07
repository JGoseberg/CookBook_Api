using CookBook_Api.Common.ErrorHandling;

namespace CookBook_Api.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public List<Error> Warnings { get; } = new();
        public Error? Error { get; }
        
        protected Result(bool isSuccess, T? value, Error? error)
        {
            IsSuccess = isSuccess;
            Error = error;
            Value = value;
        }

        public static Result<T> Fail(Error error) => new(false, value: default, error);
        public static Result<T> Success(T value) => new(true, value, error: null);
        public static Result<T> SuccessWithWarnings(T value, List<Error> warnings)
        {
            var result = new Result<T>(true, value, null);
            result.Warnings.AddRange(warnings);
            return result;
        }
    }
}
