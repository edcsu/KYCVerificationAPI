using System.Net.Mime;
using System.Security.Claims;
using Asp.Versioning;
using FluentValidation;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Core.Helpers;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;
using KYCVerificationAPI.Features.Verifications.Service;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IValidator<VerificationFilter> _verificationFilterValidator;
    private readonly ICorrelationIdGenerator _correlationIdGenerator;
    
    public VerificationsController(IVerificationService verificationService, 
        ILogger<VerificationsController> logger, 
        IValidator<CreateVerification> createValidator, 
        ICorrelationIdGenerator correlationIdGenerator, IValidator<VerificationFilter> verificationFilterValidator)
    {
        _verificationService = verificationService;
        _logger = logger;
        _createValidator = createValidator;
        _correlationIdGenerator = correlationIdGenerator;
        _verificationFilterValidator = verificationFilterValidator;
    }

    [Authorize(ApiConstants.TrustedUserPolicy)]
    [HttpPost]
    [Stability(Stability.Stable)]
    [ProducesResponseType(typeof(PendingResponse),StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
    [ProducesResponseType( StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new verification")]
    [EndpointDescription("Creates a new verification request asynchronously.")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateVerification request,
        CancellationToken token = default)
    {
        var correlationId = _correlationIdGenerator.Get();
        ILogEventEnricher[] enrichers =
        [
            new PropertyEnricher("CorrelationId", correlationId)
        ];
        
        var emailClaim = HttpContext.User.Claims.FirstOrDefault(it => it.Type == ClaimTypes.Email);
        if (emailClaim is null)
        {
            _logger.LogError("Invalid email claim");
            return Forbid();
        }
        var email = emailClaim.Value;
        
        using (LogContext.Push(enrichers))
        {
            _logger.LogInformation("Creating verification");
            var validationResult = await _createValidator.ValidateAsync(request, token);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Invalid verification request");
                return BadRequest(validationResult.ToDictionary());
            }

            var verificationResponse = await _verificationService.CreateAsync(request, correlationId, email, token);
            var pendingResponse = new PendingResponse
            {
                Code = 201,
                Status = VerificationStatus.Pending,
                CreatedAt = verificationResponse.CreatedAt,
                TransactionId = verificationResponse.TransactionId,
                CreatedBy = verificationResponse.CreatedBy,
            };

            _logger.LogInformation("Finished creating a verification");
            return Created($"api/verifications/{verificationResponse.TransactionId}", pendingResponse);
        }
    }
    
    [Authorize(ApiConstants.TrustedUserPolicy)]
    [HttpGet("{id:Guid}", Name = "GetById")]
    [ProducesResponseType(typeof(VerificationResponse), StatusCodes.Status200OK,MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
        [
            new PropertyEnricher("CorrelationId", correlationId)
        ];

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
    
    [Authorize(ApiConstants.TrustedUserPolicy)]
    [HttpGet(Name = "GetHistory")]
    [ProducesResponseType(typeof(PagedResult<VerificationResponse>), 
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Returns verification history")]
    [EndpointDescription("Returns details of verifications made by the user")]
    [Stability(Stability.Stable)]
    public async Task<IActionResult> GetAsync([FromQuery] VerificationFilter verificationFilter,
        CancellationToken token = default)
    {
        var emailClaim = HttpContext.User.Claims.FirstOrDefault(it => it.Type == ClaimTypes.Email);
        if (emailClaim is null)
        {
            _logger.LogError("Invalid email claim");
            return Forbid();
        }
        var email = emailClaim.Value;
        
        var correlationId = _correlationIdGenerator.Get();
        ILogEventEnricher[] enrichers =
        [
            new PropertyEnricher("CorrelationId", correlationId)
        ];
        
        var validationResult = await _verificationFilterValidator.ValidateAsync(verificationFilter, token);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Invalid verification history request");
            return BadRequest(validationResult.ToDictionary());
        }

        using (LogContext.Push(enrichers))
        {
            _logger.LogInformation("Getting verifications");
            var response = await _verificationService.GetHistoryAsync(verificationFilter, email, token);

            _logger.LogInformation("Finished getting verifications");
            return Ok(response);
        }
    }
    
    [Authorize(ApiConstants.AdminUserPolicy)]
    [HttpGet("report", Name = "GetComplianceReport")]
    [ProducesResponseType(typeof(FileContentResult), 
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Pdf)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Compliance report")]
    [EndpointDescription("Download a comprehensive compliance report. It is for admins only.")]
    [Stability(Stability.Stable)]
    public async Task<IActionResult> GetComplianceReportAsync(CancellationToken token = default)
    {
        var emailClaim = HttpContext.User.Claims.FirstOrDefault(it => it.Type == ClaimTypes.Email);
        if (emailClaim is null)
        {
            _logger.LogError("Invalid email claim");
            return Forbid();
        }
        var email = emailClaim.Value;
        
        var correlationId = _correlationIdGenerator.Get();
        ILogEventEnricher[] enrichers =
        [
            new PropertyEnricher("CorrelationId", correlationId)
        ];

        using (LogContext.Push(enrichers))
        {
            _logger.LogInformation("Generating compliance report");
            var fileViewModel = await _verificationService.GetComplainceFileAsync(email, token);

            _logger.LogInformation("Finished generating compliance report");
            return File(fileViewModel.Contents, fileViewModel.ContentType, fileViewModel.Name);
        }
    }
}