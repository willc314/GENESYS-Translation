using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

// Basic Roslyn test: Follows this tutorial:
// https://www.stevejgordon.co.uk/getting-started-with-the-roslyn-apis-writing-code-with-code
// Reads the type names from schema.json and generates an empty public class for each, inside a CodeGen namespace.

// 1. Read the schema. The build copies schema.json next to the program
await using var input = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "schema.json"));
var schema = await JsonSerializer.DeserializeAsync<Schema>(input, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

// 2. Build the syntax tree: one class per type, all inside the CodeGen namespace.
var members = schema?.Types.Select(type => CreateClass(type.TypeName)).ToArray() ?? Array.Empty<MemberDeclarationSyntax>();
var ns = NamespaceDeclaration(ParseName("CodeGen")).AddMembers(members);

// 3. Format it and save it. NormalizeWhitespace adds the line breaks and indentation;
var outputPath = Path.Combine(AppContext.BaseDirectory, "generated.cs");
await using (var writer = new StreamWriter(outputPath, append: false))
{
    ns.NormalizeWhitespace().WriteTo(writer);
}

Console.WriteLine(File.ReadAllText(outputPath));
Console.WriteLine();
Console.WriteLine($"Saved to {outputPath}");

static ClassDeclarationSyntax CreateClass(string name) =>
    ClassDeclaration(Identifier(name))
        .AddModifiers(Token(SyntaxKind.PublicKeyword));

// The shape of schema.json
class Schema
{
    public IReadOnlyCollection<SchemaTypes> Types { get; init; } = [];
}

class SchemaTypes
{
    public string TypeName { get; init; } = "";

    // Not used yet
    public IReadOnlyCollection<string> Properties { get; init; } = [];
}
