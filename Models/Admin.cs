using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WPFPoliclinic.Models
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Login { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(20)]
        public string Role { get; set; } = "admin";

        [NotMapped]
        public string FullName => $"{LastName} {FirstName}";

        [NotMapped]
        public string Initials => $"{LastName[0]}{FirstName[0]}";

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }

        public void SetPassword(string password)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}