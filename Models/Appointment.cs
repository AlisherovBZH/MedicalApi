namespace MedicalApi.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string DoctorName { get; set; } = "";
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; } = "";
    public string Status { get; set; } = "Scheduled";
}
