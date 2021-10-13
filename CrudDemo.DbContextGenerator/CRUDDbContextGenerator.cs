using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrudDemo.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace CrudDemo.DbContextGenerator
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
			if (context.SyntaxReceiver is not DbContextFactoryReceiver scanner || scanner.DbContexts.Count == 0)
				return;
			
			context.ReportDiagnostic(Diagnostic.Create(
				new DiagnosticDescriptor("CD01", "Generating DbContext", $"Automatically creating DbContext for entity types", "F1", DiagnosticSeverity.Info, true),
				null
			));
			try
			{
				// build the model
				var types = context.GetTypes().ToList();
				var contexts = new Dictionary<string, CrudAPIDbContext>();
				foreach (var type in types)
				{
					var autoCrud = type.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == nameof(AutoCrudAttribute));
					var dbContext = autoCrud?.ConstructorArguments[1].Value?.ToString();
					if (dbContext == null) continue;
					if (!contexts.ContainsKey(dbContext))
					{
						if (!scanner.DbContexts.Contains(dbContext))
						{
							context.ReportDiagnostic(Diagnostic.Create(
								new DiagnosticDescriptor("CD01", "Missing DbContextFactory", $"The DbContext {dbContext} requires a {dbContext}DbContextFactory class implementing the IDesignTimeDbContextFactory<{dbContext}DbContext> interface", "F1", DiagnosticSeverity.Error, true),
								null
							));
						}
						contexts.Add(dbContext, new CrudAPIDbContext(context.Compilation.AssemblyName!, $"{dbContext}DbContext"));
					}

					contexts[dbContext].DbSets.Add(new CrudAPIDbSet(type));
				}

				// read the template
				const string templateFile = "CrudDbContext.sbncs";
				var template = Template.Parse(ResourceReader.GetResource<CRUDDbContextGenerator>(templateFile), templateFile);

				// apply the template
				foreach (var model in contexts.Values)
				{
					var output = template.Render(model, member => member.Name);

					// add the file
					context.AddSource(model.Name, SourceText.From(output, Encoding.UTF8));
				}
			}
			catch (Exception e)
			{
				while (e != null)
				{
					context.ReportDiagnostic(Diagnostic.Create(
						new DiagnosticDescriptor("CD00", "Compiler exception", $"{e.Message} {e.StackTrace}", "F1", DiagnosticSeverity.Error, true),
						null
					));
					e = e.InnerException;
				}
			}
		}
	}
}
