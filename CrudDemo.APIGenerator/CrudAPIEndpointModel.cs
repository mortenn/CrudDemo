using System.Collections.Generic;

namespace CrudDemo.Generator
{
	internal class CrudAPIEndpointModel
	{
		public string Route { get; internal set; }
		public string Name { get; internal set; }
		public string FullName { get; internal set; }
		public string DbContext { get; internal set; }
		public List<string> Properties { get; internal set; }
	}
}
