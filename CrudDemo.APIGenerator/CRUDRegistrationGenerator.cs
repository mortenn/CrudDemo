using CrudDemo.Abstractions;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrudDemo.Generator
{
	[Generator]
	internal class CRUDRegistrationGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			// No initialization required
		}

		public void Execute(GeneratorExecutionContext context)
		{
			var entryPoint = new StringBuilder();
			entryPoint.AppendFormat(@"
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace {0}
{{
	public static class CrudAPI
	{{
		public static void WithAutoCRUD(this IEndpointRouteBuilder endpoint)
		{{
",
				context.Compilation.AssemblyName
			);

			var types = context.GetTypes().ToList();
			var contexts = new Dictionary<string, List<ITypeSymbol>>();
			foreach (var type in types)
			{
				var autoCrud = type.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == nameof(AutoCrudAttribute));
				if (autoCrud == null) continue;
				var route = (string)autoCrud.ConstructorArguments[0].Value;
				var dbContext = (string)autoCrud.ConstructorArguments[1].Value + "DbContext";
				var dbContextType = types.FirstOrDefault(t => t.Name == dbContext).GetFullMetadataName();
				if (string.IsNullOrWhiteSpace(dbContextType))
				{
					context.ReportDiagnostic(Diagnostic.Create(
						new DiagnosticDescriptor("CD01", "Unable to locate DbContext", $"Could not locate a DbContext named {dbContext}, missing assembly reference to EF models?", "F1", DiagnosticSeverity.Error, true),
						null
					));
					return;
				}
				AddRouteMap(entryPoint, route, type, dbContextType);
			}
			entryPoint.Append(@"
	}
");
			foreach (var type in types)
			{
				var autoCrud = type.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == nameof(AutoCrudAttribute));
				if (autoCrud == null) continue;
				entryPoint.AppendFormat(
					"\r\n\t\tprivate static void UpdateModel({0} state, {0} content)\r\n\t\t{{\r\n",
					type.GetFullMetadataName()
				);
				foreach (var member in type.GetMembers().Where(m => m.Kind == SymbolKind.Property && !m.GetAttributes().Any(a => a.AttributeClass.Name == "Key")))
				{
					entryPoint.AppendFormat(
						"\r\n\t\t\tif (state.{0} != content.{0})\r\n\t\t\t\tstate.{0} = content.{0};\r\n",
						member.Name
					);
				}
				entryPoint.Append("\r\n\t\t}\r\n");
			}

			entryPoint.Append("\r\n\t}\r\n}\r\n");
			context.AddSource("CrudAPI", entryPoint.ToString());
		}

		private void AddRouteMap(StringBuilder entryPoint, string route, ITypeSymbol type, string dbContext)
		{
			entryPoint.AppendFormat(@"
			endpoint.MapGet(
				""{0}/{1}s"",
				async ([FromServices] {2} dbContext) => Results.Ok(await dbContext.{1}s.ToListAsync())
			);

			endpoint.MapGet(
				""{0}/{1}s/{{id}}"",
				async ([FromServices] {2} dbContext, int id) => Results.Ok(await dbContext.{1}s.FirstOrDefaultAsync(o => o.Id == id))
			);

			endpoint.MapPost(
				""{0}/{1}s"",
				async ([FromServices] {2} dbContext, [FromBody] {3} content) =>
				{{
					dbContext.{1}s.Add(content);
					await dbContext.SaveChangesAsync();
					return Results.Created(""{0}/{1}s/{{content.Id}}"", content);
				}}
			);

			endpoint.MapPost(
				""{0}/{1}s/{{id}}"",
				async ([FromServices] {2} dbContext, [FromBody] {3} content, int id) =>
				{{
					var state = await dbContext.{1}s.FindAsync(id);
					UpdateModel(state, content);
					if (!dbContext.ChangeTracker.HasChanges())
					{{
						return Results.StatusCode(304);
					}}
					await dbContext.SaveChangesAsync();
					return Results.NoContent();
				}}
			);

			endpoint.MapDelete(
				""{0}/{1}s/{{id}}"",
				async ([FromServices] {2} dbContext, int id) =>
				{{
					var state = await dbContext.{1}s.FindAsync(id);
					if (state == null)
					{{
						return Results.NotFound();
					}}
					dbContext.{1}s.Remove(state);
					await dbContext.SaveChangesAsync();
					return Results.StatusCode(205);
				}}
			);
",
				route,
				type.Name,
				dbContext,
				type.GetFullMetadataName()
			);
		}
	}
}
