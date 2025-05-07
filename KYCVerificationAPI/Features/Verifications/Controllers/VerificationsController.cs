using System.Net.Mime;
using Asp.Versioning;
using FluentValidation;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Core.Helpers;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;
using KYCVerificationAPI.Features.Verifications.Service;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;

namespace KYCVerificationAPI.Features.Verifications.Controllers;

[Route("api/verifications")]
[ApiController]
[ApiVersion(1)]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class VerificationsController : ControllerBase
{
    private readonly IVerificationService _verificationService;
    private readonly ILogger<VerificationsController> _logger;
    private readonly IValidator<CreateVerification> _createValidator;
    private readonly ICorrelationIdGenerator _correlationIdGenerator;
    
    public VerificationsController(IVerificationService verificationService, 
        ILogger<VerificationsController> logger, 
        IValidator<CreateVerification> createValidator, 
        ICorrelationIdGenerator correlationIdGenerator)
    {
        _verificationService = verificationService;
        _logger = logger;
        _createValidator = createValidator;
        _correlationIdGenerator = correlationIdGenerator;
    }

    [HttpPost]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(PendingResponse),StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
    [ProducesResponseType( StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new verification.")]
    [EndpointDescription("Creates a new verification request asynchronously.")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateVerification request,
        CancellationToken token = default)
    {
        var correlationId = _correlationIdGenerator.Get();
        ILogEventEnricher[] enrichers =
        {
            new PropertyEnricher("CorrelationId", correlationId.ToString())
        };
        
        using (LogContext.Push(enrichers))
        {
            _logger.LogInformation("Creating verification");
            var validationResult = await _createValidator.ValidateAsync(request, token);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Invalid verification request");
                return BadRequest(validationResult.ToDictionary());
            }

            var result = await _verificationService.CreateAsync(request, correlationId, token);
            var pendingResponse = new PendingResponse
            {
                Code = 201,
                Status = VerificationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                TransactionId = result
            };

            _logger.LogInformation("Finished creating a verification");
            return Created($"api/verifications/{result}", pendingResponse);
        }
    }
    
    [HttpGet("{id:Guid}", Name = "GetById")]
    [ProducesResponseType(typeof(VerificationResponse), StatusCodes.Status200OK,MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Returns verification details")]
    [EndpointDescription("Returns details of a given verification request")]
    [Stability(Stability.Stable)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,
        CancellationToken token = default)
    {
        var correlationId = _correlationIdGenerator.Get();
        ILogEventEnricher[] enrichers =
        {
            new PropertyEnricher("CorrelationId", correlationId.ToString())
        };

        using (LogContext.Push(enrichers))
        {
            if (id == Guid.Empty)
            {
                _logger.LogInformation("Invalid verification Id");
                return BadRequest();
            }

            _logger.LogInformation("Getting verification");
            var response = await _verificationService.GetByIdAsync(id, token);

            if (response is null)
            {
                _logger.LogInformation("Verification with {VerificationId}not found", id);
                return NotFound();
            }

            _logger.LogInformation("Finished getting verification");
            return Ok(response);
        }
    }
}