namespace BackEnd
{

    public enum ResultCode
    {
        IsOk,
        Conflict,
        InternalError,
        NotFound,
    }

    public enum ConflictCode
    {
        NoConflict,
        Modified,
        Deleted,
        Created
    }
    
    public class Result<T>
    {
        public T TheResult { get; set; }
        
        public ResultCode Code {get; set; }

        public ConflictCode ConflictCode { get; set; }
        
        public bool IsOk => Code == ResultCode.IsOk;
        
        public string ErrorMessage { get; set; }

        public static implicit operator Result<T>(T t)
        {
            return new Result<T>()
            {
                TheResult = t,
                Code = ResultCode.IsOk
            };
        }

        public static implicit operator T(Result<T> t)
        {
            return t.TheResult;
        }

        public static Result<T> Error(ResultCode code, ConflictCode conflictCode, string errorMessage)
        {
            return new Result<T>()
            {
                TheResult = default(T),
                Code = code,
                ConflictCode = conflictCode,
                ErrorMessage = errorMessage
            };
        }
        
        public static Result<T> Error(ResultCode code,  string errorMessage)
        {
            return new Result<T>()
            {
                TheResult = default(T),
                Code = code,
                ConflictCode = ConflictCode.NoConflict,
                ErrorMessage = errorMessage
            };
        }

        public static Result<T> Ok(T result = default)
        {
            return new Result<T>()
            {
                Code = ResultCode.IsOk,
                ConflictCode = ConflictCode.NoConflict,
                ErrorMessage = null,
                TheResult = result
            };
        }

        public static Result<T> TransformError<X, T>(Result<X> x)
        {
            return Result<T>.Error(x.Code, x.ConflictCode, x.ErrorMessage);
        }
    }
}