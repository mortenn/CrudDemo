using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CrudDemo.DbContextGenerator
{
	public class ResourceReader
	{
		public static string? GetResource<TAssembly>(string endWith) => GetResource(endWith, typeof(TAssembly));

		public static string? GetResource(string endWith, Type assemblyType)
		{
			var assembly = GetAssembly(assemblyType);

			var resources = assembly?.GetManifestResourceNames().Where(r => r.EndsWith(endWith)).ToArray();

			return resources?.Length switch
			{
				null => throw new InvalidOperationException("There are no resources"),
				0 => throw new InvalidOperationException($"There is no resources that ends with '{endWith}'"),
				1 => ReadEmbeddedResource(assembly, resources.Single()),
				_ => throw new InvalidOperationException($"There is more then one resource that ends with '{endWith}'")
			};
		}

		private static Assembly? GetAssembly(Type assemblyType)
		{
			return Assembly.GetAssembly(assemblyType);
		}

		private static string? ReadEmbeddedResource(Assembly? assembly, string name)
		{
			using var resourceStream = assembly?.GetManifestResourceStream(name);
			if (resourceStream == null) return null;
			using var streamReader = new StreamReader(resourceStream);
			return streamReader.ReadToEnd();
		}
	}
}
