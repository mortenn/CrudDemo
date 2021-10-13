using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CrudDemo.DbContextGenerator
{
	internal class DbContextFactoryReceiver : ISyntaxReceiver
	{
		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode is ClassDeclarationSyntax classSyntax && classSyntax.Identifier.Text.EndsWith("DbContextFactory"))
			{
				DbContexts.Add(classSyntax.Identifier.Text.Replace("DbContextFactory", string.Empty));
			}
		}

		internal List<string> DbContexts = new();
	}
}
