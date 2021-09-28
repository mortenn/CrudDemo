using CrudDemo.Abstractions;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrudDemo.Generator
{

	[Generator]
	internal class CRUDDbContextGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new DbContextFactoryReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			System.Console.WriteLine(nameof(CRUDDbContextGenerator));
			if (!(context.SyntaxReceiver is DbContextFactoryReceiver scanner) || scanner.dbContexts.Count == 0) return;
			context.ReportDiagnostic(Diagnostic.Create(
				new DiagnosticDescriptor("CD01", "Generating DbContext", $"Automatically creating DbContext for entity types", "F1", DiagnosticSeverity.Info, true),
				null
			));
			var types = context.GetTypes().ToList();
			var contexts = new Dictionary<string, List<ITypeSymbol>>();
			foreach (var type in types)
			{
				var autoCrud = type.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == nameof(AutoCrudAttribute));
				if (autoCrud == null) continue;
				string dbContext = (string)autoCrud.ConstructorArguments[1].Value;
				if (dbContext == null) continue;
				if (!contexts.ContainsKey(dbContext))
					contexts.Add(dbContext, new List<ITypeSymbol>());
				contexts[dbContext].Add(type);
			}

			foreach (var dbContext in contexts)
			{
				if (!scanner.dbContexts.Contains(dbContext.Key))
				{
					context.ReportDiagnostic(Diagnostic.Create(
						new DiagnosticDescriptor("CD01", "Missing DbContextFactory", $"The DbContext {dbContext.Key} requires a {dbContext.Key}DbContextFactory class implementing the IDesignTimeDbContextFactory<{dbContext.Key} interface", "F1", DiagnosticSeverity.Error, true),
						null
					));
					continue;
				}
				AddDbContext(context, dbContext.Key, dbContext.Value);
			}
		}

		private static void AddDbContext(GeneratorExecutionContext context, string dbContextName, List<ITypeSymbol> entities)
		{
			var dbContext = new StringBuilder();
			dbContext.AppendFormat(@"
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace {0}
{{
	public class {1}DbContext : DbContext
	{{
		public {1}DbContext(DbContextOptions options) : base(options) {{}}
",
				context.Compilation.AssemblyName,
				dbContextName
			);

			foreach (var entity in entities)
			{
				context.ReportDiagnostic(Diagnostic.Create(
					new DiagnosticDescriptor("CD01", "Generating DbSet", $"Automatically creating DbSet {entity.Name}s in DbContext {dbContextName}", "F1", DiagnosticSeverity.Info, true),
					entity.Locations.First()
				));

				dbContext.AppendFormat(
					"\t\tpublic DbSet<{1}> {0}s {{ get; set; }}\n",
					entity.Name,
					entity.GetFullMetadataName()
				);
			}

			dbContext.Append(@"
	}
}");
			context.AddSource(dbContextName, dbContext.ToString());
		}
	}
}
