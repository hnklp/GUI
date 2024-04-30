using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GUI.Data;

public class CustomerContextFactory : IDesignTimeDbContextFactory<CustomerContext>
{
    public CustomerContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CustomerContext>();
        optionsBuilder.UseSqlite("Data Source=customers.db");

        return new CustomerContext(optionsBuilder.Options);
    }
}