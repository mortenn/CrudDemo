using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CrudDemo.Database
{
	[UsedImplicitly]
	internal class DemoDbContextFactory : IDesignTimeDbContextFactory<DemoDbContext>
	{
		public DemoDbContext CreateDbContext(string[] args)
		{
			const string connectionString = @"Server = (LocalDB)\MSSQLLocalDB; Integrated Security = true; Initial Catalog = DemoLocalDb; MultipleActiveResultSets = True;";
			var optionsBuilder = new DbContextOptionsBuilder<DemoDbContext>();

			optionsBuilder.UseSqlServer(connectionString, builder =>
			{
				builder.EnableRetryOnFailure();
			});

			return new DemoDbContext(optionsBuilder.Options);
		}
	}
}
