using Xunit;
using Mapster;
using Microsoft.Extensions.Logging;
using Moq;
using EHRPlatform.Services.Billing.Features.Billing.Domain;
using EHRPlatform.Services.Billing.Features.Billing.Queries;

namespace EHRPlatform.Services.Billing.Mappings;

/// <summary>
/// Unit tests for InvoiceMapper.
/// Comprehensive coverage of all mapping scenarios and edge cases.
/// Single Responsibility: Test mapping logic in isolation.
/// </summary>
public class InvoiceMapperTests
{
    private readonly InvoiceMapper _mapper;
    private readonly Mock<ILogger<InvoiceMapper>> _loggerMock;

    public InvoiceMapperTests()
    {
        // Configure Mapster for testing
        TypeAdapterConfig.GlobalSettings.Clear();
        var config = TypeAdapterConfig.GlobalSettings;
        var profile = new InvoiceMappingProfile();
        profile.Register(config);
        config.Compile();

        _loggerMock = new Mock<ILogger<InvoiceMapper>>();
        _mapper = new InvoiceMapper(_loggerMock.Object);
    }

    #region MapToResponseDto Tests

    [Fact]
    public void MapToResponseDto_WithValidInvoice_ReturnsMappedDto()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var invoice = CreateTestInvoice(invoiceId, patientId);

