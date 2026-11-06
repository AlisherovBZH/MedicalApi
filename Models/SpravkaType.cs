namespace MedicalApi.Models;

public class SpravkaType
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int ValidDays { get; set; }
    public bool RequiresDoctorExam { get; set; }
}
