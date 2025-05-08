using System.Text.Json.Serialization;

namespace KYCVerificationAPI.Features.Vendors.Responses;

[JsonConverter(typeof(JsonStringEnumConverter<KycStatus>))]
public enum KycStatus
{
    Ok = 0,
    Failed = 1,
    Error = 2,
}