using KitapSatis.Api.Models;

namespace KitapSatis.Api.Data
{
    public static class DbSeeder
    {
        public static void SeedAdmin(AppDbContext context)
        {
            if (!context.Users.Any(u => u.Username == "admin"))
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                    Role = "Admin"
                });

            }
            if (!context.Users.Any(u => u.Username == "user1"))
            {
                context.Users.Add(new User
                {
                    Username = "user1",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                    Role = "User"
                });
            }
            context.SaveChanges();

        }

    }
}
