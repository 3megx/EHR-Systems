using MediatR;
using Microsoft.AspNetCore.Mvc;
using EHRPlatform.Services.Billing.Features.Billing.Commands;
using EHRPlatform.Services.Billing.Features.Billing.Queries;

namespace EHRPlatform.Services.Billing.Controllers;

/// <summary>
/// Billing and invoicing endpoints.
/// Create invoices, record payments, submit to insurance, track claims.
/// </summary>
[ApiController]
[Route("api/v1/billing")]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BillingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create invoice with line items.
    /// </summary>
    [HttpPost("invoices")]
    [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateInvoice(
        [FromBody] CreateInvoiceCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetInvoice), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get invoice by ID (cached).
    /// </summary>
    [HttpGet("invoices/{id}")]
    [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoice(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetInvoiceQuery { InvoiceId = id },
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get patient invoices (cached, paginated).
    /// </summary>
    [HttpGet("patient/{patientId}/invoices")]
    [ProducesResponseType(typeof(InvoiceListDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatientInvoices(
        Guid patientId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetPatientInvoicesQuery
            {
                PatientId = patientId,
                PageNumber = page,
                PageSize = pageSize
            },
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get patient outstanding balance (cached).
    /// Summary of current balance and overdue amounts.
    /// </summary>
    [HttpGet("patient/{patientId}/balance")]
    [ProducesResponseType(typeof(OutstandingBalanceDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOutstandingBalance(
        Guid patientId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPatientOutstandingBalanceQuery { PatientId = patientId },
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Record payment for invoice.
    /// Payment methods: Credit Card, Check, ACH, Insurance.
    /// </summary>
    [HttpPost("invoices/{id}/payments")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordPayment(
        Guid id,
        [FromBody] RecordPaymentCommand command,
        CancellationToken cancellationToken)
    {
        command = command with { InvoiceId = id };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Submit invoice to insurance for claim processing.
    /// Creates insurance claim record for tracking.
    /// </summary>
    [HttpPost("invoices/{id}/submit-insurance")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitToInsurance(
        Guid id,
        [FromBody] SubmitToInsuranceCommand command,
        CancellationToken cancellationToken)
    {
        command = command with { InvoiceId = id };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Cancel invoice (e.g., duplicate billing).
    /// </summary>
    [HttpPost("invoices/{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelInvoice(
        Guid id,
        [FromBody] string reason = "",
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new CancelInvoiceCommand { InvoiceId = id, Reason = reason },
            cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Health check.
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "billing-service" });
    }
}
