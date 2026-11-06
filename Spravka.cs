using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalApi;

public class Spravka
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string PatientName { get; set; } = "";
    public decimal Price { get; set; }
}
