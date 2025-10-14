using System.Text.Json.Serialization;

namespace MediLabo.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }

        [JsonIgnore]
        public bool IsFailure => !IsSuccess;

        public T? Value { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        [JsonIgnore]
        public string? Error => Errors.FirstOrDefault();

        public Result()
        {
        }
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
