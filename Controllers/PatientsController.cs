using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    public static List<Patient> Patients = new();

    [HttpGet]
    [ProducesResponseType(typeof(List<Patient>), StatusCodes.Status200OK)]
    public List<Patient> GetAll() => Patients;

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Patient> GetById(int id)
    {
        Patient? patient = Patients.FirstOrDefault(p => p.Id == id);
        return patient is null ? NotFound() : patient;
    }

    [HttpGet("{id}/spravki")]
    [ProducesResponseType(typeof(List<Spravka>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<Spravka>> GetSpravki(int id)
    {
        if (!Patients.Any(p => p.Id == id)) return NotFound();
        return SpravkiController.Spravki.Where(s => s.PatientId == id).ToList();
    }

    [HttpPost]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public ActionResult<Patient> Create(Patient patient)
    {
        string? error = ValidatePatient(patient);
        if (error is not null) return BadRequest(error);
        patient.Id = Patients.Count == 0 ? 1 : Patients.Max(p => p.Id) + 1;
        Patients.Add(patient);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
