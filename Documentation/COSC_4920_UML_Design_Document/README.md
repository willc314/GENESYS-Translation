# UML Design Document — Team Pentateuch

## Building

    pdflatex main.tex      # twice, for the contents
    # or
    latexmk -pdf main.tex

No flags needed. Figures are read from the converted PDFs in `images/`, which
are committed alongside the SVGs. Three passes take about a second.

Do **not** set `\usesvgtrue` on Overleaf's free plan. That makes Inkscape
convert all seven diagrams on every compile, which exceeds the compile timeout.

## Diagrams

| Chapter  | File                             |
|----------|----------------------------------|
| Block    | `images/block-diagram.svg`       |
| Use case | `images/use-case-diagram.svg`    |
| Activity | `images/state-to-activity.svg`   |
| Activity | `images/import-activity.svg`     |
| Activity | `images/export-activity.svg`     |
| Sequence | `images/sequence-diagram.svg`    |
| Class    | `images/class-diagram.svg`       |
| State    | `images/state-diagram.svg`       |

All current. The editable sources for the three activity diagrams are in
`diagrams-src/`.

### After editing a diagram

1. Export from draw.io as SVG over the file of the same name in `images/`.
2. Run `tools/svg2pdf.sh` to regenerate the PDF beside it.
3. Commit both.

The figure macro resolves `.pdf` first, then `.png`, then `.svg`, so a stale PDF
will silently win over an updated SVG. Step 2 is not optional.

A file that is missing entirely renders as a visible placeholder, so the
document always builds.

## Layout

    main.tex                 preamble, title page, chapter includes
    content/                 one file per chapter, so three people can edit at once
    images/                  exported diagrams
    tools/svg2pdf.sh         optional SVG to PDF conversion

## Still to do

- Resolve the naming inconsistency flagged in Chapters 3 and 5.
- Decide whether the tool runs in parallel to GENESYS (requirements 2.3) or
  inside it, and make the block and state diagrams agree with that choice.
- Add the review step to the use case and sequence diagrams.
- Add a toolbar entry component to the block diagram, or justify its absence.
- Address the five gaps listed in Chapter 5.
