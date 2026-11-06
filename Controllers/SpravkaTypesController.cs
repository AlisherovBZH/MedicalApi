using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpravkaTypesController : ControllerBase
{
    public static List<SpravkaType> Types = new()
    {
        new() { Id = 1, Name = "Справка 083", Price = 100, ValidDays = 365, RequiresDoctorExam = true },
        new() { Id = 2, Name = "Санитарная книжка", Price = 750, ValidDays = 365, RequiresDoctorExam = true },
        new() { Id = 3, Name = "Справка в бассейн", Price = 250, ValidDays = 90, RequiresDoctorExam = true },
        new() { Id = 4, Name = "Справка для спортивной секции", Price = 400, ValidDays = 180, RequiresDoctorExam = true }
    };

    [HttpGet]
    public List<SpravkaType> GetAll() => Types;

    [HttpGet("{id}")]
    public ActionResult<SpravkaType> GetById(int id)
    {
        SpravkaType? type = Types.FirstOrDefault(t => t.Id == id);
        return type is null ? NotFound() : type;
    }

    [HttpPost]
    public ActionResult<SpravkaType> Create(SpravkaType type)
    {
        string? error = Validate(type);
        if (error is not null) return BadRequest(error);
        type.Id = Types.Max(t => t.Id) + 1;
        type.Name = type.Name.Trim();
        Types.Add(type);
        return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
    }

    [HttpPut("{id}")]
    public ActionResult<SpravkaType> Update(int id, SpravkaType type)
    {
        SpravkaType? existing = Types.FirstOrDefault(t => t.Id == id);
        if (existing is null) return NotFound();
        string? error = Validate(type);
        if (error is not null) return BadRequest(error);
        existing.Name = type.Name.Trim();
        existing.Price = type.Price;
        existing.ValidDays = type.ValidDays;
        existing.RequiresDoctorExam = type.RequiresDoctorExam;
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        SpravkaType? type = Types.FirstOrDefault(t => t.Id == id);
        if (type is null) return NotFound();
        if (SpravkiController.Spravki.Any(s => s.SpravkaTypeId == id))
            return BadRequest("Нельзя удалить вид справки, который уже выдан пациентам");
        Types.Remove(type);
        return NoContent();
    }

    private static string? Validate(SpravkaType type)
    {
        if (string.IsNullOrWhiteSpace(type.Name)) return "Название вида справки обязательно";
        if (type.Name.Trim().Length > 100) return "Название не должно быть длиннее 100 символов";
        if (type.Price <= 0) return "Цена должна быть больше 0";
        if (type.ValidDays <= 0) return "Срок действия должен быть больше 0 дней";
        return null;
    }
}
