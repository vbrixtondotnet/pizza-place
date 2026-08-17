namespace PizzaPlace.Application.Common;

public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public ValidationException(string field, string message)
        : this(new Dictionary<string, string[]> { [field] = [message] })
    {
    }
}
