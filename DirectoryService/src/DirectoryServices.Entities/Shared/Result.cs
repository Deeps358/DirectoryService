namespace DirectoryServices.Entities.Shared
{
    public class Result
    {
        public Result(bool isSuccess, string error)
        {
            if(isSuccess && error is not null)
            {
                throw new InvalidOperationException();
            }

            if(isSuccess == false && error == null)
            {
                throw new InvalidOperationException();
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public string? Error { get; set; }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public static Result Success() => new Result(true, null);

        public static implicit operator Result(string error) => new(false, error);
    }

    public class Result<TValue> : Result
    {
        private readonly TValue _value;

        public Result(TValue value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public TValue Value => IsSuccess
            ? _value
            : throw new InvalidOperationException("Значение некорректного результата не может быть доступно!");

        public static Result<TValue> Success(TValue value) => new(value, true, null);

        public static Result<TValue> Failure(string error) => new(default!, false, error);

        public static implicit operator Result<TValue>(TValue value) => new(value, true, null);

        public static implicit operator Result<TValue>(string error) => new(default!, false, error);
    }
}
