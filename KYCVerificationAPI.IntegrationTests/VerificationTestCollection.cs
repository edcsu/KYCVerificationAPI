namespace KYCVerificationAPI.IntegrationTests;

// Ensure you have this collection definition
[CollectionDefinition(nameof(VerificationTestCollection))]
public class VerificationTestCollection : ICollectionFixture<SharedFixture>
{
    // This class is empty but required for the CollectionDefinition attribute
}