using System.Net;

namespace SsoCore.Domain.Common
{
    public class Result<T>
    {
        protected Result(bool isSuccess, Error error, T? data, string message, HttpStatusCode statusCode)
        {
            IsSuccess = isSuccess;
            Error = isSuccess ? null : error;
            Data = data;
            Message = message;
            StatusCode = isSuccess ? (int)statusCode : error.statusCode;
        }

        public string Message { get; set;  }
        public bool IsSuccess { get; set;  }
        public Error? Error { get; set;  }
        public T? Data { get; set;  }
        public int StatusCode { get; set;  }
        public static Result<T> Success(T data, string message = "Successful", HttpStatusCode statusCode = HttpStatusCode.OK) => new(true, Error.None, data, message, statusCode);
        public static Result<T> Fail(Error error, HttpStatusCode statusCode = HttpStatusCode.BadRequest) => new(false, error, default, error.Description, statusCode);
    }

    public class Result : Result<string>
    {
        protected Result(bool isSuccess, Error error, string? data, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(isSuccess, error, data, message, statusCode)
        {
        }

        public static Result Success(string data, string message = "Successful") => new(true, Error.None, data, message);
        public static Result Fail(Error error) => new(false, error, null, error.Description, (HttpStatusCode)error.statusCode);
    }
}
