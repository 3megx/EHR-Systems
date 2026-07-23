namespace EHRPlatform.Services.Patient.Application.Patients.Responses;

public class PatientResponse
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? MRN { get; set; }
    public string? Status { get; set; }
}
