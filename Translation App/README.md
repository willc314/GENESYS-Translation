# Introduction
Team Pentateuch's senior design project: a tool that translates between GENESYS models and code in both directions, and keeps the two in sync afterwards. The goal is to let engineers who model in GENESYS share their logic as code, and bring existing code into GENESYS, without rebuilding it by hand.

This is an early setup. For now, each program just runs a basic test to show its API works.

| Project | Framework | What it is | Current test |
|---|---|---|---|
| `Pentateuch.Absyn` | .NET Standard 2.0 | Shared model classes used by both programs | None (a library, so it can't be run) |
| `Pentateuch.Genesys` | .NET Framework 4.8 | Talks to GENESYS through its API | Vitech's StructureTraversalSample: prints a function's behavior diagram |
| `Pentateuch.Translator` | .NET 10 | Reads and writes C# code with Roslyn | Generates classes from `schema.json` |

The GENESYS API only runs on .NET Framework 4.8, so Roslyn lives in a separate .NET 10 program.

# Getting Started
1. **Installation process**
   - Install GENESYS 2026 University Edition, and Visual Studio 2026 with the **.NET desktop development** workload.
   - Clone the repo and open `Pentateuch.slnx`. The Roslyn packages download automatically the first time it builds.
   - If GENESYS isn't installed in `C:\Program Files\Vitech\GENESYS 2026 University Edition`, set a `GenesysDir` environment variable to your install folder, for example `setx GenesysDir "D:\GENESYS 2026 University Edition"`, then restart Visual Studio.
2. **Software dependencies**
   - Windows (GENESYS only runs there)
   - .NET Framework 4.8 and the .NET 10 SDK (both come with the Visual Studio workload)
   - The GENESYS API DLLs, which are referenced from your GENESYS install folder rather than stored in the repo
   - Roslyn NuGet packages: `Microsoft.CodeAnalysis.CSharp`, `Microsoft.CodeAnalysis.CSharp.Workspaces` and `Microsoft.CodeAnalysis.Workspaces.MSBuild`
3. **Latest releases**
   - None yet.
4. **API references**
   - GENESYS API: the [API Getting Started Guide](https://vitechcorp.com/support/documentation/genesys/2026/APIGettingStartedGuide.pdf), and the full reference (`API.chm`) that comes with the [GENESYS documentation](https://www.zuken.com/us/resource/genesys-software-documentation/)
   - Roslyn: [Microsoft.CodeAnalysis.CSharp on Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp)

# Build and Test
From this folder:

```powershell
dotnet build
dotnet run --project Pentateuch.Translator
dotnet run --project Pentateuch.Genesys -- "<project name>" "<function name>"
```

- **Translator** prints the generated code and saves it to `Pentateuch.Translator\bin\Debug\net10.0\generated.cs`.
- **Genesys** logs in with GENESYS's default account (`Administrator` / `admin`, set in `Program.cs`), prints the function's behavior diagram, and waits for Enter. Without names, it uses the sample's `"SAMPLE: Fast Food"` and `"Operate FF Restaurant"`. With the GENESYS Software Example imported, try `"GENESYS Software Example" "_Do Engine States State"`.

There are no automated tests yet; running the two programs is the test for now.

# Contribute
- Work on a branch and open a pull request into `main`.
- Keep GENESYS API code in `Pentateuch.Genesys` and Roslyn code in `Pentateuch.Translator`. Anything both need goes in `Pentateuch.Absyn`.
- Don't commit the GENESYS DLLs. They're licensed, and each person's build uses their own install.
- The requirements and design documents are in the `Documentation` folder at the root of the repo.
