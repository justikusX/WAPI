using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WPFPoliclinic.Models;

public partial class PoliclinicContext : DbContext
{
    public PoliclinicContext()
    {

        
    }

    public PoliclinicContext(DbContextOptions<PoliclinicContext> options)
        : base(options)
    {

        
    }

    public virtual DbSet<Admin> Admins { get; set; }
    public virtual DbSet<Diagnosis> Diagnoses { get; set; }
    public virtual DbSet<Doctor> Doctors { get; set; }
    public virtual DbSet<Patient> Patients { get; set; }
    public virtual DbSet<Visit> Visits { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-VPI29MR\\TEACHERPC;Initial Catalog=Policlinic;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("admins");

            entity.HasIndex(e => e.Login)
                  .IsUnique()
                  .HasDatabaseName("IX_Admins_Login");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Login)
                  .IsRequired()
                  .HasMaxLength(50)
                  .IsUnicode(false)
                  .HasColumnName("login");
            entity.Property(e => e.PasswordHash)
                  .IsRequired()
                  .HasMaxLength(255)
                  .IsUnicode(false)
                  .HasColumnName("password_hash");
            entity.Property(e => e.FirstName)
                  .IsRequired()
                  .HasMaxLength(50)
                  .IsUnicode(false)
                  .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                  .IsRequired()
                  .HasMaxLength(50)
                  .IsUnicode(false)
                  .HasColumnName("last_name");
            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.LastLogin)
                  .HasColumnName("last_login")
                  .IsRequired(false);
            entity.Property(e => e.IsActive)
                  .HasColumnName("is_active")
                  .HasDefaultValue(true);
            entity.Property(e => e.Role)
                  .HasMaxLength(20)
                  .IsUnicode(false)
                  .HasColumnName("role")
                  .HasDefaultValue("admin");
        });

        modelBuilder.Entity<Diagnosis>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__diagnose__3213E83F74D6FC29");
            entity.ToTable("diagnoses");
            entity.HasIndex(e => e.Code, "UQ__diagnose__357D4CF9E396B21D").IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__doctors__3213E83F292D54D5");
            entity.ToTable("doctors");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Experience).HasColumnName("experience");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("patronymic");
            entity.Property(e => e.Specialty)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("specialty");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__patients__3213E83F9E6A22A9");
            entity.ToTable("patients");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("patronymic");
        });

        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__visits__3213E83FC5EE3E1C");
            entity.ToTable("visits");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DiagnosisId).HasColumnName("diagnosis_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.VisitDate).HasColumnName("visit_date");
            entity.HasOne(d => d.Diagnosis).WithMany(p => p.Visits)
                .HasForeignKey(d => d.DiagnosisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__visits__diagnosi__52593CB8");
            entity.HasOne(d => d.Doctor).WithMany(p => p.Visits)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__visits__doctor_i__5070F446");
            entity.HasOne(d => d.Patient).WithMany(p => p.Visits)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__visits__patient___5165187F");
        });

        
      


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}