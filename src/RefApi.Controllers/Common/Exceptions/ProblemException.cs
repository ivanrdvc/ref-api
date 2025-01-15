namespace RefApi.Controllers.Common.Exceptions;

[Serializable]
public class ProblemException(string error, string errorMessage) : Exception(errorMessage)
{
    public string Error { get; } = error;
    public string ErrorMessage { get; } = errorMessage;
}