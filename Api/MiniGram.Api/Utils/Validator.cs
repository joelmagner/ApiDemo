namespace MiniGram.Api.Utils;

public interface IValidator
{
    IValidator Validate<T>(T input);
    bool IsValid { get; }
    IReadOnlyDictionary<string, string[]> Errors { get; }
    void Check(string? value, string fieldName, string errorMessage, Func<string, bool>? predicate = null);
    void Check(byte[]? value, string fieldName, string errorMessage, Func<byte[]?, bool>? predicate = null);
}

public class Validator : IValidator
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public bool IsValid => !_errors.Any();

    public IReadOnlyDictionary<string, string[]> Errors =>
        _errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());

    public IValidator Validate<T>(T input)
    {
        _errors.Clear(); // Reset state per validation
        return this;
    }

    public void Check(string? value, string fieldName, string errorMessage, Func<string, bool>? validateFn = null)
    {
        if (string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value))
        {
            AddError(fieldName, errorMessage);
            return;
        }

        if (validateFn != null && !validateFn(value))
        {
            AddError(fieldName, errorMessage);
        }
    }
    
    public void Check(byte[]? value, string fieldName, string errorMessage, Func<byte[]?, bool>? validateFn = null)
    {
        if (value == null || value.Length == 0)
        {
            AddError(fieldName, errorMessage);
            return;
        }

        if (validateFn != null && !validateFn(value))
        {
            AddError(fieldName, errorMessage);
        }
    }

    void AddError(string field, string error)
    {
        if (!_errors.ContainsKey(field))
            _errors[field] = [];

        _errors[field].Add(error);
    }
}
