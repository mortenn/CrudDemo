using System.Collections.Generic;

namespace CrudDemo.DbContextGenerator
{
	internal class CrudAPIDbContext
	{
		public CrudAPIDbContext(string @namespace, string name)
		{
			Namespace = @namespace;
			Name = name;
			DbSets = new List<CrudAPIDbSet>();
		}

		public string Namespace { get; }

		public string Name { get; }

		public List<CrudAPIDbSet> DbSets { get; }
	}
}