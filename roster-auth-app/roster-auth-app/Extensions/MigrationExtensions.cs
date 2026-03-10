using Microsoft.EntityFrameworkCore;

namespace roster_auth_app.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<Models.ApplicationDbContext>();
                context?.Database.Migrate();
            }
        }
    }
}
