using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace CrudDemo.APIGenerator
{
	[PublicAPI]
	internal class CrudAPIEndpointModel
	{
		public CrudAPIEndpointModel(string route, string name, string fullName, string dbContext)
		{
			Route = route;
			Name = name;
			FullName = fullName;
			DbContext = dbContext;
			Key = new Dictionary<string, string>();
			Properties = new Dictionary<string, string>();
		}

		public string Route { get; }
		public string Name { get; }
		public string FullName { get; }
		public string DbContext { get; }
		public Dictionary<string, string> Key { get; }
		public Dictionary<string, string> Properties { get; }

		public string RouteTemplateKey => string.Join("/", OrderedKey.Select(k => $"{{{k.Key}{RouteTypeMapper(k.Value)}}}"));

		public string RouteKeyArguments => string.Join(", ", OrderedKey.Select(k => $"{k.Value} {k.Key}"));

		public string KeyArgumentList => string.Join(", ", OrderedKey.Select(k => k.Key));

		public string ResourceCreatedPath => string.Join("/", OrderedKey.Select(k => $"{{item.{k.Key}}}"));

		public string RecordKey => RouteKeyArguments;

		public string RecordProperties => string.Join(", ", Properties.OrderBy(p => p.Key).Select(p => $"{p.Value} {p.Key}"));

		public string RecordMap => "item." + string.Join(", item.", OrderedKeyProperties) + ", item." +string.Join(", ", OrderedProperties);

		public IEnumerable<string> OrderedKeyProperties => Key.Keys.OrderBy(k => k);
		
		public IEnumerable<string> OrderedProperties => Properties.Keys.OrderBy(k => k);

		private IEnumerable<KeyValuePair<string, string>> OrderedKey => Key.OrderBy(k => k.Key);

		private static string RouteTypeMapper(string type) => type switch
		{
			nameof(Int32) => ":int",
			_ => string.Empty
		};
	}
}
