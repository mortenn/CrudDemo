using CrudDemo;
using CrudDemo.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DemoDbContext>(
	options => options.UseSqlServer(
		@"Server = (LocalDB)\MSSQLLocalDB; Integrated Security = true; Initial Catalog = HaproArchiveLocalDb; MultipleActiveResultSets = True;"
	)
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseRouting();
app.UseSwagger();
app.UseEndpoints(endpoints => { endpoints.WithAutoCRUD(); });
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRUD Api v1");
	c.RoutePrefix = string.Empty;
});
app.Run();
