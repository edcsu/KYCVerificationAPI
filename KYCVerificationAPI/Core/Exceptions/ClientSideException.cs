namespace KYCVerificationAPI.Core.Exceptions;

public class ClientSideException : Exception
{
    protected ClientSideException(string message) : base(message)
    {
    }
}

public class ClientFriendlyException(string message) : ClientSideException(message);
public class NotFoundException(string message) : ClientSideException(message);