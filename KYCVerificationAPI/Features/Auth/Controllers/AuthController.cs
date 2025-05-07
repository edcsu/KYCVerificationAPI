using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Asp.Versioning;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Features.Auth.Requests;
using KYCVerificationAPI.Features.Auth.Responses;
using KYCVerificationAPI.Features.Verifications.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

namespace KYCVerificationAPI.Features.Auth.Controllers;

[Route("api/auth")]
[ApiController]
[ApiVersion(1)]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class AuthController: ControllerBase
{
    
    [HttpPost("token")]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(TokenResponse),StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType( StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Generates an access token.")]
    [EndpointDescription("Get an access token to use the API")]
    public IActionResult GenerateToken([FromBody]TokenGenerationRequest request)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(ApiConstants.TokenSecret);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Email),
            new(JwtRegisteredClaimNames.Email, request.Email),
            new("userid", request.UserId.ToString())
        };
        claims.AddRange(from claimPair in request.CustomClaims
            let jsonElement = (JsonElement)claimPair.Value
            let valueType = jsonElement.ValueKind switch
            {
                JsonValueKind.True => ClaimValueTypes.Boolean,
                JsonValueKind.False => ClaimValueTypes.Boolean,
                JsonValueKind.Number => ClaimValueTypes.Double,
                _ => ClaimValueTypes.String
            }
            select new Claim(claimPair.Key, claimPair.Value.ToString()!, valueType));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(ApiConstants.TokenLifetime),
            Issuer = "https://auth.ugverify.com",
            Audience = "https://kyc.ugverify.com",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwt = tokenHandler.WriteToken(token);

        var tokenResponse = new TokenResponse
        {
            AccessToken = jwt,
            ExpiresIn = ApiConstants.TokenLifetime.TotalMinutes,
        };
        
        return Ok(tokenResponse);
    }
}