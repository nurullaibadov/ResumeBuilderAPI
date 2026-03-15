namespace ResumeBuilder.Application.Common.Models;
public class Result
{
    public bool Succeeded { get; private set; }
    public string[] Errors { get; private set; } = Array.Empty<string>();
    private Result(bool succeeded, IEnumerable<string> errors) { Succeeded = succeeded; Errors = errors.ToArray(); }
    public static Result Success() => new(true, Array.Empty<string>());
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
    public static Result Failure(string error) => new(false, new[] { error });
}
public class Result<T>
{
    public bool Succeeded { get; private set; }
    public T? Data { get; private set; }
    public string[] Errors { get; private set; } = Array.Empty<string>();
    public string? Message { get; private set; }
    private Result(bool succeeded, T? data, IEnumerable<string> errors, string? message = null)
    { Succeeded = succeeded; Data = data; Errors = errors.ToArray(); Message = message; }
    public static Result<T> Success(T data, string? message = null) => new(true, data, Array.Empty<string>(), message);
    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
    public static Result<T> Failure(string error) => new(false, default, new[] { error });
}
