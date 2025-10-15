namespace Shared.ResultPattern
{
    public class Result
    {
        public Result(bool isSuccess, Error error)
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

        public Error Error { get; set; }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public static Result Success() => new Result(true, null);

        public static Result Failure(Error error) => new Result(false, error);

        public static implicit operator Result(Error error) => Failure(error);
    }

    public class Result<TValue> : Result
    {
        private readonly TValue _value = default!;

        public Result(TValue value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public TValue Value => IsSuccess
            ? _value
            : throw new InvalidOperationException("Значение некорректного результата не может быть доступно!");

        public static Result<TValue> Success(TValue value) => new(value, true, null);

        public static new Result<TValue> Failure(Error error) => new(default!, false, error);

        public static implicit operator Result<TValue>(TValue value) => Success(value);

        public static implicit operator Result<TValue>(Error error) => new(default!, false, error);

        public static implicit operator TValue(Result<TValue> value) => value._value;
    }
}
