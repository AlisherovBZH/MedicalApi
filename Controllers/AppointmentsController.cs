using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    public static List<Appointment> Appointments { get; } = new();

    [HttpGet]
    public List<Appointment> GetAll() => Appointments;

    [HttpGet("{id}")]
    public ActionResult<Appointment> GetById(int id)
    {
        Appointment? appointment = Appointments.FirstOrDefault(a => a.Id == id);
        return appointment is null ? NotFound() : appointment;
    }

    [HttpPost]
    public ActionResult<Appointment> Create(Appointment appointment)
    {
        string? error = Validate(appointment);
        if (error is not null) return BadRequest(error);
        appointment.Id = Appointments.Count + 1;
        appointment.DoctorName = appointment.DoctorName.Trim();
        appointment.Reason = appointment.Reason.Trim();
        appointment.Status = appointment.Status.Trim();
        Appointments.Add(appointment);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    public ActionResult<Appointment> Update(int id, Appointment appointment)
    {
        Appointment? existing = Appointments.FirstOrDefault(a => a.Id == id);
        if (existing is null) return NotFound();
        string? error = Validate(appointment);
        if (error is not null) return BadRequest(error);
        existing.PatientId = appointment.PatientId;
        existing.DoctorName = appointment.DoctorName.Trim();
        existing.AppointmentDate = appointment.AppointmentDate;
        existing.Reason = appointment.Reason.Trim();
        existing.Status = appointment.Status.Trim();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        Appointment? appointment = Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment is null) return NotFound();
        Appointments.Remove(appointment);
        return NoContent();
    }

    private static string? Validate(Appointment appointment)
    {
        if (!PatientsController.Patients.Any(p => p.Id == appointment.PatientId)) return "Пациент с таким ID не найден";
        if (string.IsNullOrWhiteSpace(appointment.DoctorName)) return "Имя врача обязательно";
        if (appointment.DoctorName.Trim().Length > 100) return "Имя врача не должно быть длиннее 100 символов";
        if (appointment.AppointmentDate == default) return "Дата и время приёма обязательны";
        if (appointment.AppointmentDate < DateTime.Now) return "Дата приёма не может быть в прошлом";
        if (string.IsNullOrWhiteSpace(appointment.Reason)) return "Причина приёма обязательна";
        if (appointment.Reason.Trim().Length > 250) return "Причина не должна быть длиннее 250 символов";
        if (appointment.Status is not ("Scheduled" or "Completed" or "Cancelled"))
            return "Статус должен быть Scheduled, Completed или Cancelled";
        return null;
    }
}
