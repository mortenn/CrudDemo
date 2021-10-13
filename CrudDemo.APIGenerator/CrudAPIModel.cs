using System.Collections.Generic;
using System.Linq;
using CrudDemo.Abstractions;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace CrudDemo.APIGenerator
{
	[PublicAPI]
	internal class CrudAPIModel
	{
		public CrudAPIModel(GeneratorExecutionContext context)
		{
			Namespace = context.Compilation.AssemblyName!;
			Models = new List<CrudAPIEndpointModel>();
			var types = context.GetTypes().ToList();
			foreach (var type in types)
			{
				var autoCrud = type.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == nameof(AutoCrudAttribute));
				if (autoCrud == null) continue;
				var route = (string)autoCrud.ConstructorArguments[0].Value!;
				var dbContext = $"{autoCrud.ConstructorArguments[1].Value}DbContext";
				var dbContextType = types.FirstOrDefault(t => t.Name == dbContext)?.GetFullMetadataName();
				if (string.IsNullOrWhiteSpace(dbContextType))
				{
					context.ReportDiagnostic(Diagnostic.Create(
						new DiagnosticDescriptor("CD01", "Unable to locate DbContext", $"Could not locate a DbContext named {dbContext}, missing assembly reference to EF models?", "F1", DiagnosticSeverity.Error, true),
						null
					));
					return;
				}

				var model = new CrudAPIEndpointModel(route, type.Name, type.GetFullMetadataName(), dbContextType!);
				var members = type.GetMembers();
				foreach (var prop in members.Where(PropertyIsObjectKey))
				{
					model.Key.Add(prop.Name, ((IPropertySymbol)prop).Type.Name);
				}
				foreach (var prop in members.Where(PropertyIsIncludedInModel))
				{
					model.Properties.Add(prop.Name, ((IPropertySymbol)prop).Type.Name);
				}
				Models.Add(model);
			}
		}

		public string Namespace { get; set; }
		public List<CrudAPIEndpointModel> Models { get; set; }

		private static bool PropertyIsObjectKey(ISymbol symbol)
		{
			return symbol.Kind == SymbolKind.Property
				&& symbol.GetAttributes().Any(a => a.AttributeClass?.Name == "KeyAttribute");
		}

		private static bool PropertyIsIncludedInModel(ISymbol symbol)
		{
			return symbol.Kind == SymbolKind.Property
				&& !symbol.GetAttributes().Any(a => a.AttributeClass?.Name == "KeyAttribute" || a.AttributeClass?.Name == "JsonIgnoreAttribute");
		}
	}
}
