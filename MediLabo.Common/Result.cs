namespace MediLabo.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }

        public bool IsFailure => !IsSuccess;

        public T? Value { get; private set; }

        public List<string> Errors { get; private set; }

        public string? Error => Errors.FirstOrDefault();

        private Result(bool isSuccess, T? value, List<string> errors)
        {
            IsSuccess = isSuccess;
            Value = value;
            Errors = errors ?? new List<string>();
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, new List<string>());
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, new List<string> { error });
        }

        public static Result<T> Failure(List<string> errors)
        {
            return new Result<T>(false, default, errors);
        }
    }
}
