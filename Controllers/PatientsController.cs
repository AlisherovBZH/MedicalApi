using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    public static List<Patient> Patients = new();

    [HttpGet]
    public List<Patient> GetAll() => Patients;

    [HttpGet("{id}")]
    public ActionResult<Patient> GetById(int id)
    {
        Patient? patient = Patients.FirstOrDefault(p => p.Id == id);
        return patient is null ? NotFound() : patient;
    }

    [HttpGet("{id}/spravki")]
    public ActionResult<List<Spravka>> GetSpravki(int id)
    {
        if (!Patients.Any(p => p.Id == id)) return NotFound();
        return SpravkiController.Spravki.Where(s => s.PatientId == id).ToList();
    }

    [HttpPost]
    public ActionResult<Patient> Create(Patient patient)
    {
        string? error = ValidatePatient(patient);
        if (error is not null) return BadRequest(error);
        patient.Id = Patients.Count + 1;
        Patients.Add(patient);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    public ActionResult<Patient> Update(int id, Patient patient)
    {
        Patient? existing = Patients.FirstOrDefault(p => p.Id == id);
        if (existing is null) return NotFound();
        string? error = ValidatePatient(patient);
        if (error is not null) return BadRequest(error);
        existing.Name = patient.Name;
        existing.BirthDate = patient.BirthDate;
        existing.Phone = patient.Phone;
        existing.IsActive = patient.IsActive;
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        Patient? patient = Patients.FirstOrDefault(p => p.Id == id);
        if (patient is null) return NotFound();
        Patients.Remove(patient);
        return NoContent();
    }

    private static string? ValidatePatient(Patient patient)
    {
        if (string.IsNullOrWhiteSpace(patient.Name)) return "Имя пациента обязательно";
        if (string.IsNullOrWhiteSpace(patient.Phone)) return "Телефон обязателен";
        if (patient.BirthDate == default) return "Дата рождения обязательна";
        if (patient.BirthDate.Date > DateTime.Today) return "Дата рождения не может быть в будущем";
        return null;
    }
}
