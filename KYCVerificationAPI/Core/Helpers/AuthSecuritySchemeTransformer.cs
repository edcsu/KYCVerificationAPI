using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace KYCVerificationAPI.Core.Helpers;

public sealed class AuthSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        var requirements = new Dictionary<string, OpenApiSecurityScheme>();

        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            requirements.Add("Bearer",  new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // "bearer" refers to the header name here
                In = ParameterLocation.Header,
                BearerFormat = "Json Web Token"
            });
        }

        document.Components ??= new OpenApiComponents();
        document.Info.Description = "This KYC verification REST API uses JWT access tokens for secure access. " +
                                    "It offers endpoints to submit KYC data and check verification status " + 
                                    "<h2>Authentication and Authorization </h2> " +
                                    "<p>Clients need to generate an access token, " +
                                    "which they have to include in the Authorization header for subsequent requests." +
                                    "Check out out the <strong>auth section</strong> in the left side bar for more details." +
                                    "The server validates the JWT to authenticate and authorize users, ensuring they can only access their own KYC data. Security is paramount, involving HTTPS, token expiration, input validation, access control, and data encryption. </p>";
        document.Info.Contact = new OpenApiContact 
        {
            Name = "Ssewaannonda Keith edwin",
            Email = "skeith@696.gmail",
        };
        document.Components.SecuritySchemes = requirements;
    }
}
