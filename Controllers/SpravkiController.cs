using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

[ApiController]
[Route("api/[controller]")]
public class SpravkiController : ControllerBase
{
    private static List<Spravka> _spravki = new List<Spravka>();

    [HttpGet]
    public List<Spravka> GetAll()
    {
        return _spravki;
    }

    [HttpGet("{id}")]
    public ActionResult<Spravka> GetById(int id)
    {
        Spravka? spravka = _spravki.FirstOrDefault(s => s.Id == id);
        if (spravka == null)
        {
            return NotFound();
        }
        return spravka;
    }

    [HttpPost]
    public ActionResult<Spravka> Create(Spravka spravka)
    {
        string? error = ValidateSpravka(spravka);
        if (error != null)
        {
            return BadRequest(error);
        }
        spravka.Id = _spravki.Count + 1;
        _spravki.Add(spravka);
        return CreatedAtAction(nameof(GetById), new { id = spravka.Id }, spravka);
    }

    [HttpPut("{id}")]
    public ActionResult<Spravka> Update(int id, Spravka spravka)
    {

        Spravka? existing = _spravki.FirstOrDefault(s => s.Id == id);

        if (existing == null)
        {
            return NotFound();
        }
        string? error = ValidateSpravka(spravka);
        if (error != null)
        {
            return BadRequest(error);
        }

        existing.Name = spravka.Name;
        existing.PatientName = spravka.PatientName;
        existing.Price = spravka.Price;

        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        Spravka? spravka = _spravki.FirstOrDefault(s => s.Id == id);

        if (spravka == null)
        {
            return NotFound();
        }

        _spravki.Remove(spravka);
        return NoContent();
    }

    private string? ValidateSpravka(Spravka spravka)
    {
        if (string.IsNullOrWhiteSpace(spravka.Name))
            return "Название справки обязательно";

        if (string.IsNullOrWhiteSpace(spravka.PatientName))
            return "Имя пациента обязательно";

        if (spravka.Price <= 0)
            return "Цена должна быть больше 0";

        return null;
    }
}