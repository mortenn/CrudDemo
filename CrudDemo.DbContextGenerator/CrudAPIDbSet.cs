using Microsoft.CodeAnalysis;

namespace CrudDemo.DbContextGenerator
{
	internal class CrudAPIDbSet
	{
		public CrudAPIDbSet(ISymbol entityType)
		{
			Type = entityType.GetFullMetadataName();
			Name = $"{entityType.Name}s";
		}

		public string Type { get; }

		public string Name { get; }
	}
}