using CrudDemo.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CrudDemo.Models
{
	[AutoCrud("/CRUD", "Demo")]
	public class Figure
	{
		[Key]
		public int Id { get; set; }

		public string? Name { get; set; }

		[JsonIgnore]
		public int Version { get; set; }
	}
}
