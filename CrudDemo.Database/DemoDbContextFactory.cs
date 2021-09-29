using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CrudDemo.Database
{
	internal class DemoDbContextFactory : IDesignTimeDbContextFactory<DemoDbContext>
	{
		public DemoDbContext CreateDbContext(string[] args)
		{
			const string connectionString = @"Server = (LocalDB)\MSSQLLocalDB; Integrated Security = true; Initial Catalog = HaproArchiveLocalDb; MultipleActiveResultSets = True;";
			var optionsBuilder = new DbContextOptionsBuilder<DemoDbContext>();

			optionsBuilder.UseSqlServer(connectionString, builder =>
			{
				builder.EnableRetryOnFailure();
			});

			return new DemoDbContext(optionsBuilder.Options);
		}
	}
}
