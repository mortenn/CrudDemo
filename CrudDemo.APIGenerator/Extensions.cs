using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrudDemo.Generator
{
	internal static class Extensions
	{
		public static IEnumerable<ITypeSymbol> GetTypes(this GeneratorExecutionContext context)
		{
			return context.Compilation.SourceModule.ReferencedAssemblySymbols.SelectMany(a =>
			{
				try
				{
					var main = a.Identity.Name.Split('.').Aggregate(a.GlobalNamespace, (s, c) => s.GetNamespaceMembers().Single(m => m.Name.Equals(c)));

					return GetAllTypes(main);
				}
				catch
				{
					return Enumerable.Empty<ITypeSymbol>();
				}
			});
		}

		public static string GetFullMetadataName(this ISymbol s)
		{
			if (s == null || IsRootNamespace(s))
			{
				return string.Empty;
			}

			var sb = new StringBuilder(s.MetadataName);
			var last = s;

			s = s.ContainingSymbol;

			while (!IsRootNamespace(s))
			{
				if (s is ITypeSymbol && last is ITypeSymbol)
				{
					sb.Insert(0, '+');
				}
				else
				{
					sb.Insert(0, '.');
				}

				sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
				s = s.ContainingSymbol;
			}

			return sb.ToString();
		}

		private static bool IsRootNamespace(ISymbol symbol)
		{
			return symbol is INamespaceSymbol s && s.IsGlobalNamespace;
		}

		private static IEnumerable<ITypeSymbol> GetAllTypes(INamespaceSymbol root)
		{
			foreach (var namespaceOrTypeSymbol in root.GetMembers())
			{
				switch (namespaceOrTypeSymbol)
				{
					case INamespaceSymbol @namespace:
						{
							foreach (var nested in GetAllTypes(@namespace))
							{
								yield return nested;
							}
							break;
						}
					case ITypeSymbol type:
						yield return type;
						break;
				}
			}
		}
	}
}
