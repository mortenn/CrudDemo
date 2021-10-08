using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CrudDemo.Generator
{
	internal class CrudAPIEndpointModel
	{
		public string Route { get; internal set; }
		public string Name { get; internal set; }
		public string FullName { get; internal set; }
		public string DbContext { get; internal set; }
		public Dictionary<string, string> Key { get; set; }
		public Dictionary<string, string> Properties { get; internal set; }

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
