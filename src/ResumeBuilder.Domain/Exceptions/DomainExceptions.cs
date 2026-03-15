namespace ResumeBuilder.Domain.Exceptions;
public class NotFoundException : Exception
{
    public NotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.") { }
    public NotFoundException(string message) : base(message) { }
}
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("Access is forbidden.") { }
    public ForbiddenAccessException(string message) : base(message) { }
}
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }
    public ValidationException() : base("One or more validation failures have occurred.")
    { Errors = new Dictionary<string, string[]>(); }
    public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures) : this()
    { Errors = failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage).ToDictionary(g => g.Key, g => g.ToArray()); }
}
public class UnauthorizedException : Exception
{
    public UnauthorizedException() : base("Unauthorized.") { }
    public UnauthorizedException(string message) : base(message) { }
}
public class BadRequestException : Exception { public BadRequestException(string message) : base(message) { } }
public class ConflictException : Exception { public ConflictException(string message) : base(message) { } }
