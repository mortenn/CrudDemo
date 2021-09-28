using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace CrudDemo.Generator
{
	internal class DbContextFactoryReceiver : ISyntaxReceiver
	{
		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode is ClassDeclarationSyntax classSyntax && classSyntax.Identifier.Text.EndsWith("DbContextFactory"))
			{
				dbContexts.Add(classSyntax.Identifier.Text.Replace("DbContextFactory", string.Empty));
			}
		}

		internal List<string> dbContexts = new List<string>();
	}
}
