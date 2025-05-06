using KYCVerificationAPI.Core;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;
using KYCVerificationAPI.Features.Verifications.Service;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

namespace KYCVerificationAPI.Features.Verifications.Controllers;

[Route("api/verifications")]
[ApiController]
public class VerificationController : ControllerBase
{
    private readonly IVerificationService _verificationService;
    private readonly ILogger<VerificationController> _logger;

    public VerificationController(IVerificationService verificationService, 
        ILogger<VerificationController> logger)
    {
        _verificationService = verificationService;
        _logger = logger;
    }

    [HttpPost]
    [Stability(Stability.Stable)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateVerification request,
        CancellationToken token = default)
    {
        _logger.LogInformation("Creating verification");
        
        var result = await _verificationService.CreateAsync(request, token);
        var pendingResponse = new PendingResponse
        {
            Code = 201,
            Status = VerificationStatus.Pending,
            CreatedAt = DateTime.Now,
            TransactionId = result
        };
        
        _logger.LogInformation("Finished creating a verification");
        
        return Created(string.Empty, pendingResponse);
    }
}