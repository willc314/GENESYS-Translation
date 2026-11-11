# Tool Selection Document — GENESYS Translation Tool

Team Pentateuch, COSC 4920.

## Building

    pdflatex main.tex      # twice, for the contents
    # or
    latexmk -pdf main.tex

No flags, no figures, no external dependencies.

## Layout

    main.tex                  preamble, title page, chapter includes
    content/00-introduction   purpose, evaluation criteria, scope
    content/01-runtime        .NET 4.8 / .NET 10 / .NET Standard 2.0 strategy
    content/02-adopted        GENESYS Client API, Roslyn, supporting tools
    content/03-rejected       Excel Connector, direct DB, GNSX/GNUX
    content/04-integration    tools mapped to the UML components and requirements
    content/04-summary        decision table and open items

One file per chapter, so three people can edit without merge conflicts.

## Keeping it current

No placeholders remain. One section will need revising as the build proceeds:
`01-runtime.tex`, in "Communication Between the Two Programs", records that the
mechanism is undecided and lists the three candidates. Replace that with the
choice and its reasoning once made.

## Conventions

Use `\code{...}` rather than `\texttt{...}` for anything with dots in it, such
as `Pentateuch.Absyn` or a NuGet package name. It renders the same but is allowed
to break after a dot, which `\texttt` is not, so long identifiers would otherwise
run off the right margin.

Short tables use a floating `table` with `tabular` inside. Only tables long
enough to genuinely span pages use `longtable`, since `longtable` does not float
and will break wherever it lands.
