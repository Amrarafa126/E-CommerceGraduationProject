using E_Commerce.Infrustructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace E_Commerce.Infrustructure
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDBContext>
    {
        public AppDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();

            optionsBuilder.UseSqlServer(
                "Data Source=.\\SQLEXPRESS;Initial Catalog=E-CommerceDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");

            return new AppDBContext(optionsBuilder.Options);
        }
    }
}
