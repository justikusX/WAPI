using System;
using System.Linq;

namespace WPFPoliclinic.Models
{
    public static class AdminInitializer
    {
        public static void Initialize(PoliclinicContext context)
        {
            
            if (!context.Admins.Any())
            {
                
                var admin = new Admin
                {
                    Login = "admin",
                    FirstName = "Иван",
                    LastName = "Петров",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    Role = "superadmin"
                };
                admin.SetPassword("admin123"); 

                context.Admins.Add(admin);
                context.SaveChanges();
            }
        }
    }
}