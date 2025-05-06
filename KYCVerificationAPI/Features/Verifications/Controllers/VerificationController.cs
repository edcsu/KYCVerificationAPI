using FluentValidation;
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
    private readonly IValidator<CreateVerification> _createValidator;
    public VerificationController(IVerificationService verificationService, 
        ILogger<VerificationController> logger, IValidator<CreateVerification> createValidator)
    {
        _verificationService = verificationService;
        _logger = logger;
        _createValidator = createValidator;
    }

    [HttpPost]
    [Stability(Stability.Stable)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateVerification request,
        CancellationToken token = default)
    {
        _logger.LogInformation("Creating verification");
        
        var validationResult = await _createValidator.ValidateAsync(request, token);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Invalid verification request");
            return BadRequest(validationResult.ToDictionary());
        }
        
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