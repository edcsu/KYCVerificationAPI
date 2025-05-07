using System.Text.Json.Serialization;

namespace KYCVerificationAPI.Core;

[JsonConverter(typeof(JsonStringEnumConverter<VerificationStatus>))]
public enum VerificationStatus
{
    Pending=0,
    Success=1,
    Failed=2,
}