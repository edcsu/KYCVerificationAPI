namespace KYCVerificationAPI.Core.Helpers;

public interface ICorrelationIdGenerator
{
    string Get();

    void Set(string correlationId);
}


public class CorrelationIdGenerator : ICorrelationIdGenerator
{
    private string _correlationId = Guid.NewGuid().ToString();

    public string Get() => _correlationId;

    public void Set(string correlationId)
    {
        _correlationId = correlationId;
    }
}