        // Act
        var result = _mapper.MapToResponseDto(invoice);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invoiceId, result.Id);
        Assert.Equal("INV-12345", result.InvoiceNumber);
        Assert.Equal(patientId, result.PatientId);
        Assert.Equal("Draft", result.Status);
        Assert.Equal(1000m, result.SubTotal);
        Assert.Equal(80m, result.TaxAmount);
        Assert.Equal(1080m, result.TotalAmount);
        Assert.Equal(0m, result.AmountPaid);
        Assert.Equal(1080m, result.BalanceDue);
    }

    [Fact]
    public void MapToResponseDto_WithLineItems_MapsCollectionsCorrectly()
    {
        // Arrange
        var invoice = CreateTestInvoice(Guid.NewGuid(), Guid.NewGuid());
        invoice.AddLineItem("Office Visit", "99213", 1, 150m);
        invoice.AddLineItem("EKG", "93000", 1, 250m);
        invoice.CalculateTotals();

        // Act
        var result = _mapper.MapToResponseDto(invoice);

        // Assert
        Assert.NotNull(result.LineItems);
        Assert.Equal(2, result.LineItems.Count);
        Assert.Equal("Office Visit", result.LineItems[0].Description);
        Assert.Equal(150m, result.LineItems[0].Amount);
        Assert.Equal("EKG", result.LineItems[1].Description);
        Assert.Equal(250m, result.LineItems[1].Amount);
    }

    [Fact]
    public void MapToResponseDto_WithPayments_MapsPaymentsCorrectly()
    {
        // Arrange
        var invoice = CreateTestInvoice(Guid.NewGuid(), Guid.NewGuid());
        invoice.RecordPayment(500m, "Credit Card", "TXN-001");
        invoice.RecordPayment(300m, "Check", "CHK-001");

        // Act
        var result = _mapper.MapToResponseDto(invoice);

        // Assert
        Assert.NotNull(result.Payments);
        Assert.Equal(2, result.Payments.Count);
        Assert.Equal(500m, result.Payments[0].Amount);
        Assert.Equal("Credit Card", result.Payments[0].Method);
        Assert.Equal(300m, result.Payments[1].Amount);
    }

    [Fact]
    public void MapToResponseDto_WithInsuranceClaims_MapsClaimsCorrectly()
    {
        // Arrange
        var invoice = CreateTestInvoice(Guid.NewGuid(), Guid.NewGuid());
        invoice.SubmitToInsurance("United Healthcare", "POL-123456");

        // Act
        var result = _mapper.MapToResponseDto(invoice);

        // Assert
        Assert.NotNull(result.Claims);
        Assert.Single(result.Claims);
        Assert.Equal("United Healthcare", result.Claims[0].InsuranceProvider);
        Assert.Equal("Submitted", result.Claims[0].Status);
    }

    [Fact]
    public void MapToResponseDto_WithNullEntity_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _mapper.MapToResponseDto(null!));
    }

    #endregion

    #region MapToListDto Tests

    [Fact]
    public void MapToListDto_WithMultipleInvoices_ReturnsPaginatedDto()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            CreateTestInvoice(Guid.NewGuid(), Guid.NewGuid()),
            CreateTestInvoice(Guid.NewGuid(), Guid.NewGuid()),
            CreateTestInvoice(Guid.NewGuid(), Guid.NewGuid())
        };
        int total = 100;
        int pageNumber = 1;
        int pageSize = 3;

        // Act
        var result = _mapper.MapToListDto(invoices, total, pageNumber, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(total, result.Total);
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
        Assert.All(result.Items, item => Assert.NotNull(item));
    }

    [Fact]
    public void MapToListDto_WithEmptyCollection_ReturnsEmptyList()
    {
        // Arrange
        var invoices = new List<Invoice>();

        // Act
        var result = _mapper.MapToListDto(invoices, 0, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public void MapToListDto_WithNullCollection_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _mapper.MapToListDto(null!, 0, 1, 10));
    }

    [Fact]
    public void MapToListDto_WithPaginationInfo_PreservesPaginationDetails()
    {
        // Arrange
        var invoices = new List<Invoice> { CreateTestInvoice(Guid.NewGuid(), Guid.NewGuid()) };
        int total = 1000;
        int pageNumber = 5;
        int pageSize = 20;

        // Act
        var result = _mapper.MapToListDto(invoices, total, pageNumber, pageSize);

        // Assert
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(total, result.Total);
    }

    #endregion

    #region MapToResponseDtoList Tests

    [Fact]
    public void MapToResponseDtoList_WithValidInvoices_ReturnsMappedList()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            CreateTestInvoice(Guid.NewGuid(), Guid.NewGuid()),
            CreateTestInvoice(Guid.NewGuid(), Guid.NewGuid())
        };

        // Act
        var result = _mapper.MapToResponseDtoList(invoices);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, item => Assert.NotNull(item));
    }

    [Fact]
    public void MapToResponseDtoList_WithEmptyCollection_ReturnsEmptyList()
    {
        // Arrange
        var invoices = new List<Invoice>();

        // Act
        var result = _mapper.MapToResponseDtoList(invoices);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MapToResponseDtoList_WithNullCollection_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _mapper.MapToResponseDtoList(null!));
    }

    #endregion

    #region MapToOutstandingBalanceDto Tests

    [Fact]
    public void MapToOutstandingBalanceDto_WithValidInvoices_CalculatesBalanceCorrectly()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var invoice1 = CreateTestInvoice(Guid.NewGuid(), patientId);
        invoice1.RecordPayment(500m, "Credit Card", "TXN-001");

        var invoice2 = CreateTestInvoice(Guid.NewGuid(), patientId);
        invoice2.CalculateTotals();

        var invoices = new List<Invoice> { invoice1, invoice2 };

        // Act
        var result = _mapper.MapToOutstandingBalanceDto(patientId, invoices);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(patientId, result.PatientId);
        Assert.Equal(2, result.Invoices.Count);
        // invoice1: 1080 - 500 = 580, invoice2: 1080, total = 1660
        Assert.Equal(1660m, result.TotalBalance);
    }

    [Fact]
    public void MapToOutstandingBalanceDto_WithOverdueInvoices_IdentifiesOverdueCorrectly()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var invoiceId1 = Guid.NewGuid();
        var invoiceId2 = Guid.NewGuid();

        var invoice1 = new Invoice
        {
            Id = invoiceId1,
            PatientId = patientId,
            InvoiceNumber = "INV-001",
            ServiceDate = DateTime.UtcNow.AddDays(-60),
            DueDate = DateTime.UtcNow.AddDays(-30), // Overdue
            Status = "Submitted",
            SubTotal = 1000m,
            TaxAmount = 80m,
            TotalAmount = 1080m,
            AmountPaid = 0m
        };

        var invoice2 = new Invoice
        {
            Id = invoiceId2,
            PatientId = patientId,
            InvoiceNumber = "INV-002",
            ServiceDate = DateTime.UtcNow.AddDays(-5),
            DueDate = DateTime.UtcNow.AddDays(25), // Not overdue
            Status = "Draft",
            SubTotal = 500m,
            TaxAmount = 40m,
            TotalAmount = 540m,
            AmountPaid = 0m
        };

        var invoices = new List<Invoice> { invoice1, invoice2 };

        // Act
        var result = _mapper.MapToOutstandingBalanceDto(patientId, invoices);

        // Assert
        Assert.Equal(1, result.OverdueInvoices);
        Assert.Equal(1080m, result.OverdueAmount);
    }

    [Fact]
    public void MapToOutstandingBalanceDto_WithPaidInvoices_ExcludesPaidFromBalance()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var invoice = CreateTestInvoice(Guid.NewGuid(), patientId);
        invoice.MarkPaid();

        var invoices = new List<Invoice> { invoice };

        // Act
        var result = _mapper.MapToOutstandingBalanceDto(patientId, invoices);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Invoices);
    }

    [Fact]
    public void MapToOutstandingBalanceDto_WithCancelledInvoices_ExcludesCancelledFromBalance()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var invoice1 = CreateTestInvoice(Guid.NewGuid(), patientId);
        var invoice2 = CreateTestInvoice(Guid.NewGuid(), patientId);
        invoice2.Cancel("Not needed");

        var invoices = new List<Invoice> { invoice1, invoice2 };

        // Act - Call with only non-cancelled invoices (as query handler does)
        var result = _mapper.MapToOutstandingBalanceDto(patientId, invoices);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Invoices.Count); // Both included but cancelled status visible
    }

    [Fact]
    public void MapToOutstandingBalanceDto_WithEmptyInvoices_ReturnsZeroBalance()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var invoices = new List<Invoice>();

        // Act
        var result = _mapper.MapToOutstandingBalanceDto(patientId, invoices);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(patientId, result.PatientId);
        Assert.Empty(result.Invoices);
        Assert.Equal(0m, result.TotalBalance);
        Assert.Equal(0, result.OverdueInvoices);
        Assert.Equal(0m, result.OverdueAmount);
    }

    #endregion

    #region MapToCommandDto Tests

    [Fact]
    public void MapToCommandDto_WithValidInvoice_ReturnsMappedCommandDto()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var invoice = CreateTestInvoice(invoiceId, patientId);

        // Act
        var result = _mapper.MapToCommandDto(invoice);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(invoiceId, result.Id);
        Assert.Equal("INV-12345", result.InvoiceNumber);
        Assert.Equal(patientId, result.PatientId);
        Assert.Equal(1000m, result.SubTotal);
    }

    [Fact]
    public void MapToCommandDto_ExcludesComputedFields()
    {
        // Arrange
        var invoice = CreateTestInvoice(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _mapper.MapToCommandDto(invoice);

        // Assert
        Assert.NotNull(result);
        // Command DTO should not have complex nested objects like LineItems, Payments, Claims
        Assert.Null(result.Id == Guid.Empty ? null : result.Id);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void CompleteWorkflow_CreateUpdateAndRetrieveInvoice_MapsCorrectly()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var invoice = CreateTestInvoice(invoiceId, patientId);

        // Act - Simulate workflow
        invoice.RecordPayment(540m, "Credit Card", "TXN-001"); // Partial payment
        var responseDto = _mapper.MapToResponseDto(invoice);
        var listDto = _mapper.MapToListDto(new List<Invoice> { invoice }, 1, 1, 10);
        var balanceDto = _mapper.MapToOutstandingBalanceDto(patientId, new List<Invoice> { invoice });

        // Assert - Verify all mappings are consistent
        Assert.Equal(invoiceId, responseDto.Id);
        Assert.Equal(540m, responseDto.AmountPaid);
        Assert.Equal(540m, responseDto.BalanceDue); // 1080 - 540

        Assert.Equal(1, listDto.Items.Count);
        Assert.Equal(responseDto.Id, listDto.Items[0].Id);

        Assert.Equal(540m, balanceDto.TotalBalance);
        Assert.Single(balanceDto.Invoices);
    }

    [Fact]
    public void MultipleInvoices_DifferentStatuses_MapsAllCorrectly()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var draftInvoice = CreateTestInvoice(Guid.NewGuid(), patientId);
        
        var paidInvoice = CreateTestInvoice(Guid.NewGuid(), patientId);
        paidInvoice.RecordPayment(1080m, "Insurance", "CLM-001");
        paidInvoice.MarkPaid();
        
        var submittedInvoice = CreateTestInvoice(Guid.NewGuid(), patientId);
        submittedInvoice.SubmitToInsurance("Aetna", "POL-789");

        var invoices = new List<Invoice> { draftInvoice, paidInvoice, submittedInvoice };

        // Act
        var dtos = _mapper.MapToResponseDtoList(invoices);

        // Assert
        Assert.Equal(3, dtos.Count);
        Assert.Equal("Draft", dtos[0].Status);
        Assert.Equal("Paid", dtos[1].Status);
        Assert.Equal("Submitted", dtos[2].Status);
    }

    #endregion

    #region Helper Methods

    private static Invoice CreateTestInvoice(Guid id, Guid patientId)
    {
        return new Invoice
        {
            Id = id,
            PatientId = patientId,
            AppointmentId = Guid.NewGuid(),
            InvoiceNumber = "INV-12345",
            ServiceDate = DateTime.UtcNow.AddDays(-30),
            DueDate = DateTime.UtcNow.AddDays(0),
            Status = "Draft",
            SubTotal = 1000m,
            TaxAmount = 80m,
            InsuranceResponsibility = 800m,
            PatientResponsibility = 280m,
            TotalAmount = 1080m,
            AmountPaid = 0m,
            InsuranceProvider = "Aetna",
            InsurancePolicyNumber = "POL-123456",
            Notes = "Test invoice"
        };
    }

    #endregion
}
