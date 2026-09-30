// Based on: https://github.com/zkSNACKs/WalletWasabi/blob/1b8d142d0ec6892f13f6c50c3fec5d7ceb7f06ce/WalletWasabi.Fluent.Generators/StaticViewLocatorGenerator.cs
// Prevents reflection on View Locators. Reflection is slow.
/**
 * MIT License

Copyright (c) 2023 zkSNACKs

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AccountOperationUtilities.Generators;

[Generator]
public class StaticViewLocatorGenerator : IIncrementalGenerator
{
    private const string RootNameSpace = "AccountOperationUtilities.Generators";
    private const string StaticViewLocatorAttributeDisplayString = RootNameSpace + ".StaticViewLocatorAttribute";

    private const string ViewModelSuffix = "ViewModel";

    private const string ViewSuffix = "View";

    private enum CandidateKind
    {
        None,
        Locator,
        ViewModel,
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax,
                static (ctx, ct) => Classify(ctx, ct))
            .Where(static x => x.Kind != CandidateKind.None);

        var combined = candidates.Collect().Combine(context.CompilationProvider);

        context.RegisterSourceOutput(combined, static (spc, source) => Execute(spc, source.Right, source.Left));
    }

    private static (INamedTypeSymbol? Symbol, CandidateKind Kind) Classify(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        if (context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken) is not INamedTypeSymbol namedTypeSymbol)
        {
            return (null, CandidateKind.None);
        }

        if (namedTypeSymbol.GetAttributes().Any(ad => ad?.AttributeClass?.ToDisplayString() == StaticViewLocatorAttributeDisplayString))
        {
            return (namedTypeSymbol, CandidateKind.Locator);
        }

        if (namedTypeSymbol.Name.EndsWith(ViewModelSuffix) && !namedTypeSymbol.IsAbstract)
        {
            return (namedTypeSymbol, CandidateKind.ViewModel);
        }

        return (null, CandidateKind.None);
    }

    private static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<(INamedTypeSymbol? Symbol, CandidateKind Kind)> candidates)
    {
        if (compilation.GetTypeByMetadataName(StaticViewLocatorAttributeDisplayString) is null)
        {
            return;
        }

        var locators = candidates.Where(x => x.Kind == CandidateKind.Locator).Select(x => x.Symbol!).ToList();
        if (locators.Count == 0)
        {
            return;
        }

        var viewModels = candidates.Where(x => x.Kind == CandidateKind.ViewModel).Select(x => x.Symbol!).ToList();
        viewModels.Sort((x, y) => string.CompareOrdinal(x.ToDisplayString(), y.ToDisplayString()));

        foreach (var locator in locators)
        {
            var classSource = ProcessClass(compilation, locator, viewModels);
            if (classSource is not null)
            {
                context.AddSource($"{locator.Name}_StaticViewLocator.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }
    }

    private static string? ProcessClass(Compilation compilation, INamedTypeSymbol namedTypeSymbolLocator, List<INamedTypeSymbol> namedTypeSymbolViewModels)
    {
        if (!namedTypeSymbolLocator.ContainingSymbol.Equals(namedTypeSymbolLocator.ContainingNamespace, SymbolEqualityComparer.Default))
        {
            return null;
        }

        string namespaceNameLocator = namedTypeSymbolLocator.ContainingNamespace.ToDisplayString();

        var format = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeTypeConstraints | SymbolDisplayGenericsOptions.IncludeVariance);

        string classNameLocator = namedTypeSymbolLocator.ToDisplayString(format);

        var source = new StringBuilder($@"// <auto-generated />
#nullable enable
using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace {namespaceNameLocator}
{{
    public partial class {classNameLocator}
    {{");
        source.Append($@"
		private static Dictionary<Type, Func<Control>> StaticViews = new()
		{{
");

        foreach (var namedTypeSymbolViewModel in namedTypeSymbolViewModels)
        {
            string namespaceNameViewModel = namedTypeSymbolViewModel.ContainingNamespace.ToDisplayString();
            string classNameViewModel = $"{namespaceNameViewModel}.{namedTypeSymbolViewModel.ToDisplayString(format)}";
            string classNameView = classNameViewModel.Replace(ViewModelSuffix, ViewSuffix);

            var classNameViewSymbol = compilation.GetTypeByMetadataName(classNameView);

            // TODO: Restrict Base classes
            //var userControlViewSymbol = compilation.GetTypeByMetadataName("Avalonia.Controls.UserControl");
            //classNameViewSymbol.BaseType?.Equals(userControlViewSymbol, SymbolEqualityComparer.Default) != true
            if (classNameViewSymbol is null)
            {
                source.AppendLine($@"			[typeof({classNameViewModel})] = () => new TextBlock() {{ Text = {("\"Not Found: " + classNameView + "\"")} }},");
            }
            else
            {
                //source.AppendLine($"// {classNameViewSymbol?.BaseType?.Name}");
                source.AppendLine($@"			[typeof({classNameViewModel})] = () => new {classNameView}(),");
            }
        }

        source.Append($@"		}};
	}}
}}");

        return source.ToString();
    }
}
