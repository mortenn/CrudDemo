using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using System;
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
			try
			{
				// build the model
				var model = new CrudAPIModel(context);

				// read the template
				const string templateFile = "CrudAPI.sbncs";
				var template = Template.Parse(ResourceReader.GetResource<CRUDRegistrationGenerator>(templateFile), templateFile);

				// apply the template
				var output = template.Render(model, member => member.Name);

				// add the file
				context.AddSource("CrudAPI", SourceText.From(output, Encoding.UTF8));
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
