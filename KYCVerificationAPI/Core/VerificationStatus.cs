using System.Text.Json.Serialization;

namespace KYCVerificationAPI.Core;

[JsonConverter(typeof(JsonStringEnumConverter<VerificationStatus>))]
public enum VerificationStatus
{
    Pending=0,
    Valid=1,
    Invalid=2,
}