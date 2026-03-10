using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace roster_api_app.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // design-time constant connection string; keep in sync with appsettings
        var connectionString = "Host=ep-curly-mode-adolqx8b-pooler.c-2.us-east-1.aws.neon.tech; Database=rosterdb; Username=neondb_owner; Password=npg_quXSiVZj5RM2; SSL Mode=VerifyFull; Channel Binding=Require;";
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}