﻿using CrudDemo.Abstractions;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace CrudDemo.Generator
{
	internal class CrudAPIModel
	{
		public CrudAPIModel(GeneratorExecutionContext context)
		{
			Namespace = context.Compilation.AssemblyName;
			Models = new List<CrudAPIEndpointModel>();
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
				var properties = type.GetMembers().Where(m => m.Kind == SymbolKind.Property && !m.GetAttributes().Any(a => a.AttributeClass.Name == "Key"));

				Models.Add(new CrudAPIEndpointModel
				{
					Route = route,
					Name = type.Name,
					FullName = type.GetFullMetadataName(),
					DbContext = dbContextType,
					Properties = properties.Where(prop => prop.Name != "Id").Select(prop => prop.Name).ToList()
				});
			}
		}

		public string Namespace { get; set; }
		public List<CrudAPIEndpointModel> Models { get; set; }
	}
}
