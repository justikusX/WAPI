using System;
using System.Collections.Generic;

namespace WPFPoliclinic.Models;

public partial class Visit
{
    public int Id { get; set; }

    public int DoctorId { get; set; }

    public int PatientId { get; set; }

    public int DiagnosisId { get; set; }

    public DateOnly VisitDate { get; set; }

    public virtual Diagnosis Diagnosis { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
