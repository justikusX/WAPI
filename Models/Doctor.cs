using System;
using System.Collections.Generic;

namespace WPFPoliclinic.Models;

public partial class Doctor
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Specialty { get; set; } = null!;

    public int Experience { get; set; }

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();

    public string FullName => $"{LastName} {FirstName} {Patronymic ?? ""}".Trim();
}
