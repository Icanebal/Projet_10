namespace Patients.API.Utilities
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;
        public T? Value { get; private set; }
        public string Error { get; private set; } = string.Empty;

        private Result(bool isSuccess, T? value, string error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new(true, value, string.Empty);
        public static Result<T> Failure(string error) => new(false, default, error);
    }
}