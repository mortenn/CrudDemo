using System;

namespace CrudDemo.Abstractions
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AutoCrudAttribute : Attribute
	{
		public AutoCrudAttribute(string route, string dbContext)
		{
			Route = route;
			DbContext = dbContext;
		}

		public string Route {  get; }

		public string DbContext { get; }
	}
}
