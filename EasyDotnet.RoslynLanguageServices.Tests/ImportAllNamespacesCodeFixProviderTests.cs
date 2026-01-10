using EasyDotnet.RoslynLanguageServices.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace EasyDotnet.RoslynLanguageServices.Tests;

public class ImportAllNamespacesCodeFixProviderTests
{
    [Test]
    public async Task ShouldAddMissingNamespaces_WhenMissingBuiltInTypes()
    {
        var context = new CSharpCodeFixTest<EmptyDiagnosticAnalyzer, ImportAllNamespacesCodeFixProvider, DefaultVerifier>();
        context.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;

        context.TestCode = """
            namespace MyApp
            {
                public class Program
                {
                    public static void Main(string[] args)
                    {
                        var list = new {|CS0246:List<string>|}();
                        {|CS0103:Console|}.WriteLine("Hello!");
                        var json = {|CS0103:JsonSerializer|}.Serialize(list);
                        var another = new {|CS0246:ConcurrentDictionary<string, string>|}();
                    }
                }
            }
            """;

        context.FixedCode = """
            using System;
            using System.Collections.Concurrent;
            using System.Collections.Generic;
            using System.Text.Json;

            namespace MyApp
            {
                public class Program
                {
                    public static void Main(string[] args)
                    {
                        var list = new List<string>();
                        Console.WriteLine("Hello!");
                        var json = JsonSerializer.Serialize(list);
                        var another = new ConcurrentDictionary<string, string>();
                    }
                }
            }
            """;

        await context.RunAsync();
    }
}
