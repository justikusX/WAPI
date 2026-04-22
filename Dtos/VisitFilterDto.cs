namespace WAPI.Dtos;

public class VisitFilterDto
{
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public int? DoctorId { get; set; }
    public int? PatientId { get; set; }
}