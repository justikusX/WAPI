using System;
using System.Collections.Generic;

namespace WPFPoliclinic.Models;

public partial class Patient
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateOnly BirthDate { get; set; }

    public string Address { get; set; } = null!;

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();

    public string FullName => $"{LastName} {FirstName} {Patronymic ?? ""}".Trim();

    public int Age
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - BirthDate.Year;
            if (BirthDate > today.AddYears(-age)) age--;
            return age;
        }
    }
}
