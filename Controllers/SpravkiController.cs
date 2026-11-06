using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpravkiController : ControllerBase
{
    public static List<Spravka> Spravki = new();

    [HttpGet]
    public List<Spravka> GetAll() => Spravki;

    [HttpGet("{id}")]
    public ActionResult<Spravka> GetById(int id)
    {
        Spravka? spravka = Spravki.FirstOrDefault(s => s.Id == id);
        return spravka is null ? NotFound() : spravka;
    }

    [HttpPost]
    public ActionResult<Spravka> Create(Spravka spravka)
    {
        string? error = ValidateSpravka(spravka);
        if (error is not null) return BadRequest(error);
        spravka.Id = Spravki.Count + 1;
        Spravki.Add(spravka);
        return CreatedAtAction(nameof(GetById), new { id = spravka.Id }, spravka);
    }

    [HttpPut("{id}")]
    public ActionResult<Spravka> Update(int id, Spravka spravka)
    {
        Spravka? existing = Spravki.FirstOrDefault(s => s.Id == id);
        if (existing is null) return NotFound();
        string? error = ValidateSpravka(spravka);
        if (error is not null) return BadRequest(error);
        existing.Name = spravka.Name;
        existing.PatientId = spravka.PatientId;
        existing.Price = spravka.Price;
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        Spravka? spravka = Spravki.FirstOrDefault(s => s.Id == id);
        if (spravka is null) return NotFound();
        Spravki.Remove(spravka);
        return NoContent();
    }

    private static string? ValidateSpravka(Spravka spravka)
    {
        if (string.IsNullOrWhiteSpace(spravka.Name)) return "Название справки обязательно";
        if (!PatientsController.Patients.Any(p => p.Id == spravka.PatientId)) return "Пациент с таким ID не найден";
        if (spravka.Price <= 0) return "Цена должна быть больше 0";
        return null;
    }
}
