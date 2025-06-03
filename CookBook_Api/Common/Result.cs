namespace CookBook_Api.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }
        public T? Value { get; }
        
        protected Result(bool isSuccess, T? value, string? errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Value = value;
        }

        public static Result<T> Success(T value) => new(true, value, errorMessage: null);
        public static Result<T> Fail(string error) => new(false, value: default, error);
    }
}
