namespace WorshipManager.Core.Common;

public class ValidationResult
{
    public bool IsValid { get; }
    public List<string> Errors { get; }

    private ValidationResult(bool isValid, List<string>? errors = null)
    {
        IsValid = isValid;
        Errors = errors ?? new List<string>();
    }

    public static ValidationResult Success() => new(true);
    public static ValidationResult Failure(string error) => new(false, new List<string> { error });
    public static ValidationResult Failure(List<string> errors) => new(false, errors);
}
