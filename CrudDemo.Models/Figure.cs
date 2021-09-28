using CrudDemo.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace CrudDemo.Models
{
	[AutoCrud("/CRUD", "Demo")]
	public class Figure
	{
		[Key]
		public int Id {  get; set; }
		public string? Name { get; set; }
	}
}
