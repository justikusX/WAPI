using System;
using System.Collections.Generic;

namespace WPFPoliclinic.Models;

public partial class Diagnosis
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